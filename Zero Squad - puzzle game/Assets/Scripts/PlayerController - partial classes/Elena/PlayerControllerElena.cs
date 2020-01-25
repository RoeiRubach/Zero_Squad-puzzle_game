﻿using UnityEngine;
using UnityEngine.UI;

public partial class PlayerController
{
    #region Elena attributes
    [Space(height: 20)]

    [Header("Elena attributes", order = 1)]
    [SerializeField] private Image _elenaHP;
    [SerializeField] private Button _elenaButtonRef;

    private int _elenaCurrentHP = _maxHP;

    [Header("Elena icon properties", order = 2)]
    [SerializeField] private Image _elenaIconPlaceHolder;

    [SerializeField] private Sprite _elenaStandardIconSprite, _elenaSelectedIconSprite;

    [Header("Elena skill properties", order = 3)]
    [SerializeField] private Image _elenaSkillPlaceHolder;

    [SerializeField] private Sprite _elenaStandardSkillSprite, _elenaSelectedSkillSprite;

    #endregion

    #region Elena UI manager

    public void ElenaButtonInteractivitySetter()
    {
        _elenaButtonRef.interactable = _elenaButtonRef.IsInteractable() ? false : true;
    }

    public void SwitchToElenaStateViaButton()
    {
        SetState(new ElenaState(this, _mainCamera));
    }

    public void ElenaIconSelectedON()
    {
        _elenaIconPlaceHolder.GetComponent<Image>().sprite = _elenaSelectedIconSprite;
    }

    public void ElenaIconSelectedOFF()
    {
        _elenaIconPlaceHolder.GetComponent<Image>().sprite = _elenaStandardIconSprite;
    }

    public void ElenaSkillButtonController()
    {
        _elenaSkillPlaceHolder.enabled = !_elenaSkillPlaceHolder.isActiveAndEnabled ? true : false;
    }

    public void ElenaOffSkillMode()
    {
        _elenaSkillPlaceHolder.GetComponent<Image>().sprite = _elenaStandardSkillSprite;
    }

    public void ElenaOnSkillMode()
    {
        _elenaSkillPlaceHolder.GetComponent<Image>().sprite = _elenaSelectedSkillSprite;
    }

    [ContextMenu("Apply damage to Elena - PLAYMODE ONLY!")]
    public void ElenaTakingDamage()
    {
        if (_elenaCurrentHP > 0)
        {
            _elenaCurrentHP--;
            _elenaHP.GetComponent<Image>().sprite = _hpBars[_elenaCurrentHP];
        }
        else
            print("Elena is already dead you sick fuck");
    } 

    #endregion
}
