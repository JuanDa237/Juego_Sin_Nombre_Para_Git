using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Experimental.Rendering.Universal;

public class Movimiento_Enemigo2 : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vida = 3;

    [Space(10)]
    [SerializeField] [ColorUsageAttribute(true,true)] private Color colorAtacando = new Vector4(0, 0, 0, 0);
    [SerializeField] private Light2D luz = null;

    [Space(10)]
    [SerializeField] private GameObject prefabTextoDaño = null;
    [SerializeField] private Transform puntoDeAparicionTexto = null;

    [Space(10)]
    [SerializeField] private GameObject[] hijos = null;
    
    //Variables Privadas

    [HideInInspector] [ColorUsageAttribute(true,true)] public Color colorPrincipal;
    [HideInInspector] public bool estuvoExplotando = false;
    [HideInInspector] public bool exploto = false;
    private Vector3 lugarDondeExploto;

    [HideInInspector] public Animator _animator;
    private SpriteRenderer _sprite;
    private AIPath _iaPath;

    private void Awake() 
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _iaPath = GetComponentInParent<AIPath>();

        colorPrincipal = _sprite.material.color;    
    }

    private void Update() 
    {
        if(lugarDondeExploto != null && estuvoExplotando)
        {
            transform.position = lugarDondeExploto;
        }

        if(_iaPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(_iaPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if(_iaPath.canSearch && _sprite.material.color != colorAtacando)
        {
            cambiarColor(colorAtacando);
        }
        else if(!_iaPath.canSearch && _sprite.material.color != colorPrincipal)
        {
            cambiarColor(colorPrincipal);
        }
    }

    private void LateUpdate() 
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Explotando Definitivo"))
        {
            estuvoExplotando = true;
        }
        else if(estuvoExplotando)
        {
            _iaPath.canMove = false;
            _iaPath.canSearch = false;
        }
    }

    //Vida

    public void añadirDaño(int daño)
    {
        exploto = true;

        vida -= daño;
        if(vida <= 0)
        {
            vida = 0;
        }

        GameObject texto = Instantiate(prefabTextoDaño, puntoDeAparicionTexto.position, Quaternion.identity, puntoDeAparicionTexto) as GameObject;
        texto.GetComponent<TextoDaño>().setTexto(daño);
        
        if(transform.localScale.x < 0)
        {
            texto.transform.localScale = new Vector3(-1, 1, 1);
        }

        Explotar();
    }

    public void Explotar()
    {
        _iaPath.canMove = false;
        _iaPath.canSearch = false;

        lugarDondeExploto = new Vector3(transform.position.x,transform.position.y,transform.position.z);

        for(int i = 0; i < hijos.Length; i++)
        {
            hijos[i].SetActive(false);
        }
        cambiarColor(colorPrincipal);

        _animator.SetTrigger("Explotar");
    }

    public void cambiarColor(Color color)
    {
        _sprite.material.color = color;
        luz.color = color;
    }
}
