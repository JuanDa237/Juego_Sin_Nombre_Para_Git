using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVida : MonoBehaviour
{
    public Transform[] spawns;
    public GameObject botiquinPrefab;
    
    private GameObject[] botiquines;
    private int puntaje;
    private GameObject[] player;
	private VidaJugador vidaJugador;

    void Awake() 
    {
        player = GameObject.FindGameObjectsWithTag("Player");
		vidaJugador = player[0].GetComponent<VidaJugador>(); 
    }
    
    void Start() 
    {
        botiquines = new GameObject[spawns.Length];

        for (int i = 0; i < spawns.Length; i++) 
        {
		    botiquines[i] = Instantiate(botiquinPrefab, spawns[i].position, Quaternion.identity, spawns[i]);
            botiquines[i].SetActive(false);
		}
    }

    public void activarBotiquin(int botiquin)
    {
       botiquines[botiquin].SetActive(true);
    }
}
