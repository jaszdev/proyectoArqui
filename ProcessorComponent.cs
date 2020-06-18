using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase para usar la Clase Processor en Unity
public class ProcessorComponent : MonoBehaviour
{

    public Processor processor;
    public string[] programNames;
    public float quatumTime = 2f; // duracion de un quantum

    // Start is called before the first frame update
    void Start()
    {
        processor = new JaDHeProcessor(programNames);
        StartCoroutine(RunCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator RunCoroutine()
    {
        while(true)
        {
            processor.Execute();
            yield return new WaitForSeconds(quatumTime);
        }
    }

}
