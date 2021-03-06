﻿using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private const float BULLET_IMPACT_DISTANCE = 1.65f;
    [Range(1f, 5f)]
    [SerializeField] private int _bulletDamage = 1;
    [Range(1f, 5f)]
    [SerializeField] private float _bulletMoveSpeed = 15f;

    private Vector3 _shootDirection;
    private PlayerController _playerController;

    private void Awake() => _playerController = FindObjectOfType<PlayerController>();

    public void SetUp(Vector3 shootDirection)
    {
        _shootDirection = shootDirection;
        transform.eulerAngles = new Vector3(90, GetAngleFromVectorFloat(_shootDirection), 0);
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += _shootDirection * Time.deltaTime * _bulletMoveSpeed;

        var nearestEnemy = CharactersPoolController.FindClosestCharacter(transform.position);
        float distance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

        if(distance <= BULLET_IMPACT_DISTANCE)
        {
            _playerController.DamageACharacter(nearestEnemy.transform, _bulletDamage);
            Destroy(gameObject);
        }
    }

    private float GetAngleFromVectorFloat(Vector3 direction)
    {
        direction = direction.normalized;
        float n = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }
}
