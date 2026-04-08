using System.Collections.Generic;
using UnityEngine;

// Stores all build card info data and provides lookup by BuildType
public class BuildCardInfoDatabase : MonoBehaviour
{
    [Header("Build Card Data")]
    [SerializeField] private List<BuildCardInfoData> buildCardDataList = new List<BuildCardInfoData>();

    private Dictionary<BuildType, BuildCardInfoData> buildCardDataDictionary;

    private void Awake()
    {
        BuildLookupDictionary();
    }

    private void BuildLookupDictionary()
    {
        buildCardDataDictionary = new Dictionary<BuildType, BuildCardInfoData>();

        foreach (BuildCardInfoData data in buildCardDataList)
        {
            if (data == null)
            {
                continue;
            }

            if (buildCardDataDictionary.ContainsKey(data.buildType))
            {
                Debug.LogWarning("BuildCardInfoDatabase: Duplicate entry found for " + data.buildType);
                continue;
            }

            buildCardDataDictionary.Add(data.buildType, data);
        }
    }

    public bool TryGetBuildCardInfo(BuildType buildType, out BuildCardInfoData data)
    {
        data = null;

        if (buildCardDataDictionary == null)
        {
            BuildLookupDictionary();
        }

        return buildCardDataDictionary.TryGetValue(buildType, out data);
    }
}