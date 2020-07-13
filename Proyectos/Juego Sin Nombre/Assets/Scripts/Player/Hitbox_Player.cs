using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Player : MonoBehaviour
{
    private PlayerController player;

    private void Awake() 
    {
        player = GetComponentInParent<PlayerController>();    
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Hurtbox"))
        {
            collision.SendMessage("añadirDañoAlPadre", player.getDaño());
        }
    }
}
