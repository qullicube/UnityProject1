﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathTile : MonoBehaviour
{
	[HideInInspector] public List<PathTile> connections = new List<PathTile>();

	public Vector3 positionTop
	{
		get
		{
			var ray = new Ray(transform.position + Vector3.up * 256.0f, Vector3.down);
			var topPosition = transform.position;
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				topPosition = hit.point;
			}

			return topPosition;
		}
	}

	public Vector3 localPositionTop
	{
		get
		{
			var ray = new Ray(transform.localPosition + transform.up * 256.0f, Vector3.down);
			var topPosition = transform.localPosition;
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				topPosition = hit.point;
			}

			return topPosition;
		}
	}
}