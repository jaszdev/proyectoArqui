using System.Collections;
using System.Collections.Generic;

static class MemoryConstants
{
    public const int DataMemorySize = 48 * 2;
    public const int InstructionsMemorySize = 80 * 2;
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

    public void Set(int instructionCode, int r1, int r2, int imm)
    {
        words[0] = instructionCode;
        words[1] = r1;
        words[2] = r2;
        words[3] = imm;
    }

    public override string ToString() => Code + " " + Register1 + " " + Register2 + " " + Immediate;

    public override bool Equals(object obj)
    {
        Instruction instruction = (Instruction)obj;
        return
            Code == instruction.Code &&
            Register1 == instruction.Register1 &&
            Register2 == instruction.Register2 &&
            Immediate == instruction.Immediate;
    }

}

public class Memory
{
    int[] data;
    Instruction[] instructions;

    public Memory()
    {
        data = new int[MemoryConstants.DataMemorySize];
        for (int i = 0; i < MemoryConstants.DataMemorySize; i++) // inicializar toda la memoria en 1
            data[i] = 1;

        instructions = new Instruction[MemoryConstants.InstructionsMemorySize];
        for (int i = 0; i < MemoryConstants.InstructionsMemorySize; i++)
            instructions[i] = new Instruction(0, 0, 0, 0);
    }

    public int GetData(int direction) => data[direction];
    public Instruction GetInstruction(int direction) => instructions[direction];

    public void WriteData(int direction, int value) => data[direction] = value;
    public void WriteInstruction(int direction, Instruction instruction) => instructions[direction] = instruction;

}