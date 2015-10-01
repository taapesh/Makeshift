using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	// Define fundamental projectile behavior

	// Projectile attributes
	public float speed;
	private Vector3 velocity;
	private Ability ability;		// Ability that spawned this projectile

	// Visual components
	public GameObject impactEffectPrefab;
	private string impactEffectName;
	
	// Target components
	private GameObject targetUnit;
	private UnitAttributes targetAttr;
	private bool targetSet;

	void Awake()
	{
		if (impactEffectPrefab != null)
			impactEffectName = impactEffectPrefab.name;

		if (speed == 0)
			speed = 2.5f;

	}

	void Update ()
	{
		if (targetSet)
			transform.Translate(velocity * Time.deltaTime);

		// Some collider checks
	}

	private void ImpactEffect()
	{
		if (impactEffectName != "")
		{
			PhotonNetwork.Instantiate(impactEffectName, targetAttr.hitPoint.position, Quaternion.identity, 0);
		}
	}

	public void SetTarget(GameObject unit, Vector3 spawnPos)
	{
		targetUnit = unit;
		targetAttr = unit.GetComponent<UnitAttributes>();

		// Calculate velocity of projectile (from spawn point to target hit point)
		velocity = (targetAttr.hitPoint.position - spawnPos).normalized * speed;

		// Allow projectile to begin moving
		targetSet = true;
	}

	public void SetAbility(Ability a)
	{
		ability = a;
	}
}
