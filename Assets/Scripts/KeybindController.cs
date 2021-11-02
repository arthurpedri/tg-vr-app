using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class KeybindController : MonoBehaviour
{
    public Text pressedButton; 
    public Button Menu, botaoAndar, botaoCorrer;
    bool preparadoPraSalvarAndar = false, preparadoPraSalvarCorrer = false;
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;

        botaoAndar.onClick.AddListener(delegate { SalvaBotao("Andar"); });
        botaoCorrer.onClick.AddListener(delegate { SalvaBotao("Correr"); });
        Menu.onClick.AddListener(delegate {LoadScene("Menu"); });
        
    }

    // Update is called once per frame
    void Update()
    {
        if (preparadoPraSalvarAndar || preparadoPraSalvarCorrer){
            foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
                    if(Input.GetKey(vKey)){
                        if (vKey != KeyCode.Mouse0 && vKey != KeyCode.Mouse1 && vKey != KeyCode.Mouse2){ // toque na tela não pode ser o botao usado
                            if (preparadoPraSalvarAndar){
                                Manager.Instance.botaoAndar = vKey;
                                preparadoPraSalvarAndar = false;
                            } 
                            else if (preparadoPraSalvarCorrer){
                                Manager.Instance.botaoCorrer = vKey;
                                preparadoPraSalvarCorrer = false;
                            }
                            pressedButton.text = vKey.ToString();
                        }
                    }
            }

        }
    }


    void SalvaBotao(string botao)
    {
    
        
        if (!preparadoPraSalvarAndar && !preparadoPraSalvarCorrer){
            if (botao == "Correr"){
                pressedButton.text = "Aperte o botão de " + botao;
                preparadoPraSalvarCorrer = true;
            }
            else if (botao == "Andar"){
                pressedButton.text = "Aperte o botão de " + botao;
                preparadoPraSalvarAndar = true;
            }
        }
        
    }

    public void LoadScene(string cena)
    {
        SceneManager.LoadScene (cena);
    }

}
