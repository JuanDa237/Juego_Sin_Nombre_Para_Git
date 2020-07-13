using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VidaJugador : MonoBehaviour
{
    public int vidaTotal = 5;
	public RectTransform corazonUI;
	public TextMeshProUGUI puntajeTexto;
	public TextMeshProUGUI puntajeTextoGO;
	public GameObject spawnerBotiquines;
	public GameObject gameOverUi;
	public GameObject hudUI;
	
	private SpawnVida spawnerBotiquin;
	private int vida;
	private SpriteRenderer _renderer;
	private float tamañoCorazon = 100;
	[SerializeField] private int puntaje = 0;

	public GameObject goVida;
	public GameObject goDaño;
	private AudioSource sonidoVida;
	private AudioSource sonidoDaño;

	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
		spawnerBotiquin = spawnerBotiquines.GetComponent<SpawnVida>();
		sonidoVida = goVida.GetComponent<AudioSource>();
		sonidoDaño = goDaño.GetComponent<AudioSource>();
		puntajeTexto.text = puntaje.ToString();
	}

	void Start()
    {
		vida = vidaTotal;   
    }

    public void añadirDaño(int monton)
	{
		vida -= monton;
		sonidoDaño.Play();

		StartCoroutine("FeedbackVisual");

		if (vida <= 0) 
        {
			vida = vidaTotal;
            gameObject.SetActive(false);
		}
		corazonUI.sizeDelta = new Vector2(tamañoCorazon * vida, tamañoCorazon);
	}

	public void añadirVida(int monton)
	{
		vida += monton;
		sonidoVida.Play();
		
		if (vida > vidaTotal) 
        {
			vida = vidaTotal;
		}
		corazonUI.sizeDelta = new Vector2(tamañoCorazon * vida, tamañoCorazon);
	}

	private IEnumerator FeedbackVisual()
	{
		_renderer.color = Color.red;

		yield return new WaitForSeconds(0.1f);

		_renderer.color = Color.white;
	}

	public void añadirPuntaje(int puntos)
	{
		puntaje += puntos;
		
		puntajeTexto.text = puntaje.ToString();

		if(puntaje == 100 || puntaje == 105)
		{
			spawnerBotiquin.activarBotiquin(0);
		}
 		else if(puntaje == 200 || puntaje == 205)
		{
			spawnerBotiquin.activarBotiquin(1);
		}
		else if(puntaje == 300 || puntaje == 305)
		{
			spawnerBotiquin.activarBotiquin(2);
		}
	}

	public int mostrarPuntaje()
	{
		return puntaje;
	}

	void OnDisable() 
	{
		puntajeTextoGO.text = puntaje.ToString();
		if(gameOverUi != null && hudUI != null)
		{
			gameOverUi.SetActive(true);
			hudUI.SetActive(false);
		}

		int recordMomentaneo = PlayerPrefs.GetInt("puntajeRecord", 0);

		if(recordMomentaneo < puntaje)
		{
			PlayerPrefs.SetInt("puntajeRecord", puntaje);
		}
	}
}
