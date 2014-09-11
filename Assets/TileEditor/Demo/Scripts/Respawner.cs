using UnityEngine;
using System.Collections;

public class Respawner : MonoBehaviour {

	public TileMap tileMap;
	public float deathHeight;
	public float spawnHeight;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (transform.position.y < -30)
		{
			var randomTile = tileMap.instances[Random.Range(0,tileMap.instances.Count-1)];
			transform.position = randomTile.position + new Vector3(0, 20, 0);
		}
	}
}
