﻿using UnityEngine;
using UnityEngine.AI;

public enum CharactersAnimationTransitionParameters
{
    _isMoving,
    _isSkillMode,
    _isLifting,
    _isCarrying,
    _isPushing
}

public abstract class PlayerStateManager
{
    public bool IsCabinetInteracting { get; protected set; }

    protected float stealthRunningSpeed = 3f;
    protected int runningSpeed = 5, bombWalkingSpeed = 2;
    protected PlayerController playerController;
    protected CameraController cameraController;
    protected Transform interactableObject;
    protected bool isPossibleToInteract, isInteracting;
    protected PlayerStateManager(PlayerController character)
    {
        playerController = character;
    }
    protected PlayerStateManager(PlayerController character, CameraController camera)
    {
        playerController = character;
        cameraController = camera;
    }

    #region Game state properties

    protected GameObject myCurrentCharacter;
    protected NavMeshAgent myCurrentAgent;
    protected Animator myCurrentAnimator;

    protected bool initializationComplete;
    public bool EnterSkillViaButton;

    #endregion

    #region Game state abstract methods

    public abstract void UpdateHandle();
    public abstract void OnStateEnter();
    public abstract void OnTriggerEnter(string tagReceived);
    public abstract void OnStateExit();
    public abstract void EnterOrExitSkillMode();

    #endregion

    #region Game state virtual methods

    protected virtual void PointAndClickMovement()
    {
        myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isSkillMode.ToString(), false);

        if (Input.GetMouseButtonDown(1))
        {
            if (!IsCabinetInteracting)
            {
                RaycastHit hitInfo;

                Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(myRay, out hitInfo, Mathf.Infinity, playerController.WalkableLayerMask))
                {
                    if (isPossibleToInteract)
                    {
                        isInteracting = true;
                        myCurrentAgent?.SetDestination(CharacterInteractionPlacement());
                    }
                    else
                    {
                        isInteracting = false;
                        ResetInteractable();
                        myCurrentAgent?.SetDestination(hitInfo.point);
                    }

                    if (playerController.IsLifting)
                        myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isCarrying.ToString(), true);
                    else
                        myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isMoving.ToString(), true);
                }
            }
            if (GameManager.Instance.IsReachedFinalCheckPoint) return;
            if (TutorialPopUpsController.Instance.MyTutorialHandler["Movement"])
            {
                TutorialPopUpsController.Instance.DestroyFirstChild();
                TutorialPopUpsController.Instance.DisplayFirstChild();
            }
        }

        else if (!myCurrentAgent.hasPath)
        {
            if (isInteracting)
                CharacterObjectInteraction();

            if (playerController.IsLifting)
                myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isCarrying.ToString(), false);
            else
                myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isMoving.ToString(), false);
        }
    }

    /// <summary>
    /// Stops and reset the current character's destination (if it has one).
    /// Creates an "imaginary" plane and casting a ray on it with the cursor.
    /// 
    /// Determine the point where the cursor ray intersects the plane.
    /// This will be the point that the object must look towards to be looking at the mouse.
    /// Raycasting to a Plane object only gives me a distance, so I have to take the distance,
    /// then find the point along that ray that meets that distance. This will be the point to look at.
    /// </summary>
    protected virtual void TurnTowardTheCursor()
    {
        myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isSkillMode.ToString(), true);

        ResetAIPath();
        myCurrentAgent.enabled = false;

        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, myCurrentCharacter.transform.position);

        // Generate a ray from the cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);

            // Determine the target rotation. This is the rotation if the transform looks at the target point.
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - myCurrentCharacter.transform.position);

            myCurrentCharacter.transform.rotation = targetRotation;
        }
    }
    #endregion

    protected void ResetInteractable()
    {
        if (interactableObject != null && !isInteracting)
        {
            Outline interactableOutline = interactableObject.GetComponent<Outline>();
            if (interactableOutline != null)
                interactableOutline.enabled = false;
            interactableObject = null;
        }
    }

    protected void ResetAIPath()
    {
        if (myCurrentAgent.hasPath)
        {
            myCurrentAgent.isStopped = true;
            myCurrentAgent.ResetPath();
        }
    }

    protected void ResetInteractableWhenExitCharacter()
    {
        if (isInteracting)
            isInteracting = false;
        ResetInteractable();
    }

    protected virtual void CharacterObjectInteraction()
    {
        if (interactableObject != null)
        {
            if (interactableObject.GetComponent<IInteractable>() != null)
            {
                float dis = Vector3.Distance(myCurrentCharacter.transform.position, CharacterInteractionPlacement());

                if (dis <= 2.3f)
                {
                    interactableObject.GetComponent<IInteractable>().Interact();

                    switch (interactableObject.name)
                    {
                        case "Bomb":
                            playerController.DouglasSFX.StartBombCarryingLoop();
                            myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isLifting.ToString(), true);
                            myCurrentAgent.speed = bombWalkingSpeed;
                            if (!GameManager.Instance.IsReachedFinalCheckPoint)
                            {
                                if (TutorialPopUpsController.Instance.MyTutorialHandler["Move bomb"])
                                {
                                    TutorialPopUpsController.Instance.DestroyFirstChild();
                                    TutorialPopUpsController.Instance.DisplayFirstChild();
                                }
                            }
                            break;
                        case "Cabinet":
                            myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isPushing.ToString(), true);
                            IsCabinetInteracting = true;
                            if (!GameManager.Instance.IsReachedFinalCheckPoint)
                            {
                                if (TutorialPopUpsController.Instance.MyTutorialHandler[interactableObject.name])
                                    TutorialPopUpsController.Instance.DestroyFirstChild();
                            }
                            break;
                        case "Health Pack":
                            HealthPackInteraction();
                            break;
                        case "Shooter - root":
                        case "Mindless possessed":
                        case "Summoner":
                            playerController.ElenaSFX.PlayElenaBackstab();
                            isInteracting = false;
                            if (!GameManager.Instance.IsReachedFinalCheckPoint)
                            {
                                if (TutorialPopUpsController.Instance.MyTutorialHandler["Backstab"])
                                    TutorialPopUpsController.Instance.DestroyFirstChild();
                            }
                            break;
                        case "Switch":
                        case "Console":
                            isInteracting = false;
                            break;
                    }

                    myCurrentAgent.SetDestination(CharacterInteractionPlacement());
                }
                else
                {
                    isInteracting = false;
                    ResetInteractable();
                }
            }
            else
            {
                if (IsCabinetInteracting)
                {
                    myCurrentAnimator.SetBool(CharactersAnimationTransitionParameters._isPushing.ToString(), false);
                    IsCabinetInteracting = false;
                }
                isInteracting = false;
                ResetInteractable();
            }
        }
    }

    protected void ResetCharactersControl()
    {
        myCurrentCharacter = null;
        myCurrentAgent = null;
        myCurrentAnimator = null;
        initializationComplete = false;
    }

    protected void CharacterComponentsInitialization(string characterName)
    {
        myCurrentCharacter = GameObject.Find(characterName);
        myCurrentAnimator = myCurrentCharacter.GetComponent<Animator>();
        cameraController.SetCharacter(myCurrentCharacter);
        myCurrentAgent = myCurrentCharacter.GetComponent<NavMeshAgent>();
    }

    protected void SavePopupNeeded(string popUp)
    {
        if (GameManager.Instance.IsReachedFinalCheckPoint) return;
        if (TutorialPopUpsController.Instance.MyTutorialHandler[popUp])
            TutorialPopUpsController.Instance.HideFirstChild();
    }

    protected void ShowPopupNeeded(string popUp)
    {
        if (GameManager.Instance.IsReachedFinalCheckPoint) return;
        if (TutorialPopUpsController.Instance.MyTutorialHandler[popUp])
            TutorialPopUpsController.Instance.DisplayFirstChild();
    }

    private void HealthPackInteraction()
    {
        HealthRegenCollectables healthRegenCollectables = interactableObject.GetComponent<HealthRegenCollectables>();

        switch (myCurrentCharacter.tag)
        {
            case "Douglas":
                playerController.DouglasSFX.PlayCollectHealthPackClip();
                playerController.DouglasGainingHealth(healthRegenCollectables.HealthToRegen);
                break;
            case "Elena":
                playerController.ElenaSFX.PlayCollectHealthPackClip();
                playerController.ElenaGainingHealth(healthRegenCollectables.HealthToRegen);
                break;
            case "Hector":
                playerController.HectorSFX.PlayCollectHealthPackClip();
                playerController.HectorGainingHealth(healthRegenCollectables.HealthToRegen);
                break;
        }

        if (healthRegenCollectables.IsInteract)
        {
            isInteracting = false;
            ResetInteractable();
            healthRegenCollectables.CallOnDestroy();
        }
    }

    private Vector3 CharacterInteractionPlacement()
    {
        if (interactableObject == null)
            return myCurrentAgent.transform.position;

        return interactableObject.GetComponent<IInteractable>().CharacterInteractionPlacement();
    }
}