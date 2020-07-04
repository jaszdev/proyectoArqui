using UnityEngine;

// Procesador descrito en el enunciado del proyecto
// Caracteristicas:
// Procesador RISC-V
// Cache Instrucciones Mapeo Directo Write Allocate
// Cache Datos Write Back Write Allocate Completamente Asociativa Alg Remplazamiento: LRU
public class JaDHeProcessor : Processor
{

    public int r1;
    public int r2;
    public int r3;
    public int imm;

    public JaDHeProcessor() : base()
    {        
    }

    public void ConsoleInit(string[] programNames, int quantum)
    {
        this.quantum = quantum;
        Init();

        threads = programNames.Length;
        contextMemory = new ContextMemory(threads);
        threadsDuration = new int[threads];
        LoadPrograms(programNames);
        

        // inicializar estado de los threads
        threadStates = new ThreadState[threads];
        for (int i = 0; i < threads; i++) threadStates[i] = ThreadState.Running;

        currentThread = 0; // se empiza a correr el primer programa en la lista de programas
        LoadContext(currentThread);
    }

    protected override void Init()
    {
        finished = false;

        // inicializar registros
        for (int i = 0; i < ProcessorConstants.NumRegisters; i++) registers[i] = 0;
        // inicializar memoria
        memory.Init();
        // inicializar caches
        dataCache.Init();
        instructionsCache.Init();

        // inicializar datos del procesador
        rl = -12;
        clock = 0;
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

        threadsDuration[currentThread]++;

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

        // variables que se usan en diferentes casos
        int dataDirection, val, address, sourceReg;
        bool writeHit;
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
                MemoryAccess++;
                LoadAccess++;
                dataDirection = TBL.DataDirToIndex(imm + registers[r2]);
                val = dataCache.Read(dataDirection);
                if (!dataCache.ReadMiss) // lw hit
                {
                    registers[r1] = val;
                }
                else // lw miss
                {
                    TotalCacheMisses++;
                    LoadMisses++;
                    dataCache.LoadBlock(dataDirection); // Write Allocate
                    val = dataCache.Read(dataDirection);
                    registers[r1] = val;
                    clock += ProcessorConstants.CacheMissDelay;
                }
                break;
            case 37: // sw
                MemoryAccess++;
                StoreAccess++;
                address = imm + r1;
                sourceReg = r2;
                dataDirection = TBL.DataDirToIndex(address);
                writeHit = dataCache.Write(dataDirection, registers[sourceReg]);
                if (!writeHit) // sw miss
                {
                    TotalCacheMisses++;
                    StoreMisses++;
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
                // x1 <- M[x2]
                dataDirection = TBL.DataDirToIndex(registers[r2]);
                val = dataCache.Read(dataDirection);
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
                rl = registers[r2]; // RL <- x2
                break;
            case 52: //sc
                int x1 = r2;
                int x2 = r1;
                if (RL == imm + registers[x2])
                {
                    address = imm + registers[x2];
                    sourceReg = x1;
                    dataDirection = TBL.DataDirToIndex(address);
                    writeHit = dataCache.Write(dataDirection, registers[sourceReg]);
                    if (!writeHit) // sw miss
                    {
                        dataCache.LoadBlock(dataDirection); // Write Allocate
                        dataCache.Write(dataDirection, registers[sourceReg]);
                        clock += ProcessorConstants.CacheMissDelay;
                    }
                }
                else
                {
                    registers[x1] = 0;
                }
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
    void LoadPrograms(string[] programNames)
    {
        int i = 0;
        int thread = 0;
        foreach (string program in programNames)
        {
            bool firstLine = true;

            if (!System.IO.File.Exists(program)) // si el archivo no existe
                continue; // continuar con el siguiente

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

    public void WriteResults(string filename)
    {
        System.IO.StreamWriter resultsFile = new System.IO.StreamWriter(filename);

        // memoria de datos
        resultsFile.Write("Memoria de Datos\n\n");
        int groups = 4;
        for (int i = 0; i < MemoryConstants.DataMemorySize; i += groups)
        {
            int dir = i * 4;
            string mem_s = "Dir: " + dir + " -> ";
            for(int k = 0; k < groups; k++)
            {
                mem_s += memory.GetData(i + k) + " ";
            }
            resultsFile.Write(mem_s + "\n");
        }

        // cache de datos
        resultsFile.Write("\nCache de Datos\n\n");
        for(int i = 0; i < dataCache.Blocks; i++)
        {
            string block_s = "Bloque " + i + " -> ";
            for(int k = 0; k < dataCache.Words; k++)
            {
                block_s += "W" + k + ": " + dataCache.GetWord(i, k) + " ";
            }
            block_s += " Estatus: " + dataCache.GetStatus(i);
            block_s += " Etiqueta: " + dataCache.GetTag(i) + "\n";
            resultsFile.Write(block_s);
        }

        // tasas de fallo
        resultsFile.Write("\nTasas de Fallo\n\n");
        // general
        float t = TotalCacheMisses / (float)MemoryAccess;
        resultsFile.Write("General: Accesos a memoria: " + MemoryAccess
            + " Total de Fallos: " + TotalCacheMisses +
            " Tasa: " + t + "\n");
        // lecturas
        t = LoadMisses / (float)LoadAccess;
        resultsFile.Write("Lecturas: Accesos a memoria: " + LoadAccess
            + " Total de Fallos: " + LoadMisses +
            " Tasa: " + t + "\n");
        // stores
        t = StoreMisses / (float)StoreAccess;
        resultsFile.Write("Escritura: Accesos a memoria: " + StoreAccess
            + " Total de Fallos: " + StoreMisses +
            " Tasa: " + t + "\n");

        // contexto de memoria
        resultsFile.Write("\nRegistros de los Hilos\n\n");
        for (int i = 0; i < threads; i++)
        {
            string rS = "H" + i + ": ";
            for(int k = 0; k < ProcessorConstants.NumRegisters; k++)
            {
                rS += contextMemory.GetRegister(i, k) + " ";
            }
            resultsFile.WriteLine(rS + " Duracion: " + threadsDuration[i] +"\n");
        }

        resultsFile.Close();
    }


}
