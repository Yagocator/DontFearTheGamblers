using UnityEngine;
using UnityEngine.UI;

public class MejoraChouseController : MonoBehaviour
{
    public GameObject DescarteOpt;
    public Button[] botones;
    public void MasPinchos()
    {
        desableButtons();
        GameManager.Instance.playerSO.maximumPlayablePinchos++;
        GameManager.Instance.LoadScene("NextState");
    }
    public void MasMano()
    {
        desableButtons();
        GameManager.Instance.playerSO.handSize++;
        GameManager.Instance.LoadScene("NextState");
    }
    public void descarte()
    {
        desableButtons();
        DescarteOpt.SetActive(true);
    }

    private void desableButtons()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
    }
}
