using System.Collections;
using System.Collections.Generic;

// Cache Datos Write Back Write Allocate Completamente Asociativa Alg Remplazamiento: LRU
// Notas:
// tag -> direccion de la primera palabra del bloque
public class DataCache : Cache<int>
{
    // Información para Alg de Remplazamiento LRU
    int[] usedOrder; // Array de enteros para tener informacion de en que orden se han usado los bloques
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
            CacheColumn column = columns[i];
            if (column.status == Status.Shared) // Si la columna es valida
            {
                // Revisar direcciones de la columns
                for(int k = 0; k < words; k++)
                {
                    if (column.tag + k == direction) // si direccion de la palabra del bloque coincide con direction 
                    {
                        readMiss = false;
                        UpdateUsedOrder(i); // actualizar orden de uso de bloques
                        return column.words[k]; // hit, retornar palabra en posicion direction
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
        for(int i = 0; i < blocks; i++)
        {
            if (i == mrub) usedOrder[i] = 1; // bloque mrub es el mas recientemente usado
            else if (usedOrder[i] != -1) usedOrder[i] = usedOrder[i] + 1; // * posible error, que termine siendo mas grande que num blocks *
            // else if usedOrder[i] == -1, dejarlo en -1
        }
    }

    public override bool Write(int direction, int value)
    {
        // Revisar si la cache tiene el bloque
        for (int i = 0; i < blocks; i++)
        {
            CacheColumn column = columns[i];
            if (column.status == Status.Shared) // Si la columna es valida
            {
                // Revisar direcciones de la columns
                for (int k = 0; k < words; k++)
                {
                    if (column.tag + k == direction) // si direccion de la palabra del bloque coincide con direction 
                    {
                        column.words[k] = value; // Actualizar cache  * me parece que es necesario ademas tener el estatus de cache Modified, para saber si hay que bajar el dato a memoria * 
                        UpdateUsedOrder(i); // actualizar orden de uso de bloques
                        return true; // hit, retornar true
                    }
                }
            }
        }

        // miss, retornar falses
        return false;
    }

    protected override void LoadBlock(int direction)
    {
        // Buscar LRU Block o Bloque Invalido
        for (int i = 0; i < blocks; i++)
        {
            CacheColumn column = columns[i];
            if (usedOrder[i] == blocks || usedOrder[i] == -1) // Cargar en bloque usado menos recientemente
            {
                // Cargar palabras
                for(int k = 0; k < words; k++)
                {
                    column.words[k] = mainMemory.GetData(direction + k);
                }
                // Actualizar datos de la cache
                column.tag = direction;
                column.status = Status.Shared;
                UpdateUsedOrder(i); // actualizar orden de uso de bloques
                return; // Bloque fue cargado a cache
            }
        }

    }
}
