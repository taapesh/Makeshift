using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour
{
	public LayerMask mask;
	public UnitAttributes attr;
	public UnitMovement movement;
	public AnimationManager animationManager;

	public int energyCost;
	public int abilityRange;
	public bool attackReady;
	
	public float applyEffectAfter;	// Apply the effect of the ability after this many seconds

	void Awake()
	{
		mask = -1;
		attr = GetComponent<UnitAttributes>();
		movement = GetComponent<UnitMovement>();
		animationManager = GetComponent<AnimationManager>();
	}

	public virtual void ToggleAbility()
	{
		// Override
	}

	public void CancelAbility()
	{
		attr.tileManager.TurnOffTiles();
		attackReady = false;
		attr.queuedAction = false;
		movement.ToggleMove();
	}

	public void InitiateAbility()
	{
		attr.gameManager.IncrActing(attr.teamId);
		attackReady = false;
		attr.queuedAction = false;
		attr.gameManager.UpdateEnergy(attr.teamId, energyCost);
		animationManager.AbilityAnimation();
		attr.tileManager.TurnOffTiles();
		Invoke("ApplyEffect", applyEffectAfter);
	}

	public virtual void ApplyEffect()
	{
		// Override
	}

	public void ToggleBase()
	{
		attackReady = true;
		attr.queuedAction = true;
		attr.tileManager.TurnOffTiles();
	}
}
