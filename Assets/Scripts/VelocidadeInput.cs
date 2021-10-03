using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocidadeInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void changeVelocidade(float speed)
    {
        Manager.Instance.defaultSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
