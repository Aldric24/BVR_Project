using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Mission")]
[Serializable]
public class Mission : ScriptableObject
{
    public Sprite missionImage;
    public string missionName;
    [TextArea(3, 10)]
    public string missionDescription;
    public string recommendedLoadout;
}


