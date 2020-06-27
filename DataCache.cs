using UnityEngine;

// Cache Datos Write Back Write Allocate Completamente Asociativa Alg Remplazamiento: LRU
// Notas:
// tag -> direccion de la primera palabra del bloque
public class DataCache : Cache<int>
{
    // Información para Alg de Remplazamiento LRU
    int[] usedOrder; // Array de enteros para tener informacion de en que orden se han usado los bloques
    int last_mrub = -1;
    // -1: bloque invalido
    // 1: bloque mas recientemente usado
    // b: b == num blocks (b > 1), bloque usado de ultimo -> bloque a ser remplazado

    public DataCache(int blocks, int words, Memory memory) : base(blocks, words, memory)
    {
        usedOrder = new int[blocks]; // Array de orden de uso tiene un espacio por cada bloque
        for (int i = 0; i < blocks; i++) usedOrder[i] = -1; // Todas las posiciones comienzan en invalido
    }

    // Mejorias:
    // Se podria mejorar el algoritmo para buscar si la cache tiene el palabra con
    // la direccion direction.
    public override int Read(int direction)
    {
        // Revisar si la cache tiene el bloque
        for(int i = 0; i < blocks; i++)
        {
            if (columns[i].status != Status.Invalid) // Si la columna es valida
            {
                // Revisar direcciones de la columns
                for (int k = 0; k < words; k++)
                {
                    if (columns[i].tag + k == direction) // si direccion de la palabra del bloque coincide con direction 
                    {
                        readMiss = false;
                        UpdateUsedOrder(i); // actualizar orden de uso de bloques
                        return columns[i].words[k]; // hit, retornar palabra en posicion direction
                    }
                }
            }
        }

        // miss, retornar dato nulo
        readMiss = true;
        return -1; 
    }

    // Actualiza el array usedOrder usando mrub: most recently used block
    void UpdateUsedOrder(int mrub)
    {
        if (mrub == last_mrub) return;

        for (int i = 0; i < blocks; i++)
        {
            if (i == mrub) usedOrder[i] = 1; // bloque mrub es el mas recientemente usado
            else if (usedOrder[i] != -1) usedOrder[i]++; // * posible error, que termine siendo mas grande que num blocks *
            // else if usedOrder[i] == -1, dejarlo en -1
        }
        last_mrub = mrub;
        // print userorder
        //string uo = "";
        //for (int i = 0; i < blocks; i++) uo += usedOrder[i] + " ";
        //Debug.Log(uo);
    }

    public override bool Write(int direction, int value)
    {
        // Revisar si la cache tiene el bloque
        for (int i = 0; i < blocks; i++)
        {
            if (columns[i].status !=  Status.Invalid) // Si la columna es valida
            {
                // Revisar direcciones de la columns
                for (int k = 0; k < words; k++)
                {
                    if (columns[i].tag + k == direction) // si direccion de la palabra del bloque coincide con direction 
                    {
                        columns[i].words[k] = value; // Actualizar cache 
                        columns[i].status = Status.Modified;
                        UpdateUsedOrder(i); // actualizar orden de uso de bloques
                        return true; // hit, retornar true
                    }
                }
            }
        }

        // miss, retornar falses
        return false;
    }

    public override void LoadBlock(int direction)
    {
        // Calcular bloque
        int res = direction % words; // * before % words
        if (res != 0) // direction no es inicio del bloque
            direction -= res; // restarle res a direction para obtener inicio del bloque 

        // Buscar LRU Block o Bloque Invalido
        for (int i = 0; i < blocks; i++)
        {
            if (usedOrder[i] == -1 || usedOrder[i] == blocks) // Cargar en bloque usado menos recientemente o bloque invalido
            {
                // Revisar si bloque esta modificado
                if (columns[i].status == Status.Modified)
                {
                    // Escribir bloque en memoria Write Back
                    for (int k = 0; k < words; k++)
                    {
                        mainMemory.WriteData(columns[i].tag + k, columns[i].words[k]);
                    }
                }

                // Cargar palabras
                for(int k = 0; k < words; k++)
                {
                    columns[i].words[k] = mainMemory.GetData(direction + k);
                }
                // Actualizar datos de la cache
                columns[i].tag = direction;
                columns[i].status = Status.Shared;
                UpdateUsedOrder(i); // actualizar orden de uso de bloques
                return; // Bloque fue cargado a cache
            }
        }

    }

    public int GetWord(int block, int word) => columns[block].words[word];
    public int GetTag(int block) => columns[block].tag;
    public Status GetStatus(int block) => columns[block].status;

}
