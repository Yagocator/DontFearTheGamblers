using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("Datos del enemigo")]
    public TextMeshProUGUI nameEnemy;
    public TextMeshProUGUI vidaEnemy;
    public EnemySO enemySO;
    public TextMeshProUGUI ShildEnemy;
    public Image ImageEnemy;
    public GameObject Padre;

    [Header("Animación de selección")]
    public Image panelHighlight;
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(1f, 0.75f, 0.1f); 
    public Vector3 escalaNormal = Vector3.one;
    public Vector3 escalaSeleccionado = Vector3.one * 1.08f;
    public float duracionAnim = 0.18f;

    private Coroutine animCoroutine;
    private bool estaSeleccionado;
    public bool EstaSeleccionado => estaSeleccionado;

    void Start()
    {
        nameEnemy.text = enemySO.name;
        vidaEnemy.text = enemySO.vida.ToString();
        ShildEnemy.text = enemySO.currentShild.ToString();
        ImageEnemy.sprite = enemySO.spriteEnemy;

        // fallback por si no asignas el panel manualmente
        if (panelHighlight == null)
            panelHighlight = Padre.GetComponentInChildren<Image>();

        // estado inicial limpio
        Padre.transform.localScale = escalaNormal;
        if (panelHighlight != null)
            panelHighlight.color = colorNormal;
    }

    public void SelectedEnemy()
    {
        if (estaSeleccionado) return;
        estaSeleccionado = true;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarSeleccion(true));
    }

    public void DeselectEnemy()
    {
        if (!estaSeleccionado) return;
        estaSeleccionado = false;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarSeleccion(false));
    }

    private IEnumerator AnimarSeleccion(bool seleccionar)
    {
        Vector3 escalaInicio = Padre.transform.localScale;
        Vector3 escalaFin = seleccionar ? escalaSeleccionado : escalaNormal;

        Color colorInicio = panelHighlight != null ? panelHighlight.color : Color.white;
        Color colorFin = seleccionar ? colorSeleccionado : colorNormal;

        float t = 0f;
        while (t < duracionAnim)
        {
            t += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(t / duracionAnim);
            float curva = Mathf.SmoothStep(0f, 1f, progreso); // easing suave

            Padre.transform.localScale = Vector3.Lerp(escalaInicio, escalaFin, curva);

            if (panelHighlight != null)
                panelHighlight.color = Color.Lerp(colorInicio, colorFin, curva);

            yield return null;
        }

        Padre.transform.localScale = escalaFin;
        if (panelHighlight != null)
            panelHighlight.color = colorFin;

        animCoroutine = null;
    }
}