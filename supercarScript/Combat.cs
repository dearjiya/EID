using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour
{

    public const float maxHealth = 100f;
    public bool destroyOnDeath = false;
    public GameObject explosionPrefab;

    [SyncVar(hook = "UpdateTeam")]
    public string team = "Team1";

    [SyncVar(hook = "UpdateHealth")]
    public float health = maxHealth;

    void Start()
    {
        UpdateTeam(team);
    }

    [Command]
    public void CmdTakeDamage(NetworkInstanceId nId, float amount) //float amount)
    {
        Debug.Log("CmdTakeDamage " + amount);
        if (!isServer)
            return;

        health -= amount;
        if (this.netId!=nId) NetworkServer.FindLocalObject(nId).GetComponent<Combat>().health -= amount;
    }

    [Command]
    public void CmdSetTeam(string t)
    {
        //wird dann automatisch auf die clients gesynced doe den hook update team aufrufen
        team = t;
    }

    void UpdateTeam(string t)
    {
        team = t;
        Debug.Log("update team"+t);
        if (t == "Team1")
        {
            GetComponent<SpriteRenderer>().material.SetColor("_ColorCar", new Vector4((isLocalPlayer ? 1f : 0.6f), 0.2f, 0.2f, 1f));
        }
        else
        {
            GetComponent<SpriteRenderer>().material.SetColor("_ColorCar", new Vector4(0.2f, (isLocalPlayer ? 1f : 0.6f), 0.2f, 1f));
        }
    }
    void UpdateHealth(float h)
    {
        //Important: h is the new value, the local "health" will be updated at the end of the frame!
        //edit: true but only if there is no hook. if a hook function is used you have to assign the value to your local value manually
        health = h;
        gameObject.GetComponentInChildren<HealthBar>().UpdateBar(health);
        if (health <= 0)
        {
            if (destroyOnDeath)
            {
                NetworkServer.Destroy(gameObject);
            }
            else
            {

                Explode();
            }

        }
    }

   // [ClientRpc]
    public void Explode()
    {
        GameObject expl = (GameObject)Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        expl.transform.parent = gameObject.transform;
        Destroy(expl, 1.0f);

        StartCoroutine(Respawn());

    }

    IEnumerator Respawn()
    {

        if (GetComponent<Player>() != null) GetComponent<Player>().enabled = false;
        for (int i = 0; i < 9; i++)
        {
            GetComponent<SpriteRenderer>().enabled = i % 2 == 0;
            GetComponent<HealthBar>().hbSprite.enabled = i % 2 == 0;
            yield return new WaitForSeconds(0.25f);
        }
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<HealthBar>().hbSprite.enabled = true;
        if (GetComponent<Player>() != null) GetComponent<Player>().enabled = true;
        if (GetComponent<Player>() != null) GetComponent<Player>().ResetPosition(false); //nicht auf spawn zurück sondern auf letzten checkpunkt
        if (GetComponent<AIPlayer>() != null) GetComponent<AIPlayer>().ResetPosition(false);

        if (isLocalPlayer) CmdRestoreHealth();
    }

    [Command]
    public void CmdRestoreHealth()
    {
        RestoreHealth();
    }

    [Server]
    public void RestoreHealth()
    {
        health = maxHealth;
    }
}
