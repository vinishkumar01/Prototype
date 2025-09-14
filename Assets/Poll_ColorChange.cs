using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poll_ColorChange : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        this.sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collided with: " + collision.name + " | Tag: " + collision.tag);

        if (collision.CompareTag("Bullet"))
        { 
            this.sprite.color = Color.red;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            this.sprite.color = Color.white;
        }
    }
}
