using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarScene : MonoBehaviour
{
	public void facilScene()
	{
		StartCoroutine("cargarFacil");
	}

    public void dificilScene()
	{
        StartCoroutine("cargarDificil");
	}

     private IEnumerator cargarFacil()
	{
		yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Facil");
	}

    private IEnumerator cargarDificil()
	{
		yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Dificil");
	}

	public void menuScene()
	{
		SceneManager.LoadScene("Menu");
	}
}
