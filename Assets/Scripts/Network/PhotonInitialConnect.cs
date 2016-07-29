using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhotonInitialConnect : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.ConnectUsingSettings ("1.0");
	}

	private void OnJoinedLobby() 
	{
		PhotonNetwork.playerName = "" + Random.Range (0.0f, 50.0f);
		Debug.Log (PhotonNetwork.playerName);
		PhotonNetwork.LoadLevel (1);
	}

	private void OnGUI()
	{
		GameObject.Find("LoadingText").GetComponent<Text>().text = (PhotonNetwork.connectionStateDetailed.ToString());
	}
}
