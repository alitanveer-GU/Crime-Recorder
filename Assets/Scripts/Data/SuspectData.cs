using System.Collections.Generic;
using UnityEngine;

public enum EmotionType
{
    Neutral,
    Sad,
    Happy,
    Afraid,
    Angry
}

[System.Serializable]
public class JournalQuestionConfig
{
    public string QuestionId;
    [TextArea] public string QuestionText;
    public bool DefaultAnswer;
}

[CreateAssetMenu(menuName = "Detective/Suspect", fileName = "NewSuspect")]
public class SuspectData : ScriptableObject
{
    [Header("Identity")]
    public string SuspectId;
    public string DisplayName;
    [TextArea] public string ShortDescription;

    [Header("Role In Case")]
    public bool IsCulprit;
    public EmotionType Emotion = EmotionType.Neutral;

    [Header("Dialogue (Yarn)")]
    public string IntroDialogueNode;
    public string AfterRecordDialogueNode;
    public string PostAccusationDialogueNode;

    [Header("Journal Questions")]
    public List<JournalQuestionConfig> JournalQuestions = new List<JournalQuestionConfig>();
}