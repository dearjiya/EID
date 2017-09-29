using UnityEngine;
using System.Collections;

//handling the GUI element of health
public class HealthBar : MonoBehaviour {

    public Transform healthBar;
    public SpriteRenderer hbSprite;
	
    //gets called by the combat script when "health" is updated (it is a syncvar there which has a hook)
    public void UpdateBar(float health)
    {   
        healthBar.localScale = new Vector3(2.0f * health / 100f, 0.4f, 1f);
        hbSprite.color = Color.Lerp(Color.red, Color.green, health / 100f);
    }
}
