using UnityEngine;
using System.Collections;

public class TileGenerator : MonoBehaviour
{
	private Transform tileParent;	// Object that contains all hex tiles
	public GameObject tilePrefab;	// Hex prefab to instantiate
	public float terrainWidth;		// Width of terrain, make it a multiple of shiftWidth for optimal results
	public float terrainHeight;		// Height of terrain, make it a multiple of shiftHeight for optimal results
	public float borderSize;		// Number of tiles bordering the map

	void Awake()
	{
		tileParent = GameObject.Find ("TileManager").transform;
		Debug.Assert (tileParent != null, "tileParent missing");
	}

	void Start ()
	{
		Debug.Log ("Tile size: " +  tilePrefab.GetComponent<Renderer>().bounds.size);
		Vector3 tileBounds = tilePrefab.GetComponent<Renderer>().bounds.size;
		float shiftWidth = tileBounds.x;
		float shiftHeight = (tileBounds.z / 2.0f) + (tileBounds.z / 4.0f);
		int numTilesGenerated = 0;

		float currWidth = -borderSize * shiftWidth;
		float currHeight = -borderSize * shiftHeight;
		int numRows = (int) (terrainWidth / shiftHeight);
		int tilesInRow = (int) (terrainHeight / shiftWidth) + 1;

		Debug.Log ("Tiles in row: " + tilesInRow);
		int currRow = 0;
		int currTile = 0;
		bool evenRow = true;

		while (currRow <= numRows + borderSize + 1)
		{
			if (evenRow) 
			{
				// Even rows start at 0
				currWidth = -borderSize * shiftWidth;
				currTile = 0;
			}
			else
			{
				// Odd rows start at +<half the width of tile>
				currWidth = (-borderSize * shiftWidth) + (shiftWidth / 2.0f);
				currTile = 1;
			}
			evenRow = !evenRow;

			while (currTile <= tilesInRow + borderSize)
			{
				Vector3 spawn = new Vector3(currWidth, .01f, currHeight);
				GameObject tileInstance = (GameObject) Instantiate(tilePrefab, spawn, Quaternion.identity);
				tileInstance.name = "Tile_" + numTilesGenerated;
				tileInstance.transform.parent = tileParent;
				tileInstance.GetComponent<TileAttributes>().tileId = numTilesGenerated;
				currWidth += shiftWidth;
				numTilesGenerated++;
				currTile++;
			}
			currHeight += shiftHeight;

			currRow++;
		}
		Debug.Log ("Created " + numTilesGenerated + " tiles");
	}
}
