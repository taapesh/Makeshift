using UnityEngine;
using System.Collections;

public class ArchitectSpawner : MonoBehaviour
{
	private const int MainScene = 2;

	private string archPrefabNameA = "ArchitectPrefabA";
	private string archPrefabNameB = "ArchitectPrefabB";
	public int teamID;

	private GameManager gameManager;
	private TileManager tileManager;
	private GameObject archUnit;

	void Awake()
	{
		DontDestroyOnLoad(gameObject.transform);
	}

	void OnLevelWasLoaded(int level)
	{
		if (level != MainScene)
			return;

		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		tileManager = GameObject.Find ("TileManager").GetComponent<TileManager>();

		if (teamID == 0)
		{
			archUnit = (GameObject) PhotonNetwork.Instantiate(
				archPrefabNameA,
				tileManager.tileArray[gameManager.archSpawnA].position,
				Quaternion.identity, 0);
		}
		else
		{
			archUnit = (GameObject) PhotonNetwork.Instantiate(
				archPrefabNameB,
				tileManager.tileArray[gameManager.archSpawnB].position,
				Quaternion.identity, 0);
		}

		archUnit.GetComponent<SetupObjects>().Setup();
		Destroy(gameObject);
	}
}
