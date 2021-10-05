using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Manager : Singleton<Manager>
{
    public float defaultSpeed = 100/3.6f; // em m/s 16.66f
    public float distanciaCruzamento; // em m
    public int passagemLonge; // -1 bug / 0 bateu / 1 correndo / 2 andando
    public int passagemPerto; // -1 bug / 0 bateu / 1 correndo / 2 andando perigoso / 3 andando tranquilo
    public int cruzamentoCorreto; // -1 bug / 0 errado / 1 correto
    public string periodo = "Dia"; 
    public string ambiente = "Cidade";
    public string dificuldade = "Mão Simples";
    public string nome = "Nome Vazio";
    public int idade = 0;
    public float altura = 1.8f;
    public string timestamp;

    public void printAll()
    {
        Debug.Log("Velocidade Carros: " + defaultSpeed + " | Distancia Cruzamento: " + distanciaCruzamento);
        Debug.Log("Período: " + periodo + " | Ambiente: " + ambiente + " | Dificuldade: " + dificuldade);
        Debug.Log("Nome: " + nome + " | Idade: " + idade + " | Altura: " + altura);
        Debug.Log("Timestamp: " + timestamp);
    }

}
