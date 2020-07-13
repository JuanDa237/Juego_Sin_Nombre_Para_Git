using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Shuriken : MonoBehaviour
{
    [SerializeField] private int daño = 1;
	[SerializeField] private float velocidad = 2f;
    [SerializeField] private float tiempoDeVida = 8f;
	public Vector2 direccion;
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorDeVuelta = new Vector4(0, 0, 0, 0);

    //Variables Privadas
    private float _tiempoDeinicio;
    private GameObject ultimoObjectoGolpeado = null;

    private Rigidbody2D _rigidbody;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();    
    }

    private void Start() 
    {
        _tiempoDeinicio = Time.time;
    }

    private void Update() 
    {
        float _tiempoDeInicio2 = Time.time - _tiempoDeinicio;
		float _porcentajeCompleto = _tiempoDeInicio2 / tiempoDeVida;

		if(_porcentajeCompleto >= 1f) 
        {
			Destroy(gameObject, 0.1f);
		}    
    }

    private void FixedUpdate() 
    {
		_rigidbody.velocity = direccion.normalized * velocidad;

        if(direccion.normalized.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * direccion.normalized.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(ultimoObjectoGolpeado != collision.gameObject)
        {
            if(collision.CompareTag("Hurtbox"))
            {
                ultimoObjectoGolpeado = collision.gameObject;
                collision.SendMessage("añadirDañoAlPadre", daño);
                Destroy(gameObject, 0.1f);
            }
            else if(collision.CompareTag("Mapa"))
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }

    public void añadirDaño(int daño)
    {
        GetComponent<SpriteRenderer>().material.color = colorDeVuelta;
        direccion *= -1;
    }
}
