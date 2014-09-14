using UnityEngine;
using System;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	#region Properties
	
		public int Orientation {
				get {
						return orientation;
				}
		}
	
	#endregion
	
	#region Inspector Variables

		public GameObject target;
		public float tileSpeed = 10.0f;
		public float rotationSpeed = 10.0f;
		public float zoomSpeed = 10.0f;
		public Vector3 trackingCenter = new Vector3 (0, 0, 0);

	#endregion

	#region Hidden Variables

		int orientation; 	//Rotate(0, 90 * orientation, 0)
		int zoomLevel; 		//Orthosize(2.5 + zoomLevel * 1.75f)
		int tiltLevel; 		//Tilt(15.0f * tiltLevel) 
	
		const float SPEED_THRESHOLD = 20.0f; //Check if bug
		const int MAX_ZOOM_LEVEL = 3;
		const int MAX_TILT_LEVEL = 2; //Do not change these unless you know what you're doing..

		Vector3 INITIAL_EULER_ANGLES = new Vector3 (30, 45, 0); //Tested values

		bool dirty = false; //TODO: use this to optimize

	#endregion

	#region Public Functions
	
		/* Tilt camera downward, reset after MAX_TILT_LEVEL is reached */
		public void Tilt ()
		{
				tiltLevel++;
		
				if (tiltLevel > MAX_TILT_LEVEL) {
						StartCoroutine (Tilt (target.transform.position, -15.0f * MAX_TILT_LEVEL));
						tiltLevel = 0;
				} else
						StartCoroutine (Tilt (target.transform.position, 15.0f));

		}
		/* Rotate 90 degree clockwise */
		public void RotateRight ()
		{
				StopAllCoroutines ();
				StartCoroutine (Rotate (target.transform.position, new Vector3 (0, -90.0f, 0)));
				orientation--;
				if (orientation < 0)
						orientation += 4;
		}
		/* Rotate 90 degree counter-clockwise */
		public void RotateLeft ()
		{
				StopAllCoroutines ();
				StartCoroutine (Rotate (target.transform.position, new Vector3 (0, 90.0f, 0)));
				orientation = (orientation + 1) % 4;
		}

		/* Zoom In until MAX_ZOOM_LEVEL is reached, after which zoom level is reset to back initial level*/
		public void Zoom ()
		{
				StopAllCoroutines ();
				zoomLevel = ++zoomLevel % MAX_ZOOM_LEVEL;
				StartCoroutine (Zoom (2.5f + zoomLevel * 1.75f));
		}
	
		/* Track target position and put it at the center of camera viewport */
		public void TrackTarget (GameObject _target)
		{
				this.target = _target;
		}
	
		/* Untrack Target, same thing as TrackTarget(null) */
		public void UntrackTarget ()
		{
				this.target = null;
		}

	#endregion
	
	#region Unity Functions

		void Start ()
		{
				orientation = 0;
				zoomLevel = 0;
				tiltLevel = 0;

				transform.rotation = Quaternion.Euler (INITIAL_EULER_ANGLES);
		}

		void Update ()
		{
				//Track target and put it at center of camera
				if (target != null) {
						if (tileSpeed > SPEED_THRESHOLD)
								tileSpeed = SPEED_THRESHOLD;
						if (tileSpeed <= 0.0f)
								tileSpeed = 0.0f;

						var position = camera.WorldToViewportPoint (target.transform.position + trackingCenter);

						//TODO: How to prevent error accumulation?
						camera.transform.position -= (0.5f - position.x) * camera.transform.right * tileSpeed * camera.orthographicSize * Time.deltaTime;
						camera.transform.position -= (0.5f - position.y) * camera.transform.up * tileSpeed * camera.orthographicSize * Time.deltaTime;

						//if (Mathf.Abs(0.5f - position.x) < 0.01f && Mathf.Abs(0.5f - position.y) < 0.01f)
						//	dirty = false;
				}
		}
	#endregion

	#region My Functions

		//TODO: Use this in InputSystem instead
	
		[Obsolete("InputSystem manage this script instead")]
		void OnInput ()
		{
				if (Input.GetButtonDown ("L1")) {
						StopAllCoroutines ();
						StartCoroutine (Rotate (target.transform.position, new Vector3 (0, 90.0f, 0)));
						orientation = (orientation + 1) % 4;
			
				}
		
				if (Input.GetButtonDown ("R1")) {
						StopAllCoroutines ();
						StartCoroutine (Rotate (target.transform.position, new Vector3 (0, -90.0f, 0)));
						orientation--;
						if (orientation < 0)
								orientation += 4;
			
				}
		
				if (Input.GetButtonDown ("Zoom")) {
						StopAllCoroutines ();
						zoomLevel = ++zoomLevel % MAX_ZOOM_LEVEL;
						StartCoroutine (Zoom (2.5f + zoomLevel * 1.75f));
			
				}
		
				if (Input.GetButtonDown ("Tilt")) {
						StopAllCoroutines ();
						tiltLevel++;
            
						if (tiltLevel > MAX_TILT_LEVEL) {
								StartCoroutine (Tilt (target.transform.position, -15.0f * MAX_TILT_LEVEL));
								tiltLevel = 0;
						} else
								StartCoroutine (Tilt (target.transform.position, 15.0f));
            
				}
		} 

		IEnumerator Zoom (float orthoSize)
		{
				var initSize = camera.orthographicSize;
				var timeCount = 0.0f;

				while (timeCount < 1.0f) {
						camera.orthographicSize = (orthoSize - initSize) * timeCount + initSize;
						yield return 0;
						timeCount += Time.deltaTime * zoomSpeed;
				}
				camera.orthographicSize = orthoSize;
		}

		//TODO: optimize this
		IEnumerator Tilt (Vector3 pivot, float angleX)
		{
				var eulerAngles = new Vector3 (angleX, 0, 0);
				var initPosition = transform.position;
				var initRotation = transform.rotation;
				var destRotation = initRotation * Quaternion.Euler (eulerAngles);
				var destPosition = RotatePointAroundPivot (initPosition, pivot, Quaternion.AngleAxis (angleX, transform.right));

				var timeCount = 0.0f; 
				var accAngles = Vector3.zero;
				var accRotation = initRotation;

				while (timeCount < 1.0f) {
						var incrementAngle = eulerAngles * Time.deltaTime * rotationSpeed;
						accAngles += incrementAngle;
						accRotation *= Quaternion.Euler (incrementAngle);

						transform.rotation = accRotation;
						transform.position = RotatePointAroundPivot (initPosition, pivot, Quaternion.AngleAxis (accAngles.x, transform.right));
						yield return 0;

						timeCount += Time.deltaTime * rotationSpeed;
				}
				transform.position = destPosition;
				transform.rotation = destRotation;
		}
		IEnumerator Rotate (Vector3 pivot, Vector3 eulerAngles)
		{
				var initPosition = transform.position;
				var initRotation = transform.rotation.eulerAngles;
				var destPosition = RotatePointAroundPivot (transform.position, pivot, Quaternion.Euler (eulerAngles));
				var destRotation = initRotation + eulerAngles;

				var timeCount = 0.0f;

				while (timeCount < 1.0f) {
						//Using lerp?
						var incrementAngles = Vector3.Lerp (initRotation, destRotation, timeCount);
						transform.position = RotatePointAroundPivot (initPosition, pivot, Quaternion.Euler (incrementAngles - initRotation));
						transform.rotation = Quaternion.Euler (incrementAngles);
						yield return 0;

						timeCount += Time.deltaTime * rotationSpeed;
				}
				transform.position = destPosition;
				transform.rotation = Quaternion.Euler (destRotation);
		}
	
		static Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Quaternion angle)
		{
				return angle * (point - pivot) + pivot;
		}
	#endregion
}
