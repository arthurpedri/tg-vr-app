using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // constantes 
    const string esqPraDireita = "NasceEsqVaiDir";  // tag para direcao do carro
    const string dirPraEsquerda = "NasceDirVaiEsq";
    const string stringDia = "Dia";
    const string stringTarde = "Tarde";
    const string stringNoite = "Noite";
    const float posicaoZFaixa0 = 3.5f; // carros faixa 0: z=3.5   |   faixa 1: z=8
    const float posicaoZFaixa1 = 8f;
    const float posicaoXEsquerda = -250f;
    const float posicaoXDireita = 250f;
    const float posicaoY = 0f;
    // velocidade em unidades/s  (m/s)
    float velocidadeAndando = 1.52f;
    float velocidadeJogador = 5.77f;
    float comprimentoCarro = 5.2f;
    float sentidoRua = 1;
    float hitboxLarguraCarro = 3f; // largura maior que a do carro 
    float larguraRua = 10f; // largura um pouco maior do que a rua em si (largura de onde o personagem comeca a atravessar e onde termina)
    float larguraFaixa = 4f; // largura da faixa de pedestres
    float meioDaRua = 6; // coordenada z especifica da metade da rua (posicao) 
    private float cruzamentoPos, distanciaCarroX;
    private float distanciaPerto, distanciaLonge;
    private bool atravessando = false;
    private bool emMovimento = false; // carros em movimento
    bool carroInstanciado = false;
    public GameObject rua;
    GameObject cruzamento;
    public GameObject cars;
    public GameObject carroPadrao; // carro padrao para ser copiado
    Vector3 oldPosition; // comeco da travessia
    Vector3 newPosition; // destino da travessia
    float currentTime = 0;
    public AudioClip somFreio;
    DateTime tempoInicialCena;
    DateTime ultimoCarroFaixa0, ultimoCarroFaixa1;
    DateTime ultimaOlhadaEsquerda, ultimaOlhadaDireita;
    public ContadorCarros contCarros;
    bool olhandoEsquerda = false, olhandoDireita = false;
    int olhadasEsquerda = 0, olhadasDireita = 0;
    public GameObject cameraprincipal;
    public Material skyboxDia;
    public Material skyboxTarde;
    public Material skyboxNoite;
    public GameObject sol, chao;
    
    

    

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft; // iniciando o VR
        StartCoroutine(LoadDevice("cardboard"));

        oldPosition = transform.position;
        newPosition = transform.position + new Vector3(0,0,larguraRua);
        Manager.Instance.ResetarDadosSimulacao();
        currentTime = 0;

        SetaAmbiente();

        cruzamento = rua.transform.GetChild(0).gameObject;

        tempoInicialCena = DateTime.Now;
        ultimaOlhadaEsquerda = tempoInicialCena;
        ultimaOlhadaDireita = tempoInicialCena;
        ultimoCarroFaixa0 = tempoInicialCena;
        ultimoCarroFaixa1 = tempoInicialCena;

        Manager.Instance.timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        transform.position = transform.position - new Vector3(0, 1.8f, 0);
        transform.position = transform.position + new Vector3(0, Manager.Instance.altura/100f, 0);


        emMovimento = true;

        CriaPrimeirosCarrosDaCena();
        

    }

    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown("space"))
        {
            PrepararTravessia();
        }  

        // checa rotação da camera
        ChecaOlhar();

        if (atravessando)
        {
            if(emMovimento){
                Atravessar();
                // calcula se carro bate
                CalculoColisao();
            }

        }

         
        ControladorDeTransito();


        //update the position
        transform.position = transform.position + new Vector3(horizontalInput * velocidadeAndando * Time.deltaTime, 0, 0);

    }


    IEnumerator LoadDevice(string newDevice)
    {
        // if (String.Compare(XRSettings.loadedDeviceName, newDevice, true) != 0)
        // {
            XRSettings.LoadDeviceByName(XRSettings.supportedDevices);
            yield return null;
            XRSettings.enabled = true;
        // }
    }

    void SetaAmbiente() // 90 e 90
    {
        // Manager.Instance.periodo = stringNoite;

        Material grama = chao.GetComponent<Terrain>().materialTemplate;
        GameObject postes = rua.transform.GetChild(1).gameObject;
        MeshRenderer bulboPoste;
        Material[] materiaisPoste;
        Color bulboCor;



        if (Manager.Instance.periodo == stringDia){
            cameraprincipal.GetComponent<Skybox>().material = skyboxDia;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.reflectionIntensity = 1;
            sol.GetComponent<Light>().intensity = 1;
            sol.GetComponent<Light>().shadowStrength = 0.7f;
            sol.GetComponent<Light>().color = new Color32(250,241,210,255);
            grama.color = new Color32(207,207,207,255);
            grama = chao.GetComponent<Terrain>().materialTemplate;

            foreach (Transform poste in postes.transform)
            {
                poste.GetChild(0).gameObject.SetActive(false); // ativa luz do poste
            }
            
            
        }
        else if (Manager.Instance.periodo == stringTarde){
            cameraprincipal.GetComponent<Skybox>().material = skyboxTarde;
            RenderSettings.ambientIntensity = 0.9f;
            RenderSettings.reflectionIntensity = 0.8f;
            sol.GetComponent<Light>().intensity = 0.3f;
            sol.GetComponent<Light>().shadowStrength = 0.9f;
            sol.GetComponent<Light>().color = new Color32(238,142,35,255);
            grama.color = new Color32(207,207,207,255);
            grama = chao.GetComponent<Terrain>().materialTemplate;

            foreach (Transform poste in postes.transform)
            {
                poste.GetChild(0).gameObject.SetActive(false); // ativa luz do poste
            }
        }
        else if (Manager.Instance.periodo == stringNoite){
            cameraprincipal.GetComponent<Skybox>().material = skyboxNoite;
            RenderSettings.ambientIntensity = 0.3f;
            RenderSettings.reflectionIntensity = 0;
            sol.GetComponent<Light>().intensity = 0;
            // sol.GetComponent<Light>().color; intensidade zero, nao importa a cor 
            grama.color = new Color32(90,90,90,255);
            grama = chao.GetComponent<Terrain>().materialTemplate;

            foreach (Transform poste in postes.transform)
            {
                poste.GetChild(0).gameObject.SetActive(true); // ativa luz do poste
                bulboPoste = poste.GetChild(1).gameObject.GetComponent<MeshRenderer>(); // ativa emissao do bulbo do poste
                materiaisPoste = bulboPoste.materials;
                bulboCor = materiaisPoste[0].color;
                materiaisPoste[0].SetColor("_EmissionColor", new Color(bulboCor.r,bulboCor.g,bulboCor.b,1f));
                bulboPoste.materials = materiaisPoste;
            }
        }
    }


    void CriaNovoCarro(string tagDirecao, Vector3 posicao) // cria carro com posicao especifica duplicando o carro padrao
    {
        Quaternion novarotacao = carroPadrao.transform.rotation; 
        if (tagDirecao == dirPraEsquerda){
            novarotacao *= Quaternion.Euler(Vector3.up * 180); // rotaciona o carro em 180 graus se a direcao for oposta
        }
        GameObject novocarro = GameObject.Instantiate(carroPadrao, posicao, novarotacao, cars.transform);  // duplica o carro padrao com a posicao passada e a rotacao, setando o pai
        RandomizaCorCarro(novocarro); // faz o corpo do carro ter uma cor aleatoria
        if (Manager.Instance.periodo == stringTarde || Manager.Instance.periodo == stringNoite){
            LigaLuzCarro(novocarro);
        }
        novocarro.tag = tagDirecao; // seta a tag do carro apropriadamente
        novocarro.SetActive(true); // ativa o carro duplicado (carro padrão é inativo)
    }

    void CriaPrimeirosCarrosDaCena() // funcao chamada no start para inicializar a cena com os carros que já estão presentes
    {
        for (int i = 0; i < 6; i++)
        {
            CriaNovoCarro(esqPraDireita, new Vector3(posicaoXEsquerda + i*100 , posicaoY, posicaoZFaixa0));
            CriaNovoCarro(dirPraEsquerda, new Vector3(posicaoXDireita - 15 - i*100, posicaoY, posicaoZFaixa1));
            
        }
    }

    void ControladorDeTransito() // determina quando criar carros novos 
    {
        DateTime now = DateTime.Now;
        TimeSpan ts0 = now.Subtract(ultimoCarroFaixa0);
        TimeSpan ts1 = now.Subtract(ultimoCarroFaixa1);
        

        if (ts0.Seconds >= 5 && carroInstanciado == false){
            carroInstanciado = true;
            CriaCarroPadraoEsqPraDireita(0);
        }

        if (ts1.Seconds >= 10){
            ultimoCarroFaixa0 = now;
            ultimoCarroFaixa1 = now;
            carroInstanciado = false;
            CriaCarroPadraoDirPraEsquerda(1);
        }
                
    }

    void CriaCarroPadraoEsqPraDireita(int faixa) // cria carro esquerda pra direita na posicao padrao
    {
        if (faixa == 0){
            CriaNovoCarro(esqPraDireita, new Vector3(posicaoXEsquerda, posicaoY, posicaoZFaixa0));
        }
        else if (faixa == 1){
            CriaNovoCarro(esqPraDireita, new Vector3(posicaoXEsquerda, posicaoY, posicaoZFaixa1));
        }
    }

    void CriaCarroPadraoDirPraEsquerda(int faixa) // cria carro direita pra esquerda na posicao padrao
    {
        if (faixa == 0){
            CriaNovoCarro(dirPraEsquerda, new Vector3(posicaoXDireita, posicaoY, posicaoZFaixa0));
        }
        else if (faixa == 1){
            CriaNovoCarro(dirPraEsquerda, new Vector3(posicaoXDireita, posicaoY, posicaoZFaixa1));
        }
    }

    void RandomizaCorCarro(GameObject carro)
    {
        MeshRenderer corpo = carro.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        Material[] materials = corpo.materials;
        materials[0].color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value); // materials[0] é o chassi
        corpo.materials = materials;
    }

    void LigaLuzCarro(GameObject carro)
    {
        MeshRenderer corpo = carro.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();  // primeiro filho é o corpo
        carro.transform.GetChild(5).gameObject.SetActive(true); // sexto filho é o objeto pai das luzes
        Material[] materials = corpo.materials;
        materials[6].SetColor("_EmissionColor", new Color(1f,1f,1f,1f)); // branco
        corpo.materials = materials;
    }   


    // velocidade normal de andar = 5.5km/h 1.52m/s   (6.5s para atravessar rua)
    // jog = <10km/h <2.77m/s   (3.6s para atravessar rua)
    // carro 5.2 unidades

    void CalculoColisao()
    {
        foreach (Transform carro in cars.transform)
        {
            // testando o caso de bater na frente do carro
            if ((carro.position.z - hitboxLarguraCarro/2) <= transform.position.z && transform.position.z <= (carro.position.z + hitboxLarguraCarro/2)){ // 
                if (Mathf.Abs((carro.position.x + sentidoRua * comprimentoCarro/2) - transform.position.x) <= 1.5){ // carro vai parar a 1.5 unidade de distancia
                    // Debug.Log("Player x: "+transform.position.x + " z: " + transform.position.z);
                    // Debug.Log("Carro x: "+ (carro.position.x + comprimentoCarro/2) + " z: " + (carro.position.z + hitboxLarguraCarro/2));
                    PararCarros();
                    // Debug.Log("Bateu na frente");
                    return;
                }
            }
            // testando o caso de bater no lado do carro
            if ((Mathf.Abs(carro.position.z - hitboxLarguraCarro/2) - transform.position.z) < 1.5){ 
                if ((carro.position.x - comprimentoCarro/2) <= 
                transform.position.x && transform.position.x <= (carro.position.x + comprimentoCarro/2)){//voce esta do lado do carro
                    if (Mathf.Abs(transform.position.x - (carro.position.x - sentidoRua * comprimentoCarro/2)) / Manager.Instance.defaultSpeed >= // tempo que o carro leva para passar a traseira pelo ponto onde esta o jogador deve ser maior ou igual do que o tempo que o jogador leva para chegar até o carro para colidir
                    (Mathf.Abs((carro.position.z - hitboxLarguraCarro/2) - transform.position.z)) // distancia do jogador ate o carro 
                    / velocidadeJogador){  
                        PararCarros();
                        // Debug.Log("Bateu no meio");
                        return;
                }
                }
            }
            
        }
    }


    public void Andar(int direcao) // -1 esquerda     1 direita
    {
        transform.position = transform.position + new Vector3(direcao * velocidadeAndando * Time.deltaTime * 10, 0, 0);
    }

    public void PrepararTravessia(){
        
        oldPosition = transform.position;
        newPosition = transform.position + new Vector3(0,0,larguraRua);

        SalvarDadosDecisaoAtravessar();
        

        setAtravessando(true);
    }

    public void Atravessar()
    {       
        float endTime;
        float lerpPercent = 0;

        
        // if ((Manager.Instance.passagemPerto == 2 || Manager.Instance.passagemPerto == 3) && Manager.Instance.passagemLonge == 2){
        //     endTime = larguraRua/velocidadeAndando;
        //     Debug.Log("Foi Andando");
        // }
        // else {
            
        //     endTime = larguraRua/velocidadeJogador;
        //     Debug.Log("Foi Correndo");
        // }
        
        endTime = larguraRua/velocidadeJogador;

        if(currentTime < endTime){
            currentTime += Time.deltaTime;
            lerpPercent = currentTime / endTime;
            transform.position = Vector3.Lerp(oldPosition, newPosition, lerpPercent);
        } 
        else {
            transform.position = Vector3.Lerp(oldPosition, newPosition, 1);
            ConcluirSimulacao();
        }

    }


    public void PararCarros(){

        foreach (Transform carro in cars.transform)
        {
            carro.GetComponent<Car>().setMovimento(false);
            AudioSource somCarro = carro.GetComponent<AudioSource>();
            somCarro.Stop();
            AudioSource.PlayClipAtPoint(somFreio, carro.position,4.0f);
            foreach (Transform roda in carro){
                roda.GetComponent<Animator>().enabled = false;
            }
        }
        // pegar posição, salvar faixa e que ocorrou acidente
        Manager.Instance.faixaAcidente = transform.position.z <= meioDaRua ? 0 : 1; // Acidente primeira faixa 0 / segunda 1
        Manager.Instance.houveAcidente = 1; // houve acidente

        ConcluirSimulacao();
    }

    void SalvarDadosDecisaoAtravessar()
    {
        // Cruzamento correto e distancia do cruzamento
        Manager.Instance.distanciaCruzamento = Mathf.Abs(cruzamento.transform.position.x - transform.position.x);
        if(Manager.Instance.distanciaCruzamento <= larguraFaixa / 2){ // no cruzamento
            Manager.Instance.cruzamentoCorreto = 1;
            Manager.Instance.distanciaCruzamento = 0;
        }
        else {
            Manager.Instance.distanciaCruzamento -=  larguraFaixa / 2;
            Manager.Instance.cruzamentoCorreto = 0;
        }

        // distancia do carro mais proximo quando decidiu atravessar e a faixa dele
        SalvarDistanciaCarroMaisProximo();

        // quantos segundos jogador demorou para decidir atravessar
        DateTime now = DateTime.Now;
        TimeSpan ts = now.Subtract(tempoInicialCena);
        Manager.Instance.tempoParaTomadaDeDecisao = ts.Seconds;

        // segundos desde a ultima olhada para cada lado
        if (ultimaOlhadaEsquerda != DateTime.MinValue){
            ts = now.Subtract(ultimaOlhadaEsquerda);
            Manager.Instance.ultimaOlhadaEsquerda = ts.Seconds;
        } 
        else {
           Manager.Instance.ultimaOlhadaEsquerda = -1; 
        }

        if (ultimaOlhadaDireita != DateTime.MinValue){
            ts = now.Subtract(ultimaOlhadaDireita);
            Manager.Instance.ultimaOlhadaDireita = ts.Seconds;
        } 
        else {
           Manager.Instance.ultimaOlhadaDireita = -1; 
        }

        // quantos carros passaram até a travessia ocorrer
        Manager.Instance.quantidadeDeCarrosQueJaPassaram = contCarros.quantidade;

        // quantidades de olhada antes de atravessar
        Manager.Instance.quantidadeDeOlhadasEsquerda = olhadasEsquerda;
        Manager.Instance.quantidadeDeOlhadasDireita = olhadasDireita;

        // velocidade escolhida para atravessar (primeira)

        // se acelerou enquanto estava atravessando

    }

    void SalvarDadosConclusao()
    {
        // quantos carros passaram enquanto atravessava
        Manager.Instance.quantidadeDeCarrosEnquantoAtravessava = contCarros.quantidade - Manager.Instance.quantidadeDeCarrosQueJaPassaram;
    }

    void SalvarDistanciaCarroMaisProximo(){ // guarda a distancia e faixa do carro mais proximo no singleton
        (Manager.Instance.distanciaCarroMaisProximo, Manager.Instance.faixaCarroMaisProximo) = DistanciaCarroMaisProximo();
    }

    (float, int) DistanciaCarroMaisProximo(){
        float menorDistancia = Mathf.Infinity;
        int faixa = -1;
        foreach (Transform carro in cars.transform){
            if (carro.tag == esqPraDireita){
                if (carro.position.x + comprimentoCarro/2 <= transform.position.x){
                    if (Mathf.Abs((carro.position.x + comprimentoCarro/2) - transform.position.x) < menorDistancia){
                        menorDistancia = Mathf.Abs((carro.position.x + comprimentoCarro/2) - transform.position.x);
                        faixa = carro.position.z < meioDaRua ? 0 : 1;
                    }
                } 
            }
            else if (carro.tag == dirPraEsquerda){
                if (carro.position.x - comprimentoCarro/2 >= transform.position.x){
                    if (Mathf.Abs((carro.position.x - comprimentoCarro/2) - transform.position.x) < menorDistancia){
                        menorDistancia = Mathf.Abs((carro.position.x - comprimentoCarro/2) - transform.position.x);
                        faixa = carro.position.z < meioDaRua ? 0 : 1;
                    }
                }
            }
        }
        if (menorDistancia == Mathf.Infinity){
            menorDistancia = -1;
        }
        return (menorDistancia, faixa);
    }

    // rotação da camera quando olha para os carros 325 x 20   E  300 y 240    D   60 y 120
    void ChecaOlhar(){ 
        // Debug.Log("Eu: " + cameraprincipal.localEulerAngles + " Rot: " + cameraprincipal.rotation);
        // Debug.Log("E: " + CameraDentroCoordenadas(325, 20, 240, 300) + " D: " + CameraDentroCoordenadas(325, 20, 60, 120));

        
        
        if (olhandoEsquerda){ // 325 <= x || x <= 20   E  300 y 240  
            if (!CameraDentroCoordenadas(325, 20, 240, 300)){
                olhandoEsquerda = false;
            } 
            else {
                ultimaOlhadaEsquerda = DateTime.Now;
            }
        }
        else {
            if (CameraDentroCoordenadas(325, 20, 240, 300)){
                olhandoEsquerda = true;
                olhadasEsquerda += 1;
            }
        }
        if (olhandoDireita){ // 325 x 20  D   60 y 120
            if (!CameraDentroCoordenadas(325, 20, 60, 120)){
                olhandoDireita = false;
            }
            else {
                ultimaOlhadaDireita = DateTime.Now;
            }
        }
        else {
            if (CameraDentroCoordenadas(325, 20, 60, 120)){
                olhandoDireita = true;
                olhadasDireita += 1;
            }
        }
    }

    bool CameraDentroCoordenadas(float menorX, float maiorX, float menorY, float maiorY)
    {
        if (cameraprincipal.transform.localEulerAngles.x >= menorX || cameraprincipal.transform.localEulerAngles.x <= maiorX){
            if (cameraprincipal.transform.localEulerAngles.y >= menorY && cameraprincipal.transform.localEulerAngles.y <= maiorY){
                return true;
            }
        }
        return false;
    }

    void setAtravessando(bool valor)
    {
        atravessando = valor;
    }

    public void setMovimento(bool movimento)
    {
        emMovimento = movimento;
    }

    IEnumerator ChamarEndMenu()
    {
        Debug.Log("Jogo Acabou, Menu Em 3 Segundos");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("EndMenu");
    }

    void ConcluirSimulacao(){
        setMovimento(false);
        SalvarDadosConclusao();
        Manager.Instance.SalvarBancoDeDados();
        StartCoroutine(ChamarEndMenu());
        
    }
}