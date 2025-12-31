
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementsData", menuName = "Elements /Elements Data")]
public class ElementsData : ScriptableObject
{
    public List<BaseTerrainElement> ElementList = new();
}
