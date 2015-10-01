using UnityEngine;
using System.Collections;

public class InteractManager : Photon.MonoBehaviour
{
	private GameManager gameManager;
	private TileManager tileManager;
	private ArchManager archManager;
	private GameObject selectedUnit;
	private bool myTurn;
	private bool isActing;
	public int teamId;
	private bool checkTurn;

	void Awake()
	{
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		tileManager = GameObject.Find ("TileManager").GetComponent<TileManager>();
		archManager = GameObject.Find ("ArchManager").GetComponent<ArchManager>();
	}

	void Update ()
	{
		if (!checkTurn && gameManager.GetEnergy(teamId) == 0 && gameManager.IsMyTurn(teamId) && !gameManager.IsActing())
		{
			checkTurn = true;
			gameManager.NextTurn();
		}
		if (checkTurn && !gameManager.IsMyTurn(teamId))
		{
			checkTurn = false;
		}

		CheckForSelected();
		CheckAction();

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Select(hit.collider);
			}
		}
	}
	
	// Attempt to select something
	private void Select(Collider col)
	{
		UnitAttributes attr = col.GetComponent<UnitAttributes>();

		if (attr != null && attr.isActive)
		{
			// If selecting same unit, do nothing
			if (selectedUnit == col.gameObject)
				return;

			// Un-select current selected unit first
			if (selectedUnit != null)
			{
				UnitAttributes currAttr = selectedUnit.GetComponent<UnitAttributes>();
				currAttr.isSelected = false;
				currAttr.friendlyRing.SetActive(false);
				currAttr.hostileRing.SetActive(false);
			}
			selectedUnit = col.gameObject;
			tileManager.TurnOffTiles();

			// If clicked on friendly unit
			if (attr.teamId == teamId)
			{
				attr.isSelected = true;
				attr.friendlyRing.SetActive(true);

				// Show movement options only during the player's own turn and only if not waiting for an action
				if (gameManager.IsMyTurn(teamId) && !gameManager.IsActing())
				{
					attr.GetComponent<UnitMovement>().ToggleMove();
				}
			}
			// If clicked on enemy unit
			else
			{
				attr.hostileRing.SetActive(true);
				tileManager.TurnOffTiles();
			}
		}
	}
	
	private void CheckForSelected()
	{	
		// Call ToggleMove() if any of my units are selected
		if (!myTurn && gameManager.IsMyTurn(teamId))
		{
			// It is now my turn
			myTurn = true;

			if (selectedUnit != null && selectedUnit.GetComponent<UnitAttributes>().teamId == teamId)
			{
				selectedUnit.GetComponent<UnitMovement>().ToggleMove();
			}
		}
		else if (myTurn && !gameManager.IsMyTurn(teamId))
		{
			// It is no longer my turn
			myTurn = false;
		}
	}

	private void CheckAction()
	{
		if (isActing && !gameManager.IsActing() && gameManager.IsMyTurn(teamId))
		{
			isActing = false;

			if (selectedUnit != null && selectedUnit.GetComponent<UnitAttributes>().teamId == teamId)
			{
				selectedUnit.GetComponent<UnitMovement>().ToggleMove();
			}
		}
		else if (!isActing && gameManager.IsActing())
		{
			isActing = true;
		}
	}
}
