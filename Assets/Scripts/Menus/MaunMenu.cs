using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MaunMenu : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction Pause;

    public GameObject settings;
    public GameObject Tutorial;

    private bool InTutorial;
    private bool InSettings;
    public Button[] botones;

    public AudioClip gameMusic;

    public AudioClip buttonClic;

    public AudioClip mainMenuMusic; 
    void OnEnable()
    {
        inputActions.Enable();
        GameManager.Instance.PlayMusic(mainMenuMusic);
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
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        GameManager.Instance.PlayMusic(gameMusic);
        GameManager.Instance.PlaySFX(buttonClic);
        GameManager.Instance.nivelAct = 1;
        GameManager.Instance.playerSO.vida = 100;
        GameManager.Instance.playerSO.vidaMax = 100;
        GameManager.Instance.playerSO.shild = 0;
        GameManager.Instance.playerSO.handSize = 4;
        GameManager.Instance.playerSO.maximumPlayablePinchos = 3; 

        GameManager.Instance.createHand();
        GameManager.Instance.playerSO.objetos.Clear();
        GameManager.Instance.LoadScene("CombateTurnos");
    }

    public void settingsEnter()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        settings.SetActive(true);
        InSettings = true;
    }
    public void exitSettings()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        settings.SetActive(false);
        InSettings = false;
    }

    public void enterTutorial()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        Tutorial.SetActive(true);
        InTutorial = true;
    }
    public void exitTutorial()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        Tutorial.SetActive(false);
        InTutorial = false;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
