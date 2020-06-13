using System.Collections;
using System.Collections.Generic;

static class MemoryConstants
{
    public const int DataMemorySize = 48;
    public const int InstructionsMemorySize = 80;
    public const int Width = 1;
    public const int Latency = 20;
    public const int BusLatency = 1;
}

public struct Instruction
{
    int[] words;

    public Instruction(int instructionCode, int r1, int r2, int imm)
    {
        words = new int[] { instructionCode, r1, r2, imm };
    }

    public int Code => words[0];
    public int Register1 => words[1];
    public int Register2 => words[2];
    public int Immediate => words[3];

}

public class Memory
{

    int[] data = new int[MemoryConstants.DataMemorySize];
    Instruction[] instructions = new Instruction[MemoryConstants.InstructionsMemorySize];

    public int GetData(int direction) => data[direction];
    public Instruction GetInstruction(int direction) => instructions[direction];

    public void SetData(int direction, int value) => data[direction] = value;


}