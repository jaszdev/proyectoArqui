using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Clase para usar la Clase Processor en Unity
public class ProcessorComponent : MonoBehaviour
{

    public Processor processor;
    public string[] programNames;
    public float quatumTime = 2f; // duracion de un quantum

    public bool finished = false;

    // UI
    public Text pc;
    public Text ir;
    public Text cycle;
    public GameObject registersContainer;
    Text[] registers;
    public DataCacheUI dataCacheUI;
    public InstructionCacheUI instructionCacheUI;
    public MemoryUI memoryUI;

    // Start is called before the first frame update
    void Start()
    {
        processor = new JaDHeProcessor(programNames);
        memoryUI.UpdateInstructionMemoryUI();

        registers = registersContainer.GetComponentsInChildren<Text>();
        // indices pares: tag 
        // indices impares: valores

        StartCoroutine(RunCoroutine());

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator RunCoroutine()
    {
        yield return new WaitForSeconds(2f);

        while (true && !finished)
        {
            processor.Execute();

            //UI
            UpdateUI();

            finished = processor.Finished;
            yield return new WaitForSeconds(quatumTime);
        }
    }

    void UpdateUI()
    {
        UpdateProcessorComponentsUI();
        UpdateRegistersUI();
        dataCacheUI.UpdateDataCacheUI();
        instructionCacheUI.UpdateInstructionCacheUI();
        memoryUI.UpdateDataMemoryUI();
    }

    void UpdateProcessorComponentsUI()
    {
        pc.text = processor.PC.ToString();
        ir.text = processor.IR.ToString();
        cycle.text = processor.Clock.ToString();
    }

    void UpdateRegistersUI()
    {
        for(int i = 0; i < ProcessorConstants.NumRegisters; i++)
        {
            registers[i + i + 1].text = processor.GetRegister(i).ToString();
        }
    }


}
