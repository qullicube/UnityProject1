﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
	PAUSE,
	BATTLE
}

public class GameSystem : MonoBehaviour
{
	#region Inspector Variables
	
	public GameState state;
	public List<Player> players;
	public TileMap tileMap;
	public TileSelector tileSelector;
	public GameCamera gameCamera;
	
	public Player selectedPlayer;
	
	#endregion
	
	#region Camera Functions
	
	public void Camera_Tilt()
	{
		if (gameCamera == null)
			return;
			
		gameCamera.Tilt();
	}
	public void Camera_Zoom()
	{
		if (gameCamera == null)
			return;
		
		gameCamera.Zoom();
	}
	public void Camera_RotateLeft()
	{
		if (gameCamera == null)
			return;
			
		gameCamera.RotateLeft();
	}
	public void Camera_RotateRight()
	{
		if (gameCamera == null)
			return;
		
		gameCamera.RotateRight();
	}
	
	#endregion
	
	#region Tile Selector Functions
	
	public void Selector_MoveTo(int x, int z)
	{
		if (tileSelector == null)
			return;
			
		Orientation(ref x, ref z);
		tileSelector.MoveTo(x, z);
	}
	public void Selector_MoveBy(int x, int z)
	{
		if (tileSelector == null || tile)
			return;
		
		Orientation(ref x, ref z);
		tileSelector.MoveBy(TileMap, x, z);
	}
	public void Selector_SelectPlayer()
	{
		Transform tile = tileMap.GetTile(tileSelector.transform);
		
		foreach (var player in players)
		{
			if (Mathf.Abs(player.transform.position.x - tile.transform.position.x) < Mathf.Epsilon &&
				Mathf.Abs(player.transform.position.z - tile.transform.position.z) < Mathf.Epsilon)
			{
				selectedPlayer = player;	
			}
		}
		selectedPlayer = null;
	}
	
	void Orientation(ref float x, ref float z)
	{
		if (gameCamera != null)
		{
			var orientation = gameCamera.Orientation;
			var t = 0.0f;
			
			//TODO: Change this according to presetting
			
			//This is done to make sure that tile selector consistently move respective to camera orientation
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
	
	#region Update
	
	void Update()
	{
		
	}
	
	#endregion
}
