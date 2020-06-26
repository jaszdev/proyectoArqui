
// Memoria para guardar los contextos de los hilos
// del procesador
// Formato
// PC de hilo i -> posicion i * (1 + ProcessorConstants.NumRegisters)
// Reg de hilo i -> posicion i + 1 a i + 1 + ProcessorConstants.NumRegisters
public class ContextMemory 
{
    int[] memory; // array para guardar pCs y registros
    int size;

    public ContextMemory(int numThreads)
    {
        size = numThreads * (1 + ProcessorConstants.NumRegisters);
        memory = new int[size];
    }
   
    public void SetPC(int thread, int value)
    {
        int pos = thread * (1 + ProcessorConstants.NumRegisters);
        memory[pos] = value;
    }

    public int GetPC(int thread)
    {
        int pos = thread * (1 + ProcessorConstants.NumRegisters);
        return memory[pos];
    }

    // reg : 0 a ProcessorConstants.NumRegisters - 1
    public void SetReg(int thread, int reg, int value)
    {
        int pos = thread * (1 + ProcessorConstants.NumRegisters) + (reg + 1);
        memory[pos] = value;
    }

    // reg : 0 a ProcessorConstants.NumRegisters - 1
    public int GetReg(int thread, int reg)
    {
        int pos = thread * (1 + ProcessorConstants.NumRegisters) + (reg + 1);
        return memory[pos];
    }

}
