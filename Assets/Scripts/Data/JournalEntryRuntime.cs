using System.Collections.Generic;

public class JournalEntryRuntime
{
    public string SuspectId;
    public Dictionary<string, bool> Answers = new Dictionary<string, bool>();
    public bool RecordUsed;
    public bool HasInterrogated;

    public JournalEntryRuntime(SuspectData suspect)
    {
        SuspectId = suspect != null ? suspect.SuspectId : string.Empty;

        if (suspect != null && suspect.JournalQuestions != null)
        {
            for (int i = 0; i < suspect.JournalQuestions.Count; i++)
            {
                var q = suspect.JournalQuestions[i];
                if (q == null || string.IsNullOrEmpty(q.QuestionId))
                    continue;

                if (!Answers.ContainsKey(q.QuestionId))
                {
                    Answers.Add(q.QuestionId, q.DefaultAnswer);
                }
            }
        }
    }
}