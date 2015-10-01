/* ========================================
 * CameraScroll.cs
 * 
 * Script to control movement of the camera
 * =======================================*/

using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour
{
	// ====================
	// Private Variables //
	// ====================
	private Vector3 upDir;
	private Vector3 downDir;
	private Vector3 rightDir;
	private Vector3 leftDir;

	// ===========================
	// Camera control variables //
	// ===========================
	public float camSpeed;				// Scroll speed
	public float damping;				// Scroll damping
	private Transform followObject;		// Object that camera Lerp's to
	private Transform relativeObject;	// Follow object moves relative to this object

	// =====================================================
	// Boundaries of the map to prevent scrolling too far //
	// =====================================================
	public float leftBoundary;		// Left boundary of map (0)
	public float rightBoundary;		// Right boundary of map (terrain x)
	public float topBoundary;		// Top boudary of map (slightly less than terrain z)
	public float bottomBoundary;	// Bottom boundary of map (0)
	
	void Awake()
	{
		followObject = new GameObject("CameraFollow").transform;
		relativeObject = GameObject.Find ("RelativeObject").transform;
		upDir = camSpeed * Vector3.forward;
		downDir = camSpeed * Vector3.back;
		rightDir = camSpeed * Vector3.right;
		leftDir = camSpeed * Vector3.left;
	}

	void  Start ()
	{
		followObject.position = transform.position;
	}
	
	void  Update ()
	{
		// Camera object constantly Lerps to the follow object's position
		transform.position = Vector3.Lerp(transform.position, followObject.position, damping * Time.deltaTime);

		// D or Right Arrow to scroll right
		if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
		{
			if (followObject.position.x < rightBoundary)
				followObject.Translate(rightDir, relativeObject);
		}

		// A or Left Arrow to scroll left
		if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
		{
			if (followObject.position.x > leftBoundary)
				followObject.Translate(leftDir, relativeObject);
		}

		// W or Up Arrow to scroll up
		if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
		{
			if (followObject.position.z < topBoundary)
				followObject.Translate(upDir, relativeObject);
		}

		// S or Down Arrow to scroll down
		if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
		{
			if (followObject.position.z > bottomBoundary)
				followObject.Translate(downDir, relativeObject);
		}	
	}
}