using UnityEngine;
using System.Collections;

public class TileHighlightAnimation : MonoBehaviour
{
		public float speed = 1.0f;
		public float step = 0.0f;
		// Use this for initialization
		void Start ()
		{
				step = 0.0f;
		}
	
		// Update is called once per frame
		void Update ()
		{
				this.renderer.material.SetFloat ("_Step", step);
				step += speed * Time.deltaTime;
				if (step >= 1.0f)
						step = 0.0f;
		}
}
