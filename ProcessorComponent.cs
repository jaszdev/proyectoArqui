using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Clase para usar la Clase Processor en Unity
public class ProcessorComponent : MonoBehaviour
{

    public JaDHeProcessor processor;
    public float quatumTime = 2f; // duracion de un quantum

    public bool finished = false;

    // UI
    public Text thread;
    public Text pc;
    public Text ir;
    public Text cycle;
    public Text rl;
    public GameObject registersContainer;
    Text[] registers;
    public DataCacheUI dataCacheUI;
    public InstructionCacheUI instructionCacheUI;
    public MemoryUI memoryUI;

    protected Coroutine currentCoroutine;

    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        processor = new JaDHeProcessor();

        memoryUI.UpdateInstructionMemoryUI();

        registers = registersContainer.GetComponentsInChildren<Text>();
        // indices pares: tag 
        // indices impares: valores

        UpdateUI();
    }

    public void StartProcessor(string[] programNames, int quantum, float quantumTime)
    {
        finished = false;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        this.quatumTime = quantumTime;

        processor.ConsoleInit(programNames, quantum);
        memoryUI.UpdateInstructionMemoryUI();
        currentCoroutine = StartCoroutine(RunCoroutine());
    }

    IEnumerator RunCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true && !finished)
        { 
            if (debug)
            {
                Debug.Log("Debug");
            }
            processor.Execute();

            //UI
            UpdateUI();

            finished = processor.Finished;
            yield return new WaitForSeconds(quatumTime);
        }
        processor.SaveContext(processor.CurrentThread);
        if (finished) processor.WriteResults("resultados.txt");
    }

    public void Abort()
    {
        finished = false;
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        processor.SaveContext(processor.CurrentThread);
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
        thread.text = processor.CurrentThread.ToString();
        pc.text = processor.PC.ToString();
        ir.text = processor.IR.ToString();
        cycle.text = processor.Clock.ToString();
        rl.text = processor.RL.ToString();
    }

    void UpdateRegistersUI()
    {
        for(int i = 0; i < ProcessorConstants.NumRegisters; i++)
        {
            registers[i + i + 1].text = processor.GetRegister(i).ToString();
        }
    }


}
