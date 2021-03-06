﻿using UnityEngine;
using UnityEngine.UI;

public partial class PlayerController
{
    #region Douglas attributes

    [Header("Douglas attributes:", order = 0)]
    [SerializeField] private Image _douglasHP;
    [SerializeField] private Button _douglasButtonRef;

    private int _douglasCurrentHP = 5;

    [Header("Douglas icon properties:", order = 1)]
    [SerializeField] private Image _douglasIconPlaceHolder;

    [SerializeField] private Sprite _douglasStandardIconSprite;
    [SerializeField] private Sprite _douglasSelectedIconSprite;

    [Header("Douglas skill properties:", order = 2)]
    [SerializeField] private Image _douglasSkillPlaceHolder;

    [SerializeField] private Sprite _douglasStandardSkillSprite;
    [SerializeField] private Sprite _douglasSelectedSkillSprite;

    #endregion

    #region Douglas UI manager

    public void DouglasButtonInteractivityToggle()
    {
        if(_douglasButtonRef != null)
            _douglasButtonRef.interactable = _douglasButtonRef.IsInteractable() ? false : true;
    }

    public void SwitchToDouglasStateViaButton()
    {
        if (GameObject.FindWithTag(CharactersEnum.Douglas.ToString()))
            SetState(new DouglasState(this, _mainCamera));
    }

    public void DouglasIconSelectedON()
    {
        if (_douglasIconPlaceHolder != null)
            _douglasIconPlaceHolder.sprite = _douglasSelectedIconSprite;
    }

    public void DouglasIconSelectedOFF()
    {
        if(_douglasIconPlaceHolder != null)
           _douglasIconPlaceHolder.sprite = _douglasStandardIconSprite;
    }

    public void DouglasSkillButtonToggle()
    {
        if (_douglasSkillPlaceHolder != null)
            _douglasSkillPlaceHolder.enabled = !_douglasSkillPlaceHolder.isActiveAndEnabled ? true : false;
    }

    public void DouglasSpriteOffSkillMode()
    {
        if (_douglasSkillPlaceHolder != null)
            _douglasSkillPlaceHolder.sprite = _douglasStandardSkillSprite;
    }

    public void DouglasSpriteOnSkillMode()
    {
        if(_douglasSkillPlaceHolder != null)
            _douglasSkillPlaceHolder.sprite = _douglasSelectedSkillSprite;
    }
    
    public void DouglasTakingDamage(int damageAmount = 1)
    {
        if(_douglasCurrentHP != 0)
        {
            for (int i = 0; i < damageAmount; i++)
            {
                _douglasCurrentHP--;

                if (_douglasHP != null)
                    _douglasHP.sprite = _hpBars[_douglasCurrentHP];

                if (_douglasCurrentHP <= 0)
                {
                    DouglasSFX.PlayRandomDeathClip();
                    SceneController.LoadScene(_buildIndex: 1);
                    break;
                }
            }
        }
    }

    public void DouglasGainingHealth(int regenAmount)
    {
        for (int i = 0; i < regenAmount; i++)
        {
            if (_douglasCurrentHP < 10)
            {
                _douglasCurrentHP++;
                _douglasHP.sprite = _hpBars[_douglasCurrentHP];
            }
            else
                break;
        }
    }
    #endregion
}
