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
    public GameObject Inventario;

    [Header("Audios")]
    public AudioClip buttonClic;

    public AudioClip abrirMaletin;

    public AudioClip cerrarMaletin;
    private bool InTutorial = false;


    public GameObject Maletin;
    public bool InMaletin = false;
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
        GameManager.Instance.PauseAllSFX(); 
        print(" pulso esc");
        if (InMaletin)
        {
            Inventario.SetActive(false);
            InMaletin = false;
            return;
        }
        if (InTutorial)
        {
            print("en tuto esc");
            ExitTutorialMenu();
            return;
        }

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
            Maletin.SetActive(false);
        }
        else
        {
            ResumeMenu();
        }
    }

    public void ResumeMenu()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        GameManager.Instance.ResumeAllSFX(); 
        Maletin.SetActive(true);
        MenuPausa.SetActive(false);
        MenuOpciones = false;
        opcionesMenu.SetActive(false);

        enPause = false;
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        Time.timeScale = 1;
        GameManager.Instance.LoadScene("Menu");
    }

    public void EnterOpciones()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        MenuOpciones = true;
        opcionesMenu.SetActive(true);
        MenuPausa.SetActive(false);
    }

    public void SalirOpciones()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        MenuOpciones = false;
        opcionesMenu.SetActive(false);
        MenuPausa.SetActive(true);
    }

    public void EnterTutorialMenu()
    {
        InTutorial = true;
        GameManager.Instance.PlaySFX(buttonClic);
        MenuPausa.SetActive(false);
        TutorialMenu.SetActive(true);
    }

    public void ExitTutorialMenu()
    {
        InTutorial = false;
        GameManager.Instance.PlaySFX(buttonClic);
        MenuPausa.SetActive(true);
        TutorialMenu.SetActive(false);
    }

    public void EnterInventario()
    {
        GameManager.Instance.PauseAllSFX(); 
        GameManager.Instance.PlaySFX(abrirMaletin);
        Inventario.SetActive(true);
        Time.timeScale = 0;
        InMaletin = true;
    }
    public void ExitInventario()
    {
        GameManager.Instance.PlaySFX(cerrarMaletin);
        GameManager.Instance.ResumeAllSFX();
        Inventario.SetActive(false);
        Time.timeScale = 1;
        InMaletin = false;
    }
}
