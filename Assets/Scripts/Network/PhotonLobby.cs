using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PhotonLobby : MonoBehaviour 
{
	void Start()
	{
		for (int x = 0; x < 4; x++) 
		{
			try{
				if (PhotonNetwork.playerList [x] != null) 
				{
					GameObject current = GameObject.Find ("Player(" + (x+1)+ ")").gameObject;
					if (PhotonNetwork.playerList [x] == PhotonNetwork.player) 
					{
						current.GetComponent<Text> ().text = PhotonNetwork.playerName;
					} 
				}
			}
			catch(IndexOutOfRangeException)
			{
			}
		}
		if (!PhotonNetwork.player.isMasterClient) 
		{
			GameObject.Find ("Start").gameObject.SetActive (false);
		}
	}

	void Update()
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

	public void startGame()
	{
		PhotonNetwork.LoadLevel (3);
	}
}
