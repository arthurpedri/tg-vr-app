using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class ExportController : MonoBehaviour
{

    NativeShare share;
    public Button Exportar, Menu;
    // Start is called before the first frame update
    void Start()
    {
        Exportar.onClick.AddListener(delegate {ChamarShareAndroid(); });
        Menu.onClick.AddListener(delegate {LoadScene("Menu"); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChamarShareAndroid()
    {

        string diretorio = Application.dataPath + "/saves/";
        if (!Directory.Exists(diretorio))
				Directory.CreateDirectory(diretorio);
        string caminhoArquivo = diretorio + "db.json";

        if (File.Exists(caminhoArquivo)){
            new NativeShare().AddFile(caminhoArquivo).SetSubject("Banco de Dados").SetText( "Arquivo Json" ).Share();
        }
        else {
            Debug.Log("Arquivo não existe");
        }


        
    }

    public void LoadScene(string cena)
    {
        SceneManager.LoadScene (cena);
    }

}
