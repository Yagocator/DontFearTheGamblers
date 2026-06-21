using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputActionAsset inputActions;
    public GameObject MenuPausa;

    private InputAction Pause;
    private bool enPause = false;
    public GameObject TutorialMenu;
    public GameObject opcionesMenu;
    private bool MenuOpciones = false;

    void OnEnable()
    {
        Pause = inputActions.FindActionMap("UI").FindAction("Pause");
        Pause.performed += MenuPerformed;
    }

    void OnDisable()
    {
        Pause.performed -= MenuPerformed;
    }

    private void MenuPerformed(InputAction.CallbackContext context)
    {
        if (MenuOpciones)
        {
            SalirOpciones();
            return;
        }

        enPause = !enPause;

        if (enPause)
        {
            Time.timeScale = 0;
            MenuPausa.SetActive(true);
        }
        else
        {
            ResumeMenu();
        }
    }

    public void ResumeMenu()
    {
        MenuPausa.SetActive(false);
        MenuOpciones = false;
        opcionesMenu.SetActive(false);

        enPause = false;
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.LoadScene("Menu");
    }

    public void EnterOpciones()
    {
        MenuOpciones = true;
        opcionesMenu.SetActive(true);
        MenuPausa.SetActive(false);
    }

    public void SalirOpciones()
    {
        MenuOpciones = false;
        opcionesMenu.SetActive(false);
        MenuPausa.SetActive(true);
    }

    public void EnterTutorialMenu()
    {
        Pause.performed -= MenuPerformed;
        MenuPausa.SetActive(false);
        TutorialMenu.SetActive(true);
    }

    public void ExitTutorialMenu()
    {
        Pause.performed += MenuPerformed;
        MenuPausa.SetActive(true);
        TutorialMenu.SetActive(false);
    }
}
