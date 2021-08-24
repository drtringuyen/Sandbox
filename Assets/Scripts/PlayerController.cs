//Simple 3D FPS controller
using System.Collections;
using System.Collections.Generic;
//using MilkShake.Demo;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public Transform cameraHolder;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 85.0f;

    public CharacterController characterController;

    public HUDController HUD;

    Vector3 moveDirection = Vector3.zero;
    float rotationX, rotationY = 0;



    //public ShakeEvent cameraShaker;

    //public AudioPreset audioPreset;

    [HideInInspector]
    //enable/disable player Input control
    private bool clickable, rightclickable, scrollable, movable = true;
    private Quaternion targetRotCam;
    private Quaternion targetRotBody;
    private bool isInCameraAnimation;
    private IEnumerator cameraCoroutine;
    private bool currentRunningState = false;

    public enum Action
    {
        clickable,
        rightclickable,
        scrollable,
        moveable
    };

    public enum ReactionPreset { earthQuake, tumble, denied }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        //gameplayController = GetComponent<GamePlayController>();
        //audioPreset = FindObjectOfType<AudioPreset>();
        //cameraShaker = FindObjectOfType<ShakeEvent>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //avoid spamming
        clickable = true;
        rightclickable = true;
        scrollable = true;

        LeanTween.moveLocalZ(cameraHolder.gameObject , 0.3f, 3).setEaseInOutBack().setLoopPingPong();
    }

    void Update()
    {
        ////check if nobody touch the controller => dont waste time update any stats on the player
        //if (ControllIntact())
        //    return;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = movable ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = movable ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        ReactToRun(isRunning);

        if (Input.GetButton("Jump") && movable && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (movable)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Input.GetAxis("Mouse X") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);

        }

        handleMouse();

    }

    private void ReactToRun(bool isRunning)
    {
        if (isRunning == currentRunningState) return;
        if (isRunning)
        {
            HUD.startRun();
        }
        else
        {
            HUD.stopRun();
        }

        currentRunningState = isRunning;
    }

    private void handleMouse()
    {
        if (Input.mouseScrollDelta.y != 0 && scrollable)
            HUD.ScrollForward( Input.mouseScrollDelta.y > 0 ? true : false);
        if (Input.GetMouseButtonDown(0) && clickable)
            HUD.PerformLeft();

        if (Input.GetMouseButtonDown(1) && rightclickable)
            HUD.PerformRight();

        if (Input.GetMouseButtonUp(0) && clickable)
            HUD.CancelLeft();

        if (Input.GetMouseButtonUp(1) && rightclickable)
            HUD.CancelRight();
    }


    private IEnumerator CoolDownAction(float cooldownTime, Action action)
    {
        switchActionState(action, false);
        yield return new WaitForSeconds(cooldownTime);
        switchActionState(action, true);
    }

    private void switchActionState(Action action, bool state)
    {
        switch (action)
        {
            case Action.clickable:
                clickable = state;
                break;
            case Action.rightclickable:
                rightclickable = state;
                break;
            case Action.scrollable:
                scrollable = state;
                break;
            case Action.moveable:
                movable = state;
                break;
        }
    }

    public void disableInputAction(Action action)
    {
        switchActionState(action, false);
    }

    public void disableInputActionForSeconds(Action action, float cooldownTime)
    {
        StartCoroutine(CoolDownAction(cooldownTime, action));
    }

    public void disableAllInputActions(float cooldownTime)
    {
        StartCoroutine(CoolDownAction(cooldownTime, Action.clickable));
        StartCoroutine(CoolDownAction(cooldownTime, Action.rightclickable));
        StartCoroutine(CoolDownAction(cooldownTime, Action.moveable));
        StartCoroutine(CoolDownAction(cooldownTime, Action.scrollable));
    }

    public void enableInputAction(Action action)
    {
        switchActionState(action, true);
    }

    public void React(ReactionPreset gameFeedBack)
    {
        //switch (gameFeedBack)
        //{
        //    case ReactionPreset.earthQuake:
        //        cameraShaker.UIShakeOnce(0);
        //        audioPreset.Play(0);
        //        break;
        //    case ReactionPreset.tumble:
        //        cameraShaker.UIShakeOnce(1);
        //        audioPreset.Play(1);
        //        print("playing tumble");
        //        break;
        //    case ReactionPreset.denied:
        //        cameraShaker.UIShakeOnce(2);
        //        audioPreset.Play(2);
        //        break;

        //}
    }

    private bool ControllIntact()
    {
        return (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0
            && Input.GetAxis("Mouse Y") == 0 && Input.GetAxis("Mouse X") == 0);
    }

    public void LookAt(Transform target)
    {
        Vector3 targetDir = target.position - transform.position;
        targetDir.y = 0;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDir), 90f);

        //#if UNITY_EDITOR_OSX
        //        LeanTween.rotateX(playerCamera.gameObject, 0f, 3);
        //#endif
    }
}