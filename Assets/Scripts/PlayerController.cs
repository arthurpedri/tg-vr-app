using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
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
    GameObject carroQueBate;
    public Vector3 oldPosition; // comeco da travessia
    public Vector3 middlePosition; // metade da travessia
    public Vector3 newPosition; // destino da travessia
    public float currentTime = 0;
    public AudioClip somFreio;
    

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft; // iniciando o VR
        StartCoroutine(LoadDevice("cardboard"));

        oldPosition = transform.position;
        newPosition = transform.position + new Vector3(0,0,larguraRua);
        currentTime = 0;

        emMovimento = true;

        cruzamentoPos = cruzamento.transform.position.x;
        Manager.Instance.distanciaCruzamento = Mathf.Abs(cruzamentoPos - transform.position.x);
    }

    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown("space"))
        {
            // CalculoColisao();
            PrepararTravessia();
        }

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
        Manager.Instance.distanciaCruzamento = Mathf.Abs(cruzamentoPos - transform.position.x);

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

    // velocidade normal de andar = 5.5km/h 1.52m/s   (6.5s para atravessar rua)
    // jog = <10km/h <2.77m/s   (3.6s para atravessar rua)
    // carro 5.2 unidades

    public void CalculoColisao()
    {
        foreach (GameObject carro in carros)
        {
            if ((carro.transform.position.z - hitboxLarguraCarro/2) <= transform.position.z && transform.position.z <= (carro.transform.position.z + hitboxLarguraCarro/2)){ // 
                if (Mathf.Abs((carro.transform.position.x + sentidoRua * comprimentoCarro/2) - transform.position.x) <= 1.5){ // carro vai parar a 1.5 unidade de distancia
                    Debug.Log("Player x: "+transform.position.x + " z: " + transform.position.z);
                    Debug.Log("Carro x: "+ (carro.transform.position.x + comprimentoCarro/2) + " z: " + (carro.transform.position.z + hitboxLarguraCarro/2));
                    PararCarros();
                    Debug.Log("Bateu na frente");
                    return;
                }
            }
            if ((Mathf.Abs(carro.transform.position.z - hitboxLarguraCarro/2) - transform.position.z) < 1.5){
                if ((carro.transform.position.x - comprimentoCarro/2) <= 
                transform.position.x && transform.position.x <= (carro.transform.position.x + comprimentoCarro/2)){//voce esta do lado do carro
                    if (Mathf.Abs(transform.position.x - (carro.transform.position.x - sentidoRua * comprimentoCarro/2)) / Manager.Instance.defaultSpeed >= // tempo que o carro leva para passar a traseira pelo ponto onde esta o jogador deve ser maior ou igual do que o tempo que o jogador leva para chegar até o carro para colidir
                    (Mathf.Abs((carro.transform.position.z - hitboxLarguraCarro/2) - transform.position.z)) // distancia do jogador ate o carro 
                    / velocidadeJogador){  
                        PararCarros();
                        Debug.Log("Bateu no meio");
                        return;
                }
                }
            }
            
        }
    }

    public void oldCalculoColisao()
    {

        distanciaPerto = Mathf.Infinity;
        distanciaLonge = Mathf.Infinity;
        Manager.Instance.passagemLonge = -1;
        Manager.Instance.passagemPerto = -1;
        Manager.Instance.cruzamentoCorreto = -1;
        foreach (GameObject carro in carros)
        {
            //    TODO   tratar casos de carros grudados no jogador (muito proximos do x do player)
            if ((carro.transform.position.x + comprimentoCarro/2) < transform.position.x) // carro esta na esquerda   |  posicao carro + comprimentoCarro/2 é o nariz do carro
            {
                distanciaCarroX = Mathf.Abs((carro.transform.position.x + comprimentoCarro/2) - transform.position.x); // distancia do carro com o jogador no eixo X
                if (carro.transform.position.z < meioDaRua && distanciaPerto > distanciaCarroX) // lado perto (posicao z < meioDaRua) e carro mais proximo ate agora
                {
                    distanciaPerto = distanciaCarroX;
                    if (distanciaCarroX / Manager.Instance.defaultSpeed > larguraRua / velocidadeAndando) // passou andando sem carro passar por tras enquanto na rua (inicialmente era 7.2s, avaliar o pq disso)
                    {
                        Manager.Instance.passagemPerto = 3;
                    }
                    else if (distanciaCarroX / Manager.Instance.defaultSpeed > (larguraRua / 2) / velocidadeAndando) // passou andando  (larguraRua / 2) é o fim da primeira faixa
                    {
                        Manager.Instance.passagemPerto = 2;
                    }
                    else if (distanciaCarroX / Manager.Instance.defaultSpeed > (larguraRua / 2) / velocidadeJogador) // passou correndo 
                    {
                        Manager.Instance.passagemPerto = 1;
                    }
                    else // bateu
                    {
                        carroQueBate = carro; // sempre que um carro da primeira faixa bater ele será o carro que bate
                        Manager.Instance.passagemPerto = 0;

                    }
                }
                if (carro.transform.position.z >= meioDaRua && distanciaLonge > distanciaCarroX) // lado longe da rua e carro mais proximo ate agora
                {
                    distanciaLonge = distanciaCarroX;
                    if (distanciaCarroX / Manager.Instance.defaultSpeed > larguraRua / velocidadeAndando) // passou andando 
                    {
                        Manager.Instance.passagemLonge = 2;
                    } 
                    else if (distanciaCarroX / Manager.Instance.defaultSpeed > larguraRua / velocidadeJogador) // passou correndo 
                    {
                        Manager.Instance.passagemLonge = 1;
                    }
                    else // bateu
                    {
                        if (Manager.Instance.passagemPerto != 0){ // caso o carro da primeira faixa não bateu
                            carroQueBate = carro;
                        }
                        Manager.Instance.passagemLonge = 0;
                    }
                }
                
            }
            
        }

        if(Manager.Instance.distanciaCruzamento <= larguraFaixa / 2) // no cruzamento
        {
            Manager.Instance.cruzamentoCorreto = 1;
        }
        else
        {
            Manager.Instance.cruzamentoCorreto = 0;
        }

        PrepararTravessia();
        Debug.Log("DistanciaPerto: " + distanciaPerto + "| DistanciaLonge: " + distanciaLonge);
        Debug.Log("Perto: " + Manager.Instance.passagemPerto + " | Longe: " + Manager.Instance.passagemLonge + " | Cruzamento: " + Manager.Instance.cruzamentoCorreto);

    }


    public void Andar(int direcao) // -1 esquerda     1 direita
    {
        transform.position = transform.position + new Vector3(direcao * velocidadeAndando * Time.deltaTime * 10, 0, 0);
    }

    public void PrepararTravessia(){
        
        oldPosition = transform.position;
        middlePosition = transform.position + new Vector3(0,0,meioDaRua);
        newPosition = transform.position + new Vector3(0,0,larguraRua);

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
            setAtravessando(false);
            StartCoroutine(chamarEndMenu());
            Debug.Log("Jogo Acabou, Menu Em 5 Segundos");
        }

    }

    public void AtravessarFaixa() // nao esta feito
    {
        float endTime;
        float lerpPercent = 0;

        if(transform.position.z < meioDaRua){
            if (Manager.Instance.passagemPerto == 2 || Manager.Instance.passagemPerto == 3){
                endTime = larguraRua/velocidadeAndando;
                
            }
            else {
                endTime = larguraRua/velocidadeJogador;
                
            }
            if(currentTime < endTime){
            currentTime += Time.deltaTime;
            lerpPercent = currentTime / endTime;
            transform.position = Vector3.Lerp(oldPosition, middlePosition, lerpPercent);
        } 
        } 
        else {
            
            if (Manager.Instance.passagemLonge == 2){
                endTime = larguraRua/velocidadeAndando;
            }
            else {
                endTime = larguraRua/velocidadeJogador;
            }
        }

        if(currentTime < endTime){
            currentTime += Time.deltaTime;
            lerpPercent = currentTime / endTime;
            transform.position = Vector3.Lerp(oldPosition, newPosition, lerpPercent);
        } 
        else {
            transform.position = Vector3.Lerp(oldPosition, newPosition, 1);
            setAtravessando(false);
            StartCoroutine(chamarEndMenu());
            Debug.Log("Jogo Acabou, Menu Em 5 Segundos");
        }
    }

    public void PararCarros(){

        foreach (GameObject carro in carros)
        {
            carro.transform.GetComponent<Car>().setMovimento(false);
            AudioSource somCarro = carro.transform.GetComponent<AudioSource>();
            somCarro.Stop();
            AudioSource.PlayClipAtPoint(somFreio, carro.transform.position,4.0f);
        }
        setMovimento(false);
        StartCoroutine(chamarEndMenu());
        Debug.Log("Jogo Acabou, Menu Em 5 Segundos");
    }

    void setAtravessando(bool valor)
    {
        atravessando = valor;
    }

    public void setMovimento(bool movimento)
    {
        emMovimento = movimento;
    }

    IEnumerator chamarEndMenu()
    {
        yield return new WaitForSeconds(5);
        // SceneManager.LoadScene("EndMenu");
    }

}