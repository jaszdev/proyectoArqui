using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase para usar la Clase Processor en Unity
public class ProcessorComponent : MonoBehaviour
{

    public Processor processor;
    public string[] programNames;

    // Start is called before the first frame update
    void Start()
    {
        processor = new JaDHeProcessor(programNames);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
