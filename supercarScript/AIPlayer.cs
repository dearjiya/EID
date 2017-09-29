using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class AIPlayer : NetworkBehaviour {

    [SyncVar]
    public bool isControlledExternaly = false;
    [SyncVar]
    public int currentCheckpoint;
    [SyncVar]
    public int nextCheckpoint;
    [SyncVar]
    public int currentLap;
    [SyncVar]
    public float finalRacetime;
    [SyncVar]
    public string state="ready";
    [SyncVar] //when players join late this is needed, otherwise the bots name will be empty in the final raceresult
    public string playerName;

    Rigidbody2D rb;
    RaycastHit2D obstacleTest;

    float accumRot = 0f;

    public Vector3 spawnPos;
    public Quaternion spawnRot;

    float maxPatience = .25f; //for going around obstacles
    public float patience = .25f;
    public bool isAngry = false; // :D
    public float turniness=0f; //the sharper the turn the slower the car should be


    void Awake () {
        currentLap = -1;
        currentCheckpoint = 4;
        nextCheckpoint = 5; //has to be implemented in a more general way like doing a raycast and looking what checkpoint is next in front of the car
        rb = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        if (!isServer || isControlledExternaly || state != "racing")
            return;

        //calculate the vector to the next checkpoint and then the difference to the current angle of the car
        Vector3 v_diff = (GameManager.instance.checkpoints[nextCheckpoint].transform.position - transform.position);
        float atan2 = Mathf.Atan2(v_diff.y, v_diff.x);
        float desiredChange = Mathf.DeltaAngle(transform.rotation.eulerAngles.z,atan2 * Mathf.Rad2Deg - 90f); //yaay what a cool function :)
        if (Mathf.Abs(desiredChange) > 10f && !isAngry) //during obstacle avoidance ignore optimal angle
        {
            //accumulate degrees to only turn in 10 degree steps
            accumRot += Time.deltaTime * 160f*Mathf.Sign(desiredChange);
            if (Mathf.Abs(accumRot) > 10f)
            {
                //rb.rotation += 10f * Mathf.Sign(desiredChange);
                transform.Rotate(0f, 0f, 10f * Mathf.Sign(desiredChange));
                accumRot = 0f;
                turniness += 15f;
            }
        }
        turniness = Mathf.Clamp(turniness - Time.deltaTime * 80f, 0f,100f);
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg-90f),Time.deltaTime*5f);
        obstacleTest = Physics2D.Raycast(transform.position, transform.up, 0.5f,1<<gameObject.layer);
        Debug.DrawRay(transform.position, transform.up*0.5f, Color.red, 0.1f);
        if (obstacleTest.collider == null || isAngry) //when avoiding one obstacle it doesnt matter if there will be another
        {
            patience+= Time.deltaTime;
            if (patience >= maxPatience)
            {
                isAngry = false;
                patience = maxPatience;
                
            }
            rb.velocity = transform.up * (180f-turniness) * Time.deltaTime;
        }
        else
        {   //if blocked then wait until patience is running out (you know that feeling...:)
            patience -= Time.deltaTime;
            if (patience <= 0f)
            {
                isAngry = true;
                patience = 0f;
                obstacleTest = Physics2D.Raycast(transform.position, (transform.up+transform.right), 0.8f, 1 << gameObject.layer); //is it free on the right side of obstacle?
                Debug.DrawRay(transform.position, (transform.up + transform.right) *0.8f, Color.yellow, 0.2f);
                if (obstacleTest.collider == null)
                {
                    transform.Rotate(0f, 0f, -30f); //try to avoid
                }
                else
                {
                    transform.Rotate(0f, 0f, +30f); //try other side
                }
            }
            rb.velocity = transform.up*0f;
        }

	}

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("trigger enter AI");
        if (!isServer) return;
        if (col.gameObject.GetComponent<Checkpoint>() != null)
        {
            Checkpoint cp = col.gameObject.GetComponent<Checkpoint>();
            CmdSetCurrentCheckpoint(cp.checkpointNr, cp.isFinish); 
        }
    }

    [Command]
    public void CmdSetCurrentCheckpoint(int cpNr, bool isFinish)
    {
        //Debug.Log("set cp AI " + cpNr + " isServer" + isServer);
        if (!isServer) return;

        //Driving backwards (can be when player has control)
        if (cpNr == currentCheckpoint - 1 || currentCheckpoint==0 && cpNr== GameManager.instance.maxCheckpoint)
        {
            currentCheckpoint = cpNr;
            if (currentCheckpoint == GameManager.instance.maxCheckpoint-1)
            {
                nextCheckpoint = GameManager.instance.maxCheckpoint;
            } else
            {
                nextCheckpoint--;
            }
        }
        //Driving forwards
        if (cpNr == currentCheckpoint + 1 || cpNr == 0 && currentCheckpoint == GameManager.instance.maxCheckpoint)
        {
            currentCheckpoint = cpNr;
            if (currentCheckpoint == GameManager.instance.maxCheckpoint)
            {
                nextCheckpoint = 0;
            }
            else
            {
                nextCheckpoint++;
            }
            
            if (isFinish)
            {
                currentLap++;
                if (currentLap == GameManager.instance.maxRounds)
                {
                    finalRacetime = GameManager.instance.racetime;
                    state = "finished";
                }
                GameManager.instance.UpdateStandings();
            }
        }
     }

    [Server]
    public void Reset()
    {
        currentLap = -1;
        currentCheckpoint = 4;
        nextCheckpoint = 5;
        ResetPosition(true);
        state = "ready";
        GetComponent<Combat>().RestoreHealth();
    }

    public void ResetPosition(bool toSpawn)
    {
        if (toSpawn)
        {
            transform.position = spawnPos; //spawnpos is known here since the bots are created on the server
            transform.rotation = spawnRot;
        }
        else //after exploding
        {
            transform.position = GameManager.instance.checkpoints[currentCheckpoint].transform.position;
            transform.rotation = GameManager.instance.checkpoints[currentCheckpoint].transform.rotation;
        }

        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Cars";
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
