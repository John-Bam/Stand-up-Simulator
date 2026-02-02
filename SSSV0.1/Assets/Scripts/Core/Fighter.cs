using UnityEngine;
using System;
using System.Collections.Generic;

public enum Stance
{
    Orthodox,    // Left foot forward (right-handed)
    Southpaw     // Right foot forward (left-handed)
}

[Serializable]
public class Fighter
{
    public string fighterName;

    // BASE STATS (2.0-5.0 STARS with 0.5 increments) - CamelCase naming
    [Header("Base Stats (2.0-5.0 Stars, 0.5 increments)")]
    [Range(2f, 5f)] public float HeadHealth = 2f;
    [Range(2f, 5f)] public float BodyHealth = 2f;
    [Range(2f, 5f)] public float LegHealth = 2f;
    [Range(2f, 5f)] public float Cardio = 2f;
    [Range(2f, 5f)] public float Block = 2f;
    [Range(2f, 5f)] public float Recovery = 2f;
    [Range(2f, 5f)] public float CutResistance = 2f;

    // Striking Stats (2.0-5.0 stars)
    [Header("Striking Stats (2.0-5.0 Stars, 0.5 increments)")]
    [Range(2f, 5f)] public float PunchSpeed = 2f;
    [Range(2f, 5f)] public float PunchPower = 2f;
    [Range(2f, 5f)] public float KickSpeed = 2f;
    [Range(2f, 5f)] public float KickPower = 2f;
    [Range(2f, 5f)] public float FootWork = 2f;
    [Range(2f, 5f)] public float HeadMovement = 2f;
    [Range(2f, 5f)] public float Accuracy = 2f;

    // Grappling Stats (2.0-5.0 stars)
    [Header("Grappling Stats (2.0-5.0 Stars, 0.5 increments)")]
    [Range(2f, 5f)] public float Clinch = 2f;
    [Range(2f, 5f)] public float GrappleDefense = 2f;
    [Range(2f, 5f)] public float GrappleOffense = 2f;

    // Stance stat
    [Header("Stance (2.0-5.0 Stars, 0.5 increments)")]
    [Range(2f, 5f)] public float StanceSwitch = 2f;  // FIXED: renamed from stanceStat

    // STANCE SYSTEM
    public Stance naturalStance = Stance.Orthodox;
    public Stance currentStance = Stance.Orthodox;

    // CALCULATED POOLS (lowercase for runtime values)
    [Header("Current Pools")]
    public float headHealth;
    public float bodyHealth;
    public float legHealth;
    public float stamina;
    public float blockValue;

    private float maxHeadHealth;
    private float maxBodyHealth;
    private float maxLegHealth;
    private float maxStamina;
    public float maxBlockValue;

    // Combat state
    public bool isRocked = false;
    public bool isStunned = false;
    public bool isGuarding = false;
    public int recoveryPenalty = 0;
    public int currentDistance = 5;

    // THREE ACTIONS PER TURN
    [NonSerialized] public List<Move> selectedActions = new List<Move>(3);
    [NonSerialized] public int actionsRemaining = 3;

    // Round statistics
    [Header("Round Statistics")]
    public int strikesLanded = 0;
    public int strikesAttempted = 0;
    public float totalDamageLanded = 0f;
    public float totalDamageTaken = 0f;
    public int knockdowns = 0;
    public int timesGuarded = 0;
    public float totalDamageBlocked = 0f;
    public int aggression = 0;
    public int stanceSwitches = 0;

    [NonSerialized] public int initiative;

    public Fighter(string name, Stance preferredStance = Stance.Orthodox)
    {
        fighterName = name;
        naturalStance = preferredStance;
        currentStance = preferredStance;
        InitializePools();
    }

    public void InitializePools()
    {
        // head health = 10 + 5 × star
        maxHeadHealth = 10f + (5f * HeadHealth);
        headHealth = maxHeadHealth;

        // body health = 15 + 10 × star
        maxBodyHealth = 15f + (10f * BodyHealth);
        bodyHealth = maxBodyHealth;

        // leg health = 15 + 5 × star
        maxLegHealth = 15f + (5f * LegHealth);
        legHealth = maxLegHealth;

        // stamina = 10 + 5 × star (Cardio stat)
        maxStamina = 10f + (5f * Cardio);
        stamina = maxStamina;

        // block = 10 + 10 × star
        maxBlockValue = 10f + (10f * Block);
        blockValue = maxBlockValue;
    }

    // Check if below 50% stamina
    public bool IsFatigued()
    {
        return stamina < (maxStamina * 0.5f);
    }

    // Get effective stat with fatigue penalty
    public float GetEffectiveStat(float baseStat, string statName)
    {
        // Health, cardio, and recovery are NOT penalized
        if (statName == "headHealth" || statName == "bodyHealth" ||
            statName == "legHealth" || statName == "cardio" || statName == "recovery")
        {
            return baseStat;
        }

        // If fatigued, reduce stat by 0.5 (minimum 1.0)
        if (IsFatigued())
        {
            return Mathf.Max(1f, baseStat - 0.5f);
        }

        return baseStat;
    }

    // Check if in wrong stance
    public bool IsInWrongStance()
    {
        return currentStance != naturalStance;
    }

    // Calculate wrong stance damage penalty
    public float GetWrongStancePenalty()
    {
        if (!IsInWrongStance()) return 1.0f;

        // Penalty = 40% - (8% × stance stat)
        float penaltyPercent = 40f - (8f * StanceSwitch);
        penaltyPercent = Mathf.Max(0f, penaltyPercent);

        return 1f - (penaltyPercent / 100f);
    }

    // Switch stance
    public void SwitchStance()
    {
        currentStance = (currentStance == Stance.Orthodox) ? Stance.Southpaw : Stance.Orthodox;
        stanceSwitches++;
    }

    // Take damage with body stamina drain
    public void TakeDamage(string target, float damage)
    {
        totalDamageTaken += damage;

        switch (target.ToLower())
        {
            case "head":
                headHealth -= damage;
                if (headHealth <= 0) headHealth = 0;
                CheckRocked();
                break;
            case "body":
                bodyHealth -= damage;
                if (bodyHealth <= 0) bodyHealth = 0;

                // BODY HITS DRAIN STAMINA
                float staminaDrain = damage * 0.5f;

                // If below 50% stamina, drain more
                if (IsFatigued())
                {
                    staminaDrain *= 1.5f;
                }

                UseStamina(staminaDrain);
                break;
            case "legs":
                legHealth -= damage;
                if (legHealth <= 0) legHealth = 0;
                break;
        }
    }

    // Block damage with punch power affecting guard degradation
    public void BlockDamage(float incomingDamage, float attackerPunchPower, out float damageBlocked, out float damageTaken, out int staminaDrained)
    {
        if (!isGuarding || blockValue <= 0)
        {
            damageBlocked = 0f;
            damageTaken = incomingDamage;
            staminaDrained = 0;
            return;
        }

        float blockEffectiveness = blockValue / maxBlockValue;
        float blockPercentage = Mathf.Lerp(0.4f, 0.8f, blockEffectiveness);

        damageBlocked = incomingDamage * blockPercentage;
        damageTaken = incomingDamage - damageBlocked;

        staminaDrained = Mathf.CeilToInt(damageBlocked * 0.5f);
        UseStamina(staminaDrained);

        // PUNCH POWER AFFECTS GUARD DEGRADATION
        float baseDegradation = damageBlocked * 0.3f;
        float powerBonus = attackerPunchPower * 2f;
        float totalDegradation = baseDegradation + powerBonus;

        blockValue -= totalDegradation;
        if (blockValue < 0) blockValue = 0;

        totalDamageBlocked += damageBlocked;
        totalDamageTaken += damageTaken;

        if (stamina <= 0)
        {
            isGuarding = false;
            blockValue = Mathf.Max(0, blockValue - 20f);
        }
    }

    public void RecoverBlockValue()
    {
        if (!isGuarding)
        {
            float recoveryAmount = GetEffectiveStat(Recovery, "recovery") * 8f;
            blockValue += recoveryAmount;
            if (blockValue > maxBlockValue)
                blockValue = maxBlockValue;
        }
    }

    public void ResetGuard()
    {
        isGuarding = true;
        timesGuarded++;
        blockValue += 10f;
        if (blockValue > maxBlockValue)
            blockValue = maxBlockValue;
    }

    private void CheckRocked()
    {
        float rockedThreshold = maxHeadHealth * 0.3f;

        if (headHealth < rockedThreshold && !isRocked)
        {
            isRocked = true;
        }
        else if (headHealth >= rockedThreshold && isRocked)
        {
            isRocked = false;
        }
    }

    public bool IsKnockedOut()
    {
        return headHealth <= 0 || bodyHealth <= 0;
    }

    public void UseStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0) stamina = 0;
    }

    public void RecoverStamina(float amount)
    {
        stamina += amount;
        if (stamina > maxStamina) stamina = maxStamina;
    }

    public void EndOfRoundRecovery()
    {
        RecoverStamina(Recovery);
    }

    public void StartOfFightRoundRecovery()
    {
        RecoverStamina(Recovery * 3);
        RecoverBlockValue();
    }

    public void ResetRoundStats()
    {
        strikesLanded = 0;
        strikesAttempted = 0;
        totalDamageLanded = 0f;
        totalDamageTaken = 0f;
        knockdowns = 0;
        timesGuarded = 0;
        totalDamageBlocked = 0f;
        aggression = 0;
        stanceSwitches = 0;
    }

    public float GetStatModifier(MoveCategory category)
    {
        float baseStat = 0f;
        string statName = "";

        switch (category)
        {
            case MoveCategory.Punch:
                baseStat = PunchSpeed;
                statName = "punchSpeed";
                break;
            case MoveCategory.Kick:
                baseStat = KickSpeed;
                statName = "kickSpeed";
                break;
            case MoveCategory.Knee:
            case MoveCategory.Elbow:
                baseStat = (PunchSpeed + KickSpeed) / 2f;
                statName = "mixed";
                break;
            case MoveCategory.Movement:
                baseStat = FootWork;
                statName = "footwork";
                break;
            default:
                return 0f;
        }

        return GetEffectiveStat(baseStat, statName);
    }

    public float GetPowerModifier(MoveCategory category)
    {
        float basePower = 0f;
        string statName = "";

        switch (category)
        {
            case MoveCategory.Punch:
            case MoveCategory.Elbow:
                basePower = PunchPower;
                statName = "punchPower";
                break;
            case MoveCategory.Kick:
            case MoveCategory.Knee:
                basePower = KickPower;
                statName = "kickPower";
                break;
            default:
                return 0f;
        }

        return GetEffectiveStat(basePower, statName) * 0.2f;
    }

    public string GetHealthStatus()
    {
        string status = $"{fighterName} [{currentStance}]:\n" +
               $"Head:{headHealth:F0}/{maxHeadHealth:F0} Body:{bodyHealth:F0}/{maxBodyHealth:F0} Legs:{legHealth:F0}/{maxLegHealth:F0}\n" +
               $"Stamina:{stamina:F0}/{maxStamina:F0}";

        if (IsFatigued())
        {
            status += " [FATIGUED]";
        }

        if (IsInWrongStance())
        {
            status += " [WRONG STANCE]";
        }

        if (isGuarding || blockValue < maxBlockValue)
        {
            status += $"\nBlock: {blockValue:F0}/{maxBlockValue:F0}";
        }

        if (isRocked) status += " [ROCKED]";
        if (isGuarding) status += " [GUARDING]";

        return status;
    }

    public string GetGuardStatus()
    {
        if (blockValue >= 80f)
            return "STRONG";
        else if (blockValue >= 50f)
            return "SOLID";
        else if (blockValue >= 20f)
            return "WEAKENED";
        else
            return "BROKEN";
    }

    public float GetStaminaPercentage()
    {
        return (stamina / maxStamina) * 100f;
    }

    public float GetHeadHealthPercentage()
    {
        return (headHealth / maxHeadHealth) * 100f;
    }

    public float GetBodyHealthPercentage()
    {
        return (bodyHealth / maxBodyHealth) * 100f;
    }

    public float GetLegHealthPercentage()
    {
        return (legHealth / maxLegHealth) * 100f;
    }

    public void ResetActions()
    {
        selectedActions.Clear();
        actionsRemaining = 3;
    }

    public bool CanSelectAction()
    {
        return actionsRemaining > 0;
    }

    public void SelectAction(Move action)
    {
        if (CanSelectAction())
        {
            selectedActions.Add(action);
            actionsRemaining--;
        }
    }
}