using UnityEngine;
using System.Collections;

public class PhotonGameManager : MonoBehaviour 
{

	public static GameObject currentplayer;
	public static int playerID;

	void Start () 
	{
		if (PhotonNetwork.connected) 
		{
			addPlayer ();
		} 
		else 
		{
			PhotonNetwork.autoJoinLobby = true;
			PhotonNetwork.ConnectUsingSettings ("1.0");
		}
	}

	#region temporary
	private void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null);
	}

	public void OnJoinedRoom()
	{
		Debug.Log ("Joined Room");
		addPlayer ();
	}
	#endregion

	private void addPlayer()
	{
		GameObject spawn = GameObject.Find("Spawn"+(int)Random.Range (1f, 4f)).gameObject;
		currentplayer = (GameObject)PhotonNetwork.Instantiate ("Player", spawn.transform.position, spawn.transform.rotation,0);
		playerID = currentplayer.GetComponent<PhotonView> ().viewID;
		currentplayer.GetComponent<PersonControlller> ().enabled = true;
		currentplayer.GetComponent<CharacterController> ().enabled = true;
        currentplayer.GetComponent<HUDManager>().enabled = true;
        currentplayer.GetComponent<SkillManager>().enabled = true;
		currentplayer.transform.GetComponentInChildren<Canvas> ().enabled = true;
		currentplayer.transform.GetComponentInChildren<Camera> ().enabled = true;
		currentplayer.transform.GetComponentInChildren<AudioListener> ().enabled = true;
		currentplayer.transform.BroadcastMessage ("setUp");
        GetComponent<EnemyManager>().setUp();
	}

    public void checkGameEnded()
    {
        Player[] players = PhotonView.FindObjectsOfType<Player>();
        int playersAlive = players.Length;

        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<StatsManager>().getAlive() == false)
            {
                playersAlive--;
            }
        }

        if(playersAlive<= 0)
        {
            //GetComponent<PhotonView>().RPC("endGame", PhotonTargets.AllBuffered, null);
        }

        Debug.Log("players aliveL: " + playersAlive + " out of " + players.Length);
    }
    [PunRPC]
    void endGame()
    {
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();
    }

}
