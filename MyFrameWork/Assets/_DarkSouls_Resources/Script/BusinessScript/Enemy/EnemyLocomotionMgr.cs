using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 控制敌人的行为
/// </summary>
public class EnemyLocomotionMgr : MonoBehaviour
{
    EnemyManager enemyManager;
    public LayerMask detetionLayer;

    EnemyAnimatorMgr enemyAnimatorMgr;

    public float distanceFromTarget;
    /// <summary>
    /// 停止 的范围
    /// </summary>
    public float stoppingDistance = 1f;

    NavMeshAgent navMeshAgent;

    public float rotationSpeed = 15;

    public Rigidbody enemyRigidBody;
    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimatorMgr = GetComponentInChildren<EnemyAnimatorMgr>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();

        navMeshAgent.enabled = false;
        enemyRigidBody.isKinematic = false;
    }
    public CharacteStats curTarget;
    /// <summary>
    /// 检查是否碰到玩家
    /// </summary>
    public void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detetionLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacteStats characteStats = colliders[i].transform.GetComponent<CharacteStats>();
            if (characteStats != null)
            {
                Vector3 targetDir = characteStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDir, transform.forward);//角度判断
                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    curTarget = characteStats;
                }
            }
        }
    }


    /// <summary>
    /// 敌人跟随玩家
    /// </summary>
    public void HandleMoveToTarget()
    {
        if (enemyManager.isPreformingAction)
        {
            return;
        }
        //获得一个朝向
        Vector3 targetDic = curTarget.transform.position - transform.position;

        distanceFromTarget = Vector3.Distance(curTarget.transform.position, transform.position);

        //旋转的角度
        float viewableAngele = Vector3.Angle(targetDic, transform.forward);

        if (enemyManager.isPreformingAction)
        {
            enemyAnimatorMgr.anim.SetFloat(DarkSoulsConst.VERTICAL, 0, 0.1f, Time.deltaTime);
            navMeshAgent.enabled = false;
        }
        else
        {
            if (distanceFromTarget > stoppingDistance)
            {//移动
                enemyAnimatorMgr.anim.SetFloat(DarkSoulsConst.VERTICAL, 1, 0.1f, Time.deltaTime);
            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                enemyAnimatorMgr.anim.SetFloat(DarkSoulsConst.VERTICAL, 0, 0.1f, Time.deltaTime);
            }
        }
        HandleRotateTowardsTarget();
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleRotateTowardsTarget()
    {
        if (enemyManager.isPreformingAction)
        {
            Vector3 dir = curTarget.transform.position = transform.position;
            dir.y = 0;
            dir.Normalize();

            if (dir == Vector3.zero)
            {
                dir = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
        }
        else
        {
            //Vector3 relativeDic = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyRigidBody.velocity;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(curTarget.transform.position);
            enemyRigidBody.velocity = targetVelocity;
            transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }

    }
}
