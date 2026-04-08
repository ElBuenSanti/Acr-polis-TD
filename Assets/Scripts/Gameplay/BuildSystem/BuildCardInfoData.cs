using System;
using UnityEngine;

// Data container for a single build card info entry
[Serializable]
public class BuildCardInfoData
{
    public BuildType buildType;
    public string title;
    public string type;
    [TextArea(2, 4)] public string description;
    public string cost;
    public string role;
}