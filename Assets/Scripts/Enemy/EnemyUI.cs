using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum AccionEnemy
{
    ATACAR, DEFENDER, CURAR
}

public class EnemyUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public static event Action<EnemyUI> EnemyHovered;
    public static event Action<EnemyUI> EnemyClicked;

    [Header("Datos del enemigo")]
    [SerializeField] private TextMeshProUGUI nameEnemy;
    [SerializeField] private TextMeshProUGUI vidaEnemy;
    private EnemySO enemySO;
    [SerializeField] private TextMeshProUGUI ShildEnemy;
    [SerializeField] private Image ImageEnemy;
    [SerializeField] private GameObject Padre;
    [SerializeField] private ParticleSystem Sangre;
    [SerializeField] private ParticleSystem Escudo;
    [SerializeField] private Image IntencionSprite;
    [SerializeField] private TextMeshProUGUI IntencionText;
    [SerializeField] private Sprite[] IntencionSprites;

    [Header("Animación de selección")]
    public Image panelHighlight;
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(1f, 0.75f, 0.1f);
    public Vector3 escalaNormal = Vector3.one;
    public Vector3 escalaSeleccionado = Vector3.one * 1.08f;
    public float duracionAnim = 0.18f;

    private Coroutine animCoroutine; // mutex: lo comparten selección, fijado, daño y resaltado de turno
    private bool estaSeleccionado;
    public bool EstaSeleccionado => estaSeleccionado;

    [Header("Fijado")]
    public Color colorFijado = Color.red;
    public bool Fijado = false;
    public bool IsAlive = true;

    [Header("Animación de daño")]
    public Color colorDamage = Color.white;
    public float duracionDamage = 0.08f;
    public float shakeFuerza = 12f;
    public float shakeDuracion = 0.2f;

    [Header("Resaltado de turno (ejecutando acción)")]
    public Color colorAccion = Color.green;
    public float duracionResaltadoAccion = 0.15f;
    public float tiempoVisibleAccion = 0.5f; // cuánto se queda en verde antes de volver a su color normal

    private RectTransform padreRect;
    private Coroutine shakeCoroutine;

    private AccionEnemy accionEnemy;
    private int potencia;

    void Start()
    {
        if (panelHighlight == null)
            panelHighlight = Padre.GetComponentInChildren<Image>();

        padreRect = Padre.GetComponent<RectTransform>();

        Padre.transform.localScale = escalaNormal;
        if (panelHighlight != null)
            panelHighlight.color = colorNormal;
    }

    public void InitializeEnemy(EnemySO enemy)
    {
        enemySO = enemy;
        nameEnemy.text = enemySO.name;
        vidaEnemy.text = enemySO.vida.ToString();
        ShildEnemy.text = enemySO.currentShild.ToString();
        ImageEnemy.sprite = enemySO.spriteEnemy;
    }

    private void UpdateUI()
    {
        vidaEnemy.text = enemySO.vida.ToString();
        ShildEnemy.text = enemySO.currentShild.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsAlive) return;
        EnemyHovered?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsAlive) return;
        EnemyClicked?.Invoke(this);
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
            if (Fijado)
            {
                animCoroutine = null;
                yield break;
            }

            t += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(t / duracionAnim);
            float curva = Mathf.SmoothStep(0f, 1f, progreso);

            Padre.transform.localScale = Vector3.Lerp(escalaInicio, escalaFin, curva);

            if (panelHighlight != null)
                panelHighlight.color = Color.Lerp(colorInicio, colorFin, curva);

            yield return null;
        }

        Padre.transform.localScale = escalaFin;

        if (panelHighlight != null)
            panelHighlight.color = Fijado ? colorFijado : colorFin;

        animCoroutine = null;
    }

    public void Fijar(bool fijar)
    {
        Fijado = fijar;

        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }

        if (panelHighlight != null)
            panelHighlight.color = ColorObjetivo();
    }

    private Color ColorObjetivo()
    {
        if (Fijado) return colorFijado;
        if (estaSeleccionado) return colorSeleccionado;
        return colorNormal;
    }


    private void RecibirDamage()
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarDamage());

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(AnimarShake());
    }

    private IEnumerator AnimarDamage()
    {
        Color colorInicio = panelHighlight != null ? panelHighlight.color : Color.white;
        Color colorBase = ColorObjetivo();

        float t = 0f;
        while (t < duracionDamage)
        {
            t += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(t / duracionDamage);
            if (panelHighlight != null)
                panelHighlight.color = Color.Lerp(colorInicio, colorDamage, progreso);
            yield return null;
        }

        t = 0f;
        while (t < duracionDamage)
        {
            t += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(t / duracionDamage);
            if (panelHighlight != null)
                panelHighlight.color = Color.Lerp(colorDamage, colorBase, progreso);
            yield return null;
        }

        if (panelHighlight != null)
            panelHighlight.color = colorBase;

        animCoroutine = null;
    }

    private IEnumerator AnimarShake()
    {
        Vector2 posOriginal = padreRect.anchoredPosition;
        float t = 0f;

        while (t < shakeDuracion)
        {
            t += Time.unscaledDeltaTime;
            float fuerzaActual = shakeFuerza * (1f - t / shakeDuracion);
            Vector2 offset = UnityEngine.Random.insideUnitCircle * fuerzaActual;
            padreRect.anchoredPosition = posOriginal + offset;
            yield return null;
        }

        padreRect.anchoredPosition = posOriginal;
        shakeCoroutine = null;
    }

    public bool Damagear(int Damage)
    {
        int dañoRestante = Damage;

        RecibirDamage();

        if (enemySO.currentShild > 0)
        {
            int dañoAlEscudo = Mathf.Min(enemySO.currentShild, dañoRestante);
            enemySO.currentShild -= dañoAlEscudo;
            dañoRestante -= dañoAlEscudo;
            Escudo.Play();
        }

        if (dañoRestante > 0)
        {
            enemySO.vida -= dañoRestante;
            if (enemySO.vida <= 0)
            {
                enemySO.vida = 0;
                Morir();
                return true;
            }
            Sangre.Play();
        }
        UpdateUI();
        return false;
    }

    private void Morir()
    {
        IsAlive = false;

        if (shakeCoroutine != null) { StopCoroutine(shakeCoroutine); shakeCoroutine = null; }

        ImageEnemy.sprite = enemySO.spriteMuerto;
        DeselectEnemy();

        if (animCoroutine != null) { StopCoroutine(animCoroutine); animCoroutine = null; }

        panelHighlight.color = Color.black;
        Fijado = false;
        UpdateUI();
    }

    public Coroutine EjecutarAccionResaltada()
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(EjecutarAccionRoutine());
        return animCoroutine;
    }

    private IEnumerator EjecutarAccionRoutine()
    {
        yield return AnimarColorResaltado(colorAccion, duracionResaltadoAccion);

        EjecutarAccion();

        yield return new WaitForSeconds(tiempoVisibleAccion);

        // si ha muerto en su propia acción (poco probable) o ha sido fijado/seleccionado entretanto, respeta eso
        if (IsAlive)
            yield return AnimarColorResaltado(ColorObjetivo(), duracionResaltadoAccion);

        animCoroutine = null;
    }

    private IEnumerator AnimarColorResaltado(Color colorFin, float duracion)
    {
        if (panelHighlight == null) yield break;

        Color colorInicio = panelHighlight.color;
        float t = 0f;

        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(t / duracion);
            panelHighlight.color = Color.Lerp(colorInicio, colorFin, progreso);
            yield return null;
        }

        panelHighlight.color = colorFin;
    }


    public void Intencion()
    {
        int numRandom = UnityEngine.Random.Range(0, 3);
        if (numRandom == 0)
        {
            IntencionSprite.sprite = IntencionSprites[0];
            accionEnemy = AccionEnemy.ATACAR;
        }
        else if (numRandom == 1)
        {
            IntencionSprite.sprite = IntencionSprites[1];
            accionEnemy = AccionEnemy.DEFENDER;
        }
        else
        {
            IntencionSprite.sprite = IntencionSprites[2];
            accionEnemy = AccionEnemy.DEFENDER;
        }

        int val = UnityEngine.Random.Range(enemySO.dmg, enemySO.dmg * 3);
        potencia = val;
        IntencionText.text = val.ToString();
    }

    private void Curar(int val)
    {
        enemySO.vida += val;
        if (enemySO.vida >= enemySO.vidaMax) enemySO.vida = enemySO.vidaMax;
        UpdateUI();
    }

    private void escudo(int cuant)
    {
        enemySO.currentShild += cuant;
        UpdateUI();
    }

    private void Atacar(int val)
    {
        GameManager.Instance.AtacarPJ(val);
    }

    public void EjecutarAccion()
    {
        switch (accionEnemy)
        {
            case AccionEnemy.ATACAR:
                Atacar(potencia);
                break;
            case AccionEnemy.DEFENDER:
                escudo(potencia);
                break;
            case AccionEnemy.CURAR:
                Curar(potencia);
                break;
        }
    }
}
