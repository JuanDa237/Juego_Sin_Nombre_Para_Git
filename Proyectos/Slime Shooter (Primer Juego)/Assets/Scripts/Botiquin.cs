using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Botiquin : MonoBehaviour
{
    public int vidaQueRestaura = 1;
    public GameObject particulasIniciales;
    public GameObject particulasFinales;

    private SpriteRenderer _renderer;
    private Collider2D _collider;

    [SerializeField] private GameObject[] player;
	[SerializeField] private VidaJugador vidaJugador;

    void Awake() 
    {
        _renderer = GetComponent<SpriteRenderer>();    
        _collider = GetComponent<Collider2D>();   
        player = GameObject.FindGameObjectsWithTag("Player");
		vidaJugador = player[0].GetComponent<VidaJugador>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player"))
        {
            _renderer.enabled = false;
            _collider.enabled = false;
            particulasIniciales.SetActive(false);
            particulasFinales.SetActive(true);

            vidaJugador.añadirPuntaje(10);

            collision.SendMessageUpwards("añadirVida", vidaQueRestaura);
            Destroy(gameObject, 1f);
        }
    }
}
