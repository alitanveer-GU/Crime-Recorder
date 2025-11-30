using UnityEngine;

[CreateAssetMenu(menuName = "Detective/Clue", fileName = "NewClue")]
public class ClueData : ScriptableObject
{
    public string ClueId;
    public string Title;
    [TextArea] public string Description;
}