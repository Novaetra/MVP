using UnityEngine;
using System.Collections;

public class PhotonMenu : MonoBehaviour 
{
	//Just a loading screen
	void Start()
	{
		if (!PhotonNetwork.connected) {

			PhotonNetwork.autoJoinLobby = true;
			PhotonNetwork.ConnectUsingSettings ("1.0");
			Debug.Log ("Not connected");
		} else {
			Debug.Log ("Connected");
		}
	}


	public void joinRandom()
	{
		Debug.Log ("Joining...");
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null);
	}
		
	public void OnJoinedRoom()
	{
		Debug.Log ("Joined Room");
		PhotonNetwork.LoadLevel (2);
	}

	private void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
}
