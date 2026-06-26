using UnityEngine;
using UnityEngine.InputSystem;

public class Credits : MonoBehaviour
{

    public InputActionAsset inputActions;

    private InputAction Pause;

    private void OnEnable()
    {
        inputActions.Enable();
        Pause = inputActions.FindActionMap("UI").FindAction("Pause");
        Pause.performed += EscPerformed;
    }

    private void OnDisable()
    {
        Pause.performed -= EscPerformed;

        inputActions.Disable();
    }

    private void EscPerformed(InputAction.CallbackContext context)
    {
        GameManager.Instance.LoadScene("Menu");
    }



}
