using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingWithHenchmanState : CardGameState {

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering AttackingWithHenchmanState");
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void AddListeners() {
        base.AddListeners();
        HenchmanCard.HenchmanInPlaySelectedEvent += HandleHenchmanInPlaySelected;
        PlayerManager.PlayerSelectedEvent += HandlePlayerSelected;
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
        HenchmanCard.HenchmanInPlaySelectedEvent -= HandleHenchmanInPlaySelected;
        PlayerManager.PlayerSelectedEvent -= HandlePlayerSelected;
    }

    /*
     * This method is called whenever the player clicks a henchman in play. If that
     * henchman is controlled by the non-active player, the attacking henchman will
     * attack it, if it can (based on Ellusive), then move the RoTStateMachine back
     * to the MainPhaseState. If that henchman is controlled by the active player,
     * the MainPhaseState will also be re-entered.
     */
    private void HandleHenchmanInPlaySelected(HenchmanCard henchman) {
        //CanHenchmenFight() will make sure the henchmen are on opposing sides of the board
        BoardSpaceEnum attackingLocation = rsm.GetAttackingHenchman().GetLocation();
        BoardSpaceEnum targetLocation = henchman.GetLocation();
        if(rsm.GetBoard().CanHenchmenFight(attackingLocation, targetLocation)) {
            rsm.GetBoard().HaveHenchmenFight(attackingLocation, targetLocation);
        }
        rsm.ChangeState<MainPhaseState>();
    }

    /*
     * This method is called whenever a player clicks either player's portrait. If
     * that player is the non-active player, the attacking henchman will attack it,
     * then the RoTStateMachine will move back to the MainPhaseState. If the clicked
     * player portrait is the active player's, the MainPhaseState will also be
     * re-entered.
     */
    private void HandlePlayerSelected(PlayerManager player) {
        BoardSpaceEnum attackingLocation = rsm.GetAttackingHenchman().GetLocation();
        if(rsm.GetBoard().CanHenchmanAttackTargetPlayer(attackingLocation, player)) {
            rsm.GetBoard().HaveHenchmanAttackTargetPlayer(attackingLocation, player);
        }
        rsm.ChangeState<MainPhaseState>();
    }
}
