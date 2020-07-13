using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puntoDeGuardado : MonoBehaviour
{
    [SerializeField] private Transform puntoDeRespawn = null;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player"))
        {
            collision.SendMessage("setUltimoPuntoDeGuardado", puntoDeRespawn.position);
        }
    }
}
