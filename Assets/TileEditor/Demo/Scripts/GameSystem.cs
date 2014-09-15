using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
	BATTLE,
	PAUSE
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

	public GameObject tileHighlightPrefab;
	public List<Transform> walkableTileHighlights = new List<Transform>(100000);
	public List<PathTile> walkableTiles = new List<PathTile>(100000);

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
			
		tileSelector.MoveTo(tileMap, new Vector3(x, 0, z), new Vector3(0, 0.01f, 0));
	}
	public void Selector_MoveBy(int x, int z)
	{
		if (tileSelector == null || tileMap == null)
			return;
		
		Orientation(ref x, ref z);
		tileSelector.MoveBy(tileMap, new Vector3(x, 0, z), new Vector3(0, 0.01f, 0));
	}
	public void Selector_SelectPlayer()
	{
		PathTile tile = tileMap.GetPathTile(tileSelector.transform.position);

		if (tile == null)
		{
			return;
		}

		foreach (var player in players)
		{
			if (Mathf.Abs(player.transform.position.x - tile.transform.position.x) <= 0.01f &&
				Mathf.Abs(player.transform.position.z - tile.transform.position.z) <= 0.01f)
			{
				Selector_DeselectPlayer();
				selectedPlayer = player;
				Player_ShowMovableTile(selectedPlayer);
				break;
			}
		}
	}
	public void Selector_DeselectPlayer()
	{
		Player_HideMovableTile();
		selectedPlayer = null;
	}
	void Orientation(ref int x, ref int z)
	{
		if (gameCamera != null)
		{
			var orientation = gameCamera.Orientation;
			var t = 0;
			
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
	
	#region Selected Player Functions
		
	public void SelectedPlayer_MoveToTileSelector()
	{
		SelectedPlayer_MoveTo(tileSelector.transform.position);
	}
	public void SelectedPlayer_MoveTo(Vector3 position)
	{
		selectedPlayer.MoveTo(tileMap, position);
	}
	public void SelectedPlayer_MoveToSelectedMovable()
	{
		if(walkableTiles.Count <= 0)
			return;

		var moveTo = tileMap.GetPathTile(tileSelector.transform.position);

		if(walkableTiles.IndexOf(moveTo) >= 0)
		{
			selectedPlayer.MoveTo(tileMap, tileSelector.transform.position, walkableTiles, Selector_DeselectPlayer);
		}
	}

	public void Player_ShowMovableTile(Player player)
	{
		Player_HideMovableTile();

		if (FindPlayerMoveableTiles(player, walkableTiles))
		{
			foreach (var path in walkableTiles)
			{
				//should sync index
				GameObject o = (GameObject)Instantiate(tileHighlightPrefab, path.positionTop + new Vector3(0,0.001f,0), tileHighlightPrefab.transform.rotation);
				walkableTileHighlights.Add(o.transform);
			}
		}
	}
	public void Player_HideMovableTile()
	{
		for (int i = 0; i < walkableTileHighlights.Count; i++)
		{
			Destroy(walkableTileHighlights[i].gameObject);
		}
		walkableTileHighlights.Clear();
		walkableTiles.Clear();
	}

				
	#endregion

	#region Player Moveable Tiles Algorithm

	bool FindPlayerMoveableTiles(Player player, List<PathTile> path)
	{
		var pivot = tileMap.GetPathTile(player.transform.position);

		if (pivot == null)
			return false;
		
		path.Add(pivot);

		var movePower = player.walkPower;

		foreach (var connect in pivot.connections)
		{
			if (path.Contains(connect))
				continue;
			var nextMovePower = movePower - (Mathf.Abs(pivot.positionTop.x - connect.positionTop.x) +
											Mathf.Abs(pivot.positionTop.z - connect.positionTop.z));
			FindPlayerMoveableTilesRecursive(connect, path, nextMovePower);
		}

		return true;
	}
	bool FindPlayerMoveableTilesRecursive(PathTile pivot, List<PathTile> path, float movePower)
	{
		if (movePower <= 0.0f)
			return false;

		path.Add(pivot);

		foreach (var connect in pivot.connections)
		{
			if (path.Contains(connect))
				continue;
			var nextMovePower = movePower - 1;

			FindPlayerMoveableTilesRecursive(connect, path, nextMovePower);
		}

		return true;
	}

	#endregion

	#region GUI

	void OnGUI()
	{
		switch (state)
		{
			case GameState.PAUSE:
				{
					var style = new GUIStyle();
					style.fontSize = 26;
					style.normal.textColor = Color.white;

					var texture = new Texture2D(1, 1);
					texture.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
					texture.wrapMode = TextureWrapMode.Repeat;
					texture.Apply();

					GUI.Box(new Rect(0, 0, Screen.width, Screen.height), texture);

					GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();

					GUILayout.Label("Pause", style);

					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.EndArea();

				}
				break;
			case GameState.BATTLE:
				{
					if (selectedPlayer != null)
					{
						var style = new GUIStyle();
						style.fontSize = 20;
						style.normal.textColor = Color.black;

						GUI.Label(new Rect(Screen.width-100, Screen.height-40,100,40), "Selected",style);
					}
				}
				break;
			default:
				break;
		}
	}

	#endregion

	#region Start

	void Start()
	{
		var allPlayersOnScene = GameObject.FindObjectsOfType<Player>();
		players = new List<Player>();
		walkableTileHighlights = new List<Transform>();

		foreach (var player in allPlayersOnScene)
		{
			players.Add(player);
		}

		if (tileSelector != null)
		{
			Selector_MoveTo(0, -5);
		}

		state = GameState.BATTLE;
	}

	#endregion

	#region Update

	void Update()
	{
	}
	
	#endregion

}
