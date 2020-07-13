using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorPlayer : MonoBehaviour
{
    public float delayTiempo = 1f;
    private bool atacarDeNuevo = true;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player") && atacarDeNuevo)
        {
            gameObject.SendMessageUpwards("hayPlayer");
            StartCoroutine(delay());
        }
    }

    private IEnumerator delay()
    {
        atacarDeNuevo = false;

        yield return new WaitForSeconds(delayTiempo);

        atacarDeNuevo = true;
    }
}
