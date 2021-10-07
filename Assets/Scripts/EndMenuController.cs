using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System;

public class EndMenuController : MonoBehaviour {

	public Button Jogar, Opcoes, Voltar;
	public GameObject inicial, menuopcoes;
	public InputField velocidade;
	public Text resultado, perto, longe, cruzamento;

	// Use this for initialization
	void Start () {

		
		Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;

		Jogar.onClick.AddListener(delegate {LoadScene("PrimeiraCena"); });
		Opcoes.onClick.AddListener(LoadOpcoes);
		Voltar.onClick.AddListener(LoadInicial);
		velocidade.onValueChanged.AddListener(changeSpeed);
		menuopcoes.SetActive(false);
		if (Manager.Instance.cruzamentoCorreto == 1)
		{
			cruzamento.text = "Você atravessou na faixa de pedestres.";
		}
		else
		{
			cruzamento.text = "Você não atravessou na faixa de pedestres.";
		}
		// if (Manager.Instance.passagemPerto == 0 || Manager.Instance.passagemLonge == 0)
		// {
		// 	resultado.text = "Você sofreu um acidente!";
		// }
		// else if (Manager.Instance.passagemPerto == 1 || Manager.Instance.passagemLonge == 1)
		// {
		// 	resultado.text = "Você quase sofreu um acidente!";
		// }
		// else if (Manager.Instance.passagemPerto == 2)
		// {
		// 	resultado.text = "Você passou andando, mas a travessia não foi segura!";
		// }
		// else if (Manager.Instance.cruzamentoCorreto == 0)
		// {
		// 	resultado.text = "Você passou andando, mas a travessia não foi segura!";
		// }
		// else
		// {
		// 	resultado.text = "Você atravessou a rua com segurança!";
		// }

		// if (Manager.Instance.passagemPerto == 0)
		// {
		// 	perto.text = "Você bateu no carro da faixa mais próxima.";
		// }
		// else if (Manager.Instance.passagemPerto == 1)
		// {
		// 	perto.text = "Você quase bateu no carro da faixa mais próxima.";
		// }
		// else if (Manager.Instance.passagemPerto == 2)
		// {
		// 	perto.text = "Você passou andando da faixa mais próxima, mas a travessia não foi segura.";
		// }
		// else if (Manager.Instance.passagemPerto == 3)
		// {
		// 	perto.text = "Você passou andando da faixa mais próxima.";
		// }

		// if (Manager.Instance.passagemLonge == 0)
		// {
		// 	longe.text = "Você bateu no carro da faixa mais distante.";
		// }
		// else if (Manager.Instance.passagemLonge == 1)
		// {
		// 	longe.text = "Você quase bateu no carro da faixa mais distante.";
		// }
		// else if (Manager.Instance.passagemLonge == 2)
		// {
		// 	longe.text = "Você passou andando da faixa mais distante.";
		// }
		
		

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

