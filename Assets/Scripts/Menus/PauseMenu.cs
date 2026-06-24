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
        if (InMaletin)
        {
            Inventario.SetActive(false);
            InMaletin = false;
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
        GameManager.Instance.PlaySFX(buttonClic);
        Pause.performed -= MenuPerformed;
        MenuPausa.SetActive(false);
        TutorialMenu.SetActive(true);
    }

    public void ExitTutorialMenu()
    {
        GameManager.Instance.PlaySFX(buttonClic);
        Pause.performed += MenuPerformed;
        MenuPausa.SetActive(true);
        TutorialMenu.SetActive(false);
    }

    public void EnterInventario()
    {
        GameManager.Instance.PlaySFX(abrirMaletin);
        Inventario.SetActive(true);
        InMaletin = true;
    }
    public void ExitInventario()
    {
        GameManager.Instance.PlaySFX(cerrarMaletin);
        Inventario.SetActive(false);
        InMaletin = false;
    }
}
