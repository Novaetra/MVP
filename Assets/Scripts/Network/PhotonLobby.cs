using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PhotonLobby : MonoBehaviour 
{
	void Start()
	{
		//Fills the list of players in the lobby
		updateLobbyList();
		//If the current player is not the master client, he should not be allowed to start
		if (!PhotonNetwork.player.isMasterClient) 
		{
			GameObject.Find ("Start").gameObject.SetActive (false);
		}
	}
	//Updates the lobby list every frame
	void Update()
	{
		updateLobbyList ();	
	}
	//Updates the lobby list
	private void updateLobbyList()
	{

		for (int x = 0; x < 4; x++) 
		{
			try 
			{
				if (PhotonNetwork.playerList [x] != null) 
				{
					GameObject current = GameObject.Find ("Player(" + (x + 1) + ")").gameObject;
					if (PhotonNetwork.playerList [x].name == PhotonNetwork.playerName) 
					{
						current.GetComponent<Text> ().text = PhotonNetwork.playerName;
					} 
					else
					{
						current.GetComponent<Text> ().text = PhotonNetwork.playerList [x].name;
					}
				}
			} catch (IndexOutOfRangeException) { }
		}
	}

	//Loads the game
	public void startGame()
	{
		PhotonNetwork.LoadLevel (3);
	}
}
