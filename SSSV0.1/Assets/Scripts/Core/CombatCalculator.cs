using UnityEngine;

public enum GuardZone
{
    None,
    High,
    Low
}

public class CombatCalculator
{
    // Calculate initiative for turn order
    public static int CalculateInitiative(Fighter fighter, Move move)
    {
        int baseRoll = Random.Range(1, 5);  // Random 1-4

        // Check if this is a lead strike
        bool isLead = (move.strikeHand == StrikeHand.Lead);
        int moveSpeed = move.GetEffectiveSpeed(isLead);

        // GetStatModifier returns float, convert to int for initiative
        int statBonus = Mathf.RoundToInt(fighter.GetStatModifier(move.category));
        int recoveryPenalty = fighter.recoveryPenalty;

        int initiative = baseRoll + moveSpeed + statBonus - recoveryPenalty;

        // Status penalties
        if (fighter.isRocked) initiative -= 3;
        if (fighter.IsFatigued()) initiative -= 2;
        if (fighter.isGuarding) initiative -= 1;

        return Mathf.Max(1, initiative);
    }

    // Calculate if attack hits
    public static bool CalculateHitSuccess(Fighter attacker, Fighter defender, Move move, int actualDistance)
    {
        if (move.isMovement || move.category == MoveCategory.StanceSwitch) return true;

        // Check range
        if (actualDistance < move.minDistance || actualDistance > move.maxDistance)
        {
            return false;
        }

        float hitChance = 60f;

        // Attacker bonuses
        hitChance += attacker.GetEffectiveStat(attacker.Accuracy, "accuracy") * 5f;

        // Distance penalty
        int distanceFromOptimal = Mathf.Abs(actualDistance - move.distance);
        hitChance -= distanceFromOptimal * 8f;

        // Defender evasion
        hitChance -= defender.GetEffectiveStat(defender.HeadMovement, "headMovement") * 4f;
        hitChance -= defender.GetEffectiveStat(defender.FootWork, "footwork") * 3f;

        // Guard bonus - scales with block value and guard zone
        if (defender.isGuarding)
        {
            float maxBlock = 10f + 10f * defender.Block;
            float blockEffectiveness = defender.blockValue / maxBlock;
            float guardBonus = Mathf.Lerp(10f, 25f, blockEffectiveness);

            // Guard zone effectiveness on evasion
            guardBonus *= GetGuardZoneHitModifier(defender.guardZone, move.targetZone);

            hitChance -= guardBonus;
        }

        // Status effects
        if (attacker.isRocked) hitChance -= 15f;
        if (defender.isRocked) hitChance += 10f;

        // Stamina
        if (attacker.IsFatigued()) hitChance -= 15f;
        if (defender.IsFatigued()) hitChance += 5f;

        hitChance = Mathf.Clamp(hitChance, 5f, 95f);

        float roll = Random.Range(0f, 100f);
        return roll < hitChance;
    }

    // Returns a multiplier for how effective the guard zone is vs the attack zone
    private static float GetGuardZoneHitModifier(GuardZone guardZone, TargetZone targetZone)
    {
        switch (guardZone)
        {
            case GuardZone.High:
                if (targetZone == TargetZone.Head) return 1.5f;      // High guard is great vs head
                return 0.3f;                                           // But weak vs body/legs
            case GuardZone.Low:
                if (targetZone == TargetZone.Body || targetZone == TargetZone.Legs) return 1.5f; // Low guard protects body/legs
                return 0.3f;                                           // But weak vs head
            default:
                return 1.0f; // No zone = standard
        }
    }

    // Returns a multiplier for how effective the guard zone is at blocking damage
    private static float GetGuardZoneDamageModifier(GuardZone guardZone, TargetZone targetZone)
    {
        switch (guardZone)
        {
            case GuardZone.High:
                if (targetZone == TargetZone.Head) return 1.4f;
                return 0.25f;
            case GuardZone.Low:
                if (targetZone == TargetZone.Body || targetZone == TargetZone.Legs) return 1.4f;
                return 0.25f;
            default:
                return 1.0f;
        }
    }

    // Calculate damage WITH block processing
    public static DamageResult CalculateDamageWithBlock(Fighter attacker, Fighter defender, Move move, int actualDistance, bool isHit)
    {
        DamageResult result = new DamageResult();

        if (!isHit || move.isMovement || move.category == MoveCategory.StanceSwitch)
        {
            return result;
        }

        // Calculate base damage
        float rawDamage = move.power;

        // Distance multiplier
        float distanceMultiplier = CalculateDistanceMultiplier(move, actualDistance);
        rawDamage *= distanceMultiplier;

        // Power modifier (with fatigue penalty)
        rawDamage *= (1f + attacker.GetPowerModifier(move.category));

        // WRONG STANCE PENALTY
        rawDamage *= attacker.GetWrongStancePenalty();

        if (attacker.isRocked) rawDamage *= 0.7f;

        // Stamina modifier (with fatigue)
        float maxStamina = 10f + 5f * attacker.Cardio;
        float staminaMod = Mathf.Lerp(0.6f, 1f, attacker.stamina / maxStamina);
        rawDamage *= staminaMod;

        if (move.targetZone == TargetZone.Head)
        {
            float cutMod = 1f - (defender.GetEffectiveStat(defender.CutResistance, "cutResistance") * 0.05f);
            rawDamage *= cutMod;
        }

        if (defender.isRocked) rawDamage *= 1.4f;

        result.rawDamage = rawDamage;

        if (defender.isGuarding)
        {
            // Pass attacker's punch power for guard degradation
            float attackerPunchPower = attacker.GetEffectiveStat(attacker.PunchPower, "punchPower");

            // Calculate zone-modified block
            float zoneModifier = GetGuardZoneDamageModifier(defender.guardZone, move.targetZone);

            defender.BlockDamage(rawDamage, attackerPunchPower, zoneModifier,
                out result.damageBlocked,
                out result.damageTaken,
                out result.staminaDrained);

            result.wasBlocked = true;
            result.guardZoneMatch = (zoneModifier > 1f);
        }
        else
        {
            result.damageTaken = rawDamage;
            result.damageBlocked = 0f;
            result.staminaDrained = 0;
            result.wasBlocked = false;
            result.guardZoneMatch = false;
        }

        return result;
    }

    // Distance damage multiplier
    private static float CalculateDistanceMultiplier(Move move, int actualDistance)
    {
        if (actualDistance == move.distance)
        {
            return 1.0f; // 100% damage at optimal range
        }

        int distanceFromOptimal = Mathf.Abs(actualDistance - move.distance);
        float multiplier = 1f - (distanceFromOptimal * 0.15f);

        return Mathf.Max(0.3f, multiplier); // Minimum 30%
    }

    // Calculate new distance after movement
    public static int CalculateNewDistance(int currentDistance, int movement, bool isClosing)
    {
        int newDistance = isClosing ? currentDistance - movement : currentDistance + movement;
        return Mathf.Clamp(newDistance, 0, 8);
    }
}

// Result struct for damage calculations
public struct DamageResult
{
    public float rawDamage;
    public float damageBlocked;
    public float damageTaken;
    public int staminaDrained;
    public bool wasBlocked;
    public bool guardZoneMatch; // True if the guard zone matched the attack zone
}
