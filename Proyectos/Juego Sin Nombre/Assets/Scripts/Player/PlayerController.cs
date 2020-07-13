using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))] [RequireComponent(typeof(Animator))] [RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] [Range(100, 1000)] private float velocidad = 200;
    [SerializeField] [Range(0, 0.3f)] private float efectoSuavizante = 0.05f;

    [Header("Salto")]
    [SerializeField] [Range(5, 20)] private float fuerzaDeSalto = 7;
    [SerializeField] [Range(1, 5)] private int cantidadDeSaltos = 2;

    [Space(15)]
    [SerializeField] private Transform posicionSuelo = null;
    [SerializeField] private LayerMask layerSuelo = 0;
    [SerializeField] [Tooltip("Esta variable es el radio del raycast para mirar si hay suelo.")][Range(0, 1)] 
    private float radioDeDistacia = 0.1f;

    [Space(15)]
    [SerializeField] [Tooltip("Si activas esta variable debes de poner un valor en los dos numero de abajo.")]
    private bool efectoDeMantenerPulsado = false;
    [SerializeField] [Tooltip("Si el efecto esta desactivado este multiplicador no tendra ningun efecto en la jugabilidad.\nSi esta en 1 no tiene efecto.\nEsto determina que tanto debes mantener pulsado para saltar hasta el punto maximo.")] [Range(1, 5)]
    float multiplicadorDeSaltoBajo = 1.5f;
    [SerializeField] [Tooltip("Si el efecto esta desactivado este multiplicador no tendra ningun efecto en la jugabilidad.\nEsto determina la rapidez a la que caes.")]
    float multiplicadorDeCaida = 2f;

    [Space(15)]
    [SerializeField] [Tooltip("Si activas esta variable debes llenar los campos de abajo")]
    private bool interactuarConLaPared = false;
    [SerializeField] [Range(0, 5)] private int cantidadDeSaltosDesdeLaPared = 1;
    [SerializeField] private Transform posicionDelFrente = null;
    [SerializeField] private float velocidadDeDeslizamiento = 0;
    [SerializeField] private float fuerzaDelSaltoEnLaParedX = 15;
    [SerializeField] [Tooltip("Se recomienda la misma fuerza que tiene el salto normal")]
    private float fuerzaDelSaltoEnLaParedY = 7;

    [Header("Plataformas")]
    [SerializeField] [Tooltip("Si activas esta variable debes llenar los campos de abajo")]
    private bool configuracionParaPlataformas = false;
    [SerializeField] private LayerMask layerPlataformas = 0;
    [SerializeField] private int numeroLayerPlataformas = 0;

    [Header("Ataque")]
    [SerializeField] [Tooltip("Si activas esta variable debes llenar los campos de abajo")]
    private bool ataque = false;

    [Header("Particulas")]
    [SerializeField] [Tooltip("Si activas esta variable debes llenar los campos de abajo")]
    private bool particulas = false;
    [SerializeField] private ParticleSystem polvo = null;

    [Header("Vida")]
    [SerializeField] private int vida = 2;
    [SerializeField] private int daño = 2;

    [Space(10)]
    [SerializeField] private float fuerzaXPorGolpe = 300;
    [SerializeField] private float fuerzaYPorGolpe = 300;
    [SerializeField] private float tiempoDeRecuperacion = 0.5f;
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorFull = new Vector4(0, 0, 0, 0);
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorMedio = new Vector4(0, 0, 0, 0);
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorMalo = new Vector4(0, 0, 0, 0);
    [SerializeField] private Light2D luz = null;

    [Header("Punto De Guardado")]
    [SerializeField] [Tooltip("Si activas esto activaras el sistema de guardado")]
    private bool guardar = false;
    
    [Header("UI")]
    [SerializeField] private GameObject prefabTextoDaño = null;
    [SerializeField] private Transform puntoDeAparicionTexto = null;
    [SerializeField] private Image barraDeVida = null;
    [SerializeField] private Image caraPlayer = null;

    [Space(10)]
    [SerializeField] private Color colorFullRgb = new Color(0, 0, 0);
    [SerializeField] private Sprite imagenFull = null;
    [SerializeField] private Color colorMedioRgb = new Color(0, 0, 0);
    [SerializeField] private Sprite imagenMedio = null;
    [SerializeField] private Color colorMaloRgb = new Color(0, 0, 0);
    [SerializeField] private Sprite imagenMalo = null;

    //Variables Privadas

    private float _horizontalInput;
    private bool _salto;
    private bool _mirandoDerecha = true;
    private int _saltosRestantes;
    private bool pulsandoSalto;
    private bool pisandoSuelo;
    private bool _deslizandoseEnLaPared = false;
    private bool _saltoEnLaPared;
    private bool _atacando = false;
    private PlatformEffector2D _plataformas;
    [ColorUsageAttribute(true,true)] private Color colorPrincipal;
    private bool siendoAtacado;
    private float porcentajeCompleto;
    private GameObject ultimoTexto;
    private Vector3 scaleBarraInicial;
    private Vector3 ultimoPuntoDeGuardado;
    private int vidaInicial;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _sprite;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        vidaInicial = vida;
        porcentajeCompleto = 100 / vida;
        colorPrincipal = _sprite.material.color;
        scaleBarraInicial = barraDeVida.transform.localScale;    
    }

    private void Update() 
    {
        _horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");

        bool tocandoSuelo = Physics2D.OverlapCircle(posicionSuelo.position, radioDeDistacia, layerSuelo);
        bool tocandoPlataforma = configuracionParaPlataformas && Physics2D.OverlapCircle(posicionSuelo.position, radioDeDistacia, layerPlataformas);
        bool tocandoPared = interactuarConLaPared && Physics2D.OverlapCircle(posicionDelFrente.position, radioDeDistacia, layerSuelo) && _horizontalInput != 0;

        if(tocandoPared && !_saltoEnLaPared)
        {
            _saltosRestantes = cantidadDeSaltosDesdeLaPared;
            pisandoSuelo = true;
        }
        else if(tocandoSuelo || tocandoPlataforma)
        {
            _saltosRestantes = cantidadDeSaltos;
            pisandoSuelo = true;
        }
        else if(!tocandoSuelo || !tocandoPlataforma || !tocandoPared)
        {
            pisandoSuelo = false;
        }

        if(tocandoPlataforma && CrossPlatformInputManager.GetButtonDown("Bajar") && _plataformas != null)
        {
            _plataformas.rotationalOffset = 180;
            Invoke("volverPlataformaALaNormalidad", 0.5f);
        }

        if(interactuarConLaPared && _deslizandoseEnLaPared && CrossPlatformInputManager.GetButtonDown("Jump") && _saltosRestantes > 0 && !_atacando)
        {
            _saltoEnLaPared = true;
            _saltosRestantes--;
        }
        else if(CrossPlatformInputManager.GetButtonDown("Jump") && _saltosRestantes > 0 && !_atacando) 
		{
            if(_saltosRestantes == cantidadDeSaltos)
            {
                if(_rigidbody.velocity.y < 0 && (!tocandoSuelo || !tocandoPlataforma || !_deslizandoseEnLaPared) && _horizontalInput != -1) // el tocandoSuelo es por que cuando camino para la izquierda nose porque la velocidad en y es negativa entonces con esto evito que se le quite el salto cuando esta tocando el suelo. basicamente esto sirve para cuando esta callendo y tiene todo los saltos que se le quite uno adicional.
                {
                    _salto = true;
                    _saltosRestantes--;
                }
                else
                {
                    _salto = true;
                }
            }
            else
            {
                _salto = true;
            }

            _saltosRestantes--;
    	}

        if(CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            pulsandoSalto = true;
        }

        if(interactuarConLaPared && tocandoPared && (!tocandoSuelo || !tocandoPlataforma) && _horizontalInput != 0 && !_atacando)
        {
            _deslizandoseEnLaPared = true;
        }
        else
        {
            _deslizandoseEnLaPared = false;
        }

        if(ataque && CrossPlatformInputManager.GetButtonDown("Ataque1") && (!_deslizandoseEnLaPared || !_salto || !_atacando) && (tocandoSuelo || tocandoPlataforma) && !siendoAtacado)
        {
            _atacando = true;
            if(_rigidbody.velocity.y < 0)
            {
                _atacando = true;
            }
        }
    }

    private void FixedUpdate() 
    {
        Mover();
        _salto = false;
        _saltoEnLaPared = false;
        pulsandoSalto = false;
    }

    private void LateUpdate()
	{
        _animator.SetBool("Quieto", _horizontalInput == 0);
        _animator.SetBool("Deslizandose", _deslizandoseEnLaPared);
        _animator.SetBool("EnSuelo", pisandoSuelo);
        _animator.SetFloat("VelocidadVertical", _rigidbody.velocity.y);

        if(ataque && _atacando)
        {
            _animator.SetTrigger("Atacar");
            _atacando = false;
        }
        
	}

    // Este metodo lo hago para que sea mas facil realizar los movimientos en fixed update.
    private void Mover()
    {
        Vector3 velocidadTarget = new Vector2(_horizontalInput * Time.fixedDeltaTime * velocidad, _rigidbody.velocity.y);
        Vector3 noseParaQueSirve = Vector3.zero; // la verdad no entiendo para que es este, yo creo que es solo para llenar ese espacio que es necesario

        if(ataque && _animator.GetCurrentAnimatorStateInfo(0).IsName("Atacando") || siendoAtacado)
        {
            velocidadTarget = Vector3.zero;
            _salto = false;
        }

        _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, velocidadTarget, ref noseParaQueSirve, efectoSuavizante);

        if(_horizontalInput < 0f && _mirandoDerecha && !siendoAtacado)
        {
            VoltearObjeto();
        } 			
        else if(_horizontalInput > 0f && !_mirandoDerecha && !siendoAtacado)
        {
            VoltearObjeto();		
        }

        if(interactuarConLaPared && _saltoEnLaPared)
        {
            _rigidbody.velocity = new Vector2((fuerzaDelSaltoEnLaParedX * -_horizontalInput), fuerzaDelSaltoEnLaParedY);
            crearPolvo();
        }
        else if(_salto)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, fuerzaDeSalto); 
            crearPolvo();
        }

        if(interactuarConLaPared && _deslizandoseEnLaPared)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y, -velocidadDeDeslizamiento, float.MaxValue)); 
        }

        if(efectoDeMantenerPulsado)
        {
            if(_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (multiplicadorDeCaida - 1) * Time.fixedDeltaTime;
            }
            else if(_rigidbody.velocity.y > 0 && !pulsandoSalto)
            {
                _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (multiplicadorDeSaltoBajo - 1) * Time.fixedDeltaTime;
            }
            // Para ver la explicacion de esta fraccion de codigo meterse en este link. https://www.youtube.com/watch?v=7KiK0Aqtmzc&list=PL5KbKbJ6Gf99ehMwKiofNQNDfQRcTi0hf
        }
    }

    private void VoltearObjeto()
	{
		_mirandoDerecha = !_mirandoDerecha;
		transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        crearPolvo();

        if(ultimoTexto != null)
        {
            ultimoTexto.transform.localScale = new Vector3(ultimoTexto.transform.localScale.x * -1, ultimoTexto.transform.localScale.y, ultimoTexto.transform.localScale.z);
        }
	}

    private void volverPlataformaALaNormalidad()
    {
        _plataformas.rotationalOffset = 0;
        _plataformas = null;
    }

    private void crearPolvo()
    {
        if(particulas)
        {
            polvo.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.layer == numeroLayerPlataformas)
        {
           _plataformas = collision.gameObject.GetComponent<PlatformEffector2D>();
        }
    }

    //Control De Vida

    public void añadirDaño(int daño)
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Atacando"))
        {
            _animator.SetTrigger("Reset");
        }

        vida -= daño;
        siendoAtacado = true;

        ultimoTexto = Instantiate(prefabTextoDaño, puntoDeAparicionTexto.position, Quaternion.identity, puntoDeAparicionTexto) as GameObject;
        ultimoTexto.GetComponent<TextoDaño>().setTexto(daño);
        
        if(transform.localScale.x < 0)
        {
            _rigidbody.AddForce(new Vector2(fuerzaXPorGolpe, fuerzaYPorGolpe));
            ultimoTexto.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _rigidbody.AddForce(new Vector2(fuerzaXPorGolpe * -1, fuerzaYPorGolpe));
        }

        StartCoroutine(recibioDaño());
    }
    
    private IEnumerator recibioDaño()
    {
        float porcentajeActual = vida * porcentajeCompleto;
        
        if(vida > 0)
        {
            barraDeVida.transform.localScale = new Vector3((scaleBarraInicial.x * porcentajeActual)/100, scaleBarraInicial.y, scaleBarraInicial.z);
        }
        else if (vida <= 0)
        {
            barraDeVida.transform.localScale = new Vector3(0, 0, 0);
        }
        
        if(porcentajeActual >= 70)
        {
            cambiarColor(colorFull);
            cambiarColorUI(colorFullRgb);
            caraPlayer.sprite = imagenFull;
        }
        else if(porcentajeActual >= 40)
        {
            cambiarColor(colorMedio);
            cambiarColorUI(colorMedioRgb);
            caraPlayer.sprite = imagenMedio;
        }
        else if(porcentajeActual >= 0)
        {
            cambiarColor(colorMalo);
            cambiarColorUI(colorMaloRgb);
            caraPlayer.sprite = imagenMalo;
        }

        yield return new WaitForSeconds(tiempoDeRecuperacion);

        cambiarColor(colorPrincipal);

        siendoAtacado = false;

        if(vida <= 0)
        {
            vida = 0;
            gameObject.SetActive(false);
        }
    }

    private void cambiarColor(Color color)
    {
        GetComponent<SpriteRenderer>().material.color = color;
        luz.color = color;
    }

    private void cambiarColorUI(Color color)
    {
        if(ultimoTexto != null)
        {
            ultimoTexto.GetComponent<TextMesh>().color = color;
        }
        barraDeVida.color = color;
    }

    public int getDaño()
    {
        return daño;
    }

    //Guardado De Partida

    public void setUltimoPuntoDeGuardado(Vector3 punto)
    {
        if(guardar)
        {
            ultimoPuntoDeGuardado = punto;
        }
    }

    private void OnDisable() 
    {
        if(ultimoPuntoDeGuardado != null)
        {
            transform.position = ultimoPuntoDeGuardado;
        }
    }

    private void OnEnable() 
    {
        vida = vidaInicial;

        ultimoTexto = null;

        cambiarColor(colorPrincipal);
        cambiarColorUI(colorFullRgb);
        barraDeVida.transform.localScale = new Vector3(scaleBarraInicial.x , scaleBarraInicial.y, scaleBarraInicial.z);
    }
}
