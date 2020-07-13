using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAenemigo : MonoBehaviour
{
   public float velocidad = 1f;
	public float muroEnterado = 0.5f;
	public LayerMask layerDeEnteracion;
	public float jugadorEnterado = 3f;
	public float tiempoDeApuntado = 0.5f;
	public float tiempoDeTiro = 1.5f;
	public LayerMask layerDelPlayer;
	public float enteradoDelPlayer = 2f;

	private Rigidbody2D _rigidbody;
	private Animator _animator;
	private Disparar _disparar;
	private AudioSource _audio;

	private Vector2 _movimiento;
	private bool _mirandoDerecha;

	private bool _Atacando;

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();
		_disparar = GetComponentInChildren<Disparar>();
		_audio = GetComponentInChildren<AudioSource>();
	}

	void Start()
	{
		if(transform.localScale.x < 0f) 
        {
			_mirandoDerecha = false;
		} 
        else if (transform.localScale.x > 0f) 
        {
			_mirandoDerecha = true;
		}
	}
	
	void Update()
	{
		Vector2 direccion = Vector2.right;

		if (_mirandoDerecha == false) 
        {
			direccion = Vector2.left;
		}

		Vector2 direccionAlRevez = direccion * -1;

		if (_Atacando == false) 
        {
			if (Physics2D.Raycast(transform.position, direccion, muroEnterado, layerDeEnteracion)) 
            {
				Flip();
			}
			if (Physics2D.Raycast(transform.position, direccionAlRevez, enteradoDelPlayer, layerDelPlayer)) 
            {
				Flip();
			}
		}

	}

    public void Flip()
	{
		_mirandoDerecha = !_mirandoDerecha;
		float localScaleX = transform.localScale.x;
		localScaleX = localScaleX * -1f;
		transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
	}

	private void FixedUpdate()
	{
		float horizontalVelocity = velocidad;

		if (_mirandoDerecha == false) 
        {
			horizontalVelocity *= -1f;
		}

		if (_Atacando) 
        {
			horizontalVelocity = 0f;
		}

		_rigidbody.velocity = new Vector2(horizontalVelocity, _rigidbody.velocity.y);
	}

	private void LateUpdate()
	{
		_animator.SetBool("Idle", _rigidbody.velocity == Vector2.zero);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (_Atacando == false && collision.CompareTag("Player")) 
        {
			StartCoroutine("AimAndShoot");
		}
	}

	private IEnumerator AimAndShoot()
	{
		_Atacando = true;

		yield return new WaitForSeconds(tiempoDeApuntado);

		_animator.SetTrigger("Shoot");

		yield return new WaitForSeconds(tiempoDeTiro);

		_Atacando = false;
	}

	void CanShoot()
	{
		if (_disparar != null) 
        {
			_disparar.Shoot();
			_audio.Play();
		}
	}

	private void OnEnable()
	{
		_Atacando = false;
	}

	private void OnDisable()
	{
		StopCoroutine("AimAndShoot");
		_Atacando = false;
	}
}
