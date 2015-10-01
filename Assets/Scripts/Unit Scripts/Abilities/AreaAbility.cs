using UnityEngine;
using System.Collections;

public class AreaAbility : Ability
{
	private Transform[] tilesInRange;
	private Transform hoverTile;
	private bool allTilesOff;
	public Transform hitTile;

	void Update()
	{
		if (attackReady)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
			{
				CheckForTileHover(hit.collider);
			}

			if (Input.GetMouseButtonDown(0))
			{
				Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit _hit = new RaycastHit();
				
				if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, mask.value))
				{
					// Check for valid hit
					Transform tile = CheckForTileHit(hit.collider);
					if (tile != null)
					{
						hitTile = tile;
						InitiateAbility();
					}
					else
					{
						// Cannot use ability there
						CancelAbility();
					}
				}
			}

			// Right click to cancel summon
			if (Input.GetMouseButtonDown(1))
			{
				CancelAbility();
			}
		}
	}

	public override void ToggleAbility()
	{
		tilesInRange = attr.tileManager.GetTilesInRange(attr.tileOccupied, abilityRange, true, true);
		ToggleBase();
	}

	private void CheckForTileHover(Collider col)
	{
		if (col.tag == "tile")
		{
			if (attr.tileManager.ValidTile(col.transform, tilesInRange))
			{
				if (col.transform != hoverTile)
					SetHoverTile(col.transform);
			}
			else
			{
				DeactivateTiles();
			}
		}
		else if (col.tag == "unit")
		{
			Transform tile = col.GetComponent<UnitAttributes>().GetTileOccupied();
			if (attr.tileManager.ValidTile(col.transform, tilesInRange))
			{
				if (tile != hoverTile)
					SetHoverTile(tile);
			}
			else
			{
				DeactivateTiles();
			}
		}
		else
		{
			DeactivateTiles();
		}
	}

	private void DeactivateTiles()
	{
		if (!allTilesOff)
		{
			attr.tileManager.TurnOffTiles();
			allTilesOff = true;
		}
	}

	private void SetHoverTile(Transform tile)
	{
		hoverTile = tile;
		Transform[] tiles = attr.tileManager.GetTilesInRange(
			hoverTile.GetComponent<TileAttributes>().tileId, abilityRange, true, true);

		attr.tileManager.TurnOnTiles(tiles, attr.tileManager.activeMaterial);
		allTilesOff = false;
	}

	// If we hit a tile and it is valid, return the tile
	// If we did not hit a tile or the tile is not valid, return null
	private Transform CheckForTileHit(Collider col)
	{
		if (col.tag == "tile")
		{
			return attr.tileManager.ValidTile(col.transform, tilesInRange) ? col.transform : null;
		}
		else if (col.tag == "unit")
		{
			Transform unitTile = col.GetComponent<UnitAttributes>().GetTileOccupied();
			return attr.tileManager.ValidTile(unitTile, tilesInRange) ? unitTile : null;
		}
		return null;
	}
}
