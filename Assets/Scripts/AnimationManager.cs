using UnityEngine;
using System.Collections;

public class AnimationManager : Photon.MonoBehaviour
{
	// Components
	private Animation anim;
	private GameManager gameManager;
	private UnitAttributes attr;

	// Move animation settings
	private bool isMoving;
	private Vector3 targetPos;
	public float stopDistance;

	// Summon animation settings
	private bool checkSummon;
	public UnitAttributes summonedAttr;

	// Attack animation settings
	private bool checkAbility;

	// Animation names
	public string idleAnimation;
	public string moveAnimation;
	public string deathAnimation;
	public string summonAnimation;
	public string abilityAnimation;

	// Animation lengths
	public float deathLength;
	public float summonLength;
	public float abilityLength;

	void Awake()
	{
		anim = GetComponent<Animation>();
		attr = GetComponent<UnitAttributes>();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();

		if (idleAnimation != "")
		{
			anim.Play(idleAnimation);
		}
	}

	void Update ()
	{
		if (isMoving)
			CheckMoveComplete();

		// Make sure the summoned unit's attributes are all set before returning control to player
		if (checkSummon)
		{
			if (summonedAttr != null && summonedAttr.isActive)
			{
				checkSummon = false;
				gameManager.DecrActing(attr.teamId);
			}
		}
	}

	public void DeathAnimation()
	{
		if (deathAnimation != "")
		{
			anim.CrossFade(deathAnimation);
		}
		Invoke("DeathComplete", deathLength);
	}

	private void DeathComplete()
	{
		gameManager.DecrActing(attr.teamId);
		PhotonNetwork.Destroy(GetComponent<PhotonView>());
	}

	public void MoveAnimation(Vector3 pos)
	{
		targetPos = pos;
		isMoving = true;

		if (moveAnimation != "")
		{
			anim.CrossFade(moveAnimation);
		}
	}
	
	private void CheckMoveComplete()
	{
		Vector3 currPos = new Vector3(transform.position.x, 0, transform.position.z);
		if (Vector3.Distance(currPos, targetPos) <= stopDistance)
		{
			isMoving = false;
			if (idleAnimation != "")
			{
				anim.CrossFade(idleAnimation);
			}

			gameManager.DecrActing(attr.teamId);
		}
	}

	public void SummonAnimation()
	{
		if (summonAnimation != "")
		{
			anim.CrossFade(summonAnimation);
		}
		Invoke("SummonComplete", summonLength);
	}

	private void SummonComplete()
	{
		if (idleAnimation != "")
		{
			anim.CrossFade(idleAnimation);
		}
		checkSummon = true;
	}

	public void AbilityAnimation()
	{
		if (abilityAnimation != "")
		{
			anim.CrossFade(abilityAnimation);
		}
		Invoke("AbilityComplete", abilityLength);
	}

	private void AbilityComplete()
	{
		if (idleAnimation != "")
		{
			anim.CrossFade(idleAnimation);
		}
		gameManager.DecrActing(attr.teamId);
	}
}
