using UnityEngine;

public enum GuardZone
{
    None,
    High,
    Low
}

public class CombatCalculator
{
    public static int CalculateInitiative(Fighter fighter, Move move)
    {
        int baseRoll = Random.Range(1, 5);

        bool isLead = (move.strikeHand == StrikeHand.Lead);
        int moveSpeed = move.GetEffectiveSpeed(isLead);

        int statBonus = Mathf.RoundToInt(fighter.GetStatModifier(move.category));
        int recoveryPenalty = fighter.recoveryPenalty;

        int initiative = baseRoll + moveSpeed + statBonus - recoveryPenalty;

        if (fighter.isRocked) initiative -= 3;
        if (fighter.IsFatigued()) initiative -= 2;
        if (fighter.isGuarding) initiative -= 1;

        return Mathf.Max(1, initiative);
    }

    public static bool CalculateHitSuccess(Fighter attacker, Fighter defender, Move move, int actualDistance)
    {
        if (move.isMovement || move.category == MoveCategory.StanceSwitch) return true;

        if (actualDistance < move.minDistance || actualDistance > move.maxDistance)
        {
            return false;
        }

        float hitChance = 60f;

        hitChance += attacker.GetEffectiveStat(attacker.Accuracy, "accuracy") * 5f;

        int distanceFromOptimal = Mathf.Abs(actualDistance - move.distance);
        hitChance -= distanceFromOptimal * 8f;

        hitChance -= defender.GetEffectiveStat(defender.HeadMovement, "headMovement") * 4f;
        hitChance -= defender.GetEffectiveStat(defender.FootWork, "footwork") * 3f;

        if (defender.isGuarding)
        {
            float maxBlock = 10f + 10f * defender.Block;
            float blockEffectiveness = defender.blockValue / maxBlock;
            float guardBonus = Mathf.Lerp(10f, 25f, blockEffectiveness);

            guardBonus *= GetGuardZoneHitModifier(defender.guardZone, move.targetZone);

            hitChance -= guardBonus;
        }

        if (attacker.isRocked) hitChance -= 15f;
        if (defender.isRocked) hitChance += 10f;

        if (attacker.IsFatigued()) hitChance -= 15f;
        if (defender.IsFatigued()) hitChance += 5f;

        hitChance = Mathf.Clamp(hitChance, 5f, 95f);

        float roll = Random.Range(0f, 100f);
        return roll < hitChance;
    }

    private static float GetGuardZoneHitModifier(GuardZone guardZone, TargetZone targetZone)
    {
        switch (guardZone)
        {
            case GuardZone.High:
                if (targetZone == TargetZone.Head) return 1.5f;
                return 0.3f;
            case GuardZone.Low:
                if (targetZone == TargetZone.Body || targetZone == TargetZone.Legs) return 1.5f;
                return 0.3f;
            default:
                return 1.0f;
        }
    }

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

    public static DamageResult CalculateDamageWithBlock(Fighter attacker, Fighter defender, Move move, int actualDistance, bool isHit)
    {
        DamageResult result = new DamageResult();

        if (!isHit || move.isMovement || move.category == MoveCategory.StanceSwitch)
        {
            return result;
        }

        float rawDamage = move.power;

        float distanceMultiplier = CalculateDistanceMultiplier(move, actualDistance);
        rawDamage *= distanceMultiplier;

        rawDamage *= (1f + attacker.GetPowerModifier(move.category));

        rawDamage *= attacker.GetWrongStancePenalty();

        if (attacker.isRocked) rawDamage *= 0.7f;

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
            float attackerPunchPower = attacker.GetEffectiveStat(attacker.PunchPower, "punchPower");

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

    private static float CalculateDistanceMultiplier(Move move, int actualDistance)
    {
        if (actualDistance == move.distance)
        {
            return 1.0f;
        }

        int distanceFromOptimal = Mathf.Abs(actualDistance - move.distance);
        float multiplier = 1f - (distanceFromOptimal * 0.15f);

        return Mathf.Max(0.3f, multiplier);
    }

    public static int CalculateNewDistance(int currentDistance, int movement, bool isClosing)
    {
        int newDistance = isClosing ? currentDistance - movement : currentDistance + movement;
        return Mathf.Clamp(newDistance, 0, 8);
    }
}

public struct DamageResult
{
    public float rawDamage;
    public float damageBlocked;
    public float damageTaken;
    public int staminaDrained;
    public bool wasBlocked;
    public bool guardZoneMatch;
}
