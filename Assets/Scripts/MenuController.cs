using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Estados do menu
	public GameObject inicial, menuopcoes;

	// Botões de interação
	public Button Jogar, Opcoes, Voltar;

	// Velocidade carros
	public Slider VelocidadeSlider;
	public Text VelocidadeCarrosText;

	// Informações Jogador
	public InputField Nome;
	public InputField Idade;
	public InputField Altura;

	// Informações Ambiente
	public Dropdown Periodo;
	public Dropdown Ambiente;
	public Dropdown Dificuldade;


	// Use this for initialization
	void Start () {

		Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;

		Jogar.onClick.AddListener(delegate {LoadScene("PrimeiraCena"); });
		Opcoes.onClick.AddListener(LoadOpcoes);
		Voltar.onClick.AddListener(LoadInicial);
		VelocidadeSlider.onValueChanged.AddListener(delegate {changeSpeed(); });

		Periodo.onValueChanged.AddListener(delegate {changePeriodo(); });
		Ambiente.onValueChanged.AddListener(delegate {changeAmbiente(); });
		Dificuldade.onValueChanged.AddListener(delegate {changeDificuldade(); });
		
		menuopcoes.SetActive(false); // começando com o menu de opcoes desativado
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadScene(string cena)
    {
		// salvar informações do jogo
		Manager.Instance.nome = Nome.text != "" ? Nome.text : "Nome Vazio"; // nome padrão Nome Vazio
		Manager.Instance.idade = Idade.text != "" ? int.Parse(Idade.text) : 0; // idade padrão 0
		Manager.Instance.altura = Altura.text != "" ? float.Parse(Altura.text) / 100 : 1.8f;  // altura padrão 1.8
		
		// Manager.Instance.printAll();
        SceneManager.LoadScene (cena);
    }

    public void LoadOpcoes()
    {
    	inicial.SetActive(false);
    	menuopcoes.SetActive(true);
    }

	public void changeSpeed()
	{
		float speed = VelocidadeSlider.value * 20; // slider tem valores baixos, é incrementado em 1
		VelocidadeCarrosText.text = "Velocidade dos carros: " + speed + "km/h";
		Manager.Instance.defaultSpeed = speed / 3.6f;
		// Debug.Log(Manager.Instance.defaultSpeed);
	}

	public void LoadInicial()
    {
		if(int.Parse(Altura.text) < 100 || int.Parse(Altura.text) > 280){
			Altura.text = "";
			Altura.placeholder.GetComponent<Text>().text = "Entre 100 e 280cm";
			Altura.GetComponent<Image>().color = new Color32(255,177,177,255);
			return;
		}
		else{
    		menuopcoes.SetActive(false);
	    	inicial.SetActive(true);
			Altura.GetComponent<Image>().color = new Color32(255,255,255,255);
		}
    	
    }

	void changeAltura()
	{
		if(int.Parse(Altura.text) <= 100 || int.Parse(Altura.text) >= 280){
			Altura.text = "";
			Altura.placeholder.GetComponent<Text>().text = "Entre 100 e 280cm";
		}
	}

	void changePeriodo()
	{
		Manager.Instance.periodo = Periodo.captionText.text;
	}

	void changeAmbiente()
	{
		Manager.Instance.ambiente = Ambiente.captionText.text;
	}

	void changeDificuldade()
	{
		Manager.Instance.dificuldade = Dificuldade.captionText.text;
	}


}

