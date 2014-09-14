using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
		public bool displayPathLine;
		public float walkSpeed;
		public float gravity;
	
		TileMap tileMap;
		List<PathTile> path = new List<PathTile> ();
		LineRenderer lineRenderer;
		bool busy;

		void Start ()
		{
				lineRenderer = GetComponent<LineRenderer> ();
				tileMap = FindObjectOfType (typeof(TileMap)) as TileMap;
				enabled = tileMap != null;
				busy = false;
		}

		void Update ()
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
		}

		public bool IsBusy ()
		{
				return busy;
		}

		public void MoveTo (Vector3 target)
		{
				if (tileMap.FindPath (transform.position, target, path)) {
						lineRenderer.SetVertexCount (path.Count);

						if (displayPathLine) {
								for (int i = 0; i < path.Count; i++)
										lineRenderer.SetPosition (i, path [i].transform.position);
						} else {
								lineRenderer.enabled = false;
						}

						StopAllCoroutines ();
						StartCoroutine (WalkPath ());
				}
		}

		IEnumerator WalkPath ()
		{
				var index = 0;
				busy = true;
				while (index < path.Count) {
						yield return StartCoroutine (WalkTo2 (path [index].positionTop));
						index++;
				}
				busy = false;
		}

		IEnumerator WalkTo (Vector3 position)
		{
				while (Vector3.Distance(transform.position, position) > 0.01f) {
						transform.position = Vector3.MoveTowards (transform.position, position, walkSpeed * Time.deltaTime);
						yield return 0;
				}
				transform.position = position;
		}

		IEnumerator WalkTo2 (Vector3 position)
		{
				var time = 0.0f;
				var start = transform.position;
		
				while (time < 1.0f) {
						transform.position = Plerp (start, position, time);
						time += Time.deltaTime * walkSpeed;
						yield return 0;
				}
				//transform.position = position;
		}

		Vector3 Plerp (Vector3 start, Vector3 finish, float normalized_time)
		{
				//var height = new Vector3(0, (finish - start).y, 0);
				var length = (finish - start);

				var factor = Mathf.Abs (length.y) * 5.0f;
				var isJump = Mathf.Abs (length.y) < 0.01f ? 0 : 1;
				var gravityVector = new Vector3 (0, gravity * factor * isJump, 0);
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
}
