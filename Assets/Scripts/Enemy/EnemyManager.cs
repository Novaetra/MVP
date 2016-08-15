using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public bool spawnEnemies;

    private int currentEnemyCount;
    private int currentWaveCount;
    private int enemysToSpawn;
    private float timeBetweenRounds;
    private float timeBetweenSpawns;

    private Transform currentRoom;

    private Dictionary<string, Transform[]> spawnPointsInRoom = new Dictionary<string, Transform[]>();
    private List<Transform[]> adjacentRooms = new List<Transform[]>();

    private List<Transform> spawnPointsAvailable = new List<Transform>();

    private Door[] doors;

    private HUDManager hudMan;

    //Sets starting values
    public void setUp()
    {
        if(spawnEnemies == true)
        {
            hudMan = PhotonGameManager.currentplayer.GetComponent<HUDManager>();
            enemysToSpawn = 3;
            currentWaveCount = 0;
            timeBetweenRounds = 3f;
            timeBetweenSpawns = 2f;
            setupSpawnLists();
            StartCoroutine(waitToStartNewRound());
            doors = GameObject.FindObjectsOfType<Door>();
        }
    }
    
    //Fills the list that contains all adjacent rooms and links the room to its spawn points
    private void setupSpawnLists()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < floors.Length; i++)
        {
            Transform rooms = floors[i].transform.FindChild("Rooms");
            for (int r = 0; r < rooms.childCount; r++)
            {
                Transform room = rooms.GetChild(r);
                linkRoomsToSpawns(room);
            }
            addAdjacentRooms(floors[i].transform);
        }
    }
    //Adds the adjacent rooms to list
    private void addAdjacentRooms(Transform floor)
    {
        foreach(Transform door in floor.FindChild("Doors"))
        {
            Transform[] _adjacentRooms = new Transform[3];
            int i = 0;
            foreach (Transform t in door.GetComponent<Door>().getAdjacentRooms())
            {
                _adjacentRooms[i] = t;
                i++;
            }

            adjacentRooms.Add(new Transform[] { _adjacentRooms[0], _adjacentRooms[1], _adjacentRooms[2] } );
        }
    }
    //Links room to the spawn points inside it
    private void linkRoomsToSpawns(Transform room)
    {
        if (room.FindChild("Spawns") != null)
        {

            Transform[] spawns = new Transform[room.FindChild("Spawns").childCount];
            int z = 0;
            foreach (Transform spawn in room.FindChild("Spawns"))
            {
                spawns[z] = spawn;
                z++;
            }
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
            else if(adjacentRooms[i][1] == currentRoom)
            {
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

    private void startNextRound()
    {
        currentWaveCount++;
        hudMan.updateRoundsTxt(currentWaveCount);
        enemysToSpawn = Mathf.RoundToInt(enemysToSpawn * 1.5f);
        currentEnemyCount = enemysToSpawn;
        StartCoroutine(spawnWave());
    }
    
    private void checkIfRoundEnd()
    {
        if (currentEnemyCount <= 0)
        {
            StartCoroutine(waitToStartNewRound());
        }
    }

    public void decreaseEnemyCount()
    {
        currentEnemyCount--;
        checkIfRoundEnd();
    }

    public void spawnEnemy()
    {
        //Change this so it uses any of the spawn points in the array
        if(PhotonNetwork.isMasterClient)
        {
            int randIndx = (int)Random.Range(0, (spawnPointsAvailable.Count - 1));
            Transform spawn = spawnPointsAvailable[randIndx];
            GameObject enemy = (GameObject)PhotonNetwork.Instantiate("Enemy", spawn.position, spawn.rotation, 0);
        }
    }

    private IEnumerator spawnWave()
    {
        for(int x = 0; x<enemysToSpawn; x++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            spawnEnemy();
        }
    }

    private IEnumerator waitToStartNewRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        startNextRound();
    }

    public void setCurrentRoom(Transform room)
    {
        currentRoom = room;
    }
}