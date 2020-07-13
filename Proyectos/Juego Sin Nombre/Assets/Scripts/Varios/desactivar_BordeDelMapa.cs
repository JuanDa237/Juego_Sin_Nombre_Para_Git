using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desactivar_BordeDelMapa : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Hurtbox"))
        {
            collision.SendMessage("añadirDañoAlPadre", 100);
        }
    }
}
