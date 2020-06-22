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

        // se empezara ejecutando en la primera palabra
        // de la memoria de instrucciones, que corresponde
        // a la siguiente direccion
        pcRegister = 4 * MemoryConstants.DataMemorySize; 

        Init();
    }

    protected override void Init()
    {
        LoadPrograms();
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


        // Decode and Execute
        DecodeAndExecute();

        pcRegister += 4; // siguiente instruccion
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
                dataDirection = TBL.DataDirToIndex(imm + registers[r2]);
                bool writeHit = dataCache.Write(dataDirection, registers[r1]);
                if (!writeHit) // sw miss
                {
                    dataCache.LoadBlock(dataDirection); // Write Allocate
                    dataCache.Write(dataDirection, registers[r1]);
                    clock += ProcessorConstants.CacheMissDelay;
                }
                break;
            case 99: // beq
                break;
            case 100: //bne
                break;
            case 51: // lr
                break;
            case 52: //sc
                break;
            case 111: // jal
                break;
            case 103: // jalr
                break;
            case 999: // end
                finished = true;
                break;
            default: // operacion no soportada
                break;
        }
        clock++;
    }

    // Cargar programas a memoria 
    // esta funcionalidad podria ser traspasada a otra clase
    void LoadPrograms()
    {
        int i = 0;
        foreach (string program in programNames)
        {
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
                i++;
            }
        }
    }

}
