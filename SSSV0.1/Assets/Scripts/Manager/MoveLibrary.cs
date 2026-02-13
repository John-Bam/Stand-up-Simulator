void InitializeMoves()
{
    // PUNCHES
    punchMoves.Add(new Move("Lead Jab", MoveCategory.Punch,
        Speed: 8, Power: 8f, Energy: 5, Recovery: 0,
        optDist: 2, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Lead));

    punchMoves.Add(new Move("Rear Cross", MoveCategory.Punch,
        Speed: 6, Power: 15f, Energy: 8, Recovery: 1,
        optDist: 2, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Rear));

    punchMoves.Add(new Move("Lead Hook", MoveCategory.Punch,
        Speed: 5, Power: 18f, Energy: 10, Recovery: 2,
        optDist: 1, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Lead));

    punchMoves.Add(new Move("Rear Hook", MoveCategory.Punch,
        Speed: 4, Power: 20f, Energy: 11, Recovery: 2,
        optDist: 1, minDist: 0, maxDist: 2, TargetZone.Head, StrikeHand.Rear));

    punchMoves.Add(new Move("Rear Uppercut", MoveCategory.Punch,
        Speed: 4, Power: 22f, Energy: 12, Recovery: 3,
        optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Rear));

    punchMoves.Add(new Move("Lead Body Shot", MoveCategory.Punch,
        Speed: 6, Power: 14f, Energy: 8, Recovery: 1,
        optDist: 1, minDist: 0, maxDist: 2, TargetZone.Body, StrikeHand.Lead));

    punchMoves.Add(new Move("Rear Body Shot", MoveCategory.Punch,
        Speed: 5, Power: 16f, Energy: 9, Recovery: 1,
        optDist: 1, minDist: 0, maxDist: 2, TargetZone.Body, StrikeHand.Rear));

    // KICKS
    kickMoves.Add(new Move("Lead Low Kick", MoveCategory.Kick,
        Speed: 5, Power: 12f, Energy: 10, Recovery: 2,
        optDist: 3, minDist: 2, maxDist: 4, TargetZone.Legs, StrikeHand.Lead));

    kickMoves.Add(new Move("Rear Low Kick", MoveCategory.Kick,
        Speed: 4, Power: 14f, Energy: 11, Recovery: 2,
        optDist: 3, minDist: 2, maxDist: 4, TargetZone.Legs, StrikeHand.Rear));

    kickMoves.Add(new Move("Lead Body Kick", MoveCategory.Kick,
        Speed: 4, Power: 16f, Energy: 12, Recovery: 3,
        optDist: 3, minDist: 2, maxDist: 4, TargetZone.Body, StrikeHand.Lead));

    kickMoves.Add(new Move("Rear Body Kick", MoveCategory.Kick,
        Speed: 3, Power: 18f, Energy: 13, Recovery: 3,
        optDist: 3, minDist: 2, maxDist: 4, TargetZone.Body, StrikeHand.Rear));

    kickMoves.Add(new Move("Lead Head Kick", MoveCategory.Kick,
        Speed: 3, Power: 28f, Energy: 15, Recovery: 4,
        optDist: 3, minDist: 2, maxDist: 4, TargetZone.Head, StrikeHand.Lead));

    kickMoves.Add(new Move("Rear Head Kick", MoveCategory.Kick,
        Speed: 2, Power: 32f, Energy: 16, Recovery: 4,
        optDist: 3, minDist: 2, maxDist: 4, TargetZone.Head, StrikeHand.Rear));

    kickMoves.Add(new Move("Front Kick", MoveCategory.Kick,
        Speed: 6, Power: 10f, Energy: 8, Recovery: 1,
        optDist: 4, minDist: 3, maxDist: 5, TargetZone.Body, StrikeHand.Lead));

    // SPECIAL MOVES
    specialMoves.Add(new Move("Lead Knee", MoveCategory.Knee,
        Speed: 5, Power: 20f, Energy: 11, Recovery: 2,
        optDist: 0, minDist: 0, maxDist: 1, TargetZone.Body, StrikeHand.Lead));

    specialMoves.Add(new Move("Rear Knee", MoveCategory.Knee,
        Speed: 4, Power: 22f, Energy: 12, Recovery: 2,
        optDist: 0, minDist: 0, maxDist: 1, TargetZone.Body, StrikeHand.Rear));

    specialMoves.Add(new Move("Lead Elbow", MoveCategory.Elbow,
        Speed: 6, Power: 17f, Energy: 10, Recovery: 2,
        optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Lead));

    specialMoves.Add(new Move("Rear Elbow", MoveCategory.Elbow,
        Speed: 5, Power: 19f, Energy: 11, Recovery: 2,
        optDist: 0, minDist: 0, maxDist: 1, TargetZone.Head, StrikeHand.Rear));

    specialMoves.Add(new Move("Flying Knee", MoveCategory.Knee,
        Speed: 3, Power: 30f, Energy: 20, Recovery: 5,
        optDist: 2, minDist: 1, maxDist: 3, TargetZone.Head, StrikeHand.Rear));

    // MOVEMENT
    movementMoves.Add(new Move("Step In", Speed: 7, Recovery: 0, Energy: 3, moveAmount: 1));
    movementMoves.Add(new Move("Step Back", Speed: 8, Recovery: 0, Energy: 3, moveAmount: 1));
    movementMoves.Add(new Move("Rush Forward", Speed: 5, Recovery: 1, Energy: 8, moveAmount: 2));
    movementMoves.Add(new Move("Retreat", Speed: 6, Recovery: 1, Energy: 6, moveAmount: 2));

    // HIGH GUARD - strong vs head attacks, weak vs body/legs
    Move highGuard = new Move("High Guard", MoveCategory.HighGuard,
        Speed: 10, Power: 0, Energy: 0, Recovery: 0,
        optDist: 0, minDist: 0, maxDist: 0);
    specialMoves.Add(highGuard);

    // LOW GUARD - strong vs body/leg attacks, weak vs head
    Move lowGuard = new Move("Low Guard", MoveCategory.LowGuard,
        Speed: 10, Power: 0, Energy: 0, Recovery: 0,
        optDist: 0, minDist: 0, maxDist: 0);
    specialMoves.Add(lowGuard);

    // WAIT - recover stamina, no block
    Move waitMove = new Move("Wait", MoveCategory.Wait,
        Speed: 10, Power: 0, Energy: 0, Recovery: 0,
        optDist: 0, minDist: 0, maxDist: 0);
    specialMoves.Add(waitMove);

    // STANCE SWITCH
    Move switchStance = new Move("Switch Stance", MoveCategory.StanceSwitch,
        Speed: 9, Power: 0, Energy: 2, Recovery: 0,
        optDist: 0, minDist: 0, maxDist: 0);
    specialMoves.Add(switchStance);
}
