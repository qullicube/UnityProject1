using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameSystem))]
public class InputSystem : MonoBehaviour
{
	GameSystem gameSystem;

	void Start()
	{
		gameSystem = GetComponent<GameSystem>();
	}
	
	// Update is called once per frame
	void Update()
	{
		OnInput();
	}

	//Input of the system
	void OnInput()
	{
		switch (gameSystem.state)
		{
			case GameState.PAUSE: 
				{	
					//Unpause game
					if (Input.GetButtonDown("Start"))
					{
						gameSystem.state = GameState.BATTLE;
					}
				}
				break;
			case GameState.BATTLE:
				{
					//Pause game
					if (Input.GetButtonDown("Start"))
					{
						gameSystem.state = GameState.PAUSE;
						return;
					}
					
					//Input states
					var rotateLeft = Input.GetButtonDown("L2");
					var rotateRight = Input.GetButtonDown("R2");
					var tilt = rotateLeft && rotateRight;
					var x = Input.GetAxis("Horizontal");
					var z = Input.GetAxis("Vertical");
						
					//Rotate-Tilt
					if (tilt)
					{
						gameSystem.Camera_Tilt();
					}
					else if (rotateLeft)
					{
						gameSystem.Camera_RotateLeft();
					}
					else if (rotateRight)
					{
						gameSystem.Camera_RotateRight();
					}
						
					//Move Tile Selector
					if (!OnMoveSelector(x, z))
					{
						//Action
						OnAction();
					}
				}
				break;
			default:
				break;
		}
	}
	
	float time;
	const float MIN_TIME = 0.1f; //TODO: find right fit (?)
	const float MAX_TIME = 0.3f;
	const float EPSILON = 0.02f;
	
	bool OnMoveSelector(float x, float z)
	{
		var processed = false;
		//if x && y is large, then move fast
		var multiplier = 1 - Mathf.Max(x * x, z * z);
		var threshold = Mathf.Lerp(MIN_TIME, MAX_TIME, multiplier);
		
		//Dirty work here.. pseudo float-to-int on same var
		if (x > EPSILON)	
			x = 1;
		else if (x < -EPSILON)
			x = -1;
		else
			x = 0;
		
		if (z > EPSILON)
			z = 1;
		else if (z < -EPSILON)
			z = -1;
		else
			z = 0;
			
		//Move only after interval is reached
		if (time >= threshold && ((Mathf.Abs(x) > EPSILON || Mathf.Abs(z) > EPSILON)))
		{
			gameSystem.Selector_MoveBy((int)x, (int)z);
			processed = true;
			time = 0.0f;
		}
		
		//Update time
		time += Time.deltaTime;
		
		return processed;
	}
	
	bool OnAction()
	{
		var processed = false; //not sure yet if used
		
		if (Input.GetButtonDown("X"))
		{
			//Select unit or DoMove
			if (gameSystem.selectedPlayer != null)
			{
				gameSystem.Selector_SelectPlayer();
			}
			else
			{
				gameSystem.SelectedPlayer_MoveToTileSelector();
			}
		}
		else if (Input.GetButtonDown("^"))
		{
			gameSystem.Selector_DeselectPlayer();
		}
		
		return processed;
	}
}
