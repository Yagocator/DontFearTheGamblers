using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InventarioDisplayCombat : MonoBehaviour
{
    public GameObject PadreRobar;
    public GameObject PadreDescarte;
    public GameObject PrefabIstanciar;
    private List<PinchoSO> baraja;
    private List<GameObject> brajaInstanciada = new List<GameObject>();


    private void OnEnable()
    {
        intitPinchosInv();
    }
    void Start()
    {
        intitPinchosInv();
    }
    public void intitPinchosInv()
    {
        foreach (GameObject item in brajaInstanciada)
        {
            Destroy(item);
        }
        baraja = GameManager.Instance.playerSO.baraja;

        foreach (PinchoSO Pinch in baraja)
        {
            GameObject pincho = null;
            if (Pinch.state == Estado.Disponible_para_robar)
            {
                pincho = Instantiate(PrefabIstanciar, PadreRobar.transform);
                brajaInstanciada.Add(pincho);
            }
            else if (Pinch.state == Estado.Mano) continue;
            else
            {
                pincho = Instantiate(PrefabIstanciar, PadreDescarte.transform);
                brajaInstanciada.Add(pincho);
            }
            pincho.GetComponent<PinchosScript>().pinchoSO = Pinch;
            pincho.GetComponent<RomperPincho>().PinchoSO = Pinch;
            pincho.GetComponent<UnityEngine.UI.Image>().sprite = Pinch.sprite;
        }

    }

}
