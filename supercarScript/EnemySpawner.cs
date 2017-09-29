using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class EnemySpawner : NetworkBehaviour {

    public GameObject enemyPrefab;
    public int numEnemies;
    public string[] botNames;
    public List<string> botNamesRandom;
    int currentNameIndex;

    public static EnemySpawner instance;
    
    void Awake()
    {
        instance = this;
        currentNameIndex = 0;
        var random = new System.Random();
        botNamesRandom = botNames.OrderBy(order => random.Next()).ToList<string>();
    }

    public override void OnStartServer()
    {
        for (int i = 0; i < numEnemies; i++)
        {
            Debug.Log("Spawn enemy " + i);
            Transform t = SupercarsNetworkManager.instance.GetStartPosition();
            var enemy = (GameObject)Instantiate(enemyPrefab, t.position, t.rotation);
            enemy.GetComponent<AIPlayer>().spawnPos = t.position;
            enemy.GetComponent<AIPlayer>().spawnRot = t.rotation;
            enemy.GetComponent<AIPlayer>().playerName = "Bot "+GenerateName();
            enemy.gameObject.name = enemy.GetComponent<AIPlayer>().playerName;
            SupercarsNetworkManager.instance.playerSlots[i] = enemy.GetComponent<AIPlayer>(); //populate all slots with bots and then replace them with joining clients (see SupercarsNetworkManager.OnServerAddPlayer() )

            if (Random.value > 0.5f)
            {
                //the spawner only exists on the server so setting the team needs no command and it gets synced to the clients
                enemy.GetComponent<Combat>().team = "Team1";
            }
            else
            {
                enemy.GetComponent<Combat>().team = "Team2";
            }

            NetworkServer.Spawn(enemy);
        }
    }

    public string GenerateName()
    {
        if (currentNameIndex==botNamesRandom.Count)
        {
            currentNameIndex = 0;
        }
        return botNamesRandom[currentNameIndex++];
    }

}
