using System.Collections;
using System.Collections.Generic;

static class ProcessorConstants
{
    public const int NumRegisters = 32;
    public const int MaxNumThreads = 5;
    public const int Quantum = 3;
}

public abstract class Processor
{

    // Processor components
    protected int[] registers = new int[ProcessorConstants.NumRegisters];
    protected Instruction instructionRegister;
    protected int pcRegister;


    // Aux Memory
    protected Memory memory;
    protected DataCache dataCache;
    protected InstructionsCache instructionsCache;

    // Processor Constructor
    public Processor()
    {
        memory = new Memory();
        dataCache = new DataCache(CacheConstants.Blocks, CacheConstants.Words, memory);
        instructionsCache = new InstructionsCache(CacheConstants.Blocks, CacheConstants.Words, memory);
    }

    protected abstract void Init();
    protected abstract void Execute();


}
