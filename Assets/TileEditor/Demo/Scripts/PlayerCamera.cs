using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public GameObject target;
	public float speed = 10.0f;
	public float rotationSpeed = 10.0f;
	public Vector3 center = new Vector3(0, 0, 0);
	public int orientation = 0;

	const float SPEED_THRESHOLD = 20.0f;

	// Use this for initialization
	void Start () {
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
	}
	
	// Update is called once per frame
	void Update () {

		OnInput();

		if (target != null)
		{
			if (speed > SPEED_THRESHOLD) speed = SPEED_THRESHOLD;
			if (speed <= 0.0f) speed = 0.0f;

			var position = camera.WorldToViewportPoint(target.transform.position + center);

			//TODO: prevent error accumulation?
			camera.transform.position -= (0.5f - position.x) * camera.transform.right * speed * camera.orthographicSize * Time.deltaTime;
			camera.transform.position -= (0.5f - position.y) * camera.transform.up * speed * camera.orthographicSize * Time.deltaTime;
		}
	}

	IEnumerator Rotate(Vector3 pivot, Vector3 eulerAngles)
	{
		var initPosition = transform.position;
		var initRotation = transform.rotation.eulerAngles;
		var destPosition = RotatePointAroundPivot(transform.position, pivot, Quaternion.Euler(eulerAngles));
		var destRotation = eulerAngles + transform.rotation.eulerAngles;

		var timeCount = 0.0f;

		while (timeCount <= 1.0f)
		{
			//Using lerp?
			var incrementAngles = Vector3.Lerp(initRotation, destRotation, timeCount);
			transform.position = RotatePointAroundPivot(initPosition, pivot, Quaternion.Euler(incrementAngles-initRotation));
			transform.rotation = Quaternion.Euler(incrementAngles);
			yield return 0;

			timeCount += Time.deltaTime * rotationSpeed;
		}
		transform.position = destPosition;
		transform.rotation = Quaternion.Euler(destRotation);
	}

	public static Vector3 PositiveEulerAngles(Vector3 angles)
	{
		var eulerAngles = angles;
		while (eulerAngles.x < 0.0f)
			eulerAngles.x += 360.0f;
		while (eulerAngles.y < 0.0f)
			eulerAngles.y += 360.0f;
		while (eulerAngles.z < 0.0f)
			eulerAngles.z += 360.0f;

		return eulerAngles;
	}
	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return angle * (point - pivot) + pivot;
	}

}
