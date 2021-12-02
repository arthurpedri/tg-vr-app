using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Estados do menu
	public GameObject inicial, menuopcoes;

	// Botões de interação
	public Button Jogar, Opcoes, Voltar, Exportar, Controle;

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
		Exportar.onClick.AddListener(delegate {LoadScene("ExportarMenu"); });
		Controle.onClick.AddListener(delegate {LoadScene("ConfigControle"); });
		Opcoes.onClick.AddListener(LoadOpcoes);
		Voltar.onClick.AddListener(LoadInicial);
		VelocidadeSlider.onValueChanged.AddListener(delegate {changeSpeed(); });

		Periodo.onValueChanged.AddListener(delegate {changePeriodo(); });
		Ambiente.onValueChanged.AddListener(delegate {changeAmbiente(); }); // agora controleAlternativo
		Dificuldade.onValueChanged.AddListener(delegate {changeDificuldade(); });
		
		menuopcoes.SetActive(false); // começando com o menu de opcoes desativado
	}
	

	public void LoadScene(string cena)
    {
		// salvar informações do jogo
		Manager.Instance.nome = Nome.text != "" ? Nome.text.Replace(",", "") : "Nome Vazio"; // nome padrão Nome Vazio
		Manager.Instance.idade = Idade.text != "" ? int.Parse(Idade.text) : 0; // idade padrão 0
		Manager.Instance.altura = Altura.text != "" ? int.Parse(Altura.text) : 180;  // altura padrão 180
		
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
		int speed = (int)VelocidadeSlider.value * 20; // slider tem valores baixos, é incrementado em 1
		VelocidadeCarrosText.text = "Velocidade dos carros: " + speed + "km/h";
		Debug.Log(speed);
		Manager.Instance.velocidadeKm = speed;
		Manager.Instance.defaultSpeed = speed / 3.6f;
		// Debug.Log(Manager.Instance.defaultSpeed);
	}

	public void LoadInicial()
    {
		if (validaAltura()){ // pode expandir para outra validacoes
			menuopcoes.SetActive(false);
			inicial.SetActive(true);
		}

    }

	bool validaAltura()
	{
		if (Altura.text == ""){
			Altura.GetComponent<Image>().color = new Color32(255,255,255,255);
			return true;
		} 
		if(int.Parse(Altura.text) <= 100 || int.Parse(Altura.text) >= 280){
			Altura.text = "";
			Altura.placeholder.GetComponent<Text>().text = "Entre 100 e 280cm";
			Altura.GetComponent<Image>().color = new Color32(255,177,177,255);
			return false;
		}
		else {
			Altura.GetComponent<Image>().color = new Color32(255,255,255,255);
			return true;
		}
	}

	void changePeriodo()
	{
		Manager.Instance.periodo = Periodo.captionText.text;
	}

	void changeAmbiente()
	{
		Manager.Instance.controleAlternativo = Ambiente.captionText.text;
	}

	void changeDificuldade()
	{
		Manager.Instance.dificuldade = Dificuldade.captionText.text;
	}


}

