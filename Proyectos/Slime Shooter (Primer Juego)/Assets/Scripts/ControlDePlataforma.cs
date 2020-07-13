using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDePlataforma : MonoBehaviour
{
    public Transform[] posiciones = new Transform[2];
    public float velocidad;
    //public Rigidbody2D _rigidbody;
    public Transform _transform;

    private int posicionActual;
    private int posicionProxima = 1;

    void Update()
    {
        //_rigidbody.MovePosition(Vector2.MoveTowards(_rigidbody.position, posiciones[posicionProxima].position, velocidad * Time.deltaTime));

        _transform.position = Vector2.MoveTowards(_transform.position, posiciones[posicionProxima].position, velocidad * Time.deltaTime);

        if(Vector2.Distance(_transform.position, posiciones[posicionProxima].position) <= 0)
        {
            posicionActual = posicionProxima;
            posicionProxima++; 

            if(posicionProxima > (posiciones.Length - 1))
            {
                posicionProxima = 0;
            }
        }
    }
}
