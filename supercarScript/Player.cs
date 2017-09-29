using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SyncVar (hook="HookStateChanged")] //hook is needed for GUI update 
    public string state = "not ready";

    public int playerId;

    float accumRot = 0f; //for the rotation in 10degree angles for the retro feeling ;)
    public GameObject bulletPrefab;

    Rigidbody2D rBody;

    [SyncVar]
    public int currentCheckpoint = 0;

    [SyncVar(hook ="HookLapChanged")]
    public int currentLap = -1;

    [SyncVar(hook = "HookSetPlayerName")]
    public string playerName;

    [SyncVar (hook ="HookSetFinalRacetime")]
    public float finalRacetime = 0f; //is set by the server in the finish, the hook is needed to set it instantly on the client, otherwiese the time will be 0 in the final standings
                                     //(because the automatic setting is done after the next upate()?

    public GameObject selectedCar;
    Vector3 selectionOffset;

    [SyncVar]
    public Vector3 spawnPos;
    [SyncVar]
    public Quaternion spawnRot;

    

    void HookStateChanged(string s)
    {
      Debug.Log("HookStateChanged("+s+"): isServer:" + isServer + ", isLocalPlayer:" + isLocalPlayer + ", name:" + gameObject.name + "name:" + name);
        state = s;
        GameManager.instance.UpdateStandings();
        if (state == "starting")
        {

        } else if (state == "racing")
        {

        } else if (state == "finished")
        {

        }
    }

    void HookSetFinalRacetime(float t)
    {
        finalRacetime = t;
    }

    void HookLapChanged(int lap)
    {
        //Debug.Log("isServer:" + isServer + ", isLocalPlayer:" + isLocalPlayer + ", name:"+gameObject.name+"lap:" + lap);
        currentLap = lap;
        GameManager.instance.UpdateStandings();
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        Debug.Log("localstart playername=" + playerName+" isServer: "+isServer);

        rBody = GetComponent<Rigidbody2D>();

        GameManager.instance.localPlayer = this;

        //when player has taken over a bot then he may be in the middle of a race, so he took the state of the bot
        switch (state)
        {
            case "not ready":
                GameManager.instance.readyText.text = "I'm ready";
                break;
            case "racing":
            case "finished":
                GameManager.instance.readyButton.gameObject.SetActive(false);
                break;
        }
    }

    public override void OnStartServer()
    {
        //give some random name, has to be customizable later on
        playerName = "Player" + Random.Range(1, 100);
        GameManager.instance.AddPlayer(this);
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        gameObject.name = playerName; //when a client connects later, see object creation flow (http://docs.unity3d.com/Manual/UNetSpawning.html)
        GameManager.instance.UpdateStandings();            //when the player object is created, the syncvars get updated, so this playerobject gets his name set but the hook is not called!
                                      //so the gameobject is not renamed. after that onstartclient is called, soe here the GO name is set
                                      //Hint: if you don't want to use syncvars but instead use a command for changing the playername, the command can update the clients with RPC calls
                                      //but this will only work for already connected clients. new clients wont recevie those rpc calls, they are not buffered somewhere!
        
    }

    void HookSetPlayerName(string name)
    {
        //normaly you would use the playerName in the updateStandings() method and you may omit this hook but just for the ske of testing: this is how the gameobject can be renamed as well
        //warning: this hook does NOT get called if a new player joins, he will only get his syncvar synchronized! (http://forum.unity3d.com/threads/syncvar-hook-not-called-when-client-joins-game.341710/)
        //hence in the OnStartClient() function the gameobject.name gets updated with the playerName
        //again: this is just for testing how things work, normaly the GUI would display the playerName directly and not the gameObject.name (but it is nice to the gameobject being renamed in the hierarchy view in the editor)
        //Debug.Log("HookSetPlayerName: isServer:" + isServer + ", isLocalPlayer:" + isLocalPlayer + ", name:"+gameObject.name+"name:" + name);
        playerName = name;
        gameObject.name = name;
        GameManager.instance.UpdateStandings();
    }

   


    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer || state=="starting" || state=="finished")
            return;

        

        float dist = Input.GetAxis("Vertical") * Time.deltaTime*180f;
        float rot = Input.GetAxis("Horizontal") * Time.deltaTime*160f*(Mathf.Abs(dist)>0?1.0f:0f)*Mathf.Sign(dist); //only rotate while driving
        
        if (rot == 0) accumRot = 0;
        else accumRot += rot;

        if (Mathf.Abs(accumRot) > 10f)
        {   //has the player selected another car? then rotate that one
            if (selectedCar != null)
            {
                selectedCar.transform.Rotate(new Vector3(0f, 0f, -10f * Mathf.Sign(accumRot)));
            }
            else
            {
                Debug.Log(rBody.rotation);
                transform.Rotate(new Vector3(0f, 0f, -10f * Mathf.Sign(accumRot))); // rBody.rotation+= -10f * Mathf.Sign(accumRot); //somehow this gets smoothed by the network transform so using Rotate() for now?
            }
           
            accumRot = 0f;
        }

        //has the player selected another car? then move that one
        if (selectedCar != null)
        {
            selectedCar.GetComponent<Rigidbody2D>().velocity = selectedCar.transform.up * dist;
        }
        else
        {
            rBody.velocity = transform.up * dist;
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire(); //creates bullet on server and spawns to the client
        }
        if (Input.GetMouseButtonDown(0))
        {
            Physics2D.queriesStartInColliders = true; //otherwise the raycast cant see the colliders under the mouse
            RaycastHit2D hit=Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
            Physics2D.queriesStartInColliders = false; //turn back on for the "evasion move" of the AI
            //Debug.Log(hit.collider);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                //did we select on bot of our own team?
                if (hit.collider.gameObject.GetComponent < AIPlayer >()!= null && hit.collider.gameObject.GetComponent<Combat>().team == gameObject.GetComponent<Combat>().team)
                {
                    selectedCar = hit.collider.gameObject;
                    selectionOffset = gameObject.transform.position-selectedCar.transform.position;
                    selectedCar.GetComponent<AIPlayer>().isControlledExternaly = true; //the the bot to stop moving on its own
                } else if (hit.collider.gameObject==gameObject && selectedCar!=null)
                {
                    //select own car
                    selectedCar.GetComponent<AIPlayer>().isControlledExternaly = false;
                    selectedCar = null;
                    selectionOffset = Vector3.zero;
                }
                
            }
            else if (selectedCar!= null)
            {
                //select nothing
                selectedCar.GetComponent<AIPlayer>().isControlledExternaly = false;
                selectedCar = null;
                selectionOffset = Vector3.zero;
            }
        }
        //old for moving own car and bot in parallel
        // if (selectedCar != null)
        // {
        //     selectedCar.transform.position = gameObject.transform.position - selectionOffset;
        // }
        
        //don't know why exactly but if cam does the follow in its own update() then the car jitters, has something to do with physics/rigidbody I think
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

    }

        
    [Command]
    void CmdFire()
    {
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position + transform.up/2f, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = 4 * transform.up;
        //spawn on same physics- and sprite-layer as the car, the triggers will do the rest. otherwise if the car is under the bridge the bullet would spawn above it
        bullet.GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
        bullet.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        bullet.layer = gameObject.layer;
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2.0f);
    }

    [Command]
    public void CmdSetState(string s)
    {
        state = s; //GUI gets updated with the local hook
    }
    [Command]
    void CmdSetPlayerName(string name)
    {
       // Debug.Log("CmdSetPlayerName: isServer:" + isServer + ", isLocalPlayer:" + isLocalPlayer + ", name:" + gameObject.name + "name:" + name);
        //gets synced to the client by hook (see above)
        playerName = name;
    }

    [Command]
    public void CmdToggleReadyState()
    {
        if (state == "not ready")
        {
            state = "ready";
            GameManager.instance.CheckAllPlayersReady();
        }
        else
        {
            state = "not ready";
        }

        //Debug.Log("CmdToogleState after setting: isServer:" + isServer + ", isLocalPlayer:" + isLocalPlayer + ", name:" + gameObject.name + "name:" + name);
    }

    [Server]
    public void Reset()
    {
        currentLap = -1;
        currentCheckpoint = 4;
        RpcResetPosition(true); //spawnPos is only saved on the local object, not on the server! so calling clients for also moving the camera (has to be done locally because camera has different pos on every client)
        GetComponent<Combat>().RestoreHealth();      
    }

    [ClientRpc]
    public void RpcResetPosition(bool toSpawn)
    {
        ResetPosition(toSpawn); //split up to another funciton, because that is also called from another RPC in Combat-script (and a client can't call RPCs!)
    }

    public void ResetPosition(bool toSpawn)
    {
        if (!isLocalPlayer) return;
        if (toSpawn) //e.g. starting race
        {
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }
        else //e.g. after explosion
        {
            transform.position = GameManager.instance.checkpoints[currentCheckpoint].transform.position;
            transform.rotation = GameManager.instance.checkpoints[currentCheckpoint].transform.rotation;
        }
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        //more care has to be given here if a checkpoint is below the brigde
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Cars";
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
        
    

    [Command]
    public void CmdSetCurrentCheckpoint(int cpNr, bool isFinish)
    {
        if (!isServer) return;
        
        if (cpNr == currentCheckpoint + 1 || cpNr==0 && currentCheckpoint==GameManager.instance.maxCheckpoint)
        {
            currentCheckpoint = cpNr;
            if (isFinish)
            {
                currentLap++;
                if (currentLap == GameManager.instance.maxRounds)
                {
                    finalRacetime = GameManager.instance.racetime; //if current time should be displayed then take it only locally and in the finish overwrite it with the server time (for avoiding net traffic)
                    state = "finished";
                    GameManager.instance.CheckAllPlayersFinished();
                } 
            }
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //this is complicated. because it is physics.
        //if a car A hits another car B, he gets damage and the physics engine places A out of the collision box
        //a bit later the position gets synced via network transform to the client of car B but since A now does not overlap with B, no collision event is fired on car B
        //if I subtract health from car B in this collision function this will only be locally on my client because health is not synced from client to client, just from the server to client
        //also I can't call a command for the client with car B because I don't have the authority to do so
        //my solution so far: I tell the server: collision! give damage to me and the collision partner. if I send myself as collision partner the CmdTakeDamage will only give damage once to me (can happen when colliding wiht walls etc. they don't have health)

        Debug.Log("col isLocal: " + isLocalPlayer);
        if (!isLocalPlayer) return;
        if (col.gameObject.tag == "Bullet") return; //bullets handle their damage in their own script, they are server objects and can change the health and sync it
        NetworkInstanceId colPartner = this.netId; //my network ID
        if (col.gameObject.GetComponent<Combat>() != null) colPartner = col.gameObject.GetComponent<Combat>().netId; //if collision partner as combart script then take his ID
        gameObject.GetComponent<Combat>().CmdTakeDamage(colPartner, 5f + Random.value * 1f); // I have to use the netId of the collision partner here, because commands cant be called with "complex" objects as parameters because they have to be transfered over the net
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isLocalPlayer) return;
        //Debug.Log("trigger enter");
        if (col.gameObject.GetComponent<Checkpoint>() != null)
        {
            Checkpoint cp = col.gameObject.GetComponent<Checkpoint>();
            //Debug.Log("calling cmdsetcheck "+cp.checkpointNr);
            CmdSetCurrentCheckpoint(cp.checkpointNr, cp.isFinish); //Cmd because the laps should be synced on all clients because they are shown in the GUI (could be optimized by only calling cmd when a lap change happens)
        }
    }


}
