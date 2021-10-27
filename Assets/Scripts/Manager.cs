using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;



public class Manager : Singleton<Manager>
{
    // configurações de ambiente
    public int velocidadeKm = 60;
    public float defaultSpeed = 60/3.6f; // em m/s 16.66f
    // public int passagemLonge; // -1 bug / 0 bateu / 1 correndo / 2 andando
    // public int passagemPerto; // -1 bug / 0 bateu / 1 correndo / 2 andando perigoso / 3 andando tranquilo
    public int cruzamentoCorreto = -1; // -1 bug / 0 errado / 1 correto
    public string periodo = "Dia"; 
    public string ambiente = "Cidade";
    public string dificuldade = "Mão Simples";
    public string nome = "Nome Vazio";
    public int idade = 0;
    public int altura = 180;

    // guarda no start da cena
    public string timestamp;

    // são setados na simulação
    public float distanciaCruzamento = -1; // em m
    public float distanciaCarroMaisProximo = -1; // -1 erro
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

    public string CriarArquivoParaExportar(bool tudo)
    {
        BancoDeDados banco = PegarBancoDeDados();

        string header = "Data(AAAA/MM/DD H:m)," +
                        "Nome," +
                        "Idade," +
                        "Altura(cm)," +

                        "Velocidade(km/h)," +
                        "Ambiente," +
                        "Período," +
                        "Dificuldade," +

                        "Houve acidente?," +
                        "Faixa do acidente," +
                        "Cruzamento na faixa?," +
                        "Distância da faixa(m)," +
                        "Distância carro mais próximo(m)," +
                        "Faixa carro mais próximo," +
                        "Tempo até atravessar(s)," +
                        "Qtd. de carros que passaram," +
                        "Qtd. de carros enquanto atravessava," +
                        "Qtd. de olhadas esquerda," +
                        "Qtd. de olhadas direita," +
                        "Última olhada esquerda(s)," +
                        "Última olhada direita(s)"  
                        ;


        // linha.timestamp 
        // linha.nome 
        // linha.idade 
        // linha.altura

        // linha.velocidade 
        // linha.ambiente 
        // linha.periodo  
        // linha.dificuldade 

        // linha.houveAcidente 
        // linha.faixaAcidente 
        // linha.cruzamentoCorreto 
        // linha.distanciaCruzamento
        // linha.distanciaCarroMaisProximo 
        // linha.faixaCarroMaisProximo 
        // linha.tempoParaTomadaDeDecisao 
        // linha.quantidadeDeCarrosQueJaPassaram 
        // linha.quantidadeDeCarrosEnquantoAtravessava 
        // linha.quantidadeDeOlhadasEsquerda 
        // linha.quantidadeDeOlhadasDireita 
        // linha.ultimaOlhadaEsquerda 
        // linha.ultimaOlhadaDireita 
        // 21

        string corpo = "";
        
        foreach (LinhaDB linha in banco.Table)
        {
            if (DateTime.Now.Date == DateTime.Parse(linha.timestamp).Date || tudo == true) {
                corpo += "\n";
                corpo += 
                        linha.timestamp + "," +
                        linha.nome + "," +
                        linha.idade + "," +
                        linha.altura + "," +
                        linha.velocidade + "," +
                        linha.ambiente + "," +
                        linha.periodo + "," +
                        linha.dificuldade + "," +
                        linha.houveAcidente + "," +
                        linha.faixaAcidente + "," +
                        linha.cruzamentoCorreto + "," +
                        linha.distanciaCruzamento + "," +
                        linha.distanciaCarroMaisProximo + "," +
                        linha.faixaCarroMaisProximo + "," +
                        linha.tempoParaTomadaDeDecisao + "," +
                        linha.quantidadeDeCarrosQueJaPassaram + "," +
                        linha.quantidadeDeCarrosEnquantoAtravessava + "," +
                        linha.quantidadeDeOlhadasEsquerda + "," +
                        linha.quantidadeDeOlhadasDireita + "," +
                        linha.ultimaOlhadaEsquerda + "," +
                        linha.ultimaOlhadaDireita
                        ;
            }
        }
        string caminhoArquivo = PegarCaminhoNovoArquivo();
        try {
            File.WriteAllText(caminhoArquivo, header + corpo);

        } 
        catch {
            Debug.Log("Erro caminho csv");
            caminhoArquivo = "erro";
        }
        return caminhoArquivo;
    }

    public void SalvarBancoDeDados()
    {
        BancoDeDados banco = PegarBancoDeDados();


        LinhaDB novalinhajson = new LinhaDB();

        SalvarLinha(novalinhajson); // salva informações do manager na linha que vai ser adicionada

        banco.Table.Add(novalinhajson);

        string json = JsonUtility.ToJson(banco, true);

        File.WriteAllText(PegarCaminhoBanco(), json);

        

    }

    public BancoDeDados PegarBancoDeDados()
    {
        // caminho para o arquivo de banco de dados
        string caminhoArquivo = PegarCaminhoBanco();

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

        return banco;
    }
    
    string PegarCaminhoBanco()
    {
        return PegarDiretorio() + "db.json";
    }

    string PegarCaminhoNovoArquivo()
    {
        string data = DateTime.Now.ToString("yyyyMMdd_HHmm");
        return PegarDiretorio() + "DTransito" + data + ".csv";
    }

    public string PegarDiretorio()
    {
        string diretorio;
        #if UNITY_EDITOR
        diretorio = Application.dataPath + "/saves/";
        #else
        diretorio = Application.persistentDataPath + "/";
        #endif

        if (!Directory.Exists(diretorio))
				Directory.CreateDirectory(diretorio);

        return diretorio;
    }

    public void SalvarLinha(LinhaDB linha)
    {
        linha.velocidade = Manager.Instance.velocidadeKm;
        linha.cruzamentoCorreto = Manager.Instance.cruzamentoCorreto;
        linha.periodo = Manager.Instance.periodo; 
        linha.ambiente = Manager.Instance.ambiente;
        linha.dificuldade = Manager.Instance.dificuldade;
        linha.nome = Manager.Instance.nome;
        linha.idade = Manager.Instance.idade;
        linha.altura = Manager.Instance.altura;

        linha.timestamp = Manager.Instance.timestamp;

        linha.distanciaCruzamento = Math.Round((decimal)Manager.Instance.distanciaCruzamento, 2).ToString(); 
        linha.distanciaCarroMaisProximo = Math.Round((decimal)Manager.Instance.distanciaCarroMaisProximo, 2).ToString();
        linha.faixaCarroMaisProximo = Manager.Instance.faixaCarroMaisProximo;
        linha.tempoParaTomadaDeDecisao = Manager.Instance.tempoParaTomadaDeDecisao;
        linha.quantidadeDeCarrosQueJaPassaram = Manager.Instance.quantidadeDeCarrosQueJaPassaram;
        linha.quantidadeDeCarrosEnquantoAtravessava = Manager.Instance.quantidadeDeCarrosEnquantoAtravessava;
        linha.faixaAcidente = Manager.Instance.faixaAcidente;
        linha.houveAcidente = Manager.Instance.houveAcidente;
        linha.quantidadeDeOlhadasEsquerda = Manager.Instance.quantidadeDeOlhadasEsquerda;
        linha.quantidadeDeOlhadasDireita = Manager.Instance.quantidadeDeOlhadasDireita;
        linha.ultimaOlhadaEsquerda = Manager.Instance.ultimaOlhadaEsquerda;
        linha.ultimaOlhadaDireita = Manager.Instance.ultimaOlhadaDireita;
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
    public int velocidade; // em km/h
    // public int passagemLonge; // -1 bug / 0 bateu / 1 correndo / 2 andando
    // public int passagemPerto; // -1 bug / 0 bateu / 1 correndo / 2 andando perigoso / 3 andando tranquilo
    public int cruzamentoCorreto = -1; // -1 bug / 0 errado / 1 correto
    public string periodo = "Dia"; 
    public string ambiente = "Cidade";
    public string dificuldade = "Mão Simples";
    public string nome = "Nome Vazio";
    public int idade = 0;
    public int altura;

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

}
