using UnityEngine;
using System.Collections;

public class TargetedAbility : Ability
{
	// Projectile settings
	public bool 		isProjectile;
	public float 		fireProjectileAfter;
	public Transform 	projSpawn;		// Where to instantiate the projectile
	public GameObject 	projPrefab;
	private string		projPrefabName;

	private ArrayList 	unitsInRange;
	private GameObject 	hoverUnit;
	public GameObject 	hitUnit;
	private bool 	hoverSet;
	public bool 	friendlyAbility;

	void Awake()
	{
		if (projPrefab != null)
			projPrefabName = projPrefab.name;
	}

	void Update()
	{
		if (attackReady)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
			{
				CheckForUnitHover(hit.collider);
			}

			if (Input.GetMouseButtonDown(0))
			{
				Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit _hit = new RaycastHit();
				
				if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, mask.value))
				{
					// Check for valid unit hit
					GameObject unit = CheckForUnitHit(hit.collider);
					if (unit != null)
					{
						hitUnit = unit;

						if (!isProjectile)
						{
							InitiateAbility();
						}
						else
						{
							InitiateProjectile();
						}
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
		if (friendlyAbility)
			unitsInRange = attr.tileManager.GetFriendlyUnitsInRange(attr.tileOccupied, attr.teamId, abilityRange);
		else
			unitsInRange = attr.tileManager.GetEnemyUnitsInRange(attr.tileOccupied, attr.teamId, abilityRange);

		ToggleBase();
	}

	private void SetHoverUnit(GameObject unit)
	{ 
		hoverUnit = unit;
		hoverSet = true;
		// Apply whatever visual effect
	}

	private void CheckForUnitHover(Collider col)
	{
		if (col.tag == "tile")
		{
			// Get unit occupying if exists
			GameObject unit = col.transform.GetComponent<TileAttributes>().GetUnitOccupying();
			if (unit != null && unitsInRange.Contains(unit))
			{
				if (unit != hoverUnit)
					SetHoverUnit(unit);
			}
			else if (hoverSet)
				UnsetHover();
		}
		else if (col.tag == "unit")
		{
			if (unitsInRange.Contains(col.gameObject))
			{
				if (col.gameObject != hoverUnit)
					SetHoverUnit(col.gameObject);
			}
			else if (hoverSet)
				UnsetHover();
		}
		else if (hoverSet)
		{
			// Not hovering over anything valid
			UnsetHover();
		}
	}

	private GameObject CheckForUnitHit(Collider col)
	{
		if (col.tag == "tile")
		{
			// Get unit occupying if exists
			GameObject unit = col.transform.GetComponent<TileAttributes>().GetUnitOccupying();
			if (unit != null && unitsInRange.Contains(unit))
			{
				return unit;
			}
			else
			{
				return null;
			}
		}
		else if (col.tag == "unit")
		{
			if (unitsInRange.Contains(col.gameObject))
			{
				return col.gameObject;
			}
			else
			{
				return null;
			}
		}
		return null;
	}

	private void UnsetHover()
	{
		hoverUnit = null;
		hoverSet = false;
	}

	private void InitiateProjectile()
	{
		attackReady = false;
		attr.queuedAction = false;
		attr.gameManager.UpdateEnergy(attr.teamId, energyCost);
		animationManager.AbilityAnimation();
		attr.tileManager.TurnOffTiles();
		Invoke("FireProjectile", fireProjectileAfter);
	}

	private void FireProjectile()
	{
		Projectile proj = PhotonNetwork.Instantiate(
			projPrefabName,
			projSpawn.position,
			Quaternion.identity, 0).GetComponent<Projectile>();

		proj.SetAbility(this);
		proj.SetTarget(hitUnit, projSpawn.position);
	}
}
