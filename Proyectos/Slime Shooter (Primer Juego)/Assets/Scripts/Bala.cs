using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public int daño = 1;
	public float velocidad = 2f;
	public Vector2 direccion;

	public float tiempoVida = 3f;
	public Color colorInicial = Color.white;
	public Color colorFinal;
	
    public GameObject explosion;

    private SpriteRenderer _renderer;
	private Rigidbody2D _rigidbody;
    private float _tiempoDeinicio;

	private int contadorDeColisiones = 0; // para evitar que quite vida 2 veces tanto como cuando hace la animacion como cuando coliciona

    void Awake() 
    {
        _renderer = GetComponent<SpriteRenderer>();
		_rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _tiempoDeinicio = Time.time;
    }
    
    void Update()
    {
        float _tiempoDeInicio2 = Time.time - _tiempoDeinicio;
		float _porcentajeCompleto = _tiempoDeInicio2 / tiempoVida;

		_renderer.color = Color.Lerp(colorInicial, colorFinal, _porcentajeCompleto);

		if(_porcentajeCompleto >= 1f) 
        {
			Explotar();
		}
    }

    private void FixedUpdate()
	{
		Vector2 movimiento = direccion.normalized * velocidad;
		_rigidbody.velocity = movimiento;
	}

    public void Explotar()
	{
		velocidad = 0f;
		contadorDeColisiones++;

		_renderer.enabled = false;

		if (explosion) 
        {
			explosion.SetActive(true);
		}

		Destroy(gameObject, 1.5f);
	}

	private void OnTriggerEnter2D(Collider2D collision) 
	{
		if(collision.CompareTag("Player") || collision.CompareTag("Enemigo"))
		{
			if(contadorDeColisiones == 0)
			{
				Explotar();
				collision.SendMessageUpwards("añadirDaño", daño);
			}
		}
		else if (collision.CompareTag("Mapa"))
		{
			Explotar();
		}
	}
}
