using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
	private LayerMask mask = -1;
	private AnimationManager animationManager;

	private UnitAttributes 	attr;
	private float 			remainingDistance;
	private NavMeshAgent 	navComponent;
	private Transform[] 	tilesInRange;
	public GameObject 		groundClickEffect;

	void Awake()
	{
		navComponent = GetComponent<NavMeshAgent>();
		attr = GetComponent<UnitAttributes>();
		animationManager = GetComponent<AnimationManager>();
	}

	void Update()
	{
		if (!CanMove())
		{
			return;
		}

		if (Input.GetMouseButtonDown(1))
		{
			if (attr.queuedAction)
			{
				return;
			}

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
			{
				if (CanMoveHere(hit.collider))
				{
					MoveToTile(hit.collider);
				}
			}
		}
	}

	void MoveToTile(Collider col)
	{
		attr.gameManager.IncrActing(attr.teamId);
		attr.gameManager.UpdateEnergy(attr.teamId, attr.moveCost);
		attr.tileManager.TurnOffTiles();
		attr.SetMoved(true);
		tilesInRange = null;

		attr.tileManager.ChangeTileOccupancy(attr.teamId, attr.unitId, col.GetComponent<TileAttributes>().tileId);

		// Create ground click effect
		//Instantiate(groundClickEffect, col.transform.position, Quaternion.identity);

		Vector3 targetPos = new Vector3(col.transform.position.x, 0, col.transform.position.z);
		navComponent.SetDestination(targetPos);
		animationManager.MoveAnimation(targetPos);
	}

	public void ToggleMove()
	{
		if (!CanMove())
			return;

		tilesInRange = attr.tileManager.GetTilesInRange(attr.tileOccupied, attr.moveSpeed);

		if (tilesInRange.Length > 0)
		{
			attr.tileManager.TurnOnTiles(tilesInRange, attr.tileManager.activeMaterial);
		}
	}

	private bool CanMoveHere(Collider col)
	{
		return (col.GetComponent<TileAttributes>() != null && attr.tileManager.ValidTile(col.transform, tilesInRange));
	}

	private bool CanMove()
	{
		return (
			attr.isSelected && 
	        !attr.gameManager.IsActing() &&
	        !attr.HasMoved() &&
	        attr.gameManager.IsMyTurn(attr.teamId) &&
	        !attr.HasAttacked());
	}
}
