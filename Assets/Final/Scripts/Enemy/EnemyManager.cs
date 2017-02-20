using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public bool spawnEnemies;
    public GameObject[] ENEMY_OBJECTS;
    private MeteorManager meteorMan;
    private StatsManager playerSM;
    private int currentEnemyCount;
    private int currentWaveCount;
	private int maxEnemies;
    private int enemysToSpawn;
	private int enemiesSpawned;
    private int enemiesKilled;
    private float timeBetweenRounds;
    private float timeBetweenSpawns;

    private Transform currentRoom; //This is to detect which room the player is in, and spawn enemies accordingly

	//This dictionary holds all the spawn points per room
    private Dictionary<string, Transform[]> spawnPointsInRoom = new Dictionary<string, Transform[]>();

	private Dictionary<string,float[]> statsPerEnemy = new Dictionary<string, float[]>();
    private ArrayList enemyNames = new ArrayList();

	//This list keeps track of the rooms are adjacent to other rooms and which ones
    private List<Transform[]> adjacentRooms = new List<Transform[]>();

    //This list is the list that holds all the spawn points an enemy can spawn in
    [SerializeField]
    private List<Transform> spawnPointsAvailable = new List<Transform>();

	//List of doors so we can figure out the adjacent rooms
    private Door[] doors;

	//Refrence to the player's HUD Manager
    private HUDManager hudMan;

    //Sets starting values
    public void setUp()
    {
		//This if statement is for development purposes
		if (spawnEnemies == true) 
		{
			//Sets all the starting values for the variables
			hudMan = GameManager.currentplayer.GetComponent<HUDManager> ();
			enemysToSpawn = 1;
			maxEnemies = 20;
			currentWaveCount = 0;
			enemiesSpawned = 0;
			timeBetweenRounds = 3f;
			timeBetweenSpawns = 2f;
			setupSpawnLists ();
			StartCoroutine (waitToStartNewRound ());
			doors = GameObject.FindObjectsOfType<Door> ();
            setCurrentRoom(GameObject.FindGameObjectWithTag("Spawn Room").transform);
		}
        playerSM = GameManager.currentplayer.GetComponent<StatsManager>();
        enemiesKilled = 0;
        meteorMan = GameObject.Find("MeteorManager").GetComponent<MeteorManager>();   //health, exp, melee damagem, attack distance, movement speed
		statsPerEnemy["Grunt"] = new float[5]{100f, 20f, 10f, 2.6f, 1.75f};
        enemyNames.Add("Grunt");
        statsPerEnemy["Weakling"] = new float[5] { 100f, 20f, 10f, 3.6f,1.25f};
        enemyNames.Add("Weakling");
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
                Debug.Log("linked");
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
            List<Transform> _adjacentRoomsList = new List<Transform>(0);
            //Transform[] _adjacentRooms = new Transform[3];
            int i = 0;
			//For each adjacent room
            foreach (Transform t in door.GetComponent<Door>().getAdjacentRooms())
            {
                //Add the adjacent rooms to the temporary list of adjacent rooms
                _adjacentRoomsList.Add(t);
                i++;
            }
            //Create a final array of all adjacent rooms to that door
            Transform[] finalAdjacentRoomsList = new Transform[_adjacentRoomsList.Count];
            for(int x = 0; x<_adjacentRoomsList.Count;x++)
            {
                finalAdjacentRoomsList[x] = _adjacentRoomsList[x];
            }

			//Use the temporary list of adjacent rooms to populate the dictionary storing the adjacent rooms
            adjacentRooms.Add(finalAdjacentRoomsList);
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
        if (adjacentRooms.Count > 0)
        {
            //Then it goes through the list of adjacent rooms and picks out all the elements that contain the player's current location (room)
            //I = each indivual group of adjacent rooms declared in each door object
            for (int i = 0; i < adjacentRooms.Count; i++)
            {
                for (int a = 0; a < adjacentRooms[i].Length-1; a++)
                {
                    //If a room in the list == player current room
                    if (adjacentRooms[i][a] == currentRoom)
                    {
                        //If the door linking the two rooms is open, then it adds the spawn points of that adjacent room to the list of available points
                        //(The last element in array will always be the door)
                        if (adjacentRooms[i][adjacentRooms[i].Length-1].GetComponentInChildren<Door>().getOpen())
                        {
                            //For each spawn point in _______ room 
                            for (int b=0; b<adjacentRooms[i].Length-1;b++)
                            {
                                foreach (Transform spawn in spawnPointsInRoom[adjacentRooms[i][b].name])
                                {
                                    //If the list of available spawns doesn't contain the current spawn, then add it
                                    if (!spawnPointsAvailable.Contains(spawn))
                                    {
                                        spawnPointsAvailable.Add(spawn);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        Debug.Log(currentRoom.name);
        //Add current room's spawn points
        if(spawnPointsInRoom[currentRoom.name] != null)
        {
            foreach (Transform spawn in spawnPointsInRoom[currentRoom.name])
            {
                if (!spawnPointsAvailable.Contains(spawn))
                {
                    spawnPointsAvailable.Add(spawn);
                }
            }
        }
    }

	//Increases the current round by one, and starts spawning more enemies.
	//The number of enemies to spawn increases as well
    private void startNextRound()
    {
        updateSpawnsAvailable();
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
            UpdateEnemyValues();
            StartCoroutine(waitToStartNewRound());
            StartCoroutine(meteorMan.UpdateStats(currentWaveCount,(int) playerSM.getExpGained(), enemiesKilled));
            enemiesKilled = 0;
            playerSM.resetExpGained();
        }
    }

    public int[] getStats()
    {
        int[] stats = new int[3] {currentWaveCount,(int)playerSM.getExpGained(),enemiesKilled};
        return stats;
    }

	//Decreases the number of enemies alive 
	//Is called every time an enemy is killed
    public void decreaseEnemyCount()
    {
        currentEnemyCount--;
        enemiesKilled++;
        checkIfRoundEnd();
    }

	//Spawns an enemy at a random spawn point
    public void spawnEnemy()
    {
        //Change this so it uses any of the spawn points in the array
        int randIndx = (int)Random.Range(0, (spawnPointsAvailable.Count));
        Transform spawn = spawnPointsAvailable[randIndx];
        randIndx = (int)Random.Range(0, 2);
        GameObject enemy = (GameObject)GameObject.Instantiate(ENEMY_OBJECTS[randIndx], spawn.position, spawn.rotation);
        string enemytype = (string)enemyNames[randIndx];
        setStartingStats(enemy.GetComponent<EnemyController>(),enemytype);
    }

    private void setStartingStats(EnemyController enemy, string enemyName)
    {
        enemy.setTotalHealth(statsPerEnemy[enemyName][0]);
        enemy.setExpOnKill(statsPerEnemy[enemyName][1]);
        enemy.setMeleeDamage(statsPerEnemy[enemyName][2]);
        enemy.setAttackProximity(statsPerEnemy[enemyName][3]);
        enemy.setMovementSpeed(statsPerEnemy[enemyName][4]);
    }

    //Every round increase enemy health, exp value, and dmg
    private void UpdateEnemyValues()
    {
        if (currentWaveCount < 5)
        {
            for(int i = 0;i<enemyNames.Count;i++)
            {
                statsPerEnemy[(string)enemyNames[i]][0] += 20f;
                statsPerEnemy[(string)enemyNames[i]][1] += 5f;
                statsPerEnemy[(string)enemyNames[i]][2] += 5f;
            }
        }
        else
        {
            for (int i = 0; i < enemyNames.Count; i++)
            {
                //Temporary values....These will escalate out of control in higher rounds
                statsPerEnemy[(string)enemyNames[i]][0] += statsPerEnemy[(string)enemyNames[i]][0] * .05f;
                statsPerEnemy[(string)enemyNames[i]][2] += statsPerEnemy[(string)enemyNames[i]][2] * .05f;
            }
        }
    }


	//Spawns the whole wave of enemies
    private IEnumerator spawnWave()
    {
		enemiesSpawned = 0;
        for (int x = 0; x<enemysToSpawn; x++)
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