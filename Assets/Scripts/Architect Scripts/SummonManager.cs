using UnityEngine;
using System.Collections;

public class SummonManager : Photon.MonoBehaviour
{
	private LayerMask mask = -1;
	private UnitAttributes attr;
	private UnitMovement movement;
	private AnimationManager animationManager;
	
	public int summonRange;
	private int energyCost;

	private GameObject	unitInstance;
	private int 		unitToSummon;
	private int			materialCost;
	private string		summonName;
	private int 		summonTile;
	private Vector3 	summonPos;
	private bool 		summonReady;
	private bool 		hasSummoned;
	public GameObject[] summonPrefabs;
	private Transform[] tilesInRange;
	public float summonAfter;

	// Summon keybindings
	public KeyCode createUnit0;
	public KeyCode createUnit1;
	public KeyCode createUnit2;
	public KeyCode createUnit3;
	public KeyCode createUnit4;
	public KeyCode createUnit5;

	void Awake()
	{
		animationManager = GetComponent<AnimationManager>();
		attr = GetComponent<UnitAttributes>();
		movement = GetComponent<UnitMovement>();
		energyCost = 2;
	}

	void Update ()
	{
		if (!CanSummon())
		{
			return;
		}

		if (Input.GetKeyDown(createUnit0))
		{
			if (CanSummonThis(0))
			{
				unitToSummon = 0;
				ToggleSummon();
			}
		}
		if (Input.GetKeyDown(createUnit1))
		{
			if (CanSummonThis(1))
			{
				unitToSummon = 1;
				ToggleSummon();
			}
		}
		if (Input.GetKeyDown(createUnit2))
		{
			if (CanSummonThis(2))
			{
				unitToSummon = 2;
				ToggleSummon();
			}
		}
		if (Input.GetKeyDown(createUnit3))
		{
			if (CanSummonThis(3))
			{
				unitToSummon = 3;
				ToggleSummon();
			}
		}
		if (Input.GetKeyDown(createUnit4))
		{
			if (CanSummonThis(4))
			{
				unitToSummon = 4;
				ToggleSummon();
			}
		}
		if (Input.GetKeyDown(createUnit5))
		{
			if (CanSummonThis(5))
			{
				unitToSummon = 5;
				ToggleSummon();
			}
		}
		
		if (summonReady)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();
				
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
				{
					if (CanSummonHere(hit.collider))
					{
						summonName = summonPrefabs[unitToSummon].name;
						summonTile = hit.collider.GetComponent<TileAttributes>().tileId;
						summonPos = hit.collider.transform.position;
						Summon();
					}
				}
			}
			
			// Right click to cancel summon
			if (Input.GetMouseButtonDown(1))
			{
				attr.tileManager.TurnOffTiles();
				summonReady = false;
				attr.queuedAction = false;
				movement.ToggleMove();
			}
		}
	}

	private void Summon()
	{
		attr.gameManager.IncrActing(attr.teamId);
		attr.gameManager.UpdateEnergy(attr.teamId, energyCost);
		attr.tileManager.TurnOffTiles();
		attr.queuedAction = false;
		hasSummoned = true;
		summonReady = false;
		animationManager.SummonAnimation();
		Invoke("SpawnUnit", summonAfter);
	}

	private void SpawnUnit()
	{
		GameObject unitInstance = PhotonNetwork.Instantiate(summonName, summonPos, Quaternion.identity, 0);
		animationManager.summonedAttr = unitInstance.GetComponent<UnitAttributes>();
		attr.archManager.SetupUnit(unitInstance.GetComponent<PhotonView>().viewID, attr.teamId, summonTile);
	}
	
	public void ToggleSummon()
	{
		if (hasSummoned)
		{
			// Already summoned this turn
			return;
		}

		tilesInRange = attr.tileManager.GetTilesInRange(attr.tileOccupied, summonRange);

		if (tilesInRange.Length > 0)
		{
			summonReady = true;
			attr.queuedAction = true;
			attr.tileManager.TurnOffTiles();
			attr.tileManager.TurnOnTiles(tilesInRange, attr.tileManager.summonMaterial);
		}
		else
		{
			// No tiles available
		}
	}

	// Check if we have can summon this unit
	private bool CanSummonThis(int summonId)
	{
		int materialCost = summonPrefabs[summonId].GetComponent<UnitAttributes>().materialCost;

		bool enoughMaterial = (attr.materialManager.GetMaterial(attr.teamId) >= materialCost);
		bool enoughEnergy = (attr.gameManager.GetEnergy(attr.teamId) >= energyCost);

		if (!enoughEnergy)
		{
			// Not enough energy to summon
			return false;
		}
		else if (!enoughMaterial)
		{
			// Not enough material to summon this unit
			return false;
		}
		return true;
	}
	
	private bool CanSummonHere(Collider col)
	{
		TileAttributes tileAttr = col.GetComponent<TileAttributes>();
		if (tileAttr == null)
		{
			// Not a tile
			return false;
		}

		if (!attr.tileManager.ValidTile(col.transform, tilesInRange))
	    {
			// Not a valid tile
			return false;
		}
		// Valid tile
		return true;
	}

	private bool CanSummon()
	{
		return (attr.isSelected && attr.gameManager.IsMyTurn(attr.teamId) && !attr.gameManager.IsActing() && !hasSummoned);
	}

	public void ResetSummon()
	{
		hasSummoned = false;
	}
}
