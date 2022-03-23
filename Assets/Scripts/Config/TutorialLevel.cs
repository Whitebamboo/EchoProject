using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Config/Tutorial Level")]
public class TutorialLevel : ScriptableObject
{
    public string levelDescription;
    public List<TutorialCloth> clothes;
}

[System.Serializable]
public class TutorialCloth
{
    public Sprite image;
    public string clothDescription;
}
