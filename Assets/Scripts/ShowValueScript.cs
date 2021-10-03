using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowValueScript : MonoBehaviour {
	Text valortexto;
	// Use this for initialization
	void Start () {
		valortexto = GetComponent<Text>();
	}
	
	// Update is called once per frame
	public void textUpdate (float value) {
		valortexto.text = "Variavel: " + value;
		PlayerPrefs.SetFloat("Variavel", value);
	}
}
