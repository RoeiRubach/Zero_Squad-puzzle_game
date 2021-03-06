﻿using UnityEngine;
using UnityEngine.AI;

public enum EnemyAnimationTransitionParameters
{
    _isMoving,
    _isPlayerBeenSeen,
    _isAbleToAttack,
    _isDead
}

public enum EnemyDestinations
{
    _startPosition,
    _firstDestination,
    _secondDestination
}

[RequireComponent(typeof(EnemyVisionRequirement), typeof(EnemyHealth), typeof(EnemyPoolController))]
public abstract class EnemyBase : MonoBehaviour
{
    [HideInInspector] public Transform TargetDetected;
    [HideInInspector] public bool IsPlayerSpotted;
    public bool IsAttacking { get; protected set; }

    [Header("Enemy's attributes:", order = 0)]
    [SerializeField] protected float _walkingSpeed;
    [SerializeField] protected float _runningSpeed;

    public MindlessShooterSFX MindlessShooterSFX { get; protected set; }
    protected float turningSpeed = 1.5f;
    protected EnemyHealth enemyHealth;
    protected GameObject enemyEyesRef;
    protected NavMeshAgent enemyMeshAgent;
    protected Animator enemyAnimator;
    protected bool isEnemyRoaming;
    protected float destroyTimer = 4f;
    protected Transform _elenaKillSummonerPlacement;

#if UNITY_EDITOR
    [ContextMenu("Kill enemy - PLAYMODE ONLY!")]
    public void EnemyKillingTest()
    {
        ResetAIPath();
        enemyAnimator.SetBool(EnemyAnimationTransitionParameters._isDead.ToString(), true);
    }
#endif

    protected virtual void Awake()
    {
        enemyMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        if (enemyAnimator == null)
            enemyAnimator = GetModelAnimator();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    protected virtual bool IsEnemyGotKilled() => enemyHealth.IsKilled;

    protected virtual void ResetAIPath()
    {
        enemyMeshAgent.isStopped = true;
        enemyMeshAgent.ResetPath();
    }

    protected virtual void SetDeathAnimationTrue()
    {
        if (!enemyAnimator.GetBool(EnemyAnimationTransitionParameters._isDead.ToString()))
        {
            MindlessShooterSFX.PlayRandomDeathClip();
            enemyAnimator.SetBool(EnemyAnimationTransitionParameters._isDead.ToString(), true);
        }
    }

    protected virtual void FaceTarget(Transform TargetDetected)
    {
        Vector3 _targetDirection = (TargetDetected.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(new Vector3(_targetDirection.x, 0, _targetDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turningSpeed);
    }

    private Animator GetModelAnimator()
    {
        var children = GetComponentsInChildren<Transform>();

        foreach (var item in children)
        {
            if (item.GetComponent<Animator>())
                return item.GetComponent<Animator>();
        }
        return null;
    }
}
