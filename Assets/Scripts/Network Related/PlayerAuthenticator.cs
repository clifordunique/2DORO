﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerAuthenticator : MonoBehaviour
{

    bool gameSparksAvailable;
    string username, password;
    string matchID = null;
    public ChatManager chatmanager;
    public GameObject dialogBox;
    public Text dialogText;

    void Start()
    {
        SceneManager.activeSceneChanged += setListenersOnSceneChange;
        Application.runInBackground = true;
        GameSparks.Core.GS.GameSparksAvailable = available =>
        {

            if (available)
            {
                gameSparksAvailable = true;
                Debug.Log("Servers are available");
            }

        };

    }

    // Update is called once per frame
    public void changeUsername(string name)
    {
        username = name;
    }
    public void changePassword(string pass)
    {
        password = pass;
    }

    int captchaNumb1, captchaNumb2;

    public void runCaptcha()
    {
        dialogBox.SetActive(true);
        captchaNumb1 = Random.Range(0, 9);
        captchaNumb2 = Random.Range(0, 9);
        string dialogTxt = "What is " + captchaNumb1.ToString() + " + " + captchaNumb2.ToString() + "?";
        dialogText.text = dialogTxt;
        dialogBox.GetComponent<RectTransform>().DOAnchorPosY(30, 1);
    }

    public void verifyCaptcha(int ans)
    {
        if(ans == (captchaNumb1 + captchaNumb2))
        {
            createNewPlayer();
        }
    }

    public void createNewPlayer()
    {
        if (gameSparksAvailable)
        {
            Debug.Log("Registering...");
            new RegistrationRequest()
            .SetUserName(username)
            .SetDisplayName(username)
            .SetPassword(password)
            .Send(response =>
            {

                if (response.HasErrors)
                {
                    Debug.LogError(response.Errors.JSON);
                }
                else
                {
                    Debug.Log("Registered");
                }

            });
        }
    }

    public void authorizePlayer()
    {
        if (gameSparksAvailable == true)
        {
            new AuthenticationRequest()
            .SetUserName(username)
            .SetPassword(password)
            .Send(response =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError(response.Errors.JSON);
                }
                else
                {
                    joinLobby();
                    //SceneManager.LoadScene("FightingRoom");


                }
            });
        }
        else
        {
            Debug.Log("Not connected to gamesparks");
        }
    }

    public void joinLobby()
    {
        matchFoundListener();
        new GameSparks.Api.Requests.MatchmakingRequest()
        .SetMatchShortCode("LOB")
        .SetSkill(0)
        .Send(response =>
        {

            if (!response.HasErrors)
            {
                Debug.Log("Matchmaking request succedful");
                SceneManager.LoadScene("FightingRoom");
            }
            else
                Debug.LogError(response.Errors.JSON);
        });
    }

    public void matchFoundListener()
    {
        GameSparks.Api.Messages.MatchFoundMessage.Listener = message =>
        {
            if (!message.HasErrors)
                matchID = message.MatchId;
        };
    }

    public void setListenersOnSceneChange(Scene old, Scene news)
    {
        if (chatmanager == null)
            chatmanager = GameObject.Find("ChatInput").GetComponent<ChatManager>();
        chatmanager.username = username;
        chatmanager.playerAuth = this;
        matchUpdatedListener();
        startChatListener();
    }

    void matchUpdatedListener()
    {
        GameSparks.Api.Messages.MatchUpdatedMessage.Listener = message =>
        {
            matchID = message.MatchId;
        };
    }

    void startChatListener()
    {
        GameSparks.Api.Messages.ScriptMessage.Listener = message =>
        {

            if (!message.HasErrors)
            {
                Debug.Log("got message");
                var mssg = message.Data.GetString("Message");
                var dname = message.Data.GetString("displayName");
                chatmanager.addChatMessage(dname, mssg);
            }
            else
                Debug.Log(message.Errors);

        };
    }

    public void sendMessageToAll(string leName, string leMessage)
    {
        Debug.Log(matchID);
        if (matchID == null)
            return;

        new GameSparks.Api.Requests.LogEventRequest()
         .SetEventKey("Chat_ToAll")
         .SetEventAttribute("Message", leMessage)
         .SetEventAttribute("MatchID", matchID)
         .Send(response =>
         {

         });
    }

}
