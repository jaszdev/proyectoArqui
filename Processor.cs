
static class ProcessorConstants
{
    public const int NumRegisters = 32;
    public const int MaxNumThreads = 5;
    public const int Quantum = 3;
    public const int CacheMissDelay = 44;
}

public abstract class Processor
{

    // Processor components
    protected int[] registers;
    protected Instruction instructionRegister;
    protected int pcRegister;
    protected int clock = 0;

    // Aux Memory
    protected Memory memory;
    protected DataCache dataCache;
    protected InstructionsCache instructionsCache;

    // Data
    protected bool finished = false;
    public bool Finished => finished;

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

    public Memory Memory => memory;
    public DataCache DataCache => dataCache;
    public InstructionsCache InstructionCache => instructionsCache;
    public int GetRegister(int index) => registers[index];
    public int PC => pcRegister;
    public Instruction IR => instructionRegister;
    public int Clock => clock;

}
