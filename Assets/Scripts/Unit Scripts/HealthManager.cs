using UnityEngine;
using System.Collections;

public class HealthManager : Photon.MonoBehaviour
{
	private AnimationManager animationManager;
	private UnitAttributes attr;
	private bool isDead;

	void Awake()
	{
		animationManager = GetComponent<AnimationManager>();
		attr = GetComponent<UnitAttributes>();
	}
	
	void Update ()
	{
		if (attr.health == 0 && !isDead)
		{
			isDead = true;
			animationManager.DeathAnimation();
		}
	}
}
