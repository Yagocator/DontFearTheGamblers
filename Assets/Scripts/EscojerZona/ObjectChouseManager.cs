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
    public Sprite[] marcosRareza;// c 0,u 1,r 2,e 3,l 4,lrit 5
    public Image MarcoIzquierda;
    public Image MarcoDerecha;
    public Image fotoIzquierda;
    public Image fotoiDerecha;
    public TextMeshProUGUI TextoNombreIzquierda;
    public TextMeshProUGUI TextoNombreDerecha;
    public TextMeshProUGUI TextoBuffIzquierda;
    public TextMeshProUGUI TextoBuffDerecha;
    public TipoOnj TipoEscena;
    public List<ObjetoSo> ObjDisponibles;
    public List<PinchoSO> PosiblesPinchos;
    [Header("Probabilidades por rareza (Common, Uncommon, Rare, Epic, Legendary, LetItRide)")]
    public float[] probabilidadesRareza = new float[6] { 50.3f, 25f, 12f, 8f, 3f, 1.7f };
    private ObjetoSo[] ObjSelected = new ObjetoSo[2];
    private PinchoSO[] PinchoSelected = new PinchoSO[2];

    public Button botonIzquierda;
    public Button botonDerecha;

    public AudioClip selectOptions;


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
                        indexEnemySO = ElegirIndicePorRareza(ObjDisponibles);
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
                    AsignarMarcoPorRareza(obj.rareza, MarcoIzquierda);
                }
                else
                {
                    fotoiDerecha.sprite = obj.sprite;
                    TextoNombreDerecha.text = obj.nombre;
                    TextoBuffDerecha.text = obj.desc;
                    botonDerecha.onClick.AddListener(() => PillarObj(obj, true));
                    AsignarMarcoPorRareza(obj.rareza, MarcoDerecha);
                }
            }
        }

    }
    public void PillarObj(ObjetoSo obj, bool lado)// izquierda 0 derecha 1
    {
        GameManager.Instance.PlaySFX(selectOptions);
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
        GameManager.Instance.LoadScene("CombateTurnos");
    }
    public void PillarPincho(PinchoSO pincho, bool lado)
    {
        GameManager.Instance.PlaySFX(selectOptions);
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
        GameManager.Instance.LoadScene("CombateTurnos");
    }

    private int ElegirIndicePorRareza(List<ObjetoSo> lista)
    {
        var pesos = new List<float>(probabilidadesRareza.Length);
        for (int i = 0; i < lista.Count; i++)
        {
            var rareza = lista[i].rareza;
            pesos.Add(GetPesoPorRareza(rareza));
        }

        float total = 0f;
        foreach (var peso in pesos)
            total += peso;

        if (total <= 0f)
            return UnityEngine.Random.Range(0, lista.Count);

        float valor = UnityEngine.Random.Range(0f, total);
        float acumulado = 0f;

        for (int i = 0; i < lista.Count; i++)
        {
            acumulado += pesos[i];
            if (valor <= acumulado)
                return i;
        }

        return lista.Count - 1;
    }

    private float GetPesoPorRareza(Rareza rareza)
    {
        switch (rareza)
        {
            case Rareza.Common: return probabilidadesRareza.Length > 0 ? probabilidadesRareza[0] : 50f;
            case Rareza.Uncommon: return probabilidadesRareza.Length > 1 ? probabilidadesRareza[1] : 25f;
            case Rareza.Rare: return probabilidadesRareza.Length > 2 ? probabilidadesRareza[2] : 12f;
            case Rareza.Epic: return probabilidadesRareza.Length > 3 ? probabilidadesRareza[3] : 8f;
            case Rareza.Legendary: return probabilidadesRareza.Length > 4 ? probabilidadesRareza[4] : 3f;
            case Rareza.LetItRide: return probabilidadesRareza.Length > 5 ? probabilidadesRareza[5] : 2f;
            default: return 1f;
        }
    }

    private void AsignarMarcoPorRareza(Rareza rareza, Image marco)
    {
        int index = Mathf.Clamp((int)rareza, 0, marcosRareza.Length - 1);
        if (marcosRareza != null && index < marcosRareza.Length)
            marco.sprite = marcosRareza[index];
    }

    private void desbleButtons()
    {
        botonIzquierda.interactable = false;
        botonDerecha.interactable = false;
    }

}
