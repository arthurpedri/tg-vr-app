using System;
using System.Collections;
using System.Collections.Generic;
using diag = System.Diagnostics;
using UnityEngine;

public class SightCollisionHandler : MonoBehaviour
{
    Renderer TargetRenderer;
    Renderer SightRenderer;
    diag.Stopwatch stopwatch;
    TimeSpan ts;
    public PlayerController myPC;
    // Start is called before the first frame update
    void Start()
    {
        stopwatch = new diag.Stopwatch();
        SightRenderer = gameObject.GetComponent<Renderer>();
        SightRenderer.material.color = new Color32(0,255,255,255);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Target"){
            ts = stopwatch.Elapsed;
            
            if (ts.Milliseconds >= 500){
                Debug.Log("Atravessou");
                stopwatch.Stop();
                TargetRenderer.material.color = new Color32(0,204,0,255);
                other.gameObject.SetActive(false);
                
                // myPC.CalculoColisao();
                myPC.PrepararTravessia();
            } else 
            {
                TargetRenderer.material.color = new Color32(Convert.ToByte(Mathf.Floor((1-((float)ts.Milliseconds/500))*255)),Convert.ToByte(Mathf.Floor((1-((float)ts.Milliseconds/500))*55+200)),Convert.ToByte(Mathf.Floor((1-((float)ts.Milliseconds/500))*255)),255);
            }
            //Debug.Log(ts.Seconds);

        }

        if (other.gameObject.name == "AndarEsquerda"){ //
            myPC.Andar(-1);
        }

        if (other.gameObject.name == "AndarDireita"){
            myPC.Andar(1);
        }
    }  

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Target"){
            TargetRenderer = other.gameObject.GetComponent<Renderer>();
            stopwatch.Start();
            //TargetRenderer.material.color = Color.red;
            
        }
    }  

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Target"){
            TargetRenderer.material.color = Color.white;
            stopwatch.Stop();
            stopwatch.Reset();

        }
    } 



    // Update is called once per frame
    void Update()
    {
        
    }
}
