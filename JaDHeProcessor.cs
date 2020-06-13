using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Procesador descrito en el enunciado del proyecto
// Caracteristicas:
// Procesador RISC-V
// Cache Instrucciones Mapeo Directo * preguntar por estrategias para hit y miss, Algoritmo de remplazo de esta cache *
// Cache Datos Write Back Write Allocate Completamente Asociativa Alg Remplazamiento: LRU
public class JaDHeProcessor : Processor
{
    string[] programNames;

    public JaDHeProcessor(string[] programNames) : base()
    {
        this.programNames = programNames;
        Init();
    }

    protected override void Execute()
    {
        throw new System.NotImplementedException();
    }

    protected override void Init()
    {
        LoadPrograms();
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
