﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeteorManager : MonoBehaviour
{
	//Text componets to get user input
	private Text email, password;
	//Strings that will hold the actual user input
	private string emailStr, passwordStr;
	//Prefab of a score
	public GameObject SCORE_OBJ;
	//Array of scores
	private Player[] scoreArray;

	void Start () 
	{
		//Set the email and password text components
		email = GameObject.Find ("EmailText").GetComponent<Text>();
		password = GameObject.Find ("PasswordText").GetComponent<Text>();

		//Connect to meteor server
		StartCoroutine (ConnectToMeteor ());
	}


    IEnumerator ConnectToMeteor()
    {
        Debug.Log("attempting to connect");
        yield return Meteor.Connection.Connect("ws://meteor-tommsy.rhcloud.com:8000/websocket");
        Debug.Log("connected");
    }


	//Logs user in with the given credentials
	public void Login()
	{
		emailStr = email.text;
		passwordStr = password.text;
		StartCoroutine(MeteorLogin());
	}

    //Logs user into his/her account
    IEnumerator MeteorLogin()
    {
        Debug.Log("attempting to log in");
        var login = Meteor.Accounts.LoginWith(emailStr, passwordStr);
        yield return (Coroutine)login;
        Debug.Log("Ok proceed");
        if (Meteor.Accounts.IsLoggedIn)
        {
            Debug.Log("login success!");
            DontDestroyOnLoad(this);
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Failed to log in...");
            emailStr = "";
            passwordStr = "";
            //email.text = "";
            //password.text = "";
        }
    }
}
