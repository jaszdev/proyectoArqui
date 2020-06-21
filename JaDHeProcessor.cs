using System.Collections;
using System.Collections.Generic;
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
        Init();
    }

    public override void Execute()
    {
        // Fetch
        //instructionRegister = sdfdsfsf;


        // Decode and Execute
        DecodeAndExecute();

        // op
        // r1
        // r2
        // imm


        // op(r1, r2, imm)

        // Mem 

        // escribir o leer de memoria 

        // WB

        // escribir a registros
    }

    protected override void Init()
    {
        LoadPrograms();
    }

    void DecodeAndExecute()
    {
        r1 = instructionRegister.Register1;
        r2 = instructionRegister.Register2;
        r3 = imm = instructionRegister.Immediate;
        switch(instructionRegister.Code)
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
                int val = dataCache.Read(imm + registers[r2]);
                if (!dataCache.ReadMiss) // lw hit
                {
                    registers[r1] = val;
                }
                else // lw miss
                {

                }
                break;
            case 37: // sw
                int dir = imm + registers[r2];
                int direction = -1; //req tbc
                bool writeHit = dataCache.Write(direction, registers[r1]);
                if (!writeHit) // sw miss
                {

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
                break;
            default: // operacion no soportada
                break;
        }
    }

    // Cargar programas a memoria 
    // esta funcionalidad podria ser traspasada a otra clase
    void LoadPrograms()
    {
        foreach(string program in programNames)
        {
            string[] lines = System.IO.File.ReadAllLines(program);
            int i = 0;
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
