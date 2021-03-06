﻿using UnityEngine;

public class HectorState : PlayerStateManager
{
    private bool _isUsingSkill;
    private GameObject _hectorShield;
    private GameObject _hectorAgentPlacement;
    private Transform _hectorEnemyHittingSpot;

    public HectorState(PlayerController character) : base(character)
    {

    }
    public HectorState(PlayerController character, CameraController camera) : base(character, camera)
    {
    }

    public override void UpdateHandle()
    {
        if (!_isUsingSkill)
        {
            HighlightCursorOverInteractableObject();
            PointAndClickMovement();
        }
        else
            TurnTowardTheCursor();

        EnterOrExitSkillMode();
        SwitchCharacters();
    }

    public override void EnterOrExitSkillMode()
    {
        if (initializationComplete)
        {
            if (Input.GetKeyDown(KeyCode.Space) || EnterSkillViaButton)
            {
                if (_hectorEnemyHittingSpot.GetComponent<EnemyHealth>().CurrentHealth > 0 || _hectorShield.activeSelf)
                {
                    EnterSkillViaButton = false;
                    _isUsingSkill = !_isUsingSkill ? true : false;

                    if (!_hectorShield.activeSelf)
                    {
                        playerController.HectorSFX.PlayRandomSkillClip();
                        _hectorShield.SetActive(true);
                        playerController.HectorOnSkillMode();
                    }
                    else
                    {
                        playerController.HectorSFX.PlayHectorShieldOFF();
                        _hectorShield.SetActive(false);
                        myCurrentAgent.enabled = true;
                        playerController.HectorOffSkillMode();
                    }
                }

                if (GameManager.Instance.IsReachedFinalCheckPoint) return;
                if (TutorialPopUpsController.Instance.MyTutorialHandler["Shield mode"])
                {
                    TutorialPopUpsController.Instance.DestroyFirstChild();
                    TutorialPopUpsController.Instance.DisplayFirstChild();
                }
            }
        }
    }

    public override void OnStateEnter()
    {
        HectorInitialization();
        ShowPopupNeeded("Hack consoles");
        //Debug.Log("Hector is now in control");
    }

    public override void OnTriggerEnter(string tagReceived)
    {
        switch (tagReceived)
        {
            case "Enemy":
                playerController.HectorSFX.PlayRandomHitClip();
                playerController.HectorTakingDamage();
                break;
        }
    }

    public override void OnStateExit()
    {
        if (_isUsingSkill)
            _hectorAgentPlacement.SetActive(true);

        playerController.HectorSkillButtonController();
        playerController.HectorIconSelectedOFF();
        playerController.HectorButtonInteractivitySetter();
        ResetInteractableWhenExitCharacter();
        SavePopupNeeded("Hack consoles");
        ResetCharactersControl();

        //Debug.Log("Hector is out of control");
    }

    private void HighlightCursorOverInteractableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, playerController.InteractableLayerMask))
        {
            if (hitInfo.collider.GetComponent<IHectorInteractables>() != null)
            {
                if (!isInteracting)
                {
                    if (hitInfo.collider.GetComponent<IHectorTech>() != null)
                        CursorController.Instance.SetTechCursor();
                    else
                        CursorController.Instance.SetInteractableCursor();

                    if (interactableObject == null)
                        interactableObject = hitInfo.transform;
                    interactableObject.GetComponent<Outline>().enabled = true;

                    isPossibleToInteract = true;
                }
            }
        }
        else
        {
            ResetInteractable();
            CursorController.Instance.SetStandardCursor();
            isPossibleToInteract = false;
        }
    }

    private void HectorInitialization()
    {
        CharacterComponentsInitialization(CharactersEnum.Hector.ToString());

        _hectorShield = myCurrentCharacter.transform.GetChild(2).transform.GetChild(0).gameObject;
        _hectorEnemyHittingSpot = _hectorShield.transform.GetChild(0);
        _hectorAgentPlacement = myCurrentCharacter.transform.GetChild(3).gameObject;

        playerController.HectorSkillButtonController();
        playerController.HectorIconSelectedON();
        playerController.HectorButtonInteractivitySetter();

        if (_hectorShield.activeSelf)
        {
            _isUsingSkill = true;
            _hectorAgentPlacement.SetActive(false);
            playerController.HectorOnSkillMode();
        }

        initializationComplete = true;
        playerController.HectorSFX.PlayRandomSelectedClip();
    }

    private void SwitchCharacters()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && GameObject.FindGameObjectWithTag(CharactersEnum.Douglas.ToString()))
        {
            playerController.SetState(new DouglasState(playerController, cameraController));

            if (GameManager.Instance.IsReachedFinalCheckPoint) return;
            if (TutorialPopUpsController.Instance.MyTutorialHandler["Shield mode"]) return;
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2) && GameObject.FindGameObjectWithTag(CharactersEnum.Elena.ToString()))
        {
            playerController.SetState(new ElenaState(playerController, cameraController));

            if (GameManager.Instance.IsReachedFinalCheckPoint) return;
            if (TutorialPopUpsController.Instance.MyTutorialHandler["Select Elena"])
            {
                TutorialPopUpsController.Instance.DestroyFirstChild();
                TutorialPopUpsController.Instance.DisplayFirstChild();
            }
        }
    }
}
