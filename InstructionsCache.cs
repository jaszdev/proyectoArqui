using System.Collections;
using System.Collections.Generic;


// Cache de Instrucciones: Mapeo Directo Write Allocate
public class InstructionsCache : Cache<Instruction>
{

    public static Instruction INVALID_INSTRUCTION = new Instruction(0, 0, 0, 0);

    public InstructionsCache(int blocks, int words, Memory memory) : base(blocks, words, memory)
    {
        for (int i = 0; i < blocks; i++)
            for (int k = 0; k < words; k++)
                columns[i].words[k] = new Instruction(0, 0, 0, 0);
    }

    public override Instruction Read(int direction)
    {
        for (int i  = 0; i < blocks; i++)
        {
            if (columns[i].status == Status.Invalid) continue; // revisar siguiente 

            for (int k = 0; k < words; k++)
            {
                if (columns[i].tag + k == direction) // hit
                {
                    readMiss = false;
                    return columns[i].words[k];
                }
            }

        }
        // miss
        readMiss = true;
        return INVALID_INSTRUCTION;
    }

    // no se va a escribir en la cache de datos
    // la unica manera de cargar datos a esta cache 
    // es cargando el bloque de memoria 
    // usando LoadBlock(int direction)
    public override bool Write(int direction, Instruction value)
    {
        throw new System.NotImplementedException();
    }

    public override void LoadBlock(int direction)
    {
        // Calcular bloque
        int res = direction % words;
        if (res != 0) // direction no es inicio del bloque
            direction -= res; // restarle res a direction para obtener inicio del bloque 

        int blockIndex = (direction / words) % blocks; // Mapeo Directo
        columns[blockIndex].status = Status.Shared;
        columns[blockIndex].tag = direction;
        for (int i = 0; i < words; i++)
        {
            columns[blockIndex].words[i] = mainMemory.GetInstruction(direction + i);
        }
    }

    public Instruction GetInstruction(int block, int word) => columns[block].words[word];
    public int GetTag(int block) => columns[block].tag;

}
