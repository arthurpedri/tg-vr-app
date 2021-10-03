using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System;

public class MenuController : MonoBehaviour {

	public Button Jogar, Opcoes, Voltar;
	public GameObject inicial, menuopcoes;
	public Slider slider;
	public InputField velocidade;

	// Use this for initialization
	void Start () {

		Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;

		Jogar.onClick.AddListener(delegate {LoadScene("PrimeiraCena"); });
		Opcoes.onClick.AddListener(LoadOpcoes);
		Voltar.onClick.AddListener(LoadInicial);
		velocidade.onValueChanged.AddListener(changeSpeed);
		menuopcoes.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadScene(string cena)
    {
        SceneManager.LoadScene (cena);
    }

    public void LoadOpcoes()
    {
    	inicial.SetActive(false);
    	menuopcoes.SetActive(true);
    }

	public void changeSpeed(string strSpeed)
	{
		int speed = Int32.Parse(strSpeed);
		Manager.Instance.defaultSpeed = speed / 3.6f;
		Debug.Log(Manager.Instance.defaultSpeed);

	}

	public void LoadInicial()
    {
    	menuopcoes.SetActive(false);
    	inicial.SetActive(true);
    	
    }
}

