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

    public Text dataText;
    public Text instructionsText;
    
    // Start is called before the first frame update
    void Start()
    {
        GetProcessor();
    }

    public void UpdateDataMemoryUI()
    {
        dataText.text = "";
        for (int i = 0; i < MemoryConstants.DataMemorySize; i++)
        {
            dataText.text += memory.GetData(i) + " ";
        }
    }

    public void UpdateInstructionMemoryUI()
    {
        GetProcessor();

        instructionsText.text = "";
        for (int i = 0; i < MemoryConstants.InstructionsMemorySize; i++)
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
