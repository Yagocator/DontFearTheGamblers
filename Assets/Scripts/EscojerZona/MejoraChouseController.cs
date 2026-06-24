using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class MejoraChouseController : MonoBehaviour
{

    public AudioClip selectOptions;
    public GameObject DescarteOpt;
    public Button[] botones;
    public AudioClip abrirMaletin;

    public void MasPinchos()
    {
        GameManager.Instance.PlaySFX(selectOptions);
        desableButtons();
        GameManager.Instance.playerSO.maximumPlayablePinchos++;
        GameManager.Instance.LoadScene("NextState");
    }
    public void MasMano()
    {
        GameManager.Instance.PlaySFX(selectOptions);
        desableButtons();
        GameManager.Instance.playerSO.handSize += 2;
        GameManager.Instance.LoadScene("NextState");
    }
    public void descarte()
    {
        desableButtons();
        GameManager.Instance.PlaySFX(abrirMaletin);
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
