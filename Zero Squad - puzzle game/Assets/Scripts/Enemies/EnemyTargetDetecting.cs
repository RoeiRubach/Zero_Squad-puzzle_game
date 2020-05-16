﻿using UnityEngine;
using UnityEngine.AI;

public class EnemyTargetDetecting : MonoBehaviour
{
    private string _douglasName = "Douglas", _elenaName = "Elena";
    private string _hectorName = "Hector", _hectorShieldName = "Hector Shield";

    private float _delayTime = 0.85f;
    
    public bool IsElenaBeenSpotted { get; private set; }
    private bool _isHectorShieldSpotted;

    [SerializeField] Transform _enemiesShieldHittingSpot;

    private EnemyBase _enemyBaseRef;
    private ElenaStealthManager _elenaStealthManager;

    private void Start()
    {
        _enemyBaseRef = GetComponentInParent<EnemyBase>();
    }

    private void Update()
    {
        var nearestCharacter = CharactersPoolController.FindClosestEnemy(transform.position);

        if (nearestCharacter != null && _enemyBaseRef.IsPlayerSpotted && _enemyBaseRef && _enemyBaseRef.IsAbleToReachTarget)
        {
            if (nearestCharacter.transform.CompareTag(_elenaName) && !IsElenaBeenSpotted)
                return;

            if (!_enemyBaseRef.IsAttacking)
                _enemyBaseRef.TargetDetected = nearestCharacter.transform;
            else
            {
                if (nearestCharacter.transform != _enemiesShieldHittingSpot)
                    _enemyBaseRef.TargetDetected = nearestCharacter.transform;
            }
        }
        else
            _enemyBaseRef.IsPlayerSpotted = false;
    }

    #region On Trigger States
    /// <summary>
    /// Checking if Elena entered enemy's sight.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(_elenaName))
            ElenaEnterDetected(other);
    }

    /// <summary>
    /// Checking if Elena still in enemy's sight and if she stayed/left enemy's area.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag(_elenaName) && !IsElenaBeenSpotted)
            ElenaEnterDetected(other);
    }

    /// <summary>
    /// Checking if the character left enemy's sight.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (IsElenaBeenSpotted)
        {
            if (other.CompareTag(_elenaName))
                ElenaOutOfSight();
        }
    }
    #endregion

    private void ElenaEnterDetected(Collider other)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(_enemyBaseRef.transform.position + (Vector3.up * 1.2f), DirectionToElena(other.transform), out hitInfo, 7.6f))
        {
            if (hitInfo.transform.CompareTag(_elenaName))
                ElenaBeenSpotted(hitInfo.collider);
        }
    }

    private void ElenaBeenSpotted(Collider _elena)
    {
        if (_elenaStealthManager == null)
            _elenaStealthManager = _elena.GetComponentInParent<ElenaStealthManager>();

        _enemyBaseRef.TargetDetected = _elena.transform;
        IsElenaBeenSpotted = true;
        _enemyBaseRef.IsPlayerSpotted = true;
        _elenaStealthManager.AddElenaToPool();
        Debug.Log("Elena got triggered");
    }

    private void ElenaOutOfSight()
    {
        IsElenaBeenSpotted = false;
        _elenaStealthManager.RemoveElenaFromPool();
        Debug.Log("Elena got out of trigger");
    }

    private Vector3 DirectionToElena(Transform elenaRef) => (elenaRef.position - _enemyBaseRef.transform.position).normalized;

}
