using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawneadorDelPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private float delay = 0;

    private void Update() 
    {
        if(!player.activeSelf)
        {
            StartCoroutine(respawnear());
        }    
    }

    private IEnumerator respawnear()
    {
        yield return new WaitForSeconds(delay);

        player.SetActive(true);
    }
}
