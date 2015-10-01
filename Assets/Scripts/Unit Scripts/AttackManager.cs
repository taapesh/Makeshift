using UnityEngine;
using System.Collections;

public class AbilityManager : MonoBehaviour
{
	public KeyCode attackKeybind;

	// Components
	private UnitAttributes attr;
	private Ability ability;

	// Gameplay stats
	public int energyCost;
	public int damage;

	void Awake()
	{
		attr = GetComponent<UnitAttributes>();
		ability = GetComponent<Ability>();
	}

	void Update()
	{
		if (!attr.isSelected || !attr.gameManager.IsMyTurn(attr.teamId))
		{
			return;
		}

		if (Input.GetKeyDown(attackKeybind))
		{
			if (CanAttack())
			{
				ability.ToggleAbility();
			}
		}
	}

	private bool CanAttack()
	{
		if (attr.gameManager.GetEnergy(attr.teamId) < energyCost)
		{
			// Not enough energy
			return false;
		}
		else if (attr.HasAttacked())
		{
			// Already attacked this turn
			return false;
		}
		return true;
	}
}
