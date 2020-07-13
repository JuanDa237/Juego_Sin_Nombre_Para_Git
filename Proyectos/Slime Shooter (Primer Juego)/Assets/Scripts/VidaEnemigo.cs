using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    public int vida;
    public int puntos;
    
    private int vidaTotal;
    private SpriteRenderer _renderer;

    [SerializeField] private GameObject[] player;
	[SerializeField] private VidaJugador vidaJugador;

    void Awake() 
    {
        _renderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectsWithTag("Player");
		vidaJugador = player[0].GetComponent<VidaJugador>();
    }

    private void Start() 
    {
        vidaTotal = vida;
    }

    public void añadirDaño(int daño)
    {
        vidaTotal -= daño;
        
        if(vidaTotal <= 0)
        {
            vidaTotal = vida;
            vidaJugador.añadirPuntaje(puntos);
            StartCoroutine("morir");
        }
        else
        {
            StartCoroutine("FeedbackVisual");
        }
    }

    private IEnumerator FeedbackVisual()
	{
		_renderer.color = Color.green;

		yield return new WaitForSeconds(0.1f);

		_renderer.color = Color.white;
	}

    private IEnumerator morir()
	{
		_renderer.color = Color.green;

		yield return new WaitForSeconds(0.1f);

		_renderer.color = Color.white;
        gameObject.SetActive(false);
	}
}
