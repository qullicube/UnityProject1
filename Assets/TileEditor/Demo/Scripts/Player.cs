using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerDirection
{
	DOWN = 0,
	LEFT = 1,
	UP = 2,
	RIGHT = 3
}

public enum PlayerState
{
	Idle,
	Walk,
	Jump
}

public class Player : MonoBehaviour
{
	#region Inspector Variables
	
	public bool displayPathLine;
	public float walkSpeed;
	public float jumpSpeed;
	public float gravity;

	public float walkPower;
	
	public PlayerDirection direction;
	public PlayerState state;
	
	#endregion
	
	#region Hidden Variables
	
	//TileMap tileMap;
	List<PathTile> path = new List<PathTile>();
	SpriteAnimator spriteAnimator;
	CharacterController controller;
	
	PlayerState lastState;
	PlayerDirection lastDirection;
	int lastCameraDirection;
	string currentSpriteAnimation;
	
	bool busy;
	float offGroundDuration;
	Vector3 walkVelocity;
	Vector3 jumpVelocity;

	#endregion
	
	#region Public Functions
	
	[Obsolete("Current not used, but might be used in the future")]
	public bool IsBusy()
	{
		return busy;
	}

	public void MoveTo(TileMap map, Vector3 target, List<PathTile> walkable, Action finishedCallback)
	{
		if (map.FindPath(transform.position, target, path, tile => walkable.Contains(tile)))
		{
			StopAllCoroutines();
			StartCoroutine(WalkPath(finishedCallback));
		}
	}	
	public void MoveTo(TileMap map, Vector3 target, List<PathTile> walkable)
	{
		if (map.FindPath(transform.position, target, path, tile => walkable.Contains(tile)))
		{
			StopAllCoroutines();
			StartCoroutine(WalkPath());
		}
	}
	public void MoveTo(TileMap map, Vector3 target)
	{
		if (map.FindPath(transform.position, target, path))
		{
			StopAllCoroutines();
			StartCoroutine(WalkPath());
		}
	}
	public void Move(Vector2 move)
	{
		walkVelocity.x = (int)Mathf.Round(move.x);
		walkVelocity.z = (int)Mathf.Round(move.y);
	}
	public void Jump(float jump)
	{
		if (controller.isGrounded)
		{
			jumpVelocity.y = jump * jumpSpeed;
		}
	}

	#endregion

	#region Start
	
	void Start()
	{
		//tileMap = FindObjectOfType(typeof(TileMap)) as TileMap;
		//enabled = tileMap != null;
		busy = false;

		controller = GetComponent<CharacterController>();
		spriteAnimator = GetComponentInChildren<SpriteAnimator>();
		
		state = PlayerState.Idle;
		direction = PlayerDirection.DOWN;
		lastCameraDirection = Camera.main.GetComponent<GameCamera>().Orientation;
		UpdateDirection();

		walkVelocity = Vector3.zero;
		jumpVelocity = Vector3.zero;
	}
	
	#endregion

	#region Update
	
	void Update()
	{
		/*
		if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
		{
			var plane = new Plane(Vector3.up, Vector3.zero);
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float hit;
			if (plane.Raycast(ray, out hit))
			{
				var target = ray.GetPoint(hit);

				if (tileMap.FindPath(transform.position, target, path))
				{
					lineRenderer.SetVertexCount(path.Count);
					for (int i = 0; i < path.Count; i++)
						lineRenderer.SetPosition(i, path[i].transform.position);

					StopAllCoroutines();
					StartCoroutine(WalkPath());
				}
			}
		}
		 * */
		 
		//Orientation update
		if (state != lastState || direction != lastDirection || lastCameraDirection != Camera.main.GetComponent<GameCamera>().Orientation)
		{
			UpdateDirection();
			lastState = state;
			lastDirection = direction;
			lastCameraDirection = Camera.main.GetComponent<GameCamera>().Orientation;
		}

		if (!busy)
		{
			var nextState = PlayerState.Idle ;

			if (walkVelocity.magnitude > 0.01f)
			{
				nextState = PlayerState.Walk;
				controller.SimpleMove(walkVelocity * walkSpeed);

				//Change direction
				if (walkVelocity.z > 0.0f)
					direction = PlayerDirection.LEFT;
				if (walkVelocity.z < 0.0f)
					direction = PlayerDirection.RIGHT;
				if (walkVelocity.x > 0.0f)
					direction = PlayerDirection.UP;
				if (walkVelocity.x < 0.0f)
					direction = PlayerDirection.DOWN;
			}

			if (!controller.isGrounded && offGroundDuration > 1/(jumpSpeed*jumpSpeed))
			{
				nextState = PlayerState.Jump;
			}

			if (!controller.isGrounded && offGroundDuration <= 1/(jumpSpeed*jumpSpeed))
				offGroundDuration += Time.deltaTime;

			if (controller.isGrounded)
				offGroundDuration = 0.0f;

			jumpVelocity.y -= gravity * Time.deltaTime;

			controller.Move(jumpVelocity * Time.deltaTime);
			state = nextState;
		}
	}
	
	#endregion

	#region My Functions

	void UpdateDirection()
	{
		var cameraProjectedDirection = ((int)direction - Camera.main.GetComponent<GameCamera>().Orientation + 4) % 4;
		spriteAnimator.Play(state.ToString(), cameraProjectedDirection);
	}

	IEnumerator WalkPath(Action finished)
	{
		var index = 0;
		busy = true;
		while (index < path.Count)
		{
			yield return StartCoroutine(WalkTo(path [index].positionTop));
			index++;
		}
		busy = false;
		finished();
	}

	
	IEnumerator WalkPath()
	{
		var index = 0;
		busy = true;

		var tmp = state;
		while (index < path.Count)
		{
			yield return StartCoroutine(WalkTo(path [index].positionTop));
			index++;
		}
		state = tmp;
		busy = false;
	}
	
	/*Move deterministic */
	IEnumerator WalkTo(Vector3 position)
	{
		var x = position.x - transform.position.x;
		var z = position.z - transform.position.z;
		
		if (z >= 0.01f)
		{
			direction = PlayerDirection.LEFT;
		}
		else if (z <= -0.01f)
		{
			direction = PlayerDirection.RIGHT;
		}
		
		if (x >= 0.01f)
		{
			direction = PlayerDirection.UP;
		}
		else if (x <= -0.01f)
		{
			direction = PlayerDirection.DOWN;
		}
		
		if (Math.Abs(position.y - transform.position.y) > 0.01f)
		{ 
			yield return StartCoroutine(WalkToWithJump(position));
		}
		else
		{
			yield return StartCoroutine(WalkToWithoutJump(position));
		}
	}

	/* Move without jumping */
	IEnumerator WalkToWithoutJump(Vector3 position)
	{
		var tmp = state;
		
		state = PlayerState.Walk;
		while (Vector3.Distance(transform.position, position) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, position, walkSpeed * Time.deltaTime);
			yield return 0;
		}
		state = tmp;
		
		transform.position = position;
	}

	/* This one concern about jumping */
	IEnumerator WalkToWithJump(Vector3 position)
	{
		var time = 0.0f;
		var start = transform.position;		
		var tmp = state;
		
		state = PlayerState.Jump;
		while (time < 1.0f)
		{
			transform.position = Plerp(start, position, time);
			time += Time.deltaTime * walkSpeed;
			yield return 0;
		}
		state = tmp;
		
		transform.position = position;
	}
	
	/* Projectile Lerp */
	Vector3 Plerp(Vector3 start, Vector3 finish, float normalized_time)
	{
		//var height = new Vector3(0, (finish - start).y, 0);
		var length = (finish - start);

		var factor = Mathf.Abs(length.y) * 5.0f;
		var isJump = Mathf.Abs(length.y) < 0.01f ? 0 : 1;
		var gravityVector = new Vector3(0, gravity * factor * isJump, 0);
		var velocity = length + 0.5f * gravityVector;

		var result = start + (velocity * normalized_time) - (0.5f * gravityVector * normalized_time * normalized_time);

		return result;
	}

	/*void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		for (int i = 0; i < path.Count; i++)
		{
			Gizmos.DrawSphere(path[i].transform.position, 0.05f);
			if (i > 0)
				Gizmos.DrawLine(path[i - 1].transform.position, path[i].transform.position);
		}
	}*/
	
	#endregion
}
