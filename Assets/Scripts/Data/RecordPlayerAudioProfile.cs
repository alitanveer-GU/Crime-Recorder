using UnityEngine;

[CreateAssetMenu(menuName = "Detective/Record Player Audio Profile", fileName = "RecordPlayerAudioProfile")]
public class RecordPlayerAudioProfile : ScriptableObject
{
    public AudioClip DetectiveTheme;
    public AudioClip NeutralTheme;
    public AudioClip SadTheme;
    public AudioClip HappyTheme;
    public AudioClip AfraidTheme;
    public AudioClip AngryTheme;

    public AudioClip GetClipForEmotion(EmotionType emotion)
    {
        switch (emotion)
        {
            case EmotionType.Sad:
                return SadTheme != null ? SadTheme : NeutralTheme;
            case EmotionType.Happy:
                return HappyTheme != null ? HappyTheme : NeutralTheme;
            case EmotionType.Afraid:
                return AfraidTheme != null ? AfraidTheme : NeutralTheme;
            case EmotionType.Angry:
                return AngryTheme != null ? AngryTheme : NeutralTheme;
            case EmotionType.Neutral:
            default:
                return NeutralTheme != null ? NeutralTheme : DetectiveTheme;
        }
    }
}