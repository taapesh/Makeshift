using UnityEngine;
using System.Collections;

public class SetupObjects : Photon.MonoBehaviour
{
	public GameObject selectorPrefab;

	public void Setup()
	{
		// Instantiate SelectManager
		GameObject selectManager = (GameObject) Instantiate(selectorPrefab, Vector3.zero, Quaternion.identity);
		selectManager.name = "SelectManager";
		selectManager.GetComponent<InteractManager>().teamId = GetComponent<UnitAttributes>().teamId; 

		// Set attributes for this Architect
		GameObject.Find ("GameManager").GetComponent<GameManager>().SetupArchitect(photonView.viewID);
	}
}