using UnityEngine;

public enum MoveCategory
{
    Punch,
    Kick,
    Knee,
    Elbow,
    Movement,
    Guard,
    StanceSwitch,
    HighGuard,
    LowGuard,
    Wait
}

public enum TargetZone
{
    Head,
    Body,
    Legs
}

public enum StrikeHand
{
    Lead,
    Rear,
    Both
}

[System.Serializable]
public class Move
{
    public string moveName;
    public MoveCategory category;

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

    // Constructor for attack/utility moves
    public Move(string name, MoveCategory cat, int Speed, float Power, int Energy, int Recovery,
                int optDist, int minDist, int maxDist, TargetZone target = TargetZone.Head,
                StrikeHand hand = StrikeHand.Both)
    {
        moveName = name;
        category = cat;
        speed = Speed;
        power = Power;
        energy = Energy;
        recovery = Recovery;
        distance = optDist;
        minDistance = minDist;
        maxDistance = maxDist;
        targetZone = target;
        strikeHand = hand;
        isMovement = false;
        movementAmount = 0;
    }

    // Constructor for movement moves
    public Move(string name, int Speed, int Recovery, int Energy, int moveAmount)
    {
        moveName = name;
        category = MoveCategory.Movement;
        speed = Speed;
        power = 0;
        recovery = Recovery;
        energy = Energy;
        distance = 0;
        minDistance = 0;
        maxDistance = 0;
        strikeHand = StrikeHand.Both;
        isMovement = true;
        movementAmount = moveAmount;
    }

    public int GetEffectiveSpeed(bool isLead)
    {
        if (strikeHand == StrikeHand.Lead && isLead)
            return speed + 2;
        return speed;
    }

    public int GetEffectiveRecovery(bool isLead)
    {
        if (strikeHand == StrikeHand.Lead && isLead)
            return Mathf.Max(0, recovery - 1);
        return recovery;
    }
}
