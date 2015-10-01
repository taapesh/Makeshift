using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour
{
	// Components
	private ArchManager archManager;
	private TileManager tileManager;

	// Source
	private GameObject sourceA;
	private GameObject sourceB;
	private SourceAttributes sourceAttrA;
	private SourceAttributes sourceAttrB;

	// Roots
	private GameObject topRootA;
	private GameObject topRootB;
	private GameObject bottomRootA;
	private GameObject bottomRootB;

	private RootAttributes topRootAttrA;
	private RootAttributes topRootAttrB;
	private RootAttributes bottomRootAttrA;
	private RootAttributes bottomRootAttrB;
	
	public int archSpawnA;	// Tile to spawn ArchitectA on
	public int archSpawnB;	// Tile to spawn ArchitectB on

	private int turnId;
	private int maxEnergy = 3;
	private int energyA;
	private int	energyB;
	private int numActingA;
	private int numActingB;

	private int localActingA;
	private int localActingB;

	void Awake()
	{
		archManager = GameObject.Find ("ArchManager").GetComponent<ArchManager>();
		tileManager = GameObject.Find ("TileManager").GetComponent<TileManager>();

		sourceA = GameObject.Find ("SourceA");
		sourceB = GameObject.Find ("SourceB");
		topRootA = GameObject.Find ("TopRootA");
		topRootB = GameObject.Find ("TopRootB");
		bottomRootA = GameObject.Find ("BottomRootA");
		bottomRootB = GameObject.Find ("BottomRootB");

		sourceAttrA = sourceA.GetComponent<SourceAttributes>();
		sourceAttrB = sourceB.GetComponent<SourceAttributes>();
		topRootAttrA = topRootA.GetComponent<RootAttributes>();
		topRootAttrB = topRootB.GetComponent<RootAttributes>();
		bottomRootAttrA = bottomRootA.GetComponent<RootAttributes>();
		bottomRootAttrB = bottomRootB.GetComponent<RootAttributes>();

		energyA = maxEnergy;
		energyB = maxEnergy;
	}

	void Update()
	{

	}

	public void SetupArchitect(int viewId)
	{
		photonView.RPC("SetupArchitectRPC", PhotonTargets.AllBufferedViaServer, viewId);
	}

	[PunRPC]
	public void SetupArchitectRPC(int viewId)
	{
		// Setup both Architect's before game starts
		GameObject archUnit = PhotonView.Find(viewId).gameObject;
		UnitAttributes attr = archUnit.GetComponent<UnitAttributes>();
		archManager.SetUnit(attr.teamId, 0, archUnit);
		attr.unitName = "Architect";
		attr.unitType = "Architect";

		int spawn = (attr.teamId == 0) ? archSpawnA : archSpawnB;
		tileManager.SetOccupancy(spawn, archUnit);
		attr.tileOccupied = spawn;

		attr.isActive = true;
	}

	public void UpdateEnergy(int teamId, int cost)
	{
		photonView.RPC("UpdateEnergyRPC", PhotonTargets.AllBufferedViaServer, teamId, cost);
	}

	[PunRPC]
	public void UpdateEnergyRPC(int teamId, int cost)
	{
		// Update a player's energy
		if (teamId == 0)
		{
			//numActingA += 1;
			energyA -= cost;
		}
		else
		{
			//numActingB += 1;
			energyB -= cost;
		}
	}

	public int GetEnergy(int teamId)
	{
		return (teamId == 0) ? energyA : energyB;
	}

	public void NextTurn()
	{
		photonView.RPC("NextTurnRPC", PhotonTargets.AllBufferedViaServer);
	}

	[PunRPC]
	public void NextTurnRPC()
	{
		// Move the game to the next turn
		if (turnId == 0)
		{
			ResetUnits(1);
			energyB = maxEnergy;
			turnId = 1;
		}
		else
		{
			ResetUnits(0);
			energyA = maxEnergy;
			turnId = 0;
		}
	}

	private void ResetUnits(int teamId)
	{
		GameObject[] unitArray = (teamId == 0) ? archManager.unitArrayA : archManager.unitArrayB;
		foreach(GameObject unit in unitArray)
		{
			if (unit == null)
				continue;
			
			UnitAttributes attr = unit.GetComponent<UnitAttributes>();
			attr.ResetUnit();

			if (attr.unitType == "Architect")
			{
				unit.GetComponent<SummonManager>().ResetSummon();
			}
		}
	}

	public bool IsActing()
	{
		return (localActingA != 0 || localActingB != 0);
	}

	public void IncrActing(int teamId)
	{
		if (teamId == 0)
			localActingA += 1;
		else
			localActingB += 1;
	}
	public void DecrActing(int teamId)
	{
		if (teamId == 0)
			localActingA -= 1;
		else
			localActingB -= 1;
	}

	public bool IsMyTurn(int teamId)
	{
		return (teamId == turnId);
	}

	public void DamageSource(int teamId, int amount)
	{
		photonView.RPC("DamageSource", PhotonTargets.AllBufferedViaServer, teamId);
	}

	[PunRPC]
	private void DamageSourceRPC(int teamId, int amount)
	{
		if (teamId == 0)
		{
			if (sourceAttrA.health - amount <= 0)
			{
				sourceAttrA.health = 0;
			}
			else
			{
				sourceAttrA.health -= amount;
			}
		}
		else
		{
			if (sourceAttrB.health - amount <= 0)
			{
				sourceAttrB.health = 0;
			}
			else
			{
				sourceAttrB.health -= amount;
			}
		}
	}

	public void DamageRoot(int teamId, int rootId, int amount)
	{
		photonView.RPC("DamageRootRPC", PhotonTargets.AllBufferedViaServer, teamId, rootId, amount);
	}

	[PunRPC]
	private void DamageRootRPC(int teamId, int rootId, int amount)
	{
		if (teamId == 0)
		{
			if (rootId == 0)
			{
				topRootAttrA.health -= amount;
			}
			else
			{
				bottomRootAttrA.health -= amount;
			}
		}
		else
		{
			if (rootId == 0)
			{
				topRootAttrB.health -= amount;
			}
			else
			{
				bottomRootAttrB.health -= amount;
			}
		}
	}
}
