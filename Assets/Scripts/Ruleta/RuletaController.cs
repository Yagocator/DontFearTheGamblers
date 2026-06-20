using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public int pinchosIniciales = 5;
    public int maxPinchos = 8;

    // Déjalo en 0,0 si quieres que el primer pincho aparezca EXACTAMENTE
    // en el centro del objeto Pinchos Parent.
    public Vector2 primeraPosicionPincho = Vector2.zero;

    // Separación hacia ARRIBA entre pinchos.
    public float separacionEntrePinchos = 90f;

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
        int cantidad = Mathf.Clamp(pinchosIniciales, 0, maxPinchos);

        pinchos = new GameObject[cantidad];
        pinchosScripts = new PinchosScript[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            pinchos[i] = Instantiate(pinchosTemplate, pinchosParent);
            pinchos[i].name = "Pincho_" + i;
            pinchos[i].SetActive(true);

            RectTransform rect = pinchos[i].GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                rect.anchoredPosition = new Vector2(0f, separacionEntrePinchos * i);

                rect.localRotation = Quaternion.AngleAxis(90, new Vector3(0f,0f,1f));
            }

            pinchosScripts[i] = pinchos[i].GetComponent<PinchosScript>();

            if (pinchosScripts[i] != null)
            {
                int tipo = i % 3;
                int suma = i + 1;
                int factorEscala = 1;

                pinchosScripts[i].init(tipo, suma, factorEscala);
                pinchosScripts[i].ResetCasilla();
            }

            DragDrop dragDrop = pinchos[i].GetComponent<DragDrop>();

            if (dragDrop != null)
            {
                dragDrop.InitDragDrop(canvas, ruleta.transform);
            }
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

        ResultRuleta?.Invoke(resultados);
    }
}