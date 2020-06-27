using UnityEngine;

// Procesador descrito en el enunciado del proyecto
// Caracteristicas:
// Procesador RISC-V
// Cache Instrucciones Mapeo Directo Write Allocate
// Cache Datos Write Back Write Allocate Completamente Asociativa Alg Remplazamiento: LRU
public class JaDHeProcessor : Processor
{
    string[] programNames;

    public int r1;
    public int r2;
    public int r3;
    public int imm;

    public JaDHeProcessor(string[] programNames) : base()
    {
        this.programNames = programNames;

        threads = programNames.Length; // la cantidad de threads es igual a la cantidad de programas

        threadStates = new ThreadState[threads];
        for (int i = 0; i < threads; i++) threadStates[i] = ThreadState.Running;

        // se crea una memoria para guardar contextos en base
        // a la cantidad de hilos que se calcularon
        // a la hora de cargar los programas
        contextMemory = new ContextMemory(threads);
        
        Init();
    }

    protected override void Init()
    {
        LoadPrograms();
        LoadContext(currentThread); // cargar el contexto del hilo 0 (primer programa)
    }

    public override void Execute()
    {
        // Fetch
        int instructionDirection = TBL.InstDirToIndex(pcRegister);
        instructionRegister = instructionsCache.Read(instructionDirection);

        if (instructionRegister.Equals(InstructionsCache.INVALID_INSTRUCTION)) // Miss
        {
            instructionsCache.LoadBlock(instructionDirection); // Write Allocate
            instructionRegister = instructionsCache.Read(instructionDirection);
            clock += ProcessorConstants.CacheMissDelay;
        }

        pcRegister += 4; // siguiente instruccion

        // Decode and Execute
        DecodeAndExecute();

        quantumCounter++;
        if (quantumCounter == quantum) // si el hilo ya "acabo" su quantum
            SwitchContext(); // Cambiar de contexto
    }

    void DecodeAndExecute()
    {
        // Decode 
        r1 = instructionRegister.Register1;
        r2 = instructionRegister.Register2;
        r3 = imm = instructionRegister.Immediate;

        int dataDirection;
        // Execute
        switch (instructionRegister.Code)
        {
            case 19: // addi
                registers[r1] = registers[r2] + imm;
                break;
            case 71: // add
                registers[r1] = registers[r2] + registers[r3];
                break;
            case 83: // sub
                registers[r1] = registers[r2] - registers[r3];
                break;
            case 72: // mul
                registers[r1] = registers[r2] * registers[r3];
                break;
            case 56: // div
                registers[r1] = registers[r2] / registers[r3];
                break;
            case 5: // lw
                dataDirection = TBL.DataDirToIndex(imm + registers[r2]);
                int val = dataCache.Read(dataDirection);
                if (!dataCache.ReadMiss) // lw hit
                {
                    registers[r1] = val;
                }
                else // lw miss
                {
                    dataCache.LoadBlock(dataDirection); // Write Allocate
                    val = dataCache.Read(dataDirection);
                    registers[r1] = val;
                    clock += ProcessorConstants.CacheMissDelay;
                }
                break;
            case 37: // sw
                int address = imm + r1;
                int sourceReg = r2;
                dataDirection = TBL.DataDirToIndex(address);
                bool writeHit = dataCache.Write(dataDirection, registers[sourceReg]);
                if (!writeHit) // sw miss
                {
                    dataCache.LoadBlock(dataDirection); // Write Allocate
                    dataCache.Write(dataDirection, registers[sourceReg]);
                    clock += ProcessorConstants.CacheMissDelay;
                }
                break;
            case 99: // beq
                if (registers[r1] == registers[r2])
                {
                    pcRegister += 4 * imm;
                }
                break;
            case 100: //bne
                if (registers[r1] != registers[r2])
                {
                    pcRegister += 4 * imm;
                }
                break;
            case 51: // lr
                break;
            case 52: //sc
                break;
            case 111: // jal
                registers[r1] = pcRegister;
                pcRegister += imm;
                break;
            case 103: // jalr
                registers[r1] = pcRegister;
                pcRegister = registers[r2] + imm;
                break;
            case 999: // end
                threadStates[currentThread] = ThreadState.Finished; // marcar que el thread actual finalizo
                Debug.Log("Hilo #" + currentThread + " finalizo.");
                SwitchContext(); // cambiar de contexto a siguiente hilo
                break;
            default: // operacion no soportada
                Debug.Log("Instruccion no Soportada: " + instructionRegister);
                break;
        }
        clock++;
    }

    // Cargar programas a memoria 
    // esta funcionalidad podria ser traspasada a otra clase
    void LoadPrograms()
    {
        int i = 0;
        int thread = 0;
        foreach (string program in programNames)
        {
            bool firstLine = true;
            string[] lines = System.IO.File.ReadAllLines(program);
            foreach(string line in lines)
            {
                string[] args = line.Split(' ');
                int instructionCode = int.Parse(args[0]);
                int r1 = int.Parse(args[1]);
                int r2 = int.Parse(args[2]);
                int imm = int.Parse(args[3]);
                Instruction instruction = new Instruction(instructionCode, r1, r2, imm);
                memory.WriteInstruction(i, instruction);

                if (firstLine) // guardar el inicio de ejecucion de cada hilo
                {
                    int instDir = TBL.InstIndexToDir(i); // direccion de la instruccion
                    contextMemory.SetPC(thread, instDir);
                    firstLine = false;
                    //Debug.Log("Hilo " + threads + " PC Inicial: " + instDir);
                }

                i++;
            }
            thread++;
        }
    }

}
