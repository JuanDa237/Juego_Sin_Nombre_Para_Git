using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Enemigo2 : MonoBehaviour
{
    public int dañoDeExplocion = 10;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Hurtbox"))
        {
            collision.SendMessage("añadirDañoAlPadre", dañoDeExplocion);
            gameObject.SendMessageUpwards("Explotar");
        }
    }
}
