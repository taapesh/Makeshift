/* ==========================================
 * NetworkManager.cs
 * Responsible for finding games for players
 * and initiating the start of the game
 * Also handles connection during game play
 ============================================*/

using UnityEngine;

public class NetworkManager : Photon.MonoBehaviour
{
	public bool DEBUG;

	// =============
	// Scene indexes
	// =============
	private int LoginScene 		= 0;
	private int MainLobbyScene 	= 1;
	private int DominionScene 	= 2;

	private bool inLogin = false;
	private bool inLobby = false;
	private bool inGame = false;
	private bool joiningGame = false;

	// Architect spawner prefabs
	public GameObject architectSpawnerA;
	public GameObject architectSpawnerB;

	void Awake()
	{
		if (Application.loadedLevel == LoginScene)
		{
			inLogin = true;
		}
	}

	// Called every time a new level is loaded
	void OnLevelWasLoaded(int level)
	{
		// Login screen loaded
		if (level == LoginScene)
		{
			inLobby = false;
			inGame = false;

			inLogin = true;
		}

		// Main lobby loaded
		else if (level == MainLobbyScene)
		{
			inLogin = false;
			inGame = false;

			inLobby = true;
		}

		// Dominion level loaded
		else if (level == DominionScene)
		{
			inLobby = false;
			inLogin = false;

			inGame = true;
		}
	}
		
	void CreateSpawnerA()
	{
		GameObject.Instantiate(architectSpawnerA, Vector3.zero, Quaternion.identity);
	}
	
	void CreateSpawnerB()
	{
		GameObject.Instantiate(architectSpawnerB, Vector3.zero, Quaternion.identity);
	}
	
	[PunRPC]
	void LoadDominionLevel()
	{
		Application.LoadLevel("MainScene");
	}
	
	void Login()
	{
		// Check credentials and login player
		PhotonNetwork.ConnectUsingSettings("1.0");
	}
	
	public void Logout()
	{
		// Log player out and go to login screen
		PhotonNetwork.Disconnect();
		Application.LoadLevel("LoginScene");
	}

	// Join or create room for game
	public void JoinOrCreateGame()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	// ================
	// Callback methods
	// ================

	// Called upon joining a room
	void OnJoinedRoom()
	{
		if (PhotonNetwork.playerList.Length == 2)
		{
			CreateSpawnerB();
			photonView.RPC("LoadDominionLevel", PhotonTargets.AllBufferedViaServer);
		}
		else
		{
			CreateSpawnerA();
		}
	}

	// Called after player successfully logs in
	void OnJoinedLobby()
	{
		Application.LoadLevel("LobbyScene");
	}

	// If failed to join a random game, create a game for others to join
	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null);
	}
	
	// Called when a player disconnects from the game
	void OnPhotonPlayerDisconnected()
	{
		Debug.Log ("Player disconnected");
	}

	// Called when player is disconnected from Photon Network
	void OnDisconnectedFromPhoton()
	{
		Debug.Log ("Disconnected");
	}

	// Show stuff on GUI for debugging
	void OnGUI()
	{
		// Display connection state at top left corner of screen
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		
		float btnWidth = Screen.width * .1f;
		float btnHeight = Screen.height * .05f;
		float xPos = (Screen.width * .5f) - (btnWidth / 2);
		float yPos = (Screen.height * .55f) - (btnHeight / 2);;
		
		// If on login screen, show the Login Button
		if (inLogin)
		{
			if (GUI.Button (new Rect(xPos, yPos, btnWidth, btnHeight), "Login"))
			{
				Login();
			}
		}
		else if (inLobby && !joiningGame)
		{
			if (GUI.Button (new Rect(xPos, yPos, btnWidth, btnHeight), "Play"))
			{
				JoinOrCreateGame();
				joiningGame = true;
			}
		}
		else if (inGame)
		{
			// Do something
		}
	}
}