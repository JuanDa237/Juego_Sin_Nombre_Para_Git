using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class IA_Enemigo3 : MonoBehaviour
{
    [Header("IA")]
    [SerializeField] private float radioDeDetencion = 0;
    [SerializeField] private LayerMask layerPlayer = 0;

    [Header("Movimiento De Lado A Lado")]
    [SerializeField] private float velocidad = 0;
    [SerializeField] private float distanciaARecorrer = 0;

    //Variables Privadas

    private Vector3[] posiciones = new Vector3[2];
    private int posicionActual;
    private int posicionProxima = 1;
    private bool playerEnLaMira;

    private AIPath _ia;
    private Movimiento_Enemigo3 graficos;

    private void Awake() 
    {
        _ia = GetComponent<AIPath>();
        graficos = GetComponentInChildren<Movimiento_Enemigo3>();

        posiciones[0] = new Vector3(transform.position.x + distanciaARecorrer, transform.position.y, transform.position.z);
        posiciones[1] = new Vector3(transform.position.x - distanciaARecorrer, transform.position.y, transform.position.z);
    }

    private void Update() 
    {
        if(!graficos.explotoDefinitivamente)
        {
            bool estaElPlayer = Physics2D.OverlapCircle(transform.position, radioDeDetencion, layerPlayer);

            if(playerEnLaMira || graficos._animator.GetCurrentAnimatorStateInfo(0).IsName("Atacar"))
            {
                _ia.canMove = false;
            }
            else if(estaElPlayer)
            {
                _ia.canMove = true;
            }

            _ia.canSearch = estaElPlayer;

            if(!estaElPlayer)
            {
                transform.position = Vector2.MoveTowards(transform.position, posiciones[posicionProxima], velocidad * Time.deltaTime);

                if(Vector2.Distance(transform.position, posiciones[posicionProxima]) <= 0)
                {
                    posicionActual = posicionProxima;
                    posicionProxima++; 

                    if(posicionProxima > (posiciones.Length - 1))
                    {
                        posicionProxima = 0;
                    }
                }
            }
            else if(playerEnLaMira)
            {
                playerEnLaMira = false;
                _ia.canMove = false;

                graficos._animator.SetTrigger("Atacar");
            }
        }
        else
        {
            _ia.canMove = false;
            _ia.canSearch = false;

            gameObject.SetActive(false);
        }
    }

    public void hayPlayer()
    {
        playerEnLaMira = true;
    }
}
