using UnityEngine;
using System.Collections;

public class TileAttributes : MonoBehaviour {
	
	public int tileId;					// tile index into tileArray
	private GameObject unitOccupying;	// Unit occupying this tile
	public int distanceTo;				// Distance to this tile from current tile

	public GameObject GetUnitOccupying()
	{
		return unitOccupying;
	}

	public void SetUnitOccupying(GameObject unit)
	{
		unitOccupying = unit;
	}
}
