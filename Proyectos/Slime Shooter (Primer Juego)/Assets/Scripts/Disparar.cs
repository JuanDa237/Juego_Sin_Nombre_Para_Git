using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparar : MonoBehaviour
{
    public GameObject prefabBala;
    public GameObject jugador;
	private Transform _Cañon;

    void Awake()
	{
		_Cañon = transform.Find("Cañon");
	}

    public void Shoot()
	{
		if (prefabBala != null && _Cañon != null && jugador != null) 
        {
			GameObject myBala = Instantiate(prefabBala, _Cañon.position, Quaternion.identity) as GameObject;

			Bala componenteDeLaBala = myBala.GetComponent<Bala>();

			if (jugador.transform.localScale.x < 0f)
            {
				componenteDeLaBala.direccion = Vector2.left;
			}
            else 
            {
				componenteDeLaBala.direccion = Vector2.right;
			}
		}
	}
}
