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
            blocks[i].tag.text = dataCache.GetTag(i).ToString();

            Status blockStatus = dataCache.GetStatus(i);
            string status = "";
            switch (blockStatus)
            {
                case Status.Invalid:
                    status = "I";
                    break;
                case Status.Shared:
                    status = "C";
                    break;
                case Status.Modified:
                    status = "M";
                    break;
                default:
                    break;
            }

            blocks[i].status.text = status;
        }
    }

    void GetDataCache()
    {
        if (processorComponent.processor != null)
            dataCache = processorComponent.processor.DataCache;
    }
    
}
