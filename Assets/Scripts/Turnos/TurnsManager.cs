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

    private bool seleccionBloqueada = false;

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

                foreach (EnemyUI enemigo in enemigosUI.FindAll(e => e.IsAlive))
                {
                    print("Escogiendo intencion");
                    enemigo.Intencion();
                }

                fixEnemy.performed += FixEnemy;
                yield return new WaitUntil(() => jugadorListo);
                jugadorListo = false;
                isPlayerTurn = false;
            }
            else
            {
                foreach (EnemyUI enemigo in enemigosUI.FindAll(e => e.IsAlive))// ejecutar acciones
                {
                    yield return enemigo.EjecutarAccionResaltada();
                }
                yield return new WaitForSeconds(1f); // Simulación de tiempo de acción del enemigo
                isPlayerTurn = true;
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
        EnemyUI.EnemyHovered += EnemyHovered_performed;
        EnemyUI.EnemyClicked += EnemyClicked_performed;
        inputActions.Enable();
        navegation.performed += Navegation_performed;
        fixEnemy.performed += FixEnemy;
    }

    void OnDisable()
    {
        RuletaController.ResultRuleta -= ResultadoRuleta;
        RuletaController.ButtonPressed -= BotonPressed;
        EnemyUI.EnemyHovered -= EnemyHovered_performed;
        EnemyUI.EnemyClicked -= EnemyClicked_performed;
        inputActions.Disable();
        navegation.performed -= Navegation_performed;
        fixEnemy.performed -= FixEnemy;
    }

    private void BotonPressed(bool bol)
    {
        navegation.performed -= Navegation_performed;
        fixEnemy.performed -= FixEnemy;
    }

    private void ResultadoRuleta(List<(int cantidad, PinchosType tipo)> list)
    {
        foreach (var item in list)
        {
            switch (item.tipo)
            {
                case PinchosType.Attack:
                    if (enemigoSeleccionado.Damagear(item.cantidad))
                    {
                        enemigoSeleccionado = null;
                        navegation.performed += Navegation_performed;
                        GuirarRuleta.interactable = false;
                        seleccionBloqueada = false;
                    }
                    break;
                case PinchosType.Defense:
                    GameManager.Instance.PJGetDefence(item.cantidad);
                    break;
                case PinchosType.Heal:
                    GameManager.Instance.PJGetHeald(item.cantidad);
                    break;
            }
        }
        jugadorListo = true;

    }

    private void Navegation_performed(InputAction.CallbackContext context)
    {
        float y = context.ReadValue<Vector2>().y;
        float x = context.ReadValue<Vector2>().x;
        changeEnemySelection(y > 0 ? -1 : (y < 0 ? 1 : 0));
    }

    private void EnemyHovered_performed(EnemyUI enemy)
    {
        // Si hay un enemigo fijado, o la selección está bloqueada, el ratón no puede cambiarla
        if (seleccionBloqueada) return;
        if (enemy == enemigoSeleccionado || !enemy.IsAlive) return;

        if (enemigoSeleccionado != null && enemigoSeleccionado.IsAlive)
            enemigoSeleccionado.DeselectEnemy();

        enemigoSeleccionado = enemy;
        enemy.SelectedEnemy();
    }

    public void changeEnemySelection(int direction)
    {
        if (enemigosUI.Count == 0) return;

        int currentIndex = enemigosUI.FindIndex(e => e.EstaSeleccionado);
        int newIndex = currentIndex;
        int intentos = 0;

        do
        {
            newIndex = (newIndex + direction + enemigosUI.Count) % enemigosUI.Count;
            intentos++;
        } while (!enemigosUI[newIndex].IsAlive && intentos < enemigosUI.Count);

        // Si no encontramos ninguno vivo, no hacer nada
        if (!enemigosUI[newIndex].IsAlive) return;

        if (currentIndex != -1 && enemigosUI[currentIndex].IsAlive)
            enemigosUI[currentIndex].DeselectEnemy();

        enemigoSeleccionado = enemigosUI[newIndex];
        enemigosUI[newIndex].SelectedEnemy();
    }

    private void FixEnemy(InputAction.CallbackContext context)
    {
        ToggleFijarEnemigo();
    }

    private void EnemyClicked_performed(EnemyUI enemy)
    {
        if (enemy != enemigoSeleccionado) return;
        ToggleFijarEnemigo();
    }

    private void ToggleFijarEnemigo()
    {
        if (enemigoSeleccionado == null) return;

        // TODO Soniditos de fijar y desfijar
        if (enemigoSeleccionado.Fijado)
        {
            enemigoSeleccionado.Fijar(false);
            GuirarRuleta.interactable = false;
            navegation.performed += Navegation_performed;
            seleccionBloqueada = false;
        }
        else
        {
            enemigoSeleccionado.Fijar(true);
            GuirarRuleta.interactable = true;
            navegation.performed -= Navegation_performed;
            seleccionBloqueada = true;
        }
    }




}
