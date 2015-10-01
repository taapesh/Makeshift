using UnityEngine;
using System.Collections;

public class CheckTurn : Photon.MonoBehaviour
{
	private GameManager gameManager;
	private UnitAttributes attr;
	public bool checkTurn;

	void Awake()
	{
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		attr = GetComponent<UnitAttributes>();
	}
	
	void Update () 
	{

	}
}
