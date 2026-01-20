using UnityEngine;
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
        // Format: Move(name, category, speed, power, recovery, energy, optDist, minDist, maxDist, target, hand)

        punchMoves.Add(new Move("Lead Jab", MoveCategory.Punch,
            spd: 8, pow: 8f, rec: 0, eng: 5,
            optDist: 2, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Lead));

        punchMoves.Add(new Move("Rear Cross", MoveCategory.Punch,
            spd: 6, pow: 15f, rec: 1, eng: 8,
            optDist: 2, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Rear));

        punchMoves.Add(new Move("Lead Hook", MoveCategory.Punch,
            spd: 5, pow: 18f, rec: 2, eng: 10,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Lead));

        punchMoves.Add(new Move("Rear Hook", MoveCategory.Punch,
            spd: 4, pow: 20f, rec: 2, eng: 11,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Rear));

        punchMoves.Add(new Move("Rear Uppercut", MoveCategory.Punch,
            spd: 4, pow: 22f, rec: 3, eng: 12,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Rear));

        punchMoves.Add(new Move("Lead Body Shot", MoveCategory.Punch,
            spd: 6, pow: 14f, rec: 1, eng: 8,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Body, StrikeHand.Lead));

        punchMoves.Add(new Move("Rear Body Shot", MoveCategory.Punch,
            spd: 5, pow: 16f, rec: 1, eng: 9,
            optDist: 1, minDist: 0, maxDist: 2, TargetZone.Body, StrikeHand.Rear));

        // KICKS
        kickMoves.Add(new Move("Lead Low Kick", MoveCategory.Kick,
            spd: 5, pow: 12f, rec: 2, eng: 10,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Legs, StrikeHand.Lead));

        kickMoves.Add(new Move("Rear Low Kick", MoveCategory.Kick,
            spd: 4, pow: 14f, rec: 2, eng: 11,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Legs, StrikeHand.Rear));

        kickMoves.Add(new Move("Lead Body Kick", MoveCategory.Kick,
            spd: 4, pow: 16f, rec: 3, eng: 12,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Body, StrikeHand.Lead));

        kickMoves.Add(new Move("Rear Body Kick", MoveCategory.Kick,
            spd: 3, pow: 18f, rec: 3, eng: 13,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Body, StrikeHand.Rear));

        kickMoves.Add(new Move("Lead Head Kick", MoveCategory.Kick,
            spd: 3, pow: 28f, rec: 4, eng: 15,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Head, StrikeHand.Lead));

        kickMoves.Add(new Move("Rear Head Kick", MoveCategory.Kick,
            spd: 2, pow: 32f, rec: 4, eng: 16,
            optDist: 3, minDist: 2, maxDist: 4, TargetZone.Head, StrikeHand.Rear));

        kickMoves.Add(new Move("Front Kick", MoveCategory.Kick,
            spd: 6, pow: 10f, rec: 1, eng: 8,
            optDist: 4, minDist: 3, maxDist: 5, TargetZone.Body, StrikeHand.Lead));

        // SPECIAL MOVES
        specialMoves.Add(new Move("Lead Knee", MoveCategory.Knee,
            spd: 5, pow: 20f, rec: 2, eng: 11,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Body, StrikeHand.Lead));

        specialMoves.Add(new Move("Rear Knee", MoveCategory.Knee,
            spd: 4, pow: 22f, rec: 2, eng: 12,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Body, StrikeHand.Rear));

        specialMoves.Add(new Move("Lead Elbow", MoveCategory.Elbow,
            spd: 6, pow: 17f, rec: 2, eng: 10,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Lead));

        specialMoves.Add(new Move("Rear Elbow", MoveCategory.Elbow,
            spd: 5, pow: 19f, rec: 2, eng: 11,
            optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Rear));

        specialMoves.Add(new Move("Flying Knee", MoveCategory.Knee,
            spd: 3, pow: 30f, rec: 5, eng: 20,
            optDist: 2, minDist: 1, maxDist: 3, TargetZone.Head, StrikeHand.Rear));

        // MOVEMENT
        movementMoves.Add(new Move("Step In", spd: 7, rec: 0, eng: 3, moveAmount: 1));
        movementMoves.Add(new Move("Step Back", spd: 8, rec: 0, eng: 3, moveAmount: 1));
        movementMoves.Add(new Move("Rush Forward", spd: 5, rec: 1, eng: 8, moveAmount: 2));
        movementMoves.Add(new Move("Retreat", spd: 6, rec: 1, eng: 6, moveAmount: 2));

        // GUARD
        Move guardMove = new Move("Guard", MoveCategory.Guard,
            spd: 10, pow: 0, rec: 0, eng: 0,
            optDist: 0, minDist: 0, maxDist: 0);
        specialMoves.Add(guardMove);

        // STANCE SWITCH
        Move switchStance = new Move("Switch Stance", MoveCategory.StanceSwitch,
            spd: 9, pow: 0, rec: 0, eng: 2,
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
