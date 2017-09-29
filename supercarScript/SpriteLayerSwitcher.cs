using UnityEngine;
using System.Collections;

//Helper class to move the cars below the street in a tunnel visually and also avoiding colliders "above" them
public class SpriteLayerSwitcher : MonoBehaviour {

    public int newOrder;
    public string newLayer;
    public string newColLayer;

	void OnTriggerEnter2D(Collider2D col)
    {
        // Debug.Log("Enter" + gameObject.name+":"+col.gameObject.name);
        if (col.gameObject.GetComponent<SpriteRenderer>() != null)
        {
            col.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = newLayer;
            col.gameObject.GetComponent<SpriteRenderer>().sortingOrder = newOrder;
            col.gameObject.layer = LayerMask.NameToLayer(newColLayer);
        }
    }
}
