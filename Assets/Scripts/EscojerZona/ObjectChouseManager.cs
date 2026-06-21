using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TipoOnj
{
    BAR, MAFIA
}
public class ObjectChouseManager : MonoBehaviour
{

    public GameObject[] botones;
    public Image fotoIzquierda;
    public Image fotoiDerecha;
    public TextMeshProUGUI TextoNombreIzquierda;
    public TextMeshProUGUI TextoNombreDerecha;
    public TextMeshProUGUI TextoBuffIzquierda;
    public TextMeshProUGUI TextoBuffDerecha;
    public TipoOnj TipoEscena;
    public List<ObjetoSo> ObjDisponibles;
    public List<PinchoSO> PosiblesPinchos;
    private ObjetoSo[] ObjSelected = new ObjetoSo[2];
    private PinchoSO[] PinchoSelected = new PinchoSO[2];

    public Button botonIzquierda;
    public Button botonDerecha;


    void Start()
    {

        if (TipoEscena == TipoOnj.BAR)
        {// pinchos
            for (int i = 0; i < 2; i++)
            {
                int indexEnemySO;
                if (PosiblesPinchos.Count <= 1)
                {
                    indexEnemySO = UnityEngine.Random.Range(0, PosiblesPinchos.Count);
                }
                else
                {
                    do
                    {
                        indexEnemySO = UnityEngine.Random.Range(0, PosiblesPinchos.Count);
                    } while (i == 1 && PosiblesPinchos[indexEnemySO] == PinchoSelected[0]);
                }
                PinchoSelected[i] = PosiblesPinchos[indexEnemySO];
                var pin = PinchoSelected[i];
                if (i == 0)
                {
                    fotoIzquierda.sprite = pin.sprite;
                    TextoNombreIzquierda.text = pin.nombre;
                    TextoBuffIzquierda.text = pin.desc;
                    botonIzquierda.onClick.AddListener(() => PillarPincho(pin, false));

                }
                else
                {
                    fotoiDerecha.sprite = pin.sprite;
                    TextoNombreDerecha.text = pin.nombre;
                    TextoBuffDerecha.text = pin.desc;
                    botonDerecha.onClick.AddListener(() => PillarPincho(pin, true));
                }
            }
        }
        else
        {// objetos
            for (int i = 0; i < 2; i++)
            {
                int indexEnemySO;
                if (ObjDisponibles.Count <= 1)
                {
                    indexEnemySO = UnityEngine.Random.Range(0, ObjDisponibles.Count);
                }
                else
                {
                    do
                    {
                        indexEnemySO = UnityEngine.Random.Range(0, ObjDisponibles.Count);
                    } while (i == 1 && ObjDisponibles[indexEnemySO] == ObjSelected[0]);
                }
                ObjSelected[i] = ObjDisponibles[indexEnemySO];
                var obj = ObjSelected[i];
                if (i == 0)
                {
                    fotoIzquierda.sprite = obj.sprite;
                    TextoNombreIzquierda.text = obj.nombre;
                    TextoBuffIzquierda.text = obj.desc;
                    botonIzquierda.onClick.AddListener(() => PillarObj(obj, false));

                }
                else
                {
                    fotoiDerecha.sprite = obj.sprite;
                    TextoNombreDerecha.text = obj.nombre;
                    TextoBuffDerecha.text = obj.desc;
                    botonDerecha.onClick.AddListener(() => PillarObj(obj, true));
                }
            }
        }

    }
    public void PillarObj(ObjetoSo obj, bool lado)// izquierda 0 derecha 1
    {
        desbleButtons();
        GameManager.Instance.playerSO.objetos.Add(obj);
        if (lado)
        {
            Destroy(botonDerecha.gameObject);
        }
        else
        {
            Destroy(botonIzquierda.gameObject);
        }
    }
    public void PillarPincho(PinchoSO pincho, bool lado)
    {
        desbleButtons();
        var copia = UnityEngine.Object.Instantiate(pincho);
        GameManager.Instance.playerSO.baraja.Add(copia);
        if (lado)
        {
            Destroy(botonDerecha.gameObject);
        }
        else
        {
            Destroy(botonIzquierda.gameObject);
        }
    }

    private void desbleButtons()
    {
        botonIzquierda.interactable = false;
        botonDerecha.interactable = false;
    }


    
}
