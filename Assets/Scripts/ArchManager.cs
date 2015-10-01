using UnityEngine;
using System.Collections;

public class ArchManager : Photon.MonoBehaviour
{
	public GameObject[] unitArrayA;
	public GameObject[] unitArrayB;
	private TileManager tileManager;
	private GameManager gameManager;

	void Awake()
	{
		unitArrayA = new GameObject[100];
		unitArrayB = new GameObject[100];
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		tileManager = GameObject.Find ("TileManager").GetComponent<TileManager>();
	}

	public void SetupUnit(int viewId, int teamId, int tileId)
	{
		photonView.RPC("SetupUnitRPC", PhotonTargets.AllBufferedViaServer, viewId, teamId, tileId);
	}

	[PunRPC]
	private void SetupUnitRPC(int viewId, int teamId, int tileId)
	{
		GameObject unit = PhotonView.Find(viewId).gameObject;
		UnitAttributes attr = unit.GetComponent<UnitAttributes>();

		attr.teamId = teamId;
		attr.unitId = FirstNull(teamId);
		attr.tileOccupied = tileId;
		tileManager.SetOccupancy(tileId, unit);
		SetUnit(teamId, attr.unitId, unit);

		// Activate this unit
		attr.isActive = true;
	}

	public void SetUnit(int teamId, int unitId, GameObject unit)
	{
		if (teamId == 0)
			unitArrayA[unitId] = unit;
		else
			unitArrayB[unitId] = unit;
	}

	public void RemoveUnit(int teamId, int unitId)
	{
		if (teamId == 0)
			unitArrayA[unitId] = null;
		else
			unitArrayB[unitId] = null;
	}
	
	public void DamageUnit(int teamId, int unitId, int amount)
	{
		photonView.RPC("DamageUnitRPC", PhotonTargets.AllBufferedViaServer, teamId, unitId, amount);
	}

	[PunRPC]
	private void DamageUnitRPC(int teamId, int unitId, int amount)
	{
		UnitAttributes attr = (teamId == 0) ? unitArrayA[unitId].GetComponent<UnitAttributes>() : unitArrayB[unitId].GetComponent<UnitAttributes>();

		if (attr.health - amount <= 0)
		{
			attr.isDead = true;
			attr.health = 0;
			KillUnitRPC(teamId, unitId);
		}
		else
		{
			attr.health -= amount;
		}
	}

	public void KillUnit(int teamId, int unitId)
	{
		photonView.RPC("KillUnitRPC", PhotonTargets.AllBufferedViaServer, teamId, unitId);
	}

	[PunRPC]
	private void KillUnitRPC(int teamId, int unitId)
	{
		GameObject unit = (teamId == 0) ? unitArrayA[unitId] : unitArrayB[unitId];
		UnitAttributes attr = unit.GetComponent<UnitAttributes>();
		tileManager.SetOccupancy(attr.tileOccupied);
		RemoveUnit(teamId, unitId);
	}

	private int FirstNull(int teamId)
	{
		GameObject[] unitArray = (teamId == 0) ? unitArrayA : unitArrayB;
		
		for(int i = 0; i < unitArray.Length; i++)
			if (unitArray[i] == null)
				return i;
		return -1;
	}
}
