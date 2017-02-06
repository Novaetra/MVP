using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MeteorManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ConnectToMeteor());
    }


    IEnumerator ConnectToMeteor()
    {
        Debug.Log("attempting to connect");
        yield return Meteor.Connection.Connect("ws://meteor-tommsy.rhcloud.com:8000/websocket");
        Debug.Log("connected");
        StartCoroutine(MeteorLogin());
    }

    IEnumerator tests()
    {
        var method = Meteor.Method<string>.Call("GetCollection", "PlayerStats");
        yield return (Coroutine)method;
        Debug.Log(method.Response);
    }

    //Logs user into his/her account
    IEnumerator MeteorLogin()
    {
        Debug.Log("attempting to log in");
        string emailStr = "lenny", passwordStr = "123";
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
