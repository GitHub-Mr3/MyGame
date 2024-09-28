using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    EnemyLocomotionMgr _enemyLocomotionMgr;
    public bool isPreformingAction;//敌人是否进行攻击行为
    [Header("A.I Setting")]
    public float detectionRadius = 20;//半径
    //最小检查角度
    public float minimumDetectionAngle = -50;
    //最大检查角度
    public float maximumDetectionAngle = 50;
    private void Awake()
    {
        _enemyLocomotionMgr = GetComponent<EnemyLocomotionMgr>();
        enemyAnimatorMgr = GetComponentInChildren<EnemyAnimatorMgr>();
    }
    private void Update()
    {
        HandleRecoveryTime();
    }
    private void FixedUpdate()
    {
        HandleCurrentAction();
    }
    /// <summary>
    /// 控制敌人行为
    /// </summary>
    void HandleCurrentAction()
    {
        if (_enemyLocomotionMgr.curTarget != null)
        {
            _enemyLocomotionMgr.distanceFromTarget = Vector3.Distance(_enemyLocomotionMgr.curTarget.transform.position, transform.position);

        }

        if (_enemyLocomotionMgr.curTarget == null)
        {
            _enemyLocomotionMgr.HandleDetection();
        }
        else if (_enemyLocomotionMgr.distanceFromTarget > _enemyLocomotionMgr.stoppingDistance)
        {
            _enemyLocomotionMgr.HandleMoveToTarget();
        }
        else if (_enemyLocomotionMgr.distanceFromTarget <= _enemyLocomotionMgr.stoppingDistance)
        {
            AttackTarget();
        }
    }
    public EnemyAttakAction[] enemyAttaks;
    public EnemyAttakAction currentAttack;
    EnemyAnimatorMgr enemyAnimatorMgr;

    public float currentRecoverTime = 0;
    /// <summary>
    /// 攻击冷却时间
    /// </summary>
    private void HandleRecoveryTime()
    {
        if (currentRecoverTime >= 0)
        {
            currentRecoverTime -= Time.deltaTime;
        }
        if (isPreformingAction)
        {
            if (currentRecoverTime <= 0)
            {
                isPreformingAction = false;
            }
        }
    }
    /// <summary>
    /// 攻击
    /// </summary>
    private void AttackTarget()
    {
        if (isPreformingAction)
        {
            return;
        }
        if (currentAttack == null)
        {
            GetNewAttack();
        }
        else
        {
            isPreformingAction = true;
            currentRecoverTime = currentAttack.recoveryTime;
            enemyAnimatorMgr.PlayerTargetAnimation(currentAttack.actionAniName, true);
            currentAttack = null;
        }
    }
    /// <summary>
    /// 随机一个攻击动作
    /// </summary>
    private void GetNewAttack()
    {
        Vector3 targetDic = _enemyLocomotionMgr.curTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDic, transform.forward);

        _enemyLocomotionMgr.distanceFromTarget = Vector3.Distance(_enemyLocomotionMgr.curTarget.transform.position, transform.position);

        int maxScore = 0;
        for (int i = 0; i < enemyAttaks.Length; i++)
        {
            EnemyAttakAction enemyAttakAction = enemyAttaks[i];
            if (_enemyLocomotionMgr.distanceFromTarget <= enemyAttakAction.maxmumDistanceNeededToAttack && _enemyLocomotionMgr.distanceFromTarget >= enemyAttakAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttakAction.maximumAttackAngle && viewableAngle >= enemyAttakAction.minimumAttackAngle)
                {
                    maxScore += enemyAttakAction.attackSore;
                }

            }
        }
        int randomValue = Random.Range(0, maxScore);
        int temporaryScore = 0;
        for (int i = 0; i < enemyAttaks.Length; i++)
        {
            EnemyAttakAction enemyAttakAction = enemyAttaks[i];
            if (_enemyLocomotionMgr.distanceFromTarget <= enemyAttakAction.maxmumDistanceNeededToAttack && _enemyLocomotionMgr.distanceFromTarget >= enemyAttakAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttakAction.maximumAttackAngle && viewableAngle >= enemyAttakAction.minimumAttackAngle)
                {
                    if (currentAttack != null)
                    {
                        return;

                    }
                    temporaryScore += enemyAttakAction.attackSore;
                    if (temporaryScore > randomValue)
                    {
                        currentAttack = enemyAttakAction;
                    }
                }

            }
        }
    }
}
