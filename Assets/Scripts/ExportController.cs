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
    public Button Exportar, ExportarHoje, Menu;

    void Awake() 
    {
        // Screen.orientation = ScreenOrientation.AutoRotation ; // parando VR para o menu
        XRSettings.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Exportar.onClick.AddListener(delegate {StartCoroutine( ChamarShareAndroid(true) ); });
        ExportarHoje.onClick.AddListener(delegate {StartCoroutine( ChamarShareAndroid(false) ); });
        // StartCoroutine( TakeScreenshotAndShare() );
        Menu.onClick.AddListener(delegate {LoadScene("Menu"); });
    }


    private IEnumerator ChamarShareAndroid(bool tudo)
    {
        yield return new WaitForEndOfFrame();
        // Erro.text = "Entrou na função \n";


        string caminhoArquivo = Manager.Instance.CriarArquivoParaExportar(tudo);
        string nomeArquivo = caminhoArquivo.Split('/').Last();
        


        if (caminhoArquivo != "erro") {
            if (File.Exists(caminhoArquivo)){
                new NativeShare().AddFile(caminhoArquivo).SetSubject(nomeArquivo).SetText( "Dados Trânsito" ).Share();
            }
            else {
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
