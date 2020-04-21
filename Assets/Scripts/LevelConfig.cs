using UnityEngine;

[System.Serializable]
public class Organ
{
    [Range(0, 100)]
    public float offsetPercentage;

    public OrganName organName;
    public float bpm;
    public float startAtTime;

    public KeyCode key;
}

public enum OrganName
{
    Heart_Left,
    Heart_Right,
    Lungs
} // The specific type of organ, to sync with the organ objects without using strings

[CreateAssetMenu(fileName = "Level Config", menuName = "Level/Level Config")]
public class LevelConfig : ScriptableObject
{
    public float levelLengthSeconds;
    public Organ[] levelOrgans;
}
