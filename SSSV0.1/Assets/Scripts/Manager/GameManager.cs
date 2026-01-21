using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Fighters")]
    public Fighter playerFighter;
    public Fighter aiFighter;

    [Header("UI References")]
    public TextMeshProUGUI combatLogText;
    public TextMeshProUGUI playerStatusText;
    public TextMeshProUGUI aiStatusText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI roundInfoText;
    public TextMeshProUGUI scorecardText;
    public TextMeshProUGUI actionCountText;
    public GameObject moveSelectionPanel;
    public Transform moveButtonContainer;
    public Button moveButtonPrefab;
    public Button confirmActionsButton;
    public Button restartButton;
    public ScrollRect combatLogScrollRect;

    [Header("Round System")]
    private int currentTurn = 0;
    private int currentRound = 1;
    private int currentFightRound = 1;

    private List<RoundScore> roundScores = new List<RoundScore>();

    private string combatLog = "";
    private int currentDistance = 5;
    private bool playerHasSelected = false;
    private bool aiHasSelected = false;

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
    }

    void Start()
    {
        InitializeFighters();
        UpdateUI();
        UpdateRoundDisplay();
        ShowMoveSelection();

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartFight);
        }

        if (confirmActionsButton != null)
        {
            confirmActionsButton.onClick.AddListener(OnConfirmActions);
            confirmActionsButton.gameObject.SetActive(false);
        }
    }

    void InitializeFighters()
    {
        // PLAYER - Orthodox stance - Using half-star values
        playerFighter = new Fighter("Player", Stance.Orthodox);
        playerFighter.HeadHealth = 3.5f;
        playerFighter.BodyHealth = 4f;
        playerFighter.LegHealth = 3f;
        playerFighter.Cardio = 4f;
        playerFighter.Block = 3f;
        playerFighter.Recovery = 3.5f;
        playerFighter.StanceSwitch = 3f;
        playerFighter.PunchPower = 4.5f;
        playerFighter.PunchSpeed = 4f;
        playerFighter.Accuracy = 4f;
        playerFighter.HeadMovement = 3f;
        playerFighter.FootWork = 3.5f;
        playerFighter.KickPower = 3f;
        playerFighter.KickSpeed = 3f;
        playerFighter.Clinch = 3f;
        playerFighter.GrappleDefense = 3f;
        playerFighter.GrappleOffense = 3f;
        playerFighter.CutResistance = 3f;
        playerFighter.InitializePools();

        // AI - Southpaw stance - Using half-star values
        aiFighter = new Fighter("AI Opponent", Stance.Southpaw);
        aiFighter.HeadHealth = 3f;
        aiFighter.BodyHealth = 3.5f;
        aiFighter.LegHealth = 3.5f;
        aiFighter.Cardio = 3f;
        aiFighter.Block = 4f;
        aiFighter.Recovery = 4f;
        aiFighter.StanceSwitch = 2.5f;
        aiFighter.PunchPower = 3f;
        aiFighter.PunchSpeed = 4.5f;
        aiFighter.Accuracy = 4.5f;
        aiFighter.HeadMovement = 4f;
        aiFighter.FootWork = 3.5f;
        aiFighter.KickPower = 4f;
        aiFighter.KickSpeed = 3f;
        aiFighter.Clinch = 3f;
        aiFighter.GrappleDefense = 3.5f;
        aiFighter.GrappleOffense = 2.5f;
        aiFighter.CutResistance = 3f;
        aiFighter.InitializePools();

        AddToCombatLog("=== FIGHT START ===");
        AddToCombatLog($"{playerFighter.fighterName} [{playerFighter.currentStance}] vs {aiFighter.fighterName} [{aiFighter.currentStance}]");
        AddToCombatLog($"3 Fight Rounds × 5 Rounds Each");
        AddToCombatLog($"3 Actions Per Turn\n");
    }

    void ShowMoveSelection()
    {
        playerFighter.ResetActions();
        UpdateActionCount();

        foreach (Transform child in moveButtonContainer)
        {
            Destroy(child.gameObject);
        }

        List<Move> availableMoves = MoveLibrary.Instance.GetAvailableMoves(playerFighter, currentDistance);

        foreach (Move move in availableMoves)
        {
            Button btn = Instantiate(moveButtonPrefab, moveButtonContainer);

            string buttonText = $"<b>{move.moveName}</b>";

            if (move.strikeHand == StrikeHand.Lead)
            {
                buttonText += " 🔵";
            }
            else if (move.strikeHand == StrikeHand.Rear)
            {
                buttonText += " 🔴";
            }

            buttonText += "\n";

            if (!move.isMovement && move.category != MoveCategory.Guard && move.category != MoveCategory.StanceSwitch)
            {
                bool isLead = (move.strikeHand == StrikeHand.Lead);
                int effectiveSpeed = move.GetEffectiveSpeed(isLead);
                int effectiveRecovery = move.GetEffectiveRecovery(isLead);

                buttonText += $"Spd:{effectiveSpeed} Pow:{move.power:F0} Rec:{effectiveRecovery} En:{move.energy}\n";
                buttonText += $"Range:{move.minDistance}-{move.maxDistance} (Opt:{move.distance})";
            }
            else if (move.isMovement)
            {
                buttonText += $"Spd:{move.speed} En:{move.energy} Move:{move.movementAmount} sq";
            }
            else if (move.category == MoveCategory.StanceSwitch)
            {
                buttonText += $"Spd:{move.speed} En:{move.energy} - Switch stance";
            }
            else
            {
                buttonText += $"Spd:{move.speed} - Recover stamina & block";
            }

            btn.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;

            Move selectedMove = move;
            btn.onClick.AddListener(() => OnMoveSelected(selectedMove));
        }

        moveSelectionPanel.SetActive(true);
    }

    public void OnMoveSelected(Move move)
    {
        if (!playerFighter.CanSelectAction())
        {
            AddToCombatLog("You've already selected 3 actions!");
            return;
        }

        playerFighter.SelectAction(move);
        AddToCombatLog($"Action {3 - playerFighter.actionsRemaining}: {move.moveName}");

        UpdateActionCount();

        if (playerFighter.actionsRemaining == 0)
        {
            if (confirmActionsButton != null)
            {
                confirmActionsButton.gameObject.SetActive(true);
            }
        }
    }
    public void OnConfirmActions()
    {
        if (playerFighter.selectedActions.Count != 3)
        {
            AddToCombatLog("You must select exactly 3 actions!");
            return;
        }

        playerHasSelected = true;
        moveSelectionPanel.SetActive(false);
        if (confirmActionsButton != null)
            confirmActionsButton.gameObject.SetActive(false);

        AddToCombatLog($"\n{playerFighter.fighterName} has locked in their actions!");

        AISelectActions();

        if (playerHasSelected && aiHasSelected)
        {
            Invoke("ResolveTurn", 1f);
        }
    }

    void AISelectActions()
    {
        aiFighter.ResetActions();

        for (int i = 0; i < 3; i++)
        {
            List<Move> availableMoves = MoveLibrary.Instance.GetAvailableMoves(aiFighter, currentDistance);

            Move chosenMove = null;

            if (aiFighter.stamina < 15 && i == 0)
            {
                chosenMove = availableMoves.Find(m => m.moveName == "Guard");
            }
            else if (currentDistance <= 2)
            {
                List<Move> punches = availableMoves.FindAll(m => m.category == MoveCategory.Punch);
                if (punches.Count > 0)
                    chosenMove = punches[Random.Range(0, punches.Count)];
            }
            else if (currentDistance >= 3 && currentDistance <= 4)
            {
                List<Move> kicks = availableMoves.FindAll(m => m.category == MoveCategory.Kick);
                if (kicks.Count > 0)
                    chosenMove = kicks[Random.Range(0, kicks.Count)];
            }
            else
            {
                chosenMove = availableMoves.Find(m => m.moveName == "Step In");
            }

            if (chosenMove == null && availableMoves.Count > 0)
            {
                chosenMove = availableMoves[Random.Range(0, availableMoves.Count)];
            }

            if (chosenMove != null)
            {
                aiFighter.SelectAction(chosenMove);
            }
        }

        aiHasSelected = true;
        AddToCombatLog($"{aiFighter.fighterName} has selected their actions!");
    }
    void ResolveTurn()
    {
        currentTurn++;
        AddToCombatLog($"\n=== TURN {currentTurn} RESOLUTION ===\n");

        for (int actionNum = 0; actionNum < 3; actionNum++)
        {
            AddToCombatLog($"--- Action {actionNum + 1} ---");

            if (actionNum < playerFighter.selectedActions.Count && actionNum < aiFighter.selectedActions.Count)
            {
                Move playerMove = playerFighter.selectedActions[actionNum];
                Move aiMove = aiFighter.selectedActions[actionNum];

                playerFighter.initiative = CombatCalculator.CalculateInitiative(playerFighter, playerMove);
                aiFighter.initiative = CombatCalculator.CalculateInitiative(aiFighter, aiMove);

                Fighter firstFighter, secondFighter;
                Move firstMove, secondMove;

                if (playerFighter.initiative >= aiFighter.initiative)
                {
                    firstFighter = playerFighter;
                    secondFighter = aiFighter;
                    firstMove = playerMove;
                    secondMove = aiMove;
                }
                else
                {
                    firstFighter = aiFighter;
                    secondFighter = playerFighter;
                    firstMove = aiMove;
                    secondMove = playerMove;
                }

                AddToCombatLog($"{firstFighter.fighterName} (Init:{firstFighter.initiative}) acts first");
                ExecuteAction(firstFighter, secondFighter, firstMove);
                UpdateUI();

                if (CheckFightEnd()) return;

                AddToCombatLog($"{secondFighter.fighterName} (Init:{secondFighter.initiative}) acts second");
                ExecuteAction(secondFighter, firstFighter, secondMove);
                UpdateUI();

                if (CheckFightEnd()) return;

                AddToCombatLog("");
            }
        }

        playerFighter.RecoverBlockValue();
        aiFighter.RecoverBlockValue();

        playerFighter.isGuarding = false;
        aiFighter.isGuarding = false;

        playerFighter.EndOfRoundRecovery();
        aiFighter.EndOfRoundRecovery();

        playerHasSelected = false;
        aiHasSelected = false;

        if (currentTurn % 5 == 0)
        {
            EndRound();
        }
        else
        {
            Invoke("ShowMoveSelection", 2f);
        }
    }
    void ExecuteAction(Fighter attacker, Fighter defender, Move move)
    {
        if (attacker.stamina < move.energy)
        {
            AddToCombatLog($"  {attacker.fighterName} is too exhausted for {move.moveName}!");
            return;
        }

        attacker.UseStamina(move.energy);

        bool isLead = (move.strikeHand == StrikeHand.Lead);
        attacker.recoveryPenalty = move.GetEffectiveRecovery(isLead);

        if (move.category == MoveCategory.StanceSwitch)
        {
            attacker.SwitchStance();
            AddToCombatLog($"  {attacker.fighterName} switches to {attacker.currentStance} stance!");
            return;
        }

        if (move.isMovement)
        {
            bool isClosing = (move.moveName.Contains("In") || move.moveName.Contains("Forward"));
            int oldDistance = currentDistance;
            currentDistance = CombatCalculator.CalculateNewDistance(currentDistance, move.movementAmount, isClosing);

            string direction = isClosing ? "closer" : "back";
            AddToCombatLog($"  {attacker.fighterName} moves {direction}! Distance: {oldDistance} → {currentDistance}");

            if (isClosing) attacker.aggression += move.movementAmount;
            return;
        }

        if (move.category == MoveCategory.Guard)
        {
            attacker.ResetGuard();
            attacker.RecoverStamina(10);
            AddToCombatLog($"  {attacker.fighterName} guards! [Block: {attacker.blockValue:F0}]");
            return;
        }

        attacker.strikesAttempted++;

        bool hit = CombatCalculator.CalculateHitSuccess(attacker, defender, move, currentDistance);

        if (!hit)
        {
            if (currentDistance < move.minDistance)
                AddToCombatLog($"  {attacker.fighterName}'s {move.moveName} is TOO CLOSE!");
            else if (currentDistance > move.maxDistance)
                AddToCombatLog($"  {attacker.fighterName}'s {move.moveName} is OUT OF RANGE!");
            else
                AddToCombatLog($"  {attacker.fighterName}'s {move.moveName} MISSES!");
            return;
        }
        attacker.strikesLanded++;

        DamageResult dmg = CombatCalculator.CalculateDamageWithBlock(attacker, defender, move, currentDistance, true);

        attacker.totalDamageLanded += dmg.damageTaken;
        defender.TakeDamage(move.targetZone.ToString(), dmg.damageTaken);

        if (defender.IsKnockedOut()) attacker.knockdowns++;

        string leadIndicator = isLead ? "[LEAD]" : "[REAR]";
        string stanceWarning = attacker.IsInWrongStance() ? "[WRONG STANCE]" : "";

        if (dmg.wasBlocked)
        {
            float blockPct = (dmg.damageBlocked / dmg.rawDamage) * 100f;
            AddToCombatLog($"  {attacker.fighterName} lands {move.moveName} {leadIndicator}{stanceWarning} but BLOCKED!");
            AddToCombatLog($"    Blocked: {dmg.damageBlocked:F1} ({blockPct:F0}%) | Through: {dmg.damageTaken:F1} | Stamina drained: {dmg.staminaDrained}");
            AddToCombatLog($"    Guard: {defender.blockValue:F0}/{defender.maxBlockValue:F0}");
        }
        else
        {
            string hitType = dmg.damageTaken > 20f ? "CRUSHES" : "lands";
            AddToCombatLog($"  {attacker.fighterName} {hitType} {move.moveName} {leadIndicator}{stanceWarning}!");
            AddToCombatLog($"    Damage: {dmg.damageTaken:F1} to {move.targetZone}");

            if (move.targetZone == TargetZone.Body)
            {
                AddToCombatLog($"    Body shot drains stamina!");
            }

            if (defender.isRocked)
            {
                AddToCombatLog($"    {defender.fighterName} is ROCKED!");
            }
        }
    }

    void EndRound()
    {
        AddToCombatLog($"\n========== END OF ROUND {currentRound} ==========");

        RoundScore score = RoundScorer.ScoreRound(playerFighter, aiFighter);
        roundScores.Add(score);

        AddToCombatLog($"ROUND SCORE: {score.ToString()}");

        playerFighter.ResetRoundStats();
        aiFighter.ResetRoundStats();

        currentRound++;

        if (currentRound > 5 * currentFightRound)
        {
            EndFightRound();
        }
        else
        {
            UpdateRoundDisplay();
            UpdateScorecard();
            Invoke("ShowMoveSelection", 2f);
        }
    }

    void EndFightRound()
    {
        AddToCombatLog($"\n########## END OF FIGHT ROUND {currentFightRound} ##########");

        playerFighter.StartOfFightRoundRecovery();
        aiFighter.StartOfFightRoundRecovery();

        AddToCombatLog($"Both fighters recover stamina!");

        currentFightRound++;

        if (currentFightRound > 3)
        {
            EndMatch();
        }
        else
        {
            UpdateRoundDisplay();
            UpdateScorecard();
            Invoke("ShowMoveSelection", 3f);
        }
    }
    void EndMatch()
    {
        AddToCombatLog($"\n========== MATCH COMPLETE ==========");

        int playerTotal = 0;
        int aiTotal = 0;

        foreach (RoundScore rs in roundScores)
        {
            if (rs.fighter1Name == playerFighter.fighterName)
            {
                playerTotal += rs.fighter1Score;
                aiTotal += rs.fighter2Score;
            }
            else
            {
                playerTotal += rs.fighter2Score;
                aiTotal += rs.fighter1Score;
            }
        }

        AddToCombatLog($"\nFINAL SCORE:");
        AddToCombatLog($"{playerFighter.fighterName}: {playerTotal}");
        AddToCombatLog($"{aiFighter.fighterName}: {aiTotal}");

        if (playerTotal > aiTotal)
            AddToCombatLog($"\n🏆 {playerFighter.fighterName} WINS BY DECISION! 🏆");
        else if (aiTotal > playerTotal)
            AddToCombatLog($"\n🏆 {aiFighter.fighterName} WINS BY DECISION! 🏆");
        else
            AddToCombatLog($"\n🤝 DRAW! 🤝");

        moveSelectionPanel.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(true);

    }

    bool CheckFightEnd()
    {
        if (playerFighter.IsKnockedOut())
        {
            AddToCombatLog("\n🥊 KNOCKOUT! AI WINS! 🥊");
            moveSelectionPanel.SetActive(false);
            if (restartButton != null) restartButton.gameObject.SetActive(true);
            if (confirmActionsButton != null) confirmActionsButton.gameObject.SetActive(false);
            return true;
        }
        else if (aiFighter.IsKnockedOut())
        {
            AddToCombatLog("\n🥊 KNOCKOUT! PLAYER WINS! 🥊");
            moveSelectionPanel.SetActive(false);
            if (restartButton != null) restartButton.gameObject.SetActive(true);
            if (confirmActionsButton != null) confirmActionsButton.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    void UpdateActionCount()
    {
        if (actionCountText != null)
        {
            actionCountText.text = $"Actions Remaining: {playerFighter.actionsRemaining}/3";

            if (playerFighter.actionsRemaining == 0)
            {
                actionCountText.text += "\n<color=green>✓ Ready to confirm!</color>";
            }
        }
    }

    void UpdateRoundDisplay()
    {
        if (roundInfoText != null)
        {
            int roundInFightRound = ((currentRound - 1) % 5) + 1;
            roundInfoText.text = $"<b>FIGHT ROUND {currentFightRound}/3</b>\nRound {roundInFightRound}/5";
        }
    }

    void UpdateScorecard()
    {
        if (scorecardText != null)
        {
            string scorecard = "<b>SCORECARD:</b>\n";

            for (int i = 0; i < roundScores.Count; i++)
            {
                RoundScore rs = roundScores[i];
                scorecard += $"R{i + 1}: {rs.fighter1Score}-{rs.fighter2Score}\n";
            }

            scorecardText.text = scorecard;
        }
    }

    void RestartFight()
    {
        combatLog = "";
        if (combatLogText != null) combatLogText.text = "";
        currentTurn = 0;
        currentRound = 1;
        currentFightRound = 1;
        currentDistance = 5;
        playerHasSelected = false;
        aiHasSelected = false;
        roundScores.Clear();

        InitializeFighters();
        UpdateUI();
        UpdateRoundDisplay();
        UpdateScorecard();
        ShowMoveSelection();

        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (confirmActionsButton != null) confirmActionsButton.gameObject.SetActive(false);
    }

    void AddToCombatLog(string message)
    {
        combatLog += message + "\n";
        if (combatLogText != null)
        {
            combatLogText.text = combatLog;

            if (combatLogScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                combatLogScrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }
    void UpdateUI()
    {
        if (playerStatusText != null)
            playerStatusText.text = playerFighter.GetHealthStatus();

        if (aiStatusText != null)
            aiStatusText.text = aiFighter.GetHealthStatus();

        if (distanceText != null)
            distanceText.text = $"<b>Distance: {currentDistance}</b>";
    }

}
