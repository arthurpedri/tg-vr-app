using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContadorCarros : MonoBehaviour
{
    public int quantidade;
    // Start is called before the first frame update
    void Start()
    {
        quantidade = 0;
    }

    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.parent != null){
            if (other.gameObject.transform.parent.name == "Cars"){
                // Debug.Log(other.gameObject.name);
                quantidade += 1;
                //TargetRenderer.material.color = Color.red;
            }
        }
        // Debug.Log(quantidade);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
