using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaunMenu : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction Pause;

    public GameObject settings;
    public GameObject Tutorial;

    private bool InTutorial;
    private bool InSettings;


    void OnEnable()
    {
        inputActions.Enable();
        Pause = inputActions.FindActionMap("UI").FindAction("Pause");
        Pause.performed += EscPerformed;
    }

    void OnDisable()
    {
        inputActions.Disable();
        Pause.performed -= EscPerformed;
    }

    private void EscPerformed(InputAction.CallbackContext context)
    {
        if (InSettings)
        {
            exitSettings();
        }
        else if (InTutorial)
        {
            exitTutorial();
        }
    }

    public void Play()
    {
        GameManager.Instance.createHand();
        GameManager.Instance.playerSO.objetos.Clear();
        GameManager.Instance.LoadScene("CombateTurnos");
    }

    public void settingsEnter()
    {
        settings.SetActive(true);
        InSettings = true;
    }
    public void exitSettings()
    {
        settings.SetActive(false);
        InSettings = false;
    }

    public void enterTutorial()
    {
        Tutorial.SetActive(true);
        InTutorial = true;
    }
    public void exitTutorial()
    {
        Tutorial.SetActive(false);
        InTutorial = false;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
