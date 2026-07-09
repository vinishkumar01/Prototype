using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine, PlayerDataSO playerDataSO) : base(player, playerStateMachine, playerDataSO)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        //_player.AnimationTriggerEvent(Player.AnimationTriggerType.Idle);
    }

    public override void EnterState()
    {
        base.EnterState();
        //Applying linear Drag because as we switch state from move state to idle state because of the smoothDamp we are applying to the player to move makes the player keep on sliding so that we are applying this drag
        _player.RB.drag = 5f;
        _player._animator.SetBool("isIdle", true);

    }

    public override void ExitState()
    {
        base.ExitState();
        _player.RB.drag = 0f;
        _player._animator.SetBool("isIdle", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //If the player is Attached to the Rope 
        if (_player._attachedToRope && _player.JumpPressed)
        {
            _player.Detach();
            //also we are transitioning to the jump state
            _playerStateMachine.ChangeState(_player._playerJumpState);
        }

        StateTransitions();
    }

    public override void LateFrameUpdate()
    {
        base.LateFrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void StateTransitions()
    {
        //Transit to Move
        if (Mathf.Abs(_player.MovementInputXDirection) > 0.01f)
        {
            _playerStateMachine.ChangeState(_player._playerMoveState);
        }

        //Transit to Jump
        if (_player.JumpPressed && _player._isGrounded)
        {
            _playerStateMachine.ChangeState(_player._playerJumpState);
        }

        //Transist to Dash
        if (_player.DashPressed && _player._playerDataSO.dashCount > 0)
        {
            _playerStateMachine.ChangeState(_player._playerDashState);
        }
    }
}

