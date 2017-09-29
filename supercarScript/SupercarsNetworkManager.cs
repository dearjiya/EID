using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using UnityEngine.UI;

public class SupercarsNetworkManager : NetworkManager
{
    public const short CarMsgId = 1000;

    const int maxPlayers = 4;
    public NetworkBehaviour[] playerSlots = new NetworkBehaviour[maxPlayers]; //would be better to have a base class like BasePlayer for humans ans bots

    public static SupercarsNetworkManager instance; //for easy access


    void Awake()
    {
        instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        // find empty player slot
        for (int slot = 0; slot < maxPlayers; slot++)
        {
            if (playerSlots[slot] is AIPlayer)
            {
                var enemy = (AIPlayer)playerSlots[slot];
                var playerObj = (GameObject)GameObject.Instantiate(playerPrefab, enemy.gameObject.transform.position, enemy.gameObject.transform.rotation); //spawn where the bot was and not on the spawn point
                var player = playerObj.GetComponent<Player>();
                Debug.Log("Adding player in slot " + slot+" with pos "+ (playerSlots[slot] as AIPlayer).spawnPos);
                player.playerId = slot;
                player.spawnPos = enemy.spawnPos;
                player.spawnRot = enemy.spawnRot;
                switch (enemy.state)
                {
                    case "ready":
                        player.state = "not ready";
                        break;
                    case "racing":
                        player.state = "racing";
                        break;
                    case "finished":
                        player.state = "finished";
                        break;
                }
                player.currentCheckpoint = enemy.currentCheckpoint;
                player.currentLap = enemy.currentLap;
                player.gameObject.GetComponent<Combat>().team = enemy.GetComponent<Combat>().team; 
                NetworkServer.Destroy(playerSlots[slot].gameObject);
                playerSlots[slot] = player;

                
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                return;
            }
        }

        //TODO: graceful  disconnect
        conn.Disconnect();
    }

    //swap the leaving player for a bot
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController)
    {
        Debug.Log("Spawn enemy after player left");
        // remove players from slots
        var player = playerController.gameObject.GetComponent<Player>();
        HotseatAI(player);

        base.OnServerRemovePlayer(conn, playerController);
        GameManager.instance.RpcUpdateStandings();
    }

    //swap the leaving player for a bot
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("NetMan OnServerDisconnect()");
        foreach (var playerController in conn.playerControllers)
        {
            var player = playerController.gameObject.GetComponent<Player>();
            HotseatAI(player);
            
        }

        base.OnServerDisconnect(conn);
        GameManager.instance.RpcUpdateStandings();
    }

    void HotseatAI(Player player)
    {
        playerSlots[player.playerId] = null;
        GameManager.instance.RemovePlayer(player);

        //Debug.Log(player.gameObject.transform.position + " " + player.spawnPos);
        var enemy = (GameObject)Instantiate(FindObjectOfType<EnemySpawner>().enemyPrefab, player.gameObject.transform.position, player.gameObject.transform.rotation); //place bot on the player position
        AIPlayer ai = enemy.GetComponent<AIPlayer>();
        ai.spawnPos = player.spawnPos;
        ai.spawnRot = player.spawnRot;
        ai.currentCheckpoint = player.currentCheckpoint;
        ai.nextCheckpoint = (player.currentCheckpoint == GameManager.instance.maxCheckpoint)?0:player.currentCheckpoint+1;
        ai.currentLap = player.currentLap;
        ai.playerName = "Bot: "+EnemySpawner.instance.GenerateName();
        enemy.name = enemy.GetComponent<AIPlayer>().playerName;
        enemy.GetComponent<Combat>().team = player.gameObject.GetComponent<Combat>().team;

        switch (player.state)
        {
            case "ready":
            case "not ready":
            case "starting":
                enemy.GetComponent<AIPlayer>().state = "ready";
                break;
            case "racing":
                enemy.GetComponent<AIPlayer>().state = "racing";
                break;
            case "finished":
                enemy.GetComponent<AIPlayer>().state = "finished";
                break;
        }
        playerSlots[player.playerId] = enemy.GetComponent<AIPlayer>();
        NetworkServer.Spawn(enemy);
    }


    //for the chat register message handler
    public override void OnStartClient(NetworkClient client)
    {   //Debug.Log("registering handler... name:" + gameObject.name+" client:"+client );
        //infos regarding messages: http://forum.unity3d.com/threads/chatting-with-new-api.328097/
        client.RegisterHandler(CarMsgId, OnCarMsgClient);
    }

    public override void OnStartServer()
    {
        Debug.Log("NetMan OnStartServer()");
        base.OnStartServer();
        NetworkServer.RegisterHandler(CarMsgId, OnCarMsgServer);
    }

    public override void OnStopServer()
    {
        Debug.Log("NetMan OnStopServer()");
        base.OnStopServer();  
    }

    //message receiver function fpr client
    void OnCarMsgClient(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<StringMessage>();
        Debug.Log("Client received: " + msg.value);
        GameManager.instance.DisplayMessage(msg.value);

    }
    //and for the server
    void OnCarMsgServer(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<StringMessage>();
        Debug.Log("Server received: " + msg.value);
        StringMessage sm = new StringMessage(msg.value);
        NetworkServer.SendToAll(CarMsgId, sm);

    }

    

    
}
