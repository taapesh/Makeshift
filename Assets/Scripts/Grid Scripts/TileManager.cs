using UnityEngine;
using System.Collections;

public class TileManager : Photon.MonoBehaviour
{
	private const int 	TilesInRow = 23;
	private ArchManager archManager;
	private int 		lenTileArray;

	public Transform[] 	tileArray;
	public ArrayList 	activeTiles = new ArrayList();
	public Material 	originalMaterial;
	public Material 	activeMaterial;
	public Material		summonMaterial;
	
	void Awake()
	{
		archManager = GameObject.Find ("ArchManager").GetComponent<ArchManager>();

		// Only needed once; copy and paste component values onto the script in final version
		int i = 0;
		foreach (Transform child in gameObject.transform)
		{
			tileArray[i] = child;
			i++;
		}

		lenTileArray = tileArray.Length;
	}

	// For determining hex tiles in range of movement
	public Transform[] GetTilesInRange(int id, int range, bool includeCenter=false, bool includeBlocked=false) 
	{
		ArrayList availableTiles = new ArrayList();
		
		if (includeCenter) 
			availableTiles.Add(id);
		
		// Get first level of tiles and add them to tile id array
		ArrayList ring = GetTileNeighbors(id);
		
		foreach(int tile in ring) 
		{
			availableTiles.Add(tile);
			tileArray[tile].GetComponent<TileAttributes>().distanceTo = 1;
		}
		
		// Process each ring in the range (e.g. range of 3 results in 3 rings total)
		for (int i = 2; i <= range; i++) 
		{
			ArrayList newRing = new ArrayList();
			
			// Loop through each tile in current ring
			foreach(int tile in ring) 
			{
				// Get the tile's neighbors
				ArrayList neighbors = GetTileNeighbors(tile);
				
				// Loop through neighbors and check for new values to add to new ring
				foreach(int tile2 in neighbors) 
				{
					// If tile is not already in our list, include it
					if (!availableTiles.Contains(tile2) && tile2 != id)
					{
						tileArray[tile2].GetComponent<TileAttributes>().distanceTo = i;	// Update distance to this tile based on current ring
						newRing.Add(tile2);			// Add this tile to new ring
						availableTiles.Add(tile2);	// Add this tile to list of available tiles
					}
				}
			}
			
			// Update current ring
			ring = newRing;
		}
		
		// Get the transforms of each tile from the tileArray
		int numAvailableTiles = availableTiles.Count;
		Transform[] tileTransforms = new Transform[numAvailableTiles];
		
		for(int i = 0; i < numAvailableTiles; ++i) 
			tileTransforms[i] = tileArray[(int) availableTiles[i]];
		
		return tileTransforms;
	}

	// Get all six neighbors of a hex tile
	// If includeBlocked = false, do not include occupied tiles
	// Never include out of range tiles (invalid index)
	private ArrayList GetTileNeighbors(int id, bool includeBlocked=false)
	{
		ArrayList tiles = new ArrayList();
		
		// Add valid neighbors
		int[] neighborIds = new int[] {id+1, id-1, id + TilesInRow, id + TilesInRow + 1, id - TilesInRow, id - TilesInRow - 1};
		foreach(int idx in neighborIds)
		{
			if (idx > 0 && idx < lenTileArray)
				if (includeBlocked || !IsOccupied(idx))
					tiles.Add(idx);
		}
		return tiles;
	}
	
	public void TurnOnTiles(Transform[] tiles, Material material)
	{
		foreach(Transform tile in tiles)
		{
			tile.GetComponent<Renderer>().material = material;
			activeTiles.Add(tile);
		}
	}

	// Deactivate all active tiles
	public void TurnOffTiles() {
		foreach(Transform tile in activeTiles)
		{
			tile.GetComponent<Renderer>().material = originalMaterial;
		}
		activeTiles.Clear();
	}


	public void ChangeTileOccupancy(int teamId, int unitId, int newTileId)
	{
		photonView.RPC("ChangeTileOccupancyRPC", PhotonTargets.AllBufferedViaServer, teamId, unitId, newTileId);
	}

	[PunRPC]
	public void ChangeTileOccupancyRPC(int teamId, int unitId, int newTileId)
	{
		UnitAttributes unitAttr;
		GameObject unit;

		if (teamId == 0)
		{
			unit = archManager.unitArrayA[unitId];
			unitAttr = archManager.unitArrayA[unitId].GetComponent<UnitAttributes>();
		}
		else
		{
			unit = archManager.unitArrayB[unitId];
			unitAttr = archManager.unitArrayB[unitId].GetComponent<UnitAttributes>();
		}

		SetOccupancy(unitAttr.tileOccupied);
		unitAttr.tileOccupied = newTileId;
		SetOccupancy(newTileId, unit);
	}

	public bool ValidTile(Transform tile, Transform[] tiles)
	{
		foreach(Transform _tile in tiles)
		{
			if (tile == _tile)
				return true;
		}
		return false;
	}

	public bool IsOccupied(int tileId)
	{
		return (tileArray[tileId].GetComponent<TileAttributes>().GetUnitOccupying() != null);
	}

	public void SetOccupancy(int tileId, GameObject unit=null)
	{
		tileArray[tileId].GetComponent<TileAttributes>().SetUnitOccupying(unit);
	}

	public ArrayList GetUnitsInRange(int tileId, int range)
	{
		Transform[] tilesInRange = GetTilesInRange(tileId, range, false, true);
		ArrayList units = new ArrayList();

		foreach(Transform tile in tilesInRange)
		{
			GameObject unit = tile.GetComponent<TileAttributes>().GetUnitOccupying();
			if (unit != null)
				units.Add(unit);
		}
		return units;
	}

	public ArrayList GetEnemyUnitsInRange(int tileId, int teamId, int range)
	{
		Transform[] tilesInRange = GetTilesInRange(tileId, range, false, true);
		ArrayList enemyUnits = new ArrayList();
		
		foreach(Transform tile in tilesInRange)
		{
			GameObject unit = tile.GetComponent<TileAttributes>().GetUnitOccupying();
			if (unit != null && unit.GetComponent<UnitAttributes>().teamId != teamId)
				enemyUnits.Add(unit);
		}
		return enemyUnits;
	}

	public ArrayList GetFriendlyUnitsInRange(int tileId, int teamId, int range)
	{
		Transform[] tilesInRange = GetTilesInRange(tileId, range, false, true);
		ArrayList friendlyUnits = new ArrayList();
		
		foreach(Transform tile in tilesInRange)
		{
			GameObject unit = tile.GetComponent<TileAttributes>().GetUnitOccupying();
			if (unit != null && unit.GetComponent<UnitAttributes>().teamId == teamId)
				friendlyUnits.Add(unit);
		}
		return friendlyUnits;
	}	
}
