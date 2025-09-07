using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private Transform _playerPosition;

    private float RollAttackSpeed = 50f;
    
    private float _timer;
    private float _timeBetweenAttack = 2f;

    private float _exitTimer;
    private float __timeTillExit = 3f;
    private float _distanceToCountExit = 3f;


    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        _playerPosition = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(_timer > _timeBetweenAttack)
        {
            _timer = 0f;

            Vector2 direction = (_playerPosition.position - enemy.transform.position).normalized;

            enemy.MoveEnemy(direction * RollAttackSpeed);
        }

        if(Vector2.Distance(_playerPosition.position, enemy.transform.position) > _distanceToCountExit)
        {
            _exitTimer += Time.deltaTime;

            if(_exitTimer > __timeTillExit)
            {
                enemy.stateMachine.ChangeState(enemy.chaseState);
            }
        }

        else
        {
            _exitTimer = 0f; 
        }

        _timer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
