using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiscinaEnemigos : MonoBehaviour
{
    public GameObject prefab;
	public int cantidad = 10;
	public int delay = 5;

    void Start()
    {
        for (int i = 0; i < cantidad; i++) 
        {
			añadirEnemigoALaPiscina();
		}
		InvokeRepeating("CojerEnemigoDePiscina", 1f, delay);
    }

	public int scaleEnemigo = 1;

	private GameObject CojerEnemigoDePiscina()
	{
		GameObject enemigo = null;

		for (int i = 0; i < transform.childCount; i++) 
        {
			if (!transform.GetChild(i).gameObject.activeSelf) 
            {
				enemigo = transform.GetChild(i).gameObject;
				break;
			}
		}

		if (enemigo == null) 
        {
			añadirEnemigoALaPiscina();
			enemigo = transform.GetChild(transform.childCount - 1).gameObject;
		}

		enemigo.transform.position = this.transform.position;

		IAenemigo enemigoIA = enemigo.GetComponent<IAenemigo>();
		scaleEnemigo *= -1;

		if(scaleEnemigo == -1)
		{
			enemigoIA.Flip();
		}
		enemigo.SetActive(true);
		return enemigo;
	}

    private void añadirEnemigoALaPiscina()
	{
		GameObject enemy = Instantiate(prefab, this.transform.position, Quaternion.identity, this.transform);
		enemy.SetActive(false);
	}
}
