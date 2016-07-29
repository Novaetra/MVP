using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    private int currentEnemyCount;
    private int currentWaveCount;
    private int enemysToSpawn;
    private float timeBetweenRounds;
    private float timeBetweenSpawns;
    private HUDManager hudMan;


    public void setUp()
    {
        hudMan = PhotonGameManager.currentplayer.GetComponent<HUDManager>();
        enemysToSpawn = 3;
        currentWaveCount = 0;
        timeBetweenRounds = 1f;
        timeBetweenSpawns = 2f;
        StartCoroutine(waitToStartNewRound());
    }

    //Increments the current wave count by one, spawns all enemies for that round.
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
        if(PhotonNetwork.isMasterClient)
        {
            GameObject spawn = GameObject.Find("EnemySpawn" + (int)Random.Range(1f, 4f)).gameObject;
            GameObject enemy = (GameObject)PhotonNetwork.Instantiate("Enemy", spawn.transform.position, spawn.transform.rotation, 0);
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
}
