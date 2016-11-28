using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{

	public static GameObject currentplayer;
	public static int playerID;

	public GameObject PLAYEROBJECT;

	void Start ()
    {
        addPlayer();
	}
    

	//Adds player to the scene and enables its components
	private void addPlayer()
	{
        GameObject spawn = GameObject.Find("Spawn1");
        currentplayer = (GameObject)GameObject.Instantiate(PLAYEROBJECT, spawn.transform.position, spawn.transform.rotation);
		currentplayer.GetComponent<PlayerController> ().enabled = true;
		currentplayer.GetComponent<CharacterController> ().enabled = true;
        currentplayer.GetComponent<HUDManager>().enabled = true;
        currentplayer.GetComponent<SkillManager>().enabled = true;
		currentplayer.transform.GetComponentInChildren<Canvas> ().enabled = true;
		currentplayer.transform.GetComponentInChildren<Camera> ().enabled = true;
		currentplayer.transform.GetComponentInChildren<AudioListener> ().enabled = true;
		currentplayer.transform.BroadcastMessage ("setUp");
        GetComponent<EnemyManager>().setUp();
	}

	//Checks if there's a player still alive. If there isn't, then end the game.
    public void checkGameEnded()
    {
		//Gets a list of players
        Player[] players = FindObjectsOfType<Player>();
        int playersAlive = players.Length;
		//Go through each player and check if it is alive
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<StatsManager>().getAlive() == false)
            {
				//If it's not alive, subtract from the number of 'playersAlive'
                playersAlive--;
            }
        }
		//If there are no players alive, then end the game
        if(playersAlive<= 0)
        {
            currentplayer.GetComponent<PlayerController>().toggleCursorLock(false);
            endGame();
        }
    }
	//Sends everyone back to the menu
    void endGame()
    {
        Application.LoadLevel(0);
    }

}
