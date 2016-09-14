using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public bool spawnEnemies;

    private int currentEnemyCount;
	[SerializeField]
    private int currentWaveCount;
	[SerializeField]
	private int maxEnemies;
	[SerializeField]
    private int enemysToSpawn;
	private int enemiesSpawned;

    private float timeBetweenRounds;
    private float timeBetweenSpawns;

    private Transform currentRoom; //This is to detect which room the player is in, and spawn enemies accordingly

	//This dictionary holds all the spawn points per room
    private Dictionary<string, Transform[]> spawnPointsInRoom = new Dictionary<string, Transform[]>();

	private Dictionary<string,float[]> statsPerEnemy = new Dictionary<string, float[]>();

	//This list keeps track of the rooms are adjacent to other rooms and which ones
    private List<Transform[]> adjacentRooms = new List<Transform[]>();

	//This list is the list that holds all the spawn points an enemy can spawn in
    private List<Transform> spawnPointsAvailable = new List<Transform>();

	//List of doors so we can figure out the adjacent rooms
    private Door[] doors;

	//Refrence to the player's HUD Manager
    private HUDManager hudMan;

    //Sets starting values
    public void setUp()
    {
		//This if statement is for development purposes
        if(spawnEnemies == true)
        {
			//Sets all the starting values for the variables
            hudMan = PhotonGameManager.currentplayer.GetComponent<HUDManager>();
            enemysToSpawn = 3;
            currentWaveCount = 0;
			enemiesSpawned = 0;
            timeBetweenRounds = 3f;
            timeBetweenSpawns = 2f;
            setupSpawnLists();
            StartCoroutine(waitToStartNewRound());
            doors = GameObject.FindObjectsOfType<Door>();
        }

		statsPerEnemy["BasicMelee"] = new float[3]{0f, 20f, 10f };

    }
    
    //Fills the list that contains all adjacent rooms and links the room to its spawn points
    private void setupSpawnLists()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
		//For each floor
        for (int i = 0; i < floors.Length; i++)
        {
			Transform rooms = floors[i].transform.FindChild("Rooms");
			//For each room
            for (int r = 0; r < rooms.childCount; r++)
            {
				//Add the room's spawns to the list "spawnPointsInRoom"
                Transform room = rooms.GetChild(r);
                linkRoomsToSpawns(room);
            }
			//Add the rooms that are adjacent to each other to the list "adjacentRooms"
            addAdjacentRooms(floors[i].transform);
        }
    }
    
	//Adds the adjacent rooms to list "adjacent rooms"
    private void addAdjacentRooms(Transform floor)
    {
		//For each door
        foreach(Transform door in floor.FindChild("Doors"))
        {
			//Temporary list to store adjacent rooms for later use
            Transform[] _adjacentRooms = new Transform[3];
            int i = 0;
			//For each adjacent room
            foreach (Transform t in door.GetComponent<Door>().getAdjacentRooms())
            {
				//Add the adjacent rooms to the temporary list of adjacent rooms
                _adjacentRooms[i] = t;
                i++;
            }

			//Use the temporary list of adjacent rooms to populate the dictionary storing the adjacent rooms
            adjacentRooms.Add(new Transform[] { _adjacentRooms[0], _adjacentRooms[1], _adjacentRooms[2] } );
        }
    }
    
	//Links room to the spawn points inside it
    private void linkRoomsToSpawns(Transform room)
    {
		//Makes sure there are spawns in the room
        if (room.FindChild("Spawns") != null)
        {
			//temporary list to store the spawn points per room for later use
            Transform[] spawns = new Transform[room.FindChild("Spawns").childCount];
            int z = 0;
			//For each spawn in room
            foreach (Transform spawn in room.FindChild("Spawns"))
            {
				//Add the spawn point to the temporary list
                spawns[z] = spawn;
                z++;
            }
			//Use temporary list to populate the list of spawnPointsInRoom
            spawnPointsInRoom.Add(room.name, spawns);
        }
    }

    //Updates the list of spawns available
    public void updateSpawnsAvailable()
    {
        //first it clears the list
        spawnPointsAvailable.Clear();
        //Then it goes through the list of adjacent rooms and picks out all the elements that contain the player's current location (room)
        for(int i = 0; i<adjacentRooms.Count;i++)
        {
			//If you are in a room with an adjacent room
            if(adjacentRooms[i][0] == currentRoom)
            {
                //If the door linking the two rooms is open, then it adds the spawn points of that adjacent room to the list of available points
                if(adjacentRooms[i][2].GetComponentInChildren<Door>().getOpen())
                {
                    foreach (Transform spawn in spawnPointsInRoom[adjacentRooms[i][1].name])
                    {
                        if (spawnPointsAvailable.Contains(spawn) == false)
                        {
                            spawnPointsAvailable.Add(spawn);
                        }
                    }
                }
            }

			//If you are in a room with an adjacent room
            else if(adjacentRooms[i][1] == currentRoom)
            {
				//If the door linking the two rooms is open, then it adds the spawn points of that adjacent room to the list of available points
				if (adjacentRooms[i][2].GetComponentInChildren<Door>().getOpen())
                {
                    foreach (Transform spawn in spawnPointsInRoom[adjacentRooms[i][0].name])
                    {
                        if(spawnPointsAvailable.Contains(spawn) == false)
                        {
                            spawnPointsAvailable.Add(spawn);
                        }
                    }
                }
            }
        }

        //Add current room's spawn points

        foreach(Transform spawn in spawnPointsInRoom[currentRoom.name])
        {
            spawnPointsAvailable.Add(spawn);
        }
    }

	//Increases the current round by one, and starts spawning more enemies.
	//The number of enemies to spawn increases as well
    private void startNextRound()
    {
        currentWaveCount++;
        hudMan.updateRoundsTxt(currentWaveCount);
        enemysToSpawn = Mathf.RoundToInt(enemysToSpawn * 1.5f);
        currentEnemyCount = enemysToSpawn;
        StartCoroutine(spawnWave());
    }
    
	//Checks if there are any enemies left
	//If there are no more enemies left, then start the next round
    private void checkIfRoundEnd()
    {
        if (currentEnemyCount <= 0)
        {
            StartCoroutine(waitToStartNewRound());
        }
    }

	//Decreases the number of enemies alive 
	//Is called every time an enemy is killed
    public void decreaseEnemyCount()
    {
        currentEnemyCount--;
        checkIfRoundEnd();
    }

	//Spawns an enemy at a random spawn point
    public void spawnEnemy()
    {
        //Change this so it uses any of the spawn points in the array
        if(PhotonNetwork.isMasterClient)
        {
            int randIndx = (int)Random.Range(0, (spawnPointsAvailable.Count - 1));
            Transform spawn = spawnPointsAvailable[randIndx];
            GameObject enemy = (GameObject)PhotonNetwork.Instantiate("Enemy", spawn.position, spawn.rotation, 0);
			if (currentWaveCount < 9) 
			{
				enemy.GetComponent<EnemyController> ().setTotalHealth (statsPerEnemy["BasicMelee"][0]+20f);
				statsPerEnemy["BasicMelee"] [0] =  statsPerEnemy["BasicMelee"] [0] + 20f;
			}
			else
			{
				enemy.GetComponent<EnemyController> ().setTotalHealth ((statsPerEnemy["BasicMelee"][0] + statsPerEnemy["BasicMelee"] [0]*.05f));
				enemy.GetComponent<EnemyController> ().setExpOnKill ((statsPerEnemy["BasicMelee"][1] + statsPerEnemy["BasicMelee"] [1]*.02f));
				statsPerEnemy ["BasicMelee"] [0] = statsPerEnemy ["BasicMelee"] [0] + statsPerEnemy ["BasicMelee"] [0] * .05f;
				statsPerEnemy ["BasicMelee"] [1] = statsPerEnemy ["BasicMelee"] [1] + statsPerEnemy ["BasicMelee"] [1] * .02f;
			}
        }
    }

	//Spawns the whole wave of enemies
    private IEnumerator spawnWave()
    {
		enemiesSpawned = 0;

        for(int x = 0; x<enemysToSpawn; x++)
        {
			if (enemiesSpawned < maxEnemies) 
			{

				yield return new WaitForSeconds(timeBetweenSpawns);
				spawnEnemy();
			}
        }
    }

	//Waits x seconds before starting the next round
    private IEnumerator waitToStartNewRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        startNextRound();
    }

	//Sets the room the player is currently in
    public void setCurrentRoom(Transform room)
    {
        currentRoom = room;
    }
}