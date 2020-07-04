using UnityEngine;

public class DataCacheUI : MonoBehaviour
{

    public ProcessorComponent processorComponent;
    DataCache dataCache;
    public DataCacheBlockUI[] blocks;

    void Start()
    {
        GetDataCache();
    }

    public void UpdateDataCacheUI()
    {
        if (dataCache == null) GetDataCache();

        for(int i = 0; i < CacheConstants.Blocks; i++)
        {
            blocks[i].w0.text = dataCache.GetWord(i, 0).ToString();
            blocks[i].w1.text = dataCache.GetWord(i, 1).ToString();
            blocks[i].tag.text = TBL.DataIndexToDir(dataCache.GetTag(i)).ToString();
            
            string status = dataCache.GetStatus(i);
            blocks[i].status.text = status;
        }
    }

    void GetDataCache()
    {
        if (processorComponent.processor != null)
            dataCache = processorComponent.processor.DataCache;
    }
    
}
