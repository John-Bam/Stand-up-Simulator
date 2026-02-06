using UnityEnergyine;
using System.Collections.Generic;

public class MoveLibrary : MonoBehaviour
{
    public static MoveLibrary Instance;

    public List<Move> punchMoves = new List<Move>();
    public List<Move> kickMoves = new List<Move>();
    public List<Move> specialMoves = new List<Move>();
    public List<Move> movementMoves = new List<Move>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeMoves();
    }

    void InitializeMoves()
    {
        // PUNCHES - with Lead/Rear designation
        // Format: Move(name, category, speed, Power, Recovery, energy, optDist, minDist, maxDist, target, hand)

        punchMoves.Add(new Move("Lead Jab", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 2, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Lead));

        punchMoves.Add(new Move("Rear Cross", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 2, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Rear));

        punchMoves.Add(new Move("Lead Hook", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Lead));

        punchMoves.Add(new Move("Rear Hook", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Rear));

        punchMoves.Add(new Move("Rear Uppercut", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Rear));

        punchMoves.Add(new Move("Lead Body Shot", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Body, StrikeHand.Lead));

        punchMoves.Add(new Move("Rear Body Shot", MoveCategory.Punch,
            Speed: 8, Power: 8f, Recovery: 0, Energy: 5,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Body, StrikeHand.Rear));

        // KICKS
        kickMoves.Add(new Move("Lead Low Kick", MoveCategory.Kick,
            Speed: 5, Power: 12f, Recovery: 2, Energy: 10,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Legs, StrikeHand.Lead));

        kickMoves.Add(new Move("Rear Low Kick", MoveCategory.Kick,
            Speed: 4, Power: 14f, Recovery: 2, Energy: 11,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Legs, StrikeHand.Rear));

        kickMoves.Add(new Move("Lead Body Kick", MoveCategory.Kick,
            Speed: 4, Power: 16f, Recovery: 3, Energy: 12,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Body, StrikeHand.Lead));

        kickMoves.Add(new Move("Rear Body Kick", MoveCategory.Kick,
            Speed: 3, Power: 18f, Recovery: 3, Energy: 13,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Body, StrikeHand.Rear));

        kickMoves.Add(new Move("Lead Head Kick", MoveCategory.Kick,
            Speed: 3, Power: 28f, Recovery: 4, Energy: 15,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Head, StrikeHand.Lead));

        kickMoves.Add(new Move("Rear Head Kick", MoveCategory.Kick,
            Speed: 2, Power: 32f, Recovery: 4, Energy: 16,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Head, StrikeHand.Rear));

        kickMoves.Add(new Move("Front Kick", MoveCategory.Kick,
            Speed: 6, Power: 10f, Recovery: 1, Energy: 8,
            optDist: 4, minDist: 3, maxDist: 5, TargetZone.Body, StrikeHand.Lead));

        // SPECIAL MOVES
        specialMoves.Add(new Move("Lead Knee", MoveCategory.Knee,
            Speed: 5, Power: 20f, Recovery: 2, Energy: 11,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Body, StrikeHand.Lead));

        specialMoves.Add(new Move("Rear Knee", MoveCategory.Knee,
            Speed: 4, Power: 22f, Recovery: 2, Energy: 12,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Body, StrikeHand.Rear));

        specialMoves.Add(new Move("Lead Elbow", MoveCategory.Elbow,
            Speed: 6, Power: 17f, Recovery: 2, Energy: 10,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Lead));

        specialMoves.Add(new Move("Rear Elbow", MoveCategory.Elbow,
            Speed: 5, Power: 19f, Recovery: 2, Energy: 11,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Rear));

        specialMoves.Add(new Move("Flying Knee", MoveCategory.Knee,
            Speed: 3, Power: 30f, Recovery: 5, Energy: 20,
            optDist: 2, minDist: 1, maxDist: 3, TargetZone.Head, StrikeHand.Rear));

        // MOVEMENT
        movementMoves.Add(new Move("Step In", Speed: 7, Recovery: 0, Energy: 3, moveAmount: 1));
        movementMoves.Add(new Move("Step Back", Speed: 8, Recovery: 0, Energy: 3, moveAmount: 1));
        movementMoves.Add(new Move("Rush Forward", Speed: 5, Recovery: 1, Energy: 8, moveAmount: 2));
        movementMoves.Add(new Move("Retreat", Speed: 6, Recovery: 1, Energy: 6, moveAmount: 2));

        // GUARD
        Move guardMove = new Move("Guard", MoveCategory.Guard,
            Speed: 10, Power: 0, Recovery: 0, Energy: 0,
            optDist: 0, minDist: 0, maxDist: 0);
        specialMoves.Add(guardMove);

        // STANCE SWITCH
        Move switchStance = new Move("Switch Stance", MoveCategory.StanceSwitch,
            Speed: 9, Power: 0, Recovery: 0, Energy: 2,
            optDist: 0, minDist: 0, maxDist: 0);
        specialMoves.Add(switchStance);
    }
    public List<Move> GetAvailableMoves(Fighter fighter, int currentDistance)
    {
        List<Move> available = new List<Move>();

        foreach (Move m in punchMoves)
        {
            if (currentDistance <= m.maxDistance && fighter.stamina >= m.energy)
                available.Add(m);
        }

        foreach (Move m in kickMoves)
        {
            if (currentDistance >= m.minDistance && currentDistance <= m.maxDistance && fighter.stamina >= m.energy)
                available.Add(m);
        }

        foreach (Move m in specialMoves)
        {
            if ((m.category == MoveCategory.Guard || m.category == MoveCategory.StanceSwitch) ||
                (currentDistance <= m.maxDistance && currentDistance >= m.minDistance && fighter.stamina >= m.energy))
                available.Add(m);
        }

        foreach (Move m in movementMoves)
        {
            if (fighter.stamina >= m.energy)
                available.Add(m);
        }

        return available;
    }

}
