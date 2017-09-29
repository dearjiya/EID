using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

//handling the gameflow
//a lot of publics, but I like to have a look at them in the editor
public class GameManager : NetworkBehaviour {

    public static GameManager instance; //for easier access on the game manager from other scripts

    public List<Player> players = new List<Player>();
    public Player localPlayer;

    [SyncVar] //gameState is normaly update by an RPC call so syncvar is not essential for this. however if a client joins late when the game is running this ensures his gamemanager has the right state
    public string gameState="waiting for players";

    //GUI stuff
    public Text chatText;
    public InputField chatInput;
    public Text readyText;
    public Button readyButton;
    public Text standingsText;
    public Image countdownImage;
    public Sprite[] countdownSprites;
    public GameObject finalStandingsPanel;
    public Text finalStandingsText;

    [SyncVar]
    public int maxRounds = 1; //because this can be set in editor durng runtime
    public int maxCheckpoint;
    public Checkpoint[] checkpoints;

    public float racetime = 0f; //only the server counts the time and sets the final time on players and bots when they reach the finishline. this final time is then synced to all other clients

    void Awake()
    {
        instance = this;
        checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        System.Array.Sort(checkpoints); //order by checkpointNr
        maxCheckpoint = checkpoints[checkpoints.Length-1].checkpointNr; //or checkpoints.count if the numbering is right
    }

    void Update()
    {
        if (isServer && gameState == "racing")
        {
            racetime += Time.deltaTime;
        }
    }
    [Server]
    public void AddPlayer(Player p)
    {
        players.Add(p);
    }
    [Server]
    public void RemovePlayer(Player p)
    {
        players.Remove(p);
    }

    public void DisplayMessage(string m)
    {
        chatText.text += "\n" + m;
    }

    //when the clicks the ready-button, a command is called to set and sync his state to "ready" or "not ready". after that this function is called
    [Server]
    public void CheckAllPlayersReady()
    {
        bool allReady = true;
        foreach (Player p in players)
        {
                allReady = allReady && (p.state == "ready");
        }
        if (allReady) ServerGamestateTransition("countdown");
    }

    //when the player crosses a checkpoint, a command sets and syncs that fact to the clients. if a lap is finished the currentLap gets incremented and if maxRounds is reached the player state changes to "finished".
    //after that this check is called
    [Server]
    public void CheckAllPlayersFinished()
    {
        bool allFinished=true;

        foreach (Player p in players)
        {
            allFinished = allFinished && p.state == "finished";
        }
        if (allFinished)
        {
            Invoke("ServerFinishRace",1f); //if we call the function immediately, the last finished driver has a time of 0 because the finalRacetime has not been synced yet
        }
    }

    [Server]
    void ServerGamestateTransition(string targetState)
    {
        gameState = targetState;
        switch (gameState)
        {
            case "countdown":
                {
                    ResetEnemys();
                    localPlayer.state="starting";
                    localPlayer.Reset();
                    StartCoroutine(ServerCountdown());
                    break;
                }
            case "racing":
                {
                    SetEnemysState("racing");
                    localPlayer.state="racing";
                    racetime = 0f;
                    break;
                }
        }
        RpcClientGamestateTransition(targetState); //handle the local stuff like GUI etc.
    }

    [Server]
    void SetEnemysState(string s)
    {
        foreach(AIPlayer ai in FindObjectsOfType<AIPlayer>())
        {
            ai.state = "racing";
        }
    }

    [Server]
    void ResetEnemys()
    {
        foreach(AIPlayer ai in FindObjectsOfType<AIPlayer>())
        {
            ai.Reset();
        }
    }

    //countdown timing is run only on the server...
    [Server]
    IEnumerator ServerCountdown()
    {
        for (int i = 0; i < 4; i++)
        {
            RpcClientCountdown(i);
            yield return new WaitForSeconds(1f);
        }
        ServerStartRace();
    }

    //...and the clients get informed when the countdown progresses and they change their countdown image locally
    [ClientRpc]
    void RpcClientCountdown(int step)
    {
        countdownImage.sprite = countdownSprites[step];
    }

    [Server]
    void ServerStartRace()
    {
        ServerGamestateTransition("racing");
    }

    [Server]
    void ServerFinishRace()
    {
        ServerGamestateTransition("waiting for players");
    }

    [ClientRpc]
    void RpcClientGamestateTransition(string targetState)
    {
        if (!isServer) gameState = targetState; //has already been set for the server
        switch (gameState)
        {
            case "countdown":
                {
                    countdownImage.enabled = true;
                    readyButton.interactable = false;
                    readyText.text = "Starting...";
                    break;
                }
            case "racing":
                {
                    countdownImage.enabled = false;
                    readyButton.gameObject.SetActive(false);
                    break;
                }
            case "waiting for players":
                {
                    UpdateStandings(); //player state is already correct in the GUI but the gamestate needs to be updated
                    finalStandingsPanel.SetActive(true);
                    finalStandingsText.text = "";
                    //yes, would be super clever to have a common base class for players and bots...it grew like that from the Unity tutorial and needs to be reworked ;)
                    List<NetworkBehaviour> allPlayers = new List<NetworkBehaviour>(FindObjectsOfType<Player>());
                    foreach (AIPlayer ai in FindObjectsOfType<AIPlayer>()) //allPlayers.AddRange(FindObjectsOfType<AIPlayer>());
                    {   //we use foreach here instead of AddRange, because all still racing bots need to get an estimated final racetime
                        allPlayers.Add(ai);
                        if (ai.state=="racing")
                        {
                            ai.state = "ready";
                            //15 sec. per lap and 2.5 per checkpoint...this has to be done differently in respect to the actual level later
                            ai.finalRacetime=racetime+(maxRounds-1-ai.currentLap) * 15f + (maxCheckpoint-ai.currentCheckpoint) * 2.5f + Random.value * 2f;
                        }
                    }
                    //I know...crazy shit. Sort the players by their final racetime using lambda expressions and some nasty casting...gets simpler once we got the common base class :)
                    allPlayers.Sort((x, y) => ((x is Player) ? (x as Player).finalRacetime : (x as AIPlayer).finalRacetime).CompareTo((y is Player) ? (y as Player).finalRacetime : (y as AIPlayer).finalRacetime));    
                    
                    //now building the end results as one string
                    foreach (NetworkBehaviour nb in allPlayers)
                    {
                        //have I mentioned the base class...?
                        if (nb is Player)
                            finalStandingsText.text = finalStandingsText.text + (nb as Player).playerName + " (" + (nb as Player).state + "): " + Mathf.Round((nb as Player).finalRacetime*100f)/100f + "Sec.\n";
                        if (nb is AIPlayer)
                            finalStandingsText.text = finalStandingsText.text + (nb as AIPlayer).playerName + " " + Mathf.Round((nb as AIPlayer).finalRacetime * 100f) / 100f + "Sec.\n";
                    }

                    UpdateStandings();
                    break;
                }
        }
    }

    [ClientRpc]
    public void RpcUpdateStandings()
    {
        Debug.Log("RPC standings");
        UpdateStandings();
    }
    
    //this could be more efficient, a List could be updated from only one player/bot when it has changed and here only that List should be run through, but for 4 cars its fine
    public void UpdateStandings()
    {
        //Debug.Log(maxRounds+"UpdateStandings - isServer:" + isServer + ", isLocalPlayer:" + isLocalPlayer + ", name:" + gameObject.name );
        standingsText.text = gameState+"\n";
        foreach (Player p in FindObjectsOfType<Player>())
        {
            standingsText.text = standingsText.text + p.gameObject.name + " (" + p.state + "): " + p.currentLap + " / "+maxRounds+"\n";
        }
        foreach (AIPlayer ai in FindObjectsOfType<AIPlayer>())
        {
            standingsText.text = standingsText.text + ai.gameObject.name + " (" + ai.state + "): " + ai.currentLap + " / " + maxRounds + "\n";
        }
    }

    /////--- UI ---////////////////
    public void UIReady()
    {
        if (localPlayer.state == "ready")
        {  //state will be set to "not ready" in the command below, so here the button text needs to be adjusted
            readyText.text = "I'm ready!";
        } else if(localPlayer.state=="not ready")
        {
            readyText.text = "I'm not ready!";
        }
        localPlayer.CmdToggleReadyState();
        
    }
    public void UICloseFinalStandings()
    {
        finalStandingsPanel.SetActive(false);
        readyButton.interactable = true;
        readyButton.gameObject.SetActive(true);
        localPlayer.CmdSetState("not ready");
        readyText.text = "I'm ready!";  //...=localPlayer.state  would not work here for the client who is NOT the host, because the CmdSetState sets the state on the server/host, but I GUESS it is not instantly synced to the clients but in the next Update(). puuuuhhhhh...(http://docs.unity3d.com/Manual/UNetStateSync.html)

    }
    public void UISendMessage()
    {
        // Debug.Log("UISendMessage");
        StringMessage sm = new StringMessage(localPlayer.playerName+":"+chatInput.text);
        chatInput.text = "";
        //NetworkServer.SendToAll(CarMsgId, cm); //does not work as non-server-client, I guess because we can't pretend to be the server and message all clients, so we send as client which is received by the server (see the message handler in the SupercarsNetworkManager) and there it gets sent to all clients
        SupercarsNetworkManager.instance.client.Send(SupercarsNetworkManager.CarMsgId, sm);
    }
}
