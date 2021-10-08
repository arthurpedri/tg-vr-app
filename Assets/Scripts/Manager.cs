using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;



public class Manager : Singleton<Manager>
{
    // configurações de ambiente
    public float defaultSpeed = 100/3.6f; // em m/s 16.66f
    // public int passagemLonge; // -1 bug / 0 bateu / 1 correndo / 2 andando
    // public int passagemPerto; // -1 bug / 0 bateu / 1 correndo / 2 andando perigoso / 3 andando tranquilo
    public int cruzamentoCorreto = -1; // -1 bug / 0 errado / 1 correto
    public string periodo = "Dia"; 
    public string ambiente = "Cidade";
    public string dificuldade = "Mão Simples";
    public string nome = "Nome Vazio";
    public int idade = 0;
    public float altura = 1.8f;

    // guarda no start da cena
    public string timestamp;

    // são setados na simulação
    public float distanciaCruzamento = -1; // em m
    public float distanciaCarroMaisProximo = -1;
    public int faixaCarroMaisProximo = -1; // -1 bug / 0 primeira / 1 segunda 
    public int tempoParaTomadaDeDecisao = 0;
    public int quantidadeDeCarrosQueJaPassaram = 0;
    public int quantidadeDeCarrosEnquantoAtravessava = 0;
    public int faixaAcidente = -1; // -1 nenhum acidente / 0 primeira / 1 segunda 
    public int houveAcidente = 0;
    public int quantidadeDeOlhadasEsquerda = 0;
    public int quantidadeDeOlhadasDireita = 0;
    public int ultimaOlhadaEsquerda = -1; // -1 nunca olhou
    public int ultimaOlhadaDireita = -1; // -1 nunca olhou
    // (-30 < x < 20)  (-120 < y < -60) (60 < y < 120) 

    public void printAll()
    {
        // dados ambiente
        Debug.Log("Velocidade Carros: " + defaultSpeed + " | Distancia Cruzamento: " + distanciaCruzamento + " | Cruzamento Correto: " + cruzamentoCorreto);
        Debug.Log("Período: " + periodo + " | Ambiente: " + ambiente + " | Dificuldade: " + dificuldade);
        Debug.Log("Nome: " + nome + " | Idade: " + idade + " | Altura: " + altura);
        Debug.Log("Timestamp: " + timestamp);

        // dados decisao travessia
        Debug.Log("Distancia Carro Mais Proximo: " + distanciaCarroMaisProximo + " | Faixa Carro Mais Proximo: " + faixaCarroMaisProximo);
        Debug.Log("Segundos Tomada Decisao: " + tempoParaTomadaDeDecisao + " | Quantidade Carros Ja Passaram: " + quantidadeDeCarrosQueJaPassaram);


        // dados conclusao
        Debug.Log("Quantidade Carros Enquanto Atravessava: " + quantidadeDeCarrosEnquantoAtravessava);
        Debug.Log("Houve Acidente: " + houveAcidente + " | Faixa Acidente: " + faixaAcidente);
        Debug.Log("Olhadas Esquerda: " + quantidadeDeOlhadasEsquerda + " | Ultima Olhada Esquerda: " + ultimaOlhadaEsquerda + " | Olhadas Direita: " + quantidadeDeOlhadasDireita + " | Ultima Olhada Direita: " + ultimaOlhadaDireita);
    }

    public void ResetarDadosSimulacao() // reseta dados da simulação 
    {
            distanciaCruzamento = -1; // em m
            distanciaCarroMaisProximo = -1;
            faixaCarroMaisProximo = -1; // -1 bug / 0 primeira / 1 segunda 
            tempoParaTomadaDeDecisao = 0;
            quantidadeDeCarrosQueJaPassaram = 0;
            quantidadeDeCarrosEnquantoAtravessava = 0;
            faixaAcidente = -1; // -1 nenhum acidente / 0 primeira / 1 segunda 
            houveAcidente = 0;
            quantidadeDeOlhadasEsquerda = 0;
            quantidadeDeOlhadasDireita = 0;
            ultimaOlhadaEsquerda = -1; // -1 nunca olhou
            ultimaOlhadaDireita = -1; // -1 nunca olhou
    }

    public void SalvarBancoDeDados()
    {
        // caminho para o arquivo de banco de dados
        string diretorio = Application.dataPath + "/saves/";
        if (!Directory.Exists(diretorio))
				Directory.CreateDirectory(diretorio);
        string caminhoArquivo = diretorio + "db.json";


        string arquivo;
        BancoDeDados banco;

        if (File.Exists(caminhoArquivo)){
            arquivo = File.ReadAllText (caminhoArquivo);
            banco = JsonUtility.FromJson<BancoDeDados>(arquivo);
            if (banco == null){ // arquivo vazio
                banco = new BancoDeDados();
            }
        }
        else {
            banco = new BancoDeDados();
        }

        

        
        LinhaDB novalinhajson = new LinhaDB();

        banco.Table.Add(novalinhajson);

        string json = JsonUtility.ToJson(banco, true);

        File.WriteAllText(caminhoArquivo, json);

        

    }



}

[System.Serializable]
public class BancoDeDados
{
    public List<LinhaDB> Table;

    public BancoDeDados()
    {
        Table = new List<LinhaDB>();
    }

}

[System.Serializable]
public class LinhaDB
{
    public string defaultSpeed; // em m/s 16.66f
    // public int passagemLonge; // -1 bug / 0 bateu / 1 correndo / 2 andando
    // public int passagemPerto; // -1 bug / 0 bateu / 1 correndo / 2 andando perigoso / 3 andando tranquilo
    public int cruzamentoCorreto = -1; // -1 bug / 0 errado / 1 correto
    public string periodo = "Dia"; 
    public string ambiente = "Cidade";
    public string dificuldade = "Mão Simples";
    public string nome = "Nome Vazio";
    public int idade = 0;
    public string altura;

    // guarda no start da cena
    public string timestamp;

    // são setados na simulação
    public string distanciaCruzamento; // em m
    public string distanciaCarroMaisProximo;
    public int faixaCarroMaisProximo = -1; // -1 bug / 0 primeira / 1 segunda 
    public int tempoParaTomadaDeDecisao = 0;
    public int quantidadeDeCarrosQueJaPassaram = 0;
    public int quantidadeDeCarrosEnquantoAtravessava = 0;
    public int faixaAcidente = -1; // -1 nenhum acidente / 0 primeira / 1 segunda 
    public int houveAcidente = 0;
    public int quantidadeDeOlhadasEsquerda = 0;
    public int quantidadeDeOlhadasDireita = 0;
    public int ultimaOlhadaEsquerda = -1; // -1 nunca olhou
    public int ultimaOlhadaDireita = -1; // -1 nunca olhou

    public LinhaDB()
    {
            defaultSpeed = Math.Round((decimal)Manager.Instance.defaultSpeed, 2).ToString(); 
            cruzamentoCorreto = Manager.Instance.cruzamentoCorreto;
            periodo = Manager.Instance.periodo; 
            ambiente = Manager.Instance.ambiente;
            dificuldade = Manager.Instance.dificuldade;
            nome = Manager.Instance.nome;
            idade = Manager.Instance.idade;
            altura = Math.Round((decimal)Manager.Instance.altura, 2).ToString();

            timestamp = Manager.Instance.timestamp;

            distanciaCruzamento = Math.Round((decimal)Manager.Instance.distanciaCruzamento, 2).ToString(); 
            distanciaCarroMaisProximo = Math.Round((decimal)Manager.Instance.distanciaCarroMaisProximo, 2).ToString();
            faixaCarroMaisProximo = Manager.Instance.faixaCarroMaisProximo;
            tempoParaTomadaDeDecisao = Manager.Instance.tempoParaTomadaDeDecisao;
            quantidadeDeCarrosQueJaPassaram = Manager.Instance.quantidadeDeCarrosQueJaPassaram;
            quantidadeDeCarrosEnquantoAtravessava = Manager.Instance.quantidadeDeCarrosEnquantoAtravessava;
            faixaAcidente = Manager.Instance.faixaAcidente;
            houveAcidente = Manager.Instance.houveAcidente;
            quantidadeDeOlhadasEsquerda = Manager.Instance.quantidadeDeOlhadasEsquerda;
            quantidadeDeOlhadasDireita = Manager.Instance.quantidadeDeOlhadasDireita;
            ultimaOlhadaEsquerda = Manager.Instance.ultimaOlhadaEsquerda;
            ultimaOlhadaDireita = Manager.Instance.ultimaOlhadaDireita;
    }
}
