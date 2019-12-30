﻿using UnityEngine.AI;
using UnityEngine;

public class DouglasState : PlayerStateManager
{
    string douglasName = "Douglas";

    private bool isUsingSkill;
    
    private GameObject douglasShotgun;
    private GameObject douglasAgentPlacement;

    private DouglasShootingManager douglasShootingManager;

    public DouglasState(PlayerController character, CameraController camera) : base(character, camera)
    {
    }

    public override void Handle()
    {
        if (!isUsingSkill)
            PointAndClickMovement();
        else
        {
            TurnTowardTheCursor();
            DouglasPointAndClickShooting();
        }

        EnterOrExitSkillMode();
        SwitchCharacters();
    }

    public override void OnStateEnter()
    {
        DouglasInitialization();
        Debug.Log("Douglas is now in control");
    }

    public override void OnStateExit()
    {
        if (isUsingSkill)
            douglasAgentPlacement.SetActive(true);
        else
            myCurrentAgent.enabled = true;

        myCurrentCharacter = null;
        myCurrentAgent = null;
        initializationComplete = false;

        Debug.Log("Douglas is out of control");
    }

    private void DouglasInitialization()
    {
        myCurrentCharacter = GameObject.FindWithTag(douglasName);

        cameraController.SetCharacter(myCurrentCharacter);

        myCurrentAgent = myCurrentCharacter.GetComponent<NavMeshAgent>();

        douglasShootingManager = myCurrentCharacter.GetComponent<DouglasShootingManager>();
        douglasShotgun = myCurrentCharacter.transform.GetChild(2).gameObject;

        douglasAgentPlacement = myCurrentCharacter.transform.GetChild(3).gameObject;

        if (douglasShotgun.activeSelf)
            isUsingSkill = true;

        initializationComplete = true;
    }

    public override void EnterOrExitSkillMode()
    {
        if (Input.GetKeyDown(KeyCode.Space) && initializationComplete)
        {
            isUsingSkill = !isUsingSkill ? true : false;

            if (!douglasShotgun.activeSelf)
            {
                myCurrentAgent.enabled = false;
                douglasShotgun.SetActive(true);
            }
            else
            {
                douglasShotgun.SetActive(false);
                douglasAgentPlacement.SetActive(false);
            }
        }
    }

    private void SwitchCharacters()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerController.SetState(new ElenaState(playerController, cameraController));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerController.SetState(new HectorState(playerController, cameraController));
        }
    }

    public void DouglasPointAndClickShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            douglasShootingManager.CallSimpleShoot();
        }
    }
}
