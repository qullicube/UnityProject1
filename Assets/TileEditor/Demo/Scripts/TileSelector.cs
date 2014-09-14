using UnityEngine;
using System.Collections;

public class TileSelector : MonoBehaviour
{
	#region Inspector Variables
	
	public GameCamera playerCamera;

	#endregion
	
	#region Hidden Variables

	Color red = new Color(1, 0, 0, 0.8f);
	Color blue = new Color(0, 0, 1, 0.8f);

	#endregion
	
	#region Public Functions
	
	/* Move relatively to the current tile position on a TileMap */
	public void MoveBy(TileMap map, Vector3 position, Vector3 offset = new Vector3(0, 0.01f, 0))
	{
		var currentPos = this.transform.position;
		currentPos.y = 0;
		MoveTo(map, currentPos, offset);
	}
	
	/* Move to an absolute tile position on a TileMap */
	public void MoveTo(TileMap map, Vector3 position, Vector3 offset = new Vector3(0, 0.01f, 0))
	{
		var tile = map.GetPathTile(position);
		if (tile != null)
			transform.position = tile.positionTop + offset;
	}
	
	/* Move to a point */
	public void MoveTo(Vector3 position)
	{
		transform.position = position;
	}
	
	/* Change tile highlighting color */
	public void SetColor(Color rgba)
	{
		this.renderer.sharedMaterial.SetColor("_MainColor", rgba);
	}
	
	public Vector3 GetTilePosition()
	{
		
	}
	
	#endregion
	
	#region Start
	
	void Start()
	{
		this.renderer.sharedMaterial.color = red;
		GotoTile(new Vector3(0, 0, -4));
	}
	
	#endregion
	
	#region Update
	
	//May not even need..
	void Update()
	{
		
		/*if (player.IsBusy ()) {
						this.renderer.sharedMaterial.color = blue;
						return;
				}*/

		/*
		time += Time.deltaTime;

		var x = Input.GetAxis("Horizontal");
		var z = Input.GetAxis("Vertical");

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

		Orientation(ref x, ref z);

		if ((Mathf.Abs(x) < 0.01f || Mathf.Abs(z) < 0.01f) && threshold - time < 0.0f)
		{
			MoveToTile(tileMap, transform.position + new Vector3(x, -transform.position.y, z));
			time = 0;
		}
		*/
	}
	
	#endregion
	
	#region My Functions
	
	[Obsolete("Please use MoveTo instead")]
	void GotoTile(TileMap map, Vector3 position)
	{
		var tile = map.GetPathTile(position);
		if (tile != null)
			transform.position = tile.positionTop + new Vector3(0, 0.01f, 0);
	}
	
	[Obsolete("Moved to GameSystem class")]
	void Orientation(ref float x, ref float z)
	{
		if (playerCamera != null)
		{
			var orientation = playerCamera.Orientation;
			var t = 0.0f;

			switch (orientation)
			{
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
	#endregion
}
