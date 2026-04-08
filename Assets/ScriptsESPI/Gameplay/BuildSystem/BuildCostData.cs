using System;
using System.Collections.Generic;

[Serializable]
public class BuildCostData
{
    public BuildType buildType;
    public List<ResourceAmount> costs = new List<ResourceAmount>();
}