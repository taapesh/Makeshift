/* =======================================
 * CameraZoom.cs
 * 
 * Use the mouse wheel to scroll in and out
 * ======================================*/

using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
	// Scrolling control variables
	private float distance;				// Scroll distance
	public float sensitivityDistance;	// Scroll sensitivity
	public float damping;				// Scroll damping
	public float minFOV;				// Minimum field of view
	public float maxFOV;				// Maximum field of view
	
	void Start ()
	{
		distance = GetComponent<Camera>().fieldOfView;
	}

	void Update ()
	{
		distance -= Input.GetAxis("Mouse ScrollWheel") * sensitivityDistance;
		distance = Mathf.Clamp(distance, minFOV, maxFOV);
		GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, distance, damping * Time.deltaTime);
	}
}
