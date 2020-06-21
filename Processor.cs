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
    protected int clock;

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

        instructionRegister = new Instruction(0, 0, 0, 0);
        pcRegister = 0;
        clock = 0;
    }

    protected abstract void Init();
    public abstract void Execute();

    public Memory Memory => memory;

}
