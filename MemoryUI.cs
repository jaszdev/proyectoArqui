using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase para representar la memoria del procesador en Unity
public class MemoryUI : MonoBehaviour
{

    public ProcessorComponent processorComponent;
    Processor processor;

    // Start is called before the first frame update
    void Start()
    {
        processor = processorComponent.processor;
    }

    


}
