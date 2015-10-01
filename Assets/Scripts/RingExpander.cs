using UnityEngine;
using System.Collections;

public class RingExpander : MonoBehaviour
{
	private float expandSpeed = .65f;

	void Start ()
	{
		Invoke("DestroyRing", .35f);
	}

	void Update ()
	{
		Vector3 scaleNew = transform.localScale;
		scaleNew.x += expandSpeed * Time.deltaTime;
		scaleNew.z += expandSpeed * Time.deltaTime;
		transform.localScale = scaleNew;
	}

	void DestroyRing()
	{
		Destroy(gameObject);
	}
}
