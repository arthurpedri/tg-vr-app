using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class ExportController : MonoBehaviour
{

    NativeShare share;
    public Button Exportar, Menu;
    public Text Erro;

    void Awake() 
    {
        // Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Exportar.onClick.AddListener(delegate {StartCoroutine( ChamarShareAndroid() ); });
        // StartCoroutine( TakeScreenshotAndShare() );
        Menu.onClick.AddListener(delegate {LoadScene("Menu"); });
    }


    private IEnumerator ChamarShareAndroid()
    {
        yield return new WaitForEndOfFrame();
        // Erro.text = "Entrou na função \n";


        string caminhoArquivo = Manager.Instance.CriarArquivoParaExportar();
        string nomeArquivo = caminhoArquivo.Split('/').Last();
        


        if (caminhoArquivo != "erro") {
            if (File.Exists(caminhoArquivo)){
                Erro.text += "Arquivo existe\n";
                new NativeShare().AddFile(caminhoArquivo).SetSubject(nomeArquivo).SetText( "Dados Trânsito" ).Share();
            }
            else {
                Erro.text += "Arquivo não existe\n";
                new NativeShare().SetSubject("Banco de Dados").SetText( "Arquivo Vazio" ).Share();
                Debug.Log("Arquivo não existe");
            }
        }


        
    }

    public void LoadScene(string cena)
    {
        SceneManager.LoadScene (cena);
    }

}
