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
    const float posicaoZFaixa0 = 3.5f; // carros faixa 0: z=3.5   |   faixa 1: z=8
    const float posicaoZFaixa1 = 8f;
    const float posicaoXEsquerda = -250f;
    const float posicaoXDireita = 250f;
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
    public GameObject cruzamento;
    public GameObject[] carros;
    public GameObject cars;
    public GameObject carroPadrao; // carro padrao para ser copiado
    public Vector3 oldPosition; // comeco da travessia
    public Vector3 middlePosition; // metade da travessia
    public Vector3 newPosition; // destino da travessia
    public float currentTime = 0;
    public AudioClip somFreio;
    DateTime tempoInicialCena;
    DateTime ultimaOlhadaEsquerda, ultimaOlhadaDireita;
    public ContadorCarros contCarros;
    bool olhandoEsquerda = false, olhandoDireita = false;
    int olhadasEsquerda = 0, olhadasDireita = 0;
    public GameObject camera;
    
    

    

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft; // iniciando o VR
        StartCoroutine(LoadDevice("cardboard"));

        oldPosition = transform.position;
        newPosition = transform.position + new Vector3(0,0,larguraRua);
        currentTime = 0;

        tempoInicialCena = DateTime.Now;
        Manager.Instance.ResetarDadosSimulacao();

        Manager.Instance.timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        transform.position = transform.position - new Vector3(0, 1.8f, 0);
        transform.position = transform.position + new Vector3(0, Manager.Instance.altura/100f, 0);


        emMovimento = true;

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


    GameObject CriaNovoCarro(int faixa, string tagDirecao) // duplica o carro padrao, declara o pai, seta como ativo, escolhe lado da rua e direcao (esqPraDireita, dirPraEsquerda)
    {
        Vector3 novaposicao = carroPadrao.transform.position; // if e talz os 2 com tags tambem
        Quaternion novarotacao = carroPadrao.transform.rotation;
        GameObject novocarro = GameObject.Instantiate(carroPadrao, novaposicao, novarotacao, cars.transform);
        novocarro.SetActive(true);
        novocarro.tag = tagDirecao;
        return novocarro;
    }

    void CriaPrimeirosCarrosDaCena() // funcao chamada no start para inicializar a cena com os carros que já estão presentes
    {

    }

    void ControladorDeTransito() // determina quando criar carros novos 
    {

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
        middlePosition = transform.position + new Vector3(0,0,meioDaRua);
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
        return (menorDistancia, faixa);
    }

    // rotação da camera quando olha para os carros 325 x 20   E  300 y 240    D   60 y 120
    void ChecaOlhar(){ 
        // Debug.Log("Eu: " + camera.localEulerAngles + " Rot: " + camera.rotation);
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
        if (camera.transform.localEulerAngles.x >= menorX || camera.transform.localEulerAngles.x <= maiorX){
            if (camera.transform.localEulerAngles.y >= menorY && camera.transform.localEulerAngles.y <= maiorY){
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