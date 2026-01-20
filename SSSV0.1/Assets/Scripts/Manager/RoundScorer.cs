using UnityEngine;

public class RoundScorer
{
    // Calculate round score using 10-point must system
    public static RoundScore ScoreRound(Fighter fighter1, Fighter fighter2)
    {
        RoundScore score = new RoundScore();
        score.fighter1Name = fighter1.fighterName;
        score.fighter2Name = fighter2.fighterName;

        int f1Score = 10;
        int f2Score = 10;

        // 1. Damage dealt comparison
        float damageDiff = fighter1.totalDamageLanded - fighter2.totalDamageLanded;
        if (Mathf.Abs(damageDiff) > 10f)
        {
            if (damageDiff > 0)
                f2Score -= 1;
            else
                f1Score -= 1;
        }

        // 2. Knockdowns
        if (fighter1.knockdowns > fighter2.knockdowns)
        {
            f2Score -= 1;
        }
        else if (fighter2.knockdowns > fighter1.knockdowns)
        {
            f1Score -= 1;
        }

        // 3. Strike accuracy
        float f1Accuracy = fighter1.strikesAttempted > 0 ?
            (float)fighter1.strikesLanded / fighter1.strikesAttempted : 0f;
        float f2Accuracy = fighter2.strikesAttempted > 0 ?
            (float)fighter2.strikesLanded / fighter2.strikesAttempted : 0f;

        if (Mathf.Abs(f1Accuracy - f2Accuracy) > 0.2f)
        {
            if (f1Accuracy > f2Accuracy)
                f2Score -= 1;
            else
                f1Score -= 1;
        }

        // 4. Aggression (forward movement)
        if (fighter1.aggression > fighter2.aggression + 2)
        {
            f2Score -= 1;
        }
        else if (fighter2.aggression > fighter1.aggression + 2)
        {
            f1Score -= 1;
        }

        // 5. Defense (damage taken)
        float defenseDiff = fighter2.totalDamageTaken - fighter1.totalDamageTaken;
        if (Mathf.Abs(defenseDiff) > 15f)
        {
            if (defenseDiff > 0)
                f2Score -= 1;
            else
                f1Score -= 1;
        }

        // Ensure loser gets at least 7 points
        if (f1Score < 7) f1Score = 7;
        if (f2Score < 7) f2Score = 7;

        score.fighter1Score = f1Score;
        score.fighter2Score = f2Score;
        score.winner = f1Score > f2Score ? fighter1.fighterName :
                       (f2Score > f1Score ? fighter2.fighterName : "DRAW");

        return score;
    }
}
[System.Serializable]
public class RoundScore
{
    public string fighter1Name;
    public string fighter2Name;
    public int fighter1Score;
    public int fighter2Score;
    public string winner;

    public override string ToString()
    {
        return $"{fighter1Name}: {fighter1Score} | {fighter2Name}: {fighter2Score} - Winner: {winner}";
    }


}
