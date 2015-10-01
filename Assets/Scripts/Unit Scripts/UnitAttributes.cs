using UnityEngine;
using System.Collections;

public class UnitAttributes : MonoBehaviour
{
	public TileManager tileManager;
	public GameManager gameManager;
	public ArchManager archManager;
	public MaterialManager materialManager;

	public string	unitName;
	public string	unitType;
	public int 		teamId;
	public int 		unitId;
	private bool 	hasMoved;
	private bool 	hasAttacked;
	public bool 	isSelected;
	public int 		tileOccupied;
	public bool 	isActive;
	public bool		isDead;
	public bool		queuedAction;
	public Transform	hitPoint;	// Where projectiles should be aimed at

	public GameObject friendlyRing;
	public GameObject hostileRing;

	// Gameplay attributes
	public int materialCost;
	public int health;
	public int maxHealth;
	public int moveSpeed;
	public int moveCost;

	void Awake()
	{
		archManager = GameObject.Find ("ArchManager").GetComponent<ArchManager>();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		tileManager = GameObject.Find ("TileManager").GetComponent<TileManager>();
		materialManager = GameObject.Find ("MaterialManager").GetComponent<MaterialManager>();
	}

	public void ResetUnit()
	{
		hasMoved = false;
		hasAttacked = false;
	}

	public void SetMoved(bool set)
	{
		hasMoved = set;
	}

	public void SetAttacked(bool set)
	{
		hasAttacked = set;
	}

	public bool HasMoved()
	{
		return hasMoved;
	}

	public bool HasAttacked()
	{
		return hasAttacked;
	}

	public Transform GetTileOccupied()
	{
		return tileManager.tileArray[tileOccupied];
	}
}
