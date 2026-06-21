using System;
using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Rendering;
using System.Linq;
using UnityEngine.XR;
using UnityEditor;

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

        // Contiene en id del pinchosScript que cumple que esta ahi 
        List<int> disponibles = new List<int>(); 
        List<int> descartes = new List<int>();
        List<int> mano = new List<int>();


        for (int i = 0; i < pinchosScripts.Length; i++)
        {
            if (pinchosScripts[i].pinchoSO.state == Estado.Disponible_para_robar) disponibles.Add(i); 
            if (pinchosScripts[i].pinchoSO.state == Estado.Descarte) descartes.Add(i); 
            if (pinchosScripts[i].pinchoSO.state == Estado.Mano) mano.Add(i); 
        }   

        ////////////////////////////////////////////

        // Pinchos a descarte previos 
        for (int i = mano.Count - 1; i >= 0; i--)
        {
            pinchosScripts[mano[i]].pinchoSO.state = Estado.Descarte; 
            descartes.Add(mano[i]); 
            deactivatePincho(mano[i]); 
            
            mano.RemoveAt(i);             
        } 

        ////////////////////////////////////////////
        

        // Pinchos de baraja a mano  (Si baraja insuficiente entonces robas todos + descarte a baraja y ejecutas robar de baraja
        int handSize = GameManager.Instance.playerSO.handSize; 
        if (disponibles.Count < handSize)
        {
            // Robar todos disponibles 
            int resto = handSize - disponibles.Count; 
            for (int i = disponibles.Count - 1; i >= 0; i--)
            {
                int j = UnityEngine.Random.Range(0, disponibles.Count); 
                pinchosScripts[disponibles[j]].pinchoSO.state = Estado.Mano; 
                mano.Add(disponibles[j]);  

                disponibles.RemoveAt(j); 
            }

            // Añadir descartes a la pila de robo
            for (int i = descartes.Count - 1; i >= 0; i--)
            {
                disponibles.Add(descartes[i]); 
                pinchosScripts[descartes[i]].pinchoSO.state = Estado.Disponible_para_robar; 
                
                descartes.RemoveAt(i); 
            }

            // Robar aleatorio  
            int cantidadARobar = Mathf.Min(resto, disponibles.Count); 
            for (int i = 0; i < cantidadARobar; i++)
            {
                int j = UnityEngine.Random.Range(0, disponibles.Count); 
                pinchosScripts[disponibles[j]].pinchoSO.state = Estado.Mano; 
                mano.Add(disponibles[j]); 

                disponibles.RemoveAt(j); 
            }
        }
        else
        {
            // Robar aleatorio  
            for (int i = 0; i < handSize; i++)
            {
                int j = UnityEngine.Random.Range(0, disponibles.Count); 
                pinchosScripts[disponibles[j]].pinchoSO.state = Estado.Mano; 
                mano.Add(disponibles[j]); 

                disponibles.RemoveAt(j); 
            }
        } 



        ////////////////////////////////////////////
        // Crear pinchos visuales   
        for (int i = 0; i < mano.Count; i++) activatePincho(mano[i], separacionEntrePinchos * i); 
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
}



