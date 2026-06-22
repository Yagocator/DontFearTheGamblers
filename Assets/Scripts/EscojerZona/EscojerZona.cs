using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EscojerZona : MonoBehaviour
{
    public TextMeshProUGUI HealText;
    public Button[] botones;

    private void Start()
    {
        if (GameManager.Instance.playerSO.vida == GameManager.Instance.playerSO.vidaMax)
        {
            HealText.text = "+20 MAX HEALTH";
        }
        else
        {
            HealText.text = "HEALTH 60";
        }
    }
    public void Bar()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        GameManager.Instance.LoadScene("Bar");
    }
    public void Mafia()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        GameManager.Instance.LoadScene("Mafia");
    }
    public void Heal()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        // TODO Sonido de dormir
        if (GameManager.Instance.playerSO.vida == GameManager.Instance.playerSO.vidaMax)
        {
            GameManager.Instance.playerSO.vidaMax += 20;
            GameManager.Instance.playerSO.vida = GameManager.Instance.playerSO.vidaMax;
        }
        GameManager.Instance.LoadScene("CombateTurnos");
    }

}
