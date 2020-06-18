using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Clase para representar la memoria del procesador en Unity
public class MemoryUI : MonoBehaviour
{

    public ProcessorComponent processorComponent;
    Processor processor;
    Memory memory;

    public Text instructionsText;

    int numData = MemoryConstants.DataMemorySize;
    int instructions = MemoryConstants.InstructionsMemorySize;
    

    // Start is called before the first frame update
    void Start()
    {
        GetProcessor();
    }

    void Update()
    {
        if (processor == null) { GetProcessor(); return; }

        instructionsText.text = "";
        for (int i = 0; i < instructions; i++)
        {
            instructionsText.text += memory.GetInstruction(i) + " ";
        }
    }

    void GetProcessor()
    {
        processor = processorComponent.processor;
        if(processor != null) memory = processor.Memory;
    }


}
