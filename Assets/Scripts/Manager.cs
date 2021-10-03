using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : Singleton<Manager>
{
    public float defaultSpeed = 60/3.6f; // em m/s 16.66f
    public float distanciaCruzamento; // em m
    public int passagemLonge; // -1 bug / 0 bateu / 1 correndo / 2 andando
    public int passagemPerto; // -1 bug / 0 bateu / 1 correndo / 2 andando perigoso / 3 andando tranquilo
    public int cruzamentoCorreto; // -1 bug / 0 errado / 1 correto

}
