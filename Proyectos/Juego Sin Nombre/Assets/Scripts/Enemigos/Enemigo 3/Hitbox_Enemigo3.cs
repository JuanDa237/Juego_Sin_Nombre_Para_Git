using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Enemigo3 : MonoBehaviour
{
    public int daño = 10;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Hurtbox"))
        {
            collision.SendMessage("añadirDañoAlPadre", daño);
        }
    }
}
