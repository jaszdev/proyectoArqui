using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionCacheUI : MonoBehaviour
{
    public ProcessorComponent processorComponent;
    InstructionsCache instructionCache;
    public InstructionCacheBlockUI[] blocks;

    void Start()
    {
        GetInstructionCache();   
    }

    public void UpdateInstructionCacheUI()
    {
        if (instructionCache == null) GetInstructionCache();

        for (int i = 0; i < CacheConstants.Blocks; i++)
        {
            blocks[i].w0.text = instructionCache.GetInstruction(i, 0).Code.ToString();
            blocks[i].w1.text = instructionCache.GetInstruction(i, 1).Code.ToString();

            blocks[i].tag.text = TBL.InstIndexToDir(instructionCache.GetTag(i)).ToString();
        }
    }

    void GetInstructionCache()
    {
        if (processorComponent.processor != null)
            instructionCache = processorComponent.processor.InstructionCache;
    }
}
