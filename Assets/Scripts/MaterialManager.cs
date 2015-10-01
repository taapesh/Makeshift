using UnityEngine;
using System.Collections;

public class MaterialManager : Photon.MonoBehaviour
{
	private int startingMaterial = 100;
	public int materialA;
	public int materialB;

	void Awake()
	{
		materialA = startingMaterial;
		materialB = startingMaterial;
	}

	public void AddMaterial(int teamId, int amount)
	{
		photonView.RPC("AddMaterialRPC", PhotonTargets.AllBufferedViaServer, teamId, amount);
	}

	[PunRPC]
	private void AddMaterialRPC(int teamId, int amount)
	{
		if (teamId == 0)
		{
			materialA += amount;
		}
		else
		{
			materialB += amount;
		}
	}

	public void SubtractMaterial(int teamId, int amount)
	{
		photonView.RPC("SubtractMaterialRPC", PhotonTargets.AllBufferedViaServer, teamId, amount);
	}

	[PunRPC]
	private void SubtractMaterialRPC(int teamId, int amount)
	{
		if (teamId == 0)
		{
			materialA -= amount;
		}
		else
		{
			materialB -= amount;
		}
	}

	public int GetMaterial(int teamId)
	{
		return (teamId == 0) ? materialA : materialB;
	}
}
