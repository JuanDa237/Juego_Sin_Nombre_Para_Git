using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfectoParallax : MonoBehaviour
{
    [Tooltip("Cantidad De Efecto A Aplicar (entre mas baja sea la capa mas bajo es el numero)")]
    [Range(0,1)]public float efecto;

    private GameObject[] buscador;
    private GameObject camara;
    private float largo, posicioninicio;

    void Start()
    {
        buscador = GameObject.FindGameObjectsWithTag("MainCamera");
        camara = buscador[0];
        largo = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        posicioninicio = transform.position.x;
    }

    void Update()
    {
        float temp = (camara.transform.position.x * (1 - efecto));
        float distancia = (camara.transform.position.x * efecto);

        transform.position = new Vector2(posicioninicio + distancia, transform.position.y);

        if(temp > (posicioninicio+largo))
        {
            posicioninicio += largo;
        }
        else if(temp < (posicioninicio-largo))
        {
            posicioninicio -= largo;
        }
    }
}
