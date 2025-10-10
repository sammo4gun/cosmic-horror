using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class PauseCalculator : Node
{
    public Dictionary<int, float> Pauses;
    
    [GeneratedRegex(@"({p=\d([.]\d+)?[}])")]
    private static partial Regex PauseFinderRegex();
    [GeneratedRegex(@"\d+\.\d+")]
    private static partial Regex PauseLengthRegex();

    public string ExtractPausesFromString(string sourceContent)
    {
        Pauses = [];
        CalculatePauses(sourceContent);
        return RemovePauses(sourceContent);
    }

    private void CalculatePauses(string sourceText)
    {
        MatchCollection pauseTexts = PauseFinderRegex().Matches(sourceText);
        int offset = 0;
        for (int count = 0; count < pauseTexts.Count; count++)
        {
            AddPauseFromText(pauseTexts[count].Index - offset, pauseTexts[count].Value);
            offset += pauseTexts[count].Value.Length;
        }
    }

    private void AddPauseFromText(int index, string expression)
    {
        float s = float.Parse(PauseLengthRegex().Matches(expression)[0].Value);
        Pauses[index] = s;
    }

    private static string RemovePauses(string sourceText)
    {
        return PauseFinderRegex().Replace(sourceText, "");
    }

}
