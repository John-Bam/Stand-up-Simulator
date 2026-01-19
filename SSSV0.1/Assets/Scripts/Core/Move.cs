using UnityEngine;

public enum MoveCategory
{
    Punch,
    Kick,
    Knee,
    Elbow,
    Movement,
    Guard,
    StanceSwitch
}

public enum TargetZone
{
    Head,
    Body,
    Legs
}

public enum StrikeHand
{
    Lead,      // Front hand/leg
    Rear,      // Back hand/leg
    Both       // Doesn't matter
}

[System.Serializable]
public class Move
{
    public string moveName;
    public MoveCategory category;

    // Core stats
    public int speed;
    public float power;
    public int recovery;
    public int energy;
    public int distance;
    public int minDistance;
    public int maxDistance;

    public TargetZone targetZone;
    public StrikeHand strikeHand;

    public bool isMovement;
    public int movementAmount;

    // Constructor for attack moves
    public Move(string name, MoveCategory cat, int spd, float pow, int rec, int eng,
                int optDist, int minDist, int maxDist, TargetZone target = TargetZone.Head,
                StrikeHand hand = StrikeHand.Both)
    {
        moveName = name;
        category = cat;
        speed = spd;
        power = pow;
        recovery = rec;
        energy = eng;
        distance = optDist;
        minDistance = minDist;
        maxDistance = maxDist;
        targetZone = target;
        strikeHand = hand;
        isMovement = false;
        movementAmount = 0;
    }
    // Constructor for movement moves
    public Move(string name, int spd, int rec, int eng, int moveAmount)
    {
        moveName = name;
        category = MoveCategory.Movement;
        speed = spd;
        power = 0;
        recovery = rec;
        energy = eng;
        distance = 0;
        minDistance = 0;
        maxDistance = 0;
        strikeHand = StrikeHand.Both;
        isMovement = true;
        movementAmount = moveAmount;
    }
    // Get speed with lead bonus
    public int GetEffectiveSpeed(bool isLead)
    {
        if (strikeHand == StrikeHand.Lead && isLead)
        {
            return speed + 2; // +2 speed bonus for lead strikes
        }
        return speed;
    }

    // Get recovery with lead bonus
    public int GetEffectiveRecovery(bool isLead)
    {
        if (strikeHand == StrikeHand.Lead && isLead)
        {
            return Mathf.Max(0, recovery - 1); // -1 recovery penalty (less penalty is good)
        }
        return recovery;
    }
}