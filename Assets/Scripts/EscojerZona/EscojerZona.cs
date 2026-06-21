using TMPro;
using UnityEngine;

public class EscojerZona : MonoBehaviour
{
    public TextMeshProUGUI HealText;

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
        GameManager.Instance.LoadScene("Bar");
    }
    public void Mafia()
    {
        GameManager.Instance.LoadScene("Mafia");
    }
    public void Heal()
    {
        // TODO Sonido de dormir
        if (GameManager.Instance.playerSO.vida == GameManager.Instance.playerSO.vidaMax)
        {
            GameManager.Instance.playerSO.vidaMax += 20;
            GameManager.Instance.playerSO.vida = GameManager.Instance.playerSO.vidaMax;
        }
        GameManager.Instance.LoadScene("CombateTurnos");
    }

}
