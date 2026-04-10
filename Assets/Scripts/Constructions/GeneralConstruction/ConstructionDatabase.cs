using UnityEngine;

public class ConstructionDatabase : MonoBehaviour
{
    public ConstructionData[] allData;

    public ConstructionData GetData(ConstructionType type, GodType god, int level) //te regresa la construcción que necesitas
    {
        foreach (var data in allData)
        {
            if (data.type == type && data.god == god && data.level == level)
                return data;
        }

        return null;
    }
}
