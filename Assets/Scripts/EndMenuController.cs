using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System;

public class EndMenuController : MonoBehaviour {

	public Button Menu, Exportar;
	public Text resultado, perto, longe, cruzamento;

	// Use this for initialization
	void Start () {

		
		Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;

		if (Manager.Instance.houveAcidente == 1)
		{
			resultado.text = "Você sofreu um acidente!";
		}
		else
		{
			resultado.text = "Você atravessou a rua com segurança!";
		}


		Menu.onClick.AddListener(delegate {LoadScene("Menu"); });
		Exportar.onClick.AddListener(delegate {LoadScene("ExportarMenu"); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadScene(string cena)
    {
        SceneManager.LoadScene (cena);
    }


}

