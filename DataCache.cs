using System.Collections;
using System.Collections.Generic;

public class DataCache : Cache<int>
{

    public DataCache(int blocks, int words, Memory memory) : base(blocks, words, memory)
    {
    }

    public override int Read(int direction)
    {
        throw new System.NotImplementedException();
    }

    public override bool Write(int direction, int value)
    {
        throw new System.NotImplementedException();
    }

    protected override bool LoadBlock(int direction)
    {
        throw new System.NotImplementedException();
    }
}
