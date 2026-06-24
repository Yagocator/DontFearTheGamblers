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

    public bool seleccionBloqueada = false;

    public InputActionAsset inputActions;

    
    private InputAction navegation;
    private InputAction fixEnemy;

    [SerializeField] private Button GuirarRuleta;
    public PJController PJController;
    public RuletaController RuletaController;

    public int cadaXRondas;

    [Header("Audio")]
    public AudioClip enemyConfirmed;
    public AudioClip enemyActionSFX;
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
                RuletaController.drawHand();
                fixEnemy.performed += FixEnemy;
                int countenmigos = 0;
                foreach (EnemyUI enemigo in enemigosUI.FindAll(e => e.IsAlive))
                {
                    countenmigos++;
                    enemigo.Intencion();
                }
                if (countenmigos == 0)// todos estan muertos
                {
                    win = true;
                    break;
                }
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
                yield return new WaitForSeconds(0.5f); // Simulación de tiempo de acción del enemigo
                isPlayerTurn = true;
            }
        }
        print("fin");

        if (GameManager.Instance.nivelAct % cadaXRondas == 0)
        {
            GameManager.Instance.nivelAct++;
            GameManager.Instance.LoadScene("ChouseMana");

        }
        else
        {
            GameManager.Instance.nivelAct++;
            GameManager.Instance.LoadScene("NextState");
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
                    bool muere = false;
                    if (enemigoSeleccionado != null)
                    {
                        muere = enemigoSeleccionado.Damagear(item.cantidad);
                    }
                    if (muere)
                    {
                        //enemigoSeleccionado = null;
                        navegation.performed += Navegation_performed;
                        GuirarRuleta.interactable = false;
                        seleccionBloqueada = false;
                    }
                    break;
                case PinchosType.Defense:
                    PJController.PJGetDefence(item.cantidad);
                    break;
                case PinchosType.Heal:
                    PJController.PJGetHeald(item.cantidad);
                    break;
            }
        }
        jugadorListo = true;

    }

    private void Navegation_performed(InputAction.CallbackContext context)
    {
        if (RuletaController.ruletaGirando) return;
        float y = context.ReadValue<Vector2>().y;
        float x = context.ReadValue<Vector2>().x;
        changeEnemySelection(y > 0 ? -1 : (y < 0 ? 1 : 0));
    }

    private void EnemyHovered_performed(EnemyUI enemy)
    {
        // Si hay un enemigo fijado, o la selección está bloqueada, el ratón no puede cambiarla
        if (RuletaController.ruletaGirando) return;
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
        if (RuletaController.ruletaGirando) return;
        ToggleFijarEnemigo();
    }

    private void EnemyClicked_performed(EnemyUI enemy)
    {
        if (RuletaController.ruletaGirando) return;
        if (enemy != enemigoSeleccionado) return;
        GameManager.Instance.PlaySFX(enemyConfirmed);
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
