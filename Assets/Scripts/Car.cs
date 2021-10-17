using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private Vector3 startPos;
    private bool emMovimento = false;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;

        setMovimento(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (emMovimento){
            Dirigir();
        }

    }

    void Dirigir(){
        transform.Translate(0, 0,Manager.Instance.defaultSpeed*Time.deltaTime);
        if (transform.position.x >= 250) // teoricamente o fim da rua
        {
            Destroy(gameObject);
            // transform.position = startPos;
        }
    }

    public void setMovimento(bool movimento){
        emMovimento = movimento;
    }
}
