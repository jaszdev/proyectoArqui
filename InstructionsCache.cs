using System.Collections;
using System.Collections.Generic;

public class InstructionsCache : Cache<Instruction>
{

    public static Instruction INVALID_INSTRUCTION = new Instruction(0, 0, 0, 0);

    public InstructionsCache(int blocks, int words, Memory memory) : base(blocks, words, memory)
    {
    }

    public override Instruction Read(int direction)
    {
        throw new System.NotImplementedException();
    }

    public override bool Write(int direction, Instruction value)
    {
        throw new System.NotImplementedException();
    }

    public override void LoadBlock(int direction)
    {
        throw new System.NotImplementedException();
    }
}
