using UnityEditor.Rendering.Universal;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Jump { get; private set; }
    public bool Sprint { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool ToggleCombatPressed { get; private set; }
    public bool UpgradeMenuPressed { get; private set; }

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private bool isPaused = false;

    private void Update()
    {
        AttackPressed = Input.GetMouseButtonDown(0);
        ToggleCombatPressed = Input.GetKeyDown(KeyCode.C);
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResetJump() => Jump = false;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);

    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }
#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        Move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        Look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        Jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        Sprint = newSprintState;
    }

    //TODO: see what it does

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked && !isPaused);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !newState;
    }
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetCursorState(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetCursorState(cursorLocked);
    }
}