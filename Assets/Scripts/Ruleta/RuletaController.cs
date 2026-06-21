using System;
using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class RuletaController : MonoBehaviour
{
    public GameObject ruleta;
    public GameObject slootTemplate;
    public GameObject pinchosTemplate;
    public GameObject huecoTemplate;

    [Header("Huecos")]
    public Transform huecosParent;
    public float huecoOffsetLocalY = 1f;

    [Header("Pinchos Mano")]
    public RectTransform pinchosParent;
    public Canvas canvas;
    public Vector2 primeraPosicionPincho = Vector2.zero;

    public float separacionEntrePinchos = 90f;

    public Transform zonaDeDescartes; 
    public Transform zonaDeRobos; 

    [Header("Animaciones Pinchos")]
    public float tiempoAnimacionDescarte = 0.35f;
    public float tiempoAnimacionRobo = 0.35f;
    public float offsetRotacionMovimiento = 90f;

    [Header("Trail Pinchos")]
    public bool activarTrailPinchos = true;
    public float intervaloTrail = 0.035f;
    public float duracionTrail = 0.18f;
    public float alphaTrailInicial = 0.28f;
    public float escalaTrail = 0.95f;

    private bool animandoMano = false;

    private RuletaScript ruletaScript;

    private GameObject[] sloots;
    private SlootScript[] slootsScripts;

    private GameObject[] huecos;
    private Hueco[] huecosScripts;

    private GameObject[] pinchos;
    private PinchosScript[] pinchosScripts;

    public static event Action<List<(int cantidad, PinchosType tipo)>> ResultRuleta;
    public static event Action<bool> ButtonPressed;

    private void Start()
    {
        ruletaScript = ruleta.GetComponent<RuletaScript>();

        createAllSloots();
        createAllHuecos();
        createAllPinchos();

        drawHand(); 
    }

    private void createAllSloots()
    {
        sloots = new GameObject[37];
        slootsScripts = new SlootScript[37];

        float gradosPorCasilla = 360f / 37f;

        for (int i = 0; i < 37; i++)
        {
            sloots[i] = Instantiate(slootTemplate, ruleta.transform);

            sloots[i].transform.localPosition = new Vector3(-0.00200000009f, 2.84699988f, 0f);
            sloots[i].transform.RotateAround(
                ruleta.transform.position,
                new Vector3(0f, 0f, 1f),
                -i * gradosPorCasilla
            );

            sloots[i].SetActive(true);

            slootsScripts[i] = sloots[i].GetComponent<SlootScript>();

            if (slootsScripts[i] != null)
            {
                slootsScripts[i].setValor(i);

                if (i == 0) slootsScripts[i].setColor(0);
                else if (i % 2 == 0) slootsScripts[i].setColor(1);
                else slootsScripts[i].setColor(2);

                slootsScripts[i].initSloot();
            }
        }
    }

    private void createAllHuecos()
    {
        huecos = new GameObject[37];
        huecosScripts = new Hueco[37];

        Transform parent = huecosParent != null ? huecosParent : transform;

        float gradosPorCasilla = 360f / 37f;

        for (int i = 0; i < 37; i++)
        {
            Transform slootTransform = sloots[i].transform;

            Vector3 huecoPosition = slootTransform.TransformPoint(
                new Vector3(0f, huecoOffsetLocalY, 0f)
            );

            Quaternion huecoRotation = slootTransform.rotation;

            huecos[i] = Instantiate(huecoTemplate, parent);
            huecos[i].SetActive(true);

            RectTransform huecoRect = huecos[i].GetComponent<RectTransform>();

            if (huecoRect != null)
            {
                huecoRect.position = huecoPosition;
                huecoRect.rotation = huecoRotation;
            }
            else
            {
                huecos[i].transform.position = huecoPosition;
                huecos[i].transform.rotation = huecoRotation;
            }

            huecosScripts[i] = huecos[i].GetComponent<Hueco>();

            if (huecosScripts[i] != null)
            {
                huecosScripts[i].Init(i, i * gradosPorCasilla);
            }
        }
    }

    private void createAllPinchos()
    {
        GameManager.Instance.createHand(); 
    

        int cantidad = GameManager.Instance.playerSO.baraja.Count;
        pinchos = new GameObject[cantidad];
        pinchosScripts = new PinchosScript[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            pinchos[i] = Instantiate(pinchosTemplate, pinchosParent);
            pinchos[i].name = "Pincho_" + i;
            pinchos[i].SetActive(false); 

            pinchos[i].GetComponent<UnityEngine.UI.Image>().sprite = GameManager.Instance.playerSO.baraja[i].sprite;  

            pinchosScripts[i] = pinchos[i].GetComponent<PinchosScript>();
            if (pinchosScripts[i] != null) pinchosScripts[i].pinchoSO = GameManager.Instance.playerSO.baraja[i];
        }
    }
    public void RuletaButtonPressed()
    {
        StartCoroutine(RuletaButtonPressed_WithBarrier());
        ButtonPressed?.Invoke(true);
    }

    private IEnumerator RuletaButtonPressed_WithBarrier()
    {
        ruletaScript.RotateRuleta();

        yield return new WaitUntil(() => ruletaScript.finishRotate);

        int temp = ruletaScript.casillaPrincipal();
        int valor = slootsScripts[temp].getValor();

        Debug.Log("Valor principal: " + valor);

        createResult();
    }

    private void createResult()
    {
        List<(int cantidad, PinchosType tipo)> resultados = new List<(int cantidad, PinchosType tipo)>();

        for (int i = 0; i < pinchosScripts.Length; i++)
        {
            PinchosScript pincho = pinchosScripts[i];

            if (pincho == null) continue;

            if (!pincho.EstaColocado())
            {
                Debug.Log("Pincho " + i + " no está colocado. Se ignora.");
                continue;
            }

            int suma = pincho.getSuma();
            int factorEscala = pincho.getFactorEscala();

            int casilla = ruletaScript.casillaWithRotation(pincho.getRotation());

            int valorBase = slootsScripts[casilla].getValor();

            int cantidad = (valorBase + suma) * factorEscala;

            PinchosType tipo = pincho.getType();

            resultados.Add((cantidad, tipo));

            Debug.Log(
                "Pincho " + i +
                " | hueco original = " + pincho.getCasilla() +
                " | casilla final = " + casilla +
                " | valorBase = " + valorBase +
                " | suma = " + suma +
                " | factor = " + factorEscala +
                " | cantidad = " + cantidad +
                " | tipo = " + tipo
            );
        }

        drawHand(); 
        ResultRuleta?.Invoke(resultados);
    }

    private void toDiscartActivePinchos()
    {
        
    }

   
    public void drawHand()
    {
        if (animandoMano) return;
        StartCoroutine(DrawHandAnimada());
    }

    private IEnumerator DrawHandAnimada()
    {
        animandoMano = true;

        List<int> disponibles = new List<int>(); 
        List<int> descartes = new List<int>();
        List<int> mano = new List<int>();

        for (int i = 0; i < pinchosScripts.Length; i++)
        {
            if (pinchosScripts[i].pinchoSO.state == Estado.Disponible_para_robar)
                disponibles.Add(i); 

            if (pinchosScripts[i].pinchoSO.state == Estado.Descarte)
                descartes.Add(i); 

            if (pinchosScripts[i].pinchoSO.state == Estado.Mano)
                mano.Add(i); 
        }

        // Guardamos la mano anterior para animarla al descarte
        List<int> manoAnterior = new List<int>(mano);

        for (int i = mano.Count - 1; i >= 0; i--)
        {
            int id = mano[i];
            pinchosScripts[id].pinchoSO.state = Estado.Descarte;
            descartes.Add(id);

            mano.RemoveAt(i);
        }

        // Animación de descarte. Aquí espera a que TODOS lleguen.
        yield return StartCoroutine(AnimarPinchosADescarte(manoAnterior));

        int handSize = GameManager.Instance.playerSO.handSize;

        if (disponibles.Count < handSize)
        {
            int resto = handSize - disponibles.Count;

            // Robar todos los disponibles
            while (disponibles.Count > 0)
            {
                RobarUno(disponibles, mano);
            }

            // Pasar descartes a disponibles
            for (int i = descartes.Count - 1; i >= 0; i--)
            {
                int id = descartes[i];

                disponibles.Add(id);
                pinchosScripts[id].pinchoSO.state = Estado.Disponible_para_robar;

                descartes.RemoveAt(i);
            }

            // Robar lo que falta
            int cantidadARobar = Mathf.Min(resto, disponibles.Count);

            for (int i = 0; i < cantidadARobar; i++)
            {
                RobarUno(disponibles, mano);
            }
        }

        else for (int i = 0; i < handSize; i++) RobarUno(disponibles, mano);

        // Animación de robo. Aquí salen desde ZonaDeRobos hacia su posición de mano.
        yield return StartCoroutine(AnimarPinchosARobo(mano));

        animandoMano = false;
    }

    private void RobarUno(List<int> disponibles, List<int> mano)
    {
        int j = UnityEngine.Random.Range(0, disponibles.Count);
        int id = disponibles[j];

        pinchosScripts[id].pinchoSO.state = Estado.Mano;
        mano.Add(id);

        disponibles.RemoveAt(j);
    }

    private void deactivatePincho(int id_pincho)
    {
        DragDrop dragDrop = pinchos[id_pincho].GetComponent<DragDrop>();
        if (dragDrop != null) dragDrop.VolverAPosicionOriginal(); 

        pinchos[id_pincho].SetActive(false);
    }

    private void activatePincho(int id_pincho, float posicion_relativa) 
    {
            RectTransform rect = pinchos[id_pincho].GetComponent<RectTransform>();
            if (rect != null) 
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                rect.anchoredPosition = new Vector2(0f, posicion_relativa);

                rect.localRotation = Quaternion.AngleAxis(90, new Vector3(0f,0f,1f));
            }
            DragDrop dragDrop = pinchos[id_pincho].GetComponent<DragDrop>();
            if (dragDrop != null) dragDrop.InitDragDrop(canvas, ruleta.transform);
            pinchos[id_pincho].transform.SetParent(pinchosParent); 
            pinchos[id_pincho].SetActive(true); 
    }

    private IEnumerator AnimarPinchosADescarte(List<int> ids)
    {
        if (ids == null || ids.Count == 0) yield break;

        if (zonaDeDescartes == null)
        {
            Debug.LogError("Falta asignar ZonaDeDescartes en el inspector.");
            yield break;
        }

        float duracion = Mathf.Max(0.01f, tiempoAnimacionDescarte);

        List<RectTransform> rects = new List<RectTransform>();
        List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
        List<Vector3> posicionesIniciales = new List<Vector3>();

        Vector3 posicionFinal = zonaDeDescartes.position;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];

            GameObject pincho = pinchos[id];
            pincho.SetActive(true);

            DragDrop dragDrop = pincho.GetComponent<DragDrop>();
            if (dragDrop != null)
            {
                dragDrop.LiberarHuecoYResetearSinMover();
            }

            RectTransform rect = pincho.GetComponent<RectTransform>();

            if (rect == null) continue;

            if (canvas != null)
            {
                rect.SetParent(canvas.transform, true);
            }

            CanvasGroup canvasGroup = GetOrAddCanvasGroup(pincho);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;

            rects.Add(rect);
            canvasGroups.Add(canvasGroup);
            posicionesIniciales.Add(rect.position);
        }

        float tiempo = 0f;
        float tiempoTrail = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            float suavizado = t * t * (3f - 2f * t);

            for (int i = 0; i < rects.Count; i++)
            {
                Vector3 nuevaPosicion = Vector3.Lerp(
                    posicionesIniciales[i],
                    posicionFinal,
                    suavizado
                );

                rects[i].position = nuevaPosicion;

                RotarHaciaMovimientoWorld(rects[i], posicionFinal - nuevaPosicion);

                canvasGroups[i].alpha = Mathf.Lerp(1f, 0f, suavizado);
            }

            tiempoTrail += Time.deltaTime;

            if (tiempoTrail >= intervaloTrail)
            {
                tiempoTrail = 0f;

                for (int i = 0; i < rects.Count; i++)
                {
                    CrearTrail(rects[i]);
                }
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < rects.Count; i++)
        {
            rects[i].position = posicionFinal;
            canvasGroups[i].alpha = 1f;
            canvasGroups[i].blocksRaycasts = true;

            rects[i].SetParent(pinchosParent, false);

            GameObject pincho = rects[i].gameObject;
            pincho.SetActive(false);
        }
    }
    private IEnumerator AnimarPinchosARobo(List<int> mano)
    {
        if (mano == null || mano.Count == 0) yield break;

        if (zonaDeRobos == null)
        {
            Debug.LogError("Falta asignar ZonaDeRobos en el inspector.");
            yield break;
        }

        float duracion = Mathf.Max(0.01f, tiempoAnimacionRobo);

        List<RectTransform> rects = new List<RectTransform>();
        List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
        List<Vector2> posicionesIniciales = new List<Vector2>();
        List<Vector2> posicionesFinales = new List<Vector2>();

        Vector2 posicionRoboLocal = pinchosParent.InverseTransformPoint(zonaDeRobos.position);

        for (int i = 0; i < mano.Count; i++)
        {
            int id = mano[i];

            GameObject pincho = pinchos[id];
            pincho.SetActive(true);

            RectTransform rect = pincho.GetComponent<RectTransform>();

            if (rect == null) continue;

            rect.SetParent(pinchosParent, false);

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Vector2 posicionFinal = new Vector2(0f, separacionEntrePinchos * i);

            rect.anchoredPosition = posicionRoboLocal;

            CanvasGroup canvasGroup = GetOrAddCanvasGroup(pincho);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;

            rects.Add(rect);
            canvasGroups.Add(canvasGroup);
            posicionesIniciales.Add(posicionRoboLocal);
            posicionesFinales.Add(posicionFinal);
        }

        float tiempo = 0f;
        float tiempoTrail = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            float suavizado = t * t * (3f - 2f * t);

            for (int i = 0; i < rects.Count; i++)
            {
                Vector2 nuevaPosicion = Vector2.Lerp(
                    posicionesIniciales[i],
                    posicionesFinales[i],
                    suavizado
                );

                rects[i].anchoredPosition = nuevaPosicion;

                RotarHaciaMovimientoLocal(rects[i], posicionesFinales[i] - nuevaPosicion);
            }

            tiempoTrail += Time.deltaTime;

            if (tiempoTrail >= intervaloTrail)
            {
                tiempoTrail = 0f;

                for (int i = 0; i < rects.Count; i++)
                {
                    CrearTrail(rects[i]);
                }
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < rects.Count; i++)
        {
            rects[i].anchoredPosition = posicionesFinales[i];

            // Rotación final normal de la mano.
            rects[i].localRotation = Quaternion.AngleAxis(90f, Vector3.forward);

            canvasGroups[i].alpha = 1f;
            canvasGroups[i].blocksRaycasts = true;

            DragDrop dragDrop = rects[i].GetComponent<DragDrop>();

            if (dragDrop != null)
            {
                dragDrop.InitDragDrop(canvas, ruleta.transform);
            }
        }
    }
    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        return canvasGroup;
    }

    private void RotarHaciaMovimientoWorld(RectTransform rect, Vector3 direccion)
    {
        if (direccion.sqrMagnitude < 0.001f) return;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        rect.rotation = Quaternion.Euler(0f, 0f, angulo + offsetRotacionMovimiento);
    }

    private void RotarHaciaMovimientoLocal(RectTransform rect, Vector2 direccion)
    {
        if (direccion.sqrMagnitude < 0.001f) return;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        rect.localRotation = Quaternion.Euler(0f, 0f, angulo + offsetRotacionMovimiento);
    }

    private void CrearTrail(RectTransform original)
    {
        if (!activarTrailPinchos || original == null) return;

        UnityEngine.UI.Image imageOriginal = original.GetComponent<UnityEngine.UI.Image>();
        if (imageOriginal == null || imageOriginal.sprite == null) return;

        GameObject ghost = new GameObject("Trail_" + original.name);

        RectTransform ghostRect = ghost.AddComponent<RectTransform>();
        UnityEngine.UI.Image ghostImage = ghost.AddComponent<UnityEngine.UI.Image>();
        CanvasGroup ghostCanvasGroup = ghost.AddComponent<CanvasGroup>();

        ghost.transform.SetParent(original.parent, false);

        ghostRect.anchorMin = original.anchorMin;
        ghostRect.anchorMax = original.anchorMax;
        ghostRect.pivot = original.pivot;
        ghostRect.sizeDelta = original.sizeDelta;
        ghostRect.anchoredPosition = original.anchoredPosition;
        ghostRect.position = original.position;
        ghostRect.rotation = original.rotation;
        ghostRect.localScale = original.localScale * escalaTrail;

        ghostImage.sprite = imageOriginal.sprite;
        ghostImage.color = imageOriginal.color;
        ghostImage.raycastTarget = false;
        ghostImage.preserveAspect = imageOriginal.preserveAspect;

        ghostCanvasGroup.alpha = alphaTrailInicial;
        ghostCanvasGroup.blocksRaycasts = false;
        ghostCanvasGroup.interactable = false;

        ghost.transform.SetSiblingIndex(Mathf.Max(0, original.GetSiblingIndex()));

        StartCoroutine(FadeOutTrail(ghostCanvasGroup, ghost));
    }

    private IEnumerator FadeOutTrail(CanvasGroup canvasGroup, GameObject ghost)
    {
        float tiempo = 0f;

        while (tiempo < duracionTrail)
        {
            float t = tiempo / duracionTrail;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(alphaTrailInicial, 0f, t);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (ghost != null)
        {
            Destroy(ghost);
        }
    }

}



