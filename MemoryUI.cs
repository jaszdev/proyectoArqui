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

    public GameObject dataMemoryCellPrefab;
    int numData = MemoryConstants.DataMemorySize;
    int instructions = MemoryConstants.InstructionsMemorySize;

    public Transform dataParent;
    public Transform instructionsParent;

    // Start is called before the first frame update
    void Start()
    {
        processor = processorComponent.processor;
        memory = processor.Memory;

        for(int i = 0; i < numData; i++)
        {
            GameObject dataMemoryCell = Instantiate(dataMemoryCellPrefab, dataParent);
            dataMemoryCell.GetComponent<Text>().text = memory.GetData(i).ToString();
        }

        for(int i = 0; i < instructions; i++)
        {
            GameObject dataMemoryCell = Instantiate(dataMemoryCellPrefab, instructionsParent);
            dataMemoryCell.GetComponent<Text>().text = memory.GetInstruction(i).ToString();
        }

    }


    


}
