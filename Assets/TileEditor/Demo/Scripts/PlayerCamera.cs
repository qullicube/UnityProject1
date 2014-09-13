using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

	public GameObject target;
	public float tileSpeed = 10.0f;
	public float rotationSpeed = 10.0f;
	public float zoomSpeed = 10.0f;
	public Vector3 center = new Vector3(0, 0, 0);
	public int orientation = 0;
	public int zoomLevel = 0;
	public int tiltLevel = 0;

	const float SPEED_THRESHOLD = 20.0f;

	bool dirty = false;

	// Use this for initialization
	void Start()
	{
	}

	void OnGUI()
	{
		/*if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80, 100, 80), ">>"))
		{
			StopAllCoroutines();
			StartCoroutine(Rotate(target.transform.position, new Vector3(0, -90.0f, 0)));
		}
		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 80, 100, 80), "<<"))
		{
			StopAllCoroutines();
			StartCoroutine(Rotate(target.transform.position, new Vector3(0, 90.0f, 0)));
		}*/
	}

	void OnInput()
	{
		if (Input.GetButtonDown("L1"))
		{
			StopAllCoroutines();
			StartCoroutine(Rotate(target.transform.position, new Vector3(0, 90.0f, 0)));
			orientation = (orientation + 1) % 4;

		}

		if (Input.GetButtonDown("R1"))
		{
			StopAllCoroutines();
			StartCoroutine(Rotate(target.transform.position, new Vector3(0, -90.0f, 0)));
			orientation--;
			if (orientation < 0)
				orientation += 4;

		}

		if (Input.GetButtonDown("Zoom"))
		{
			StopAllCoroutines();
			zoomLevel = ++zoomLevel % 3;
			StartCoroutine(Zoom(3 + zoomLevel * 2));

		}

		if (Input.GetButtonDown("Tilt"))
		{
			StopAllCoroutines();
			tiltLevel++;
			 
			if (tiltLevel > 3)
			{
				StartCoroutine(Tilt(target.transform.position, -30.0f));
				tiltLevel = 0;
			}
			else
				StartCoroutine(Tilt(target.transform.position, 10.0f));

		}
	}
	
	// Update is called once per frame
	void Update()
	{
		OnInput();

		if (target != null)
		{
			if (tileSpeed > SPEED_THRESHOLD)
				tileSpeed = SPEED_THRESHOLD;
			if (tileSpeed <= 0.0f)
				tileSpeed = 0.0f;

			var position = camera.WorldToViewportPoint(target.transform.position + center);

			//TODO: prevent error accumulation?
			camera.transform.position -= (0.5f - position.x) * camera.transform.right * tileSpeed * camera.orthographicSize * Time.deltaTime;
			camera.transform.position -= (0.5f - position.y) * camera.transform.up * tileSpeed * camera.orthographicSize * Time.deltaTime;

			//if (Mathf.Abs(0.5f - position.x) < 0.01f && Mathf.Abs(0.5f - position.y) < 0.01f)
			//	dirty = false;
		}
	}

	IEnumerator Zoom(float orthoSize)
	{
		var initSize = camera.orthographicSize;
		var timeCount = 0.0f;

		while (timeCount < 1.0f)
		{
			camera.orthographicSize = (orthoSize - initSize) * timeCount + initSize;
			yield return 0;
			timeCount += Time.deltaTime * zoomSpeed;
		}
		camera.orthographicSize = orthoSize;
	}

	//TODO: optimize this
	IEnumerator Tilt(Vector3 pivot, float angleX)
	{
		var eulerAngles = new Vector3(angleX, 0, 0);
		var initPosition = transform.position;
		var initRotation = transform.rotation;
		var destRotation = initRotation * Quaternion.Euler(eulerAngles);
		var destPosition = RotatePointAroundPivot(initPosition, pivot, Quaternion.AngleAxis(angleX, transform.right));

		var timeCount = 0.0f; 
		var accAngles = Vector3.zero;
		var accRotation = initRotation;

		while (timeCount <= 1.0f)
		{
			var incrementAngle = eulerAngles * Time.deltaTime * rotationSpeed;
			accAngles += incrementAngle;
			accRotation *= Quaternion.Euler(incrementAngle);

			transform.rotation = accRotation;
			transform.position = RotatePointAroundPivot(initPosition, pivot, Quaternion.AngleAxis(accAngles.x, transform.right));
			yield return 0;

			timeCount += Time.deltaTime * rotationSpeed;
		}
		transform.position = destPosition;
		transform.rotation = destRotation;
	}

	IEnumerator Rotate(Vector3 pivot, Vector3 eulerAngles)
	{
		var initPosition = transform.position;
		var initRotation = transform.rotation.eulerAngles;
		var destPosition = RotatePointAroundPivot(transform.position, pivot, Quaternion.Euler(eulerAngles));
		var destRotation = initRotation + eulerAngles;

		var timeCount = 0.0f;

		while (timeCount < 1.0f)
		{
			//Using lerp?
			var incrementAngles = Vector3.Lerp(initRotation, destRotation, timeCount);
			transform.position = RotatePointAroundPivot(initPosition, pivot, Quaternion.Euler(incrementAngles - initRotation));
			transform.rotation = Quaternion.Euler(incrementAngles);
			yield return 0;

			timeCount += Time.deltaTime * rotationSpeed;
		}
		transform.position = destPosition;
		transform.rotation = Quaternion.Euler(destRotation);
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return angle * (point - pivot) + pivot;
	}

}
