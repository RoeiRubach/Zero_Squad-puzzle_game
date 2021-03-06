﻿using UnityEngine;

public class ElenaStealthManager : MonoBehaviour
{
    [Range(5f, 20f)]
    [SerializeField] private float _stealthTime = 10f;
    [Range(2.5f, 10f)]
    [SerializeField] private float _stealthCooldownTime = 2.5f;
    [SerializeField] private Material _elenaBodyNHeadMaterial, _elenaCapeNHoodMaterial;
    [SerializeField] private Material _stealthMode;
    [SerializeField] private SkinnedMeshRenderer[] _elenaBodyNHeadArray;
    [SerializeField] private SkinnedMeshRenderer[] _elenaCapeNHoodArray;
    [SerializeField] private GameObject _elenaHairRef, _elenaHoodRef;

    private CharactersPoolController _charactersPoolController;

    [HideInInspector] public bool IsInStealthMode { get; private set; }
    [HideInInspector] public bool IsStealthOnCooldown { get; private set; }
    private float _stealthTimer, _stealthCooldownTimer;

    private void Start()
    {
        _charactersPoolController = GetComponentInChildren<CharactersPoolController>();
        _stealthTimer = _stealthTime;
        _stealthCooldownTimer = _stealthCooldownTime;
    }

    private void Update()
    {
        if (IsStealthOnCooldown)
        {
            if ((_stealthCooldownTimer -= Time.deltaTime) <= 0)
            {
                IsStealthOnCooldown = false;
                _stealthCooldownTimer = _stealthCooldownTime;
            }
        }
        else
        {
            if (IsInStealthMode)
            {
                if ((_stealthTimer -= Time.deltaTime) <= 0)
                {
                    IsStealthOnCooldown = true;
                    _stealthTimer = _stealthTime;
                    GetComponent<GameEventSubscriber>()?.OnEventFire();
                }
            }
        }

    }

    public void CallStealthMode()
    {
        for (int i = 0; i < _elenaBodyNHeadArray.Length; i++)
        {
            _elenaBodyNHeadArray[i].material = _stealthMode;
        }
        for (int i = 0; i < _elenaCapeNHoodArray.Length; i++)
        {
            _elenaCapeNHoodArray[i].material = _stealthMode;
        }

        _elenaHairRef.SetActive(false);
        _elenaHoodRef.SetActive(true);

        if (_charactersPoolController.isActiveAndEnabled)
            InvokeRemoveElenaFromPool();

        IsInStealthMode = true;
        GetComponentInChildren<BoxCollider>().enabled = false;
    }

    public void OffStealthMode()
    {
        for (int i = 0; i < _elenaBodyNHeadArray.Length; i++)
        {
            _elenaBodyNHeadArray[i].material = _elenaBodyNHeadMaterial;
        }
        for (int i = 0; i < _elenaCapeNHoodArray.Length; i++)
        {
            _elenaCapeNHoodArray[i].material = _elenaCapeNHoodMaterial;
        }

        _elenaHairRef.SetActive(true);
        _elenaHoodRef.SetActive(false);
        IsInStealthMode = false;
        _stealthTimer = _stealthTime;
        GetComponentInChildren<BoxCollider>().enabled = true;
    }

    public void AddElenaToPool()
    {
        if(!_charactersPoolController.enabled)
            _charactersPoolController.enabled = true;
    }

    public void RemoveElenaFromPool()
    {
        if (_charactersPoolController.enabled)
            _charactersPoolController.enabled = false;
    }

    private void InvokeRemoveElenaFromPool()
    {
        if (!IsInvoking("RemoveElenaFromPool"))
        {
            Invoke("RemoveElenaFromPool", 1f);
            //Debug.Log("Elena got out of trigger");
        }
    }
}
