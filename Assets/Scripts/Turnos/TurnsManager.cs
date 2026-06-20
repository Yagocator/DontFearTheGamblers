using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TurnsManager : MonoBehaviour
{
    private bool isPlayerTurn = true;
    public bool jugadorListo = false;
    public bool win = false;
    // public List<GameObject> Enemigos;
    public List<EnemyUI> enemigosUI = new List<EnemyUI>();
    public List<EnemySO> enemigosSO = new List<EnemySO>();

    public EnemyUI enemigoSeleccionado;

    public InputActionAsset inputActions;

    private InputAction navegation;
    private InputAction fixEnemy;

    [SerializeField] private Button GuirarRuleta;
    void Awake()
    {
        navegation = inputActions.FindActionMap("UI").FindAction("Navigate");
        fixEnemy = inputActions.FindActionMap("UI").FindAction("FixEnemy");

    }

    private IEnumerator Turnos()
    {
        while (!win)
        {
            if (isPlayerTurn)
            {
                fixEnemy.performed += FixEnemy;
                // Lógica para el turno del jugador
                Debug.Log("Turno del jugador");
                yield return new WaitUntil(() => jugadorListo);
                jugadorListo = false;
                isPlayerTurn = false;
            }
            else
            {
                // Lógica para el turno del enemigo
                Debug.Log("Turno del enemigo");
                // Aquí puedes agregar la lógica para que el enemigo realice su acción
                yield return new WaitForSeconds(1f); // Simulación de tiempo de acción del enemigo
                isPlayerTurn = true; // Cambiar al turno del jugador
            }
        }
    }
    void Start()
    {
        StartCoroutine(Turnos());
    }
    void OnEnable()
    {
        RuletaController.ResultRuleta += ResultadoRuleta;
        RuletaController.ButtonPressed += BotonPressed;
        inputActions.Enable();
        navegation.performed += Navegation_performed;
        fixEnemy.performed += FixEnemy;
    }
    void OnDisable()
    {
        RuletaController.ResultRuleta -= ResultadoRuleta;
        RuletaController.ButtonPressed -= BotonPressed;
        inputActions.Disable();
        navegation.performed -= Navegation_performed;
        fixEnemy.performed -= FixEnemy;
    }

    private void BotonPressed(bool bol)
    {
        navegation.performed -= Navegation_performed;
        fixEnemy.performed -= FixEnemy;
    }

    private void ResultadoRuleta(int Res)
    {
        enemigoSeleccionado.Damagear(Res);
        jugadorListo = true;

    }

    private void Navegation_performed(InputAction.CallbackContext context)
    {
        float y = context.ReadValue<Vector2>().y;
        float x = context.ReadValue<Vector2>().x;
        changeEnemySelection(y > 0 ? -1 : (y < 0 ? 1 : 0));
    }

    public void changeEnemySelection(int direction)
    {
        if (enemigosUI.Count == 0) return;

        int currentIndex = enemigosUI.FindIndex(e => e.EstaSeleccionado);
        int newIndex = (currentIndex + direction + enemigosUI.Count) % enemigosUI.Count;


        if (currentIndex != -1)
            enemigosUI[currentIndex].DeselectEnemy();

        enemigoSeleccionado = enemigosUI[newIndex];
        enemigosUI[newIndex].SelectedEnemy();
    }

    private void FixEnemy(InputAction.CallbackContext context)
    {
        if (enemigoSeleccionado == null) return;
        // TODO Soniditos de fijar y desfijar
        if (enemigoSeleccionado.Fijado)
        {
            enemigoSeleccionado.Fijar(false);
            GuirarRuleta.interactable = false;
            navegation.performed += Navegation_performed;
        }
        else
        {
            enemigoSeleccionado.Fijar(true);
            GuirarRuleta.interactable = true;
            navegation.performed -= Navegation_performed;
        }


    }




}
