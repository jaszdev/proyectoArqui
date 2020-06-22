using System.Collections;
using System.Collections.Generic;

static class CacheConstants
{
    public const int Blocks = 4;
    public const int Words = 2;
}


public enum Status { Invalid, Shared, Modified };

public struct CacheColumn<T>
{
    public T[] words;
    public int tag;
    public Status status;
    
    public CacheColumn(int numWords)
    {
        words = new T[numWords];
        tag = 0;
        status = Status.Invalid;
    }
}


// Clase abstracta para describir componentes y funcionalidad de un cache
// de manera que sea facil implementar diferentes tipos de cache
// de diferentes tamaños y diferentes estrategias de hit y miss
public abstract class Cache<T> 
{
    protected int blocks, words;
    protected CacheColumn<T>[] columns;
    protected Memory mainMemory;

    protected bool readMiss; // booleano para indicar si hubo un miss al hacer un Read
    public bool ReadMiss => readMiss;

    public Cache(int blocks, int words, Memory memory)
    {
        this.blocks = blocks;
        this.words = words;
        columns = new CacheColumn<T>[blocks];
        for (int i = 0; i < blocks; i++) columns[i] = new CacheColumn<T>(words);

        mainMemory = memory;
    }
    
    // In case of hit, returns data
    // in case of miss, pone a readMiss en verdadero
    public abstract T Read(int direction);
    
    // In case of hit, writes in cache, returns true
    // in case of miss, returns false
    public abstract bool Write(int direction, T value);

    // Load block from mainMemory 
    public abstract void LoadBlock(int direction);


}
