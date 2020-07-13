using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Movimiento_Enemigo1 : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2;
    [SerializeField] private Transform puntoEnElSuelo = null;
    [SerializeField] private Transform puntoDelantero = null;
    [SerializeField] private LayerMask[] layerSuelo = null;
    [SerializeField] [Range(0.1f, 0.5f)] private float radioDeDistacia = 0.1f;

    [Header("Ataque")]
    [SerializeField] private LayerMask layerPlayer = 0;
    [SerializeField] private float distanciaDeDeteccion = 2;
    [SerializeField] private float tiempoDeApuntado = 0.5f;
	[SerializeField] private float tiempoDeTiro = 1.5f;
    [SerializeField] private Transform puntoDeDisparo = null;
    [SerializeField] private GameObject prefabShuriken = null;

    [Header("Animaciones")]
    [SerializeField] private ParticleSystem chispas = null;

    [Space(15)]
    [SerializeField] private Light2D luz = null;
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorAtacando = new Vector4(0, 0, 0, 0);

    [Header("Vida")]
    [SerializeField] private int vida = 3;

    [Space(10)]
    [SerializeField] private float fuerzaXPorGolpe = 0;
    [SerializeField] private float fuerzaYPorGolpe = 0;
    [SerializeField] private int tiempoDeRecuperacion = 0;
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorRecuperacion = new Vector4(0, 0, 0, 0);

    [Space(10)]
    [SerializeField] private GameObject prefabTextoDaño = null;
    [SerializeField] private Transform puntoDeAparicionTexto = null;

    //Variables Privadas

    private bool disparar;
    [ColorUsageAttribute(true,true)] private Color colorPrincipal;
    private bool fueAtacado;
    private bool atacando;
    private bool muriendo;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _sprite;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        colorPrincipal = _sprite.material.color;
    }
     
    private void FixedUpdate() 
    {
        Vector2 direccionDelRaycast = new Vector2(transform.localScale.x, 0);
        Vector2 direccionDelRaycastAlRevez = direccionDelRaycast * -1;

        bool estaElPlayer = false;

        if(Physics2D.Raycast(puntoDeDisparo.position, direccionDelRaycast, distanciaDeDeteccion, layerPlayer.value))
        {
            estaElPlayer = true;
        }
        else if(Physics2D.Raycast(puntoDeDisparo.position, direccionDelRaycastAlRevez, distanciaDeDeteccion, layerPlayer.value))
        {
            estaElPlayer = true;
            VoltearObjeto();
        }

        bool tocandoSuelo = false;
        bool haySuelo = false;

        for(int i = 0; i < layerSuelo.Length; i++)
        {
            if(!tocandoSuelo && Physics2D.OverlapCircle(puntoEnElSuelo.position, radioDeDistacia, layerSuelo[i]))
            {
                tocandoSuelo = true;
            }
            if(!haySuelo && Physics2D.OverlapCircle(puntoDelantero.position, radioDeDistacia, layerSuelo[i]))
            {
                haySuelo = true;
            }
        }

        if(!haySuelo && tocandoSuelo && !fueAtacado)
        {
            VoltearObjeto();
        }

        if(!estaElPlayer && !fueAtacado)
        {
            _rigidbody.velocity = new Vector2(velocidad, _rigidbody.velocity.y);
        }
        else if(estaElPlayer && tocandoSuelo && !atacando && !fueAtacado && !muriendo)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);

            disparar = true;
        }
    }

    void LateUpdate() 
    {
        if(disparar && !fueAtacado)
        {
            StartCoroutine(ApuntarYDisparar());
        }
        disparar = false;

        bool estaDisparando = _animator.GetCurrentAnimatorStateInfo(0).IsName("Disparando") || atacando;

        if(estaDisparando && !fueAtacado)
        {
            cambiarColor(colorAtacando);
        }
        else if(!estaDisparando && !fueAtacado)
        {
            cambiarColor(colorPrincipal);
        }
    }

    private IEnumerator ApuntarYDisparar()
    {
        atacando = true;

		yield return new WaitForSeconds(tiempoDeApuntado);

		_animator.SetTrigger("Disparar");

		yield return new WaitForSeconds(tiempoDeTiro);

		atacando = false;
    }

    private void VoltearObjeto()
	{
        velocidad *= -1;
		transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
	}

    public void Disparar()
    {
        chispas.Play();
        
		GameObject shuriken = Instantiate(prefabShuriken, puntoDeDisparo.position, Quaternion.identity) as GameObject;

		Shuriken componenteDelShuriken = shuriken.GetComponent<Shuriken>();

		if (transform.localScale.x < 0f)
        {
            componenteDelShuriken.direccion = Vector2.left;
		}
        else 
        {
			componenteDelShuriken.direccion = Vector2.right;
    	}
    }

    //Control De Vida

    public void añadirDaño(int daño)
    {
        vida -= daño;
        fueAtacado = true;

        if(vida <= 0)
        {
            muriendo = true;
        }

        GameObject texto = Instantiate(prefabTextoDaño, puntoDeAparicionTexto.position, Quaternion.identity, puntoDeAparicionTexto) as GameObject;
        texto.GetComponent<TextoDaño>().setTexto(daño);
        
        if(transform.localScale.x < 0)
        {
            _rigidbody.AddForce(new Vector2(fuerzaXPorGolpe, fuerzaYPorGolpe));
            texto.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _rigidbody.AddForce(new Vector2(fuerzaXPorGolpe * -1, fuerzaYPorGolpe));
        }

        StartCoroutine(recibioDaño());
    }

    private IEnumerator recibioDaño()
    {
        cambiarColor(colorRecuperacion);

        yield return new WaitForSeconds(tiempoDeRecuperacion);

        cambiarColor(colorPrincipal);

        fueAtacado = false;

        if(vida <= 0)
        {
            vida = 0;
            gameObject.SetActive(false);
        }
    }

    private void cambiarColor(Color color)
    {
        _sprite.material.color = color;
        luz.color = color;
    }
}
