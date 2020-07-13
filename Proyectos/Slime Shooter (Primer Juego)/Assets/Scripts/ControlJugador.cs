using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ControlJugador : MonoBehaviour
{
    public float TiempolongIdle = 5f;
    public float velocidad = 2;
    public float fuerzaDeSalto = 2.5f;

    private Vector2 _movimiento;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    public Transform mirarSiToca;
	public LayerMask sueloLayer;
	public float radioDeMirarSiToca;

    private bool _mirandoDerecha = true;
    private bool _tocandoSuelo;
    private bool _Atacando;
    private float contadorDeSaltos;
    private float _ContadorlongIdle;

    public GameObject prefabBala;
    public Transform _Cañon;

    private int contadorDeDisparos; // esto es por si el player dispara mas de x veces reciba daño, para que no se quede quieto
    private VidaJugador vidaJugador;

    void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();   
        _animator = GetComponent<Animator>();
        vidaJugador = GetComponent<VidaJugador>();
    }

    void Update()
    {
        float horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
		_movimiento = new Vector2(horizontalInput, 0f);

		if (horizontalInput < 0f && _mirandoDerecha == true)
		{
            contadorDeDisparos = 0;
			Voltear();
		} 			
        else if (horizontalInput > 0f && _mirandoDerecha == false) 
		{
            contadorDeDisparos = 0;
			Voltear();			
        }

        _tocandoSuelo = Physics2D.OverlapCircle(mirarSiToca.position, radioDeMirarSiToca, sueloLayer);

        if (CrossPlatformInputManager.GetButtonDown("Jump") && _tocandoSuelo == true && _Atacando == false) 
		{
            contadorDeDisparos = 0;

			_rigidbody.AddForce(Vector2.up * fuerzaDeSalto, ForceMode2D.Impulse);

            if(contadorDeSaltos != 0)
            {
                contadorDeSaltos = 0;
            }

            contadorDeSaltos++;
    	}
        else if (CrossPlatformInputManager.GetButtonDown("Jump") && _tocandoSuelo == false && contadorDeSaltos < 2 && contadorDeSaltos != 0 && _Atacando == false)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, fuerzaDeSalto);
            contadorDeDisparos = 0;
            //_rigidbody.AddForce(Vector2.up * fuerzaDeSalto, ForceMode2D.Impulse);
            contadorDeSaltos = 0;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire1") && _tocandoSuelo == true && _Atacando == false) 
        {
			_movimiento = Vector2.zero;
			_rigidbody.velocity = Vector2.zero;
			_animator.SetTrigger("Attack");
		}
    }

    void FixedUpdate() 
    {
        if(!_Atacando)
        {
            float Velocidadhorizontal = _movimiento.normalized.x * velocidad;
		    _rigidbody.velocity = new Vector2(Velocidadhorizontal, _rigidbody.velocity.y);
        }
    }

    void LateUpdate()
	{
		_animator.SetBool("Idle", _movimiento == Vector2.zero);
		_animator.SetBool("IsGrounded", _tocandoSuelo);
		_animator.SetFloat("VerticalVelocity", _rigidbody.velocity.y);

        if(_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) 
        {
			_Atacando = true;
		}
        else 
        {
			_Atacando = false;
		}

		if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle")) 
		{
			_ContadorlongIdle += Time.deltaTime;

			if (_ContadorlongIdle >= TiempolongIdle) 
			{
				_animator.SetTrigger("LongIdle");
			}
		} 
		else 
		{
			_ContadorlongIdle = 0f;
		}
	}

    private void Voltear()
	{
		_mirandoDerecha = !_mirandoDerecha;

		float escalaLocalEnX = transform.localScale.x;
		escalaLocalEnX = escalaLocalEnX * -1f;
		transform.localScale = new Vector3(escalaLocalEnX, transform.localScale.y, transform.localScale.z);
	}

    public void Shoot()
	{
        contadorDeDisparos++;

        if( contadorDeDisparos > 5)
        {
            vidaJugador.añadirDaño(1);
        }

		if (prefabBala != null && _Cañon != null) 
        {
			GameObject myBala = Instantiate(prefabBala, _Cañon.position, Quaternion.identity) as GameObject;

			Bala componenteDeLaBala = myBala.GetComponent<Bala>();

			if (transform.localScale.x < 0f)
            {
				componenteDeLaBala.direccion = Vector2.left;
			}
            else 
            {
				componenteDeLaBala.direccion = Vector2.right;
			}
		}
	}

     void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.name.Equals("Plataforma"))
        {
            this.transform.parent = collision.transform;
        }
    }

    void OnCollisionExit2D(Collision2D collision) 
    {
        this.transform.parent = null;        
    }
}
