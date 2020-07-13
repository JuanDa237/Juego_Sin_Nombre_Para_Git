using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesactivarEnemigos : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        collision.attachedRigidbody.gameObject.SetActive(false);
    }
}
