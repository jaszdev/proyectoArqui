using UnityEngine;

static class ProcessorConstants
{
    public const int NumRegisters = 32;
    public const int MaxNumThreads = 5;
    public const int Quantum = 3;
    public const int CacheMissDelay = 44;
}

public enum ThreadState { Running, Finished }

public abstract class Processor
{

    // Processor components
    protected int[] registers;
    protected Instruction instructionRegister;
    protected int rl; // registro RL
    protected int pcRegister;
    protected int clock = 0;
    public int quantum;
    protected int quantumCounter = 0;
    public int threads = 0;
    protected int currentThread = 0;
    protected ThreadState[] threadStates; // array para indicar estado del thread

    // Aux Memory
    protected Memory memory;
    protected DataCache dataCache;
    protected InstructionsCache instructionsCache;
    protected ContextMemory contextMemory;

    // Data
    protected bool finished = false;
    public bool Finished => finished;
    public int MemoryAccess = 0;
    public int TotalCacheMisses = 0;
    public int LoadMisses = 0;
    public int LoadAccess = 0;
    public int StoreMisses = 0;
    public int StoreAccess = 0;

    public int[] threadsDuration;

    // Processor Constructor
    public Processor()
    {
        registers = new int[ProcessorConstants.NumRegisters];

        memory = new Memory();
        dataCache = new DataCache(CacheConstants.Blocks, CacheConstants.Words, memory);
        instructionsCache = new InstructionsCache(CacheConstants.Blocks, CacheConstants.Words, memory);

        instructionRegister = new Instruction(0, 0, 0, 0);
        pcRegister = 0;
        clock = 0;
    }

    protected abstract void Init();
    public abstract void Execute();

    protected void LoadContext(int thread)
    {
        pcRegister = contextMemory.GetPC(thread); // cargar pc
        for(int i = 0; i < ProcessorConstants.NumRegisters; i++) // cargar registros
        {
            registers[i] = contextMemory.GetRegister(thread, i);
        }

    }

    public void SaveContext(int thread)
    {
        contextMemory.SetPC(thread, pcRegister); // guardar pc
        for (int i = 0; i < ProcessorConstants.NumRegisters; i++) // guardar registros
        {
            contextMemory.SetRegister(thread, i, registers[i]);
        }
    }

    // cambia contexto al siguiente hilo
    protected void SwitchContext()
    {
        int nextThread = GetNextThread();

        if (nextThread == currentThread) // solo queda un hilo
        {
            //Debug.Log("Solo queda el hilo #" + nextThread);
        }
        else if (nextThread != -1)
        {
            SaveContext(currentThread);
            LoadContext(nextThread);
            //Debug.Log("Cambiando de h #" + currentThread + " a h #" + nextThread);
            currentThread = nextThread;
        }
        else  // nextThread == -1 todos los hilos se terminaron de ejecutar
        {
            finished = true;
            //Debug.Log("Se termino la ejecucion de todos los hilos");
            return;
        }
        rl = -1;
        quantumCounter = 0; // resetear contador de quantum
    }

    // Busca el siguiente hilo que no haya terminado
    // devuelve -1 si ya todos los hilos terminaron
    int GetNextThread()
    {
        for(int i = 1; i <= threads; i++)
        {
            int nextThread = (currentThread + i) % threads;
            if (threadStates[nextThread] == ThreadState.Running)
                return nextThread;
        }
        return -1;
    }


    public Memory Memory => memory;
    public DataCache DataCache => dataCache;
    public InstructionsCache InstructionCache => instructionsCache;
    public int GetRegister(int index) => registers[index];
    public int PC => pcRegister;
    public Instruction IR => instructionRegister;
    public int Clock => clock;
    public int CurrentThread => currentThread;
    public int RL => rl;

}
