using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InventarioDisplayCombat : MonoBehaviour
{
    public GameObject PadreRobar;
    public GameObject PadreDescarte;
    public GameObject PrefabIstanciar;
    private List<PinchoSO> baraja;


    void Start()
    {
        intitPinchosInv();
    }
    public void intitPinchosInv()
    {

        baraja = GameManager.Instance.playerSO.baraja;

        foreach (PinchoSO Pinch in baraja)
        {
            GameObject pincho = null;
            if (Pinch.state == Estado.Disponible_para_robar)
            {
                pincho = Instantiate(PrefabIstanciar, PadreRobar.transform);
            }
            else if (Pinch.state == Estado.Mano) continue; 
            else 
            {
                pincho = Instantiate(PrefabIstanciar, PadreDescarte.transform);
            }
            pincho.GetComponent<PinchosScript>().pinchoSO = Pinch;
            pincho.GetComponent<RomperPincho>().PinchoSO = Pinch;
            pincho.GetComponent<UnityEngine.UI.Image>().sprite = Pinch.sprite;
        }

    }

}
