using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventarioDisplay : MonoBehaviour
{
    public GameObject Padre;
    public GameObject PrefabIstanciar;
    private List<PinchoSO> baraja;

    private List<Button> bottones = new List<Button>();

    void OnEnable()
    {
        RomperPincho.EliminadoPincho += Disblebuttons;
    }


    void Start()
    {
        intitPinchosInv();
    }
    public void intitPinchosInv()
    {

        baraja = GameManager.Instance.playerSO.baraja;

        foreach (PinchoSO Pinch in baraja)
        {
            GameObject pincho = Instantiate(PrefabIstanciar, Padre.transform);
            pincho.GetComponent<PinchosScript>().pinchoSO = Pinch;
            pincho.GetComponent<RomperPincho>().PinchoSO = Pinch;
            pincho.GetComponent<RomperPincho>().init();
            pincho.GetComponent<UnityEngine.UI.Image>().sprite = Pinch.sprite;
            bottones.Add(pincho.GetComponent<Button>());
        }

    }

    private void Disblebuttons(bool bol)
    {
        foreach (Button item in bottones)
        {
            item.interactable = false;
        }
    }

}
