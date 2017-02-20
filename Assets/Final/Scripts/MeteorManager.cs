using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeteorManager : MonoBehaviour
{
	private const string CONNECTION_URL = @"wss://meteor-tommsy.rhcloud.com:8443/websocket";

    //Text componets to get user input
    private Text email, password;
    //Strings that will hold the actual user input
    private string emailStr, passwordStr;

    void Start()
    {
        //Set the email and password text components
        email = GameObject.Find("EmailText").GetComponent<Text>();
        password = GameObject.Find("PasswordText").GetComponent<Text>();
        StartCoroutine(ConnectToMeteor());
    }


    IEnumerator ConnectToMeteor()
    {
		Debug.Log("Attempting to connect to: " + CONNECTION_URL);
        yield return Meteor.Connection.Connect(CONNECTION_URL);
        Debug.Log("Connected!");
    }

    public void Login()
    {
        StartCoroutine(MeteorLogin());
    }

    //Logs user into his/her account
    IEnumerator MeteorLogin()
    {
        Debug.Log("attempting to log in");
        emailStr = email.text;
        passwordStr = password.text;
        var login = Meteor.Accounts.LoginWith(emailStr, passwordStr);
        yield return (Coroutine)login;

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
            email.text = "";
            password.text = "";
        }
    }

    public IEnumerator UpdateStats(int roundsSurvived, int experienceGained, int enemiesKilled)
    {
        if(Meteor.Accounts.IsLoggedIn)
        {
            var update = Meteor.Method.Call("SetStats", roundsSurvived, experienceGained, enemiesKilled, Meteor.Accounts.UserId);
            yield return (Coroutine)update;
            Debug.Log("Updated");
        }
        else
        {
            Debug.LogError("User is not logged in");
        }
    }
}
