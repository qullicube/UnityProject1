using UnityEngine;
using System;
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
	public void MoveBy(TileMap map, Vector3 position, Vector3 offset)
	{
		var currentPos = this.transform.position;
		currentPos.y = 0;
		MoveTo(map, currentPos, offset);
	}
	
	/* Move to an absolute tile position on a TileMap */
	public void MoveTo(TileMap map, Vector3 position, Vector3 offset)
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
