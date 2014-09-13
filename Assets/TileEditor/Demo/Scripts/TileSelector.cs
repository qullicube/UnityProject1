using UnityEngine;
using System.Collections;

public class TileSelector : MonoBehaviour
{

		public TileMap tileMap;
		public Player player;
		public PlayerCamera playerCamera;
		public float threshold = 0.2f;

		float time;
		Color red = new Color (1, 0, 0, 0.7f);
		Color blue = new Color (0, 0, 1, 0.7f);

		// Use this for initialization
		void Start ()
		{
				GotoTile (new Vector3 (0, 0, -4));
				time = 0;
		}
	
		// Update is called once per frame
		void Update ()
		{
		
				/*if (player.IsBusy ()) {
						this.renderer.sharedMaterial.color = blue;
						return;
				}*/

				this.renderer.sharedMaterial.color = red;
		
				if (Input.GetButtonDown ("Fire1")) {
						player.MoveTo (transform.position);
				}

				time += Time.deltaTime;

				var x = Input.GetAxis ("Horizontal");
				var z = Input.GetAxis ("Vertical");

				var factor = 0.2f;

				if (x > factor)
						x = 1;
				else if (x < -factor)
						x = -1;
				else
						x = 0;

				if (z > factor)
						z = 1;
				else if (z < -factor)
						z = -1;
				else
						z = 0;

				Orientation (ref x, ref z);

				if ((x != 0 || z != 0) && threshold - time < 0.0f) {
						GotoTile (transform.position + new Vector3 (x, -transform.position.y, z));
						time = 0;
				}
		}

		void GotoTile (Vector3 position)
		{
				var tile = tileMap.GetPathTile (position);
				if (tile != null)
						transform.position = tile.positionTop + new Vector3 (0, 0.01f, 0);
		}

		void Orientation (ref float x, ref float z)
		{
				if (playerCamera != null) {
						var orientation = playerCamera.orientation;
						var t = 0.0f;

						switch (orientation) {
						case 0:
								t = x;
								x = z;
								z = -t;
								break;
						case 1:
								z = -z;
								x = -x;
								break;
						case 2:
								t = z;
								z = x;
								x = -t;
								break;
						case 3:
								break;
						default:
								break;
						}
				}
		}
}
