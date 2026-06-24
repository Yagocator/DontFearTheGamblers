using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class EscojerZona : MonoBehaviour
{
    public TextMeshProUGUI HealText;
    public Button[] botones;
    public TextMeshProUGUI nextLvL;
    public Image Dificulty;
    public Sprite[] DifficultySprite;

    public AudioClip selectOptions;
    private void Start()
    {
        if (GameManager.Instance.playerSO.vida == GameManager.Instance.playerSO.vidaMax)
        {
            HealText.text = "+60 MAX HEALTH";
        }
        else
        {
            HealText.text = "HEALTH 80";
        }
        currentNextLvl();
        if (GameManager.Instance.nivelAct <= 5)
        {
            nextLvL.color = Color.green;
            Dificulty.sprite = DifficultySprite[0];
        }
        else if (GameManager.Instance.nivelAct <= 12 && GameManager.Instance.nivelAct > 5)
        {
            nextLvL.color = Color.yellow;
            Dificulty.sprite = DifficultySprite[1];
        }
        else
        {
            nextLvL.color = Color.red;
            Dificulty.sprite = DifficultySprite[2];
        }
    }
    public void Bar()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        GameManager.Instance.PlaySFX(selectOptions);
        GameManager.Instance.LoadScene("Bar");
    }
    public void Mafia()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        GameManager.Instance.PlaySFX(selectOptions);
        GameManager.Instance.LoadScene("Mafia");
    }
    public void Heal()
    {
        foreach (Button item in botones)
        {
            item.interactable = false;
        }
        // TODO Sonido de dormir
        GameManager.Instance.PlaySFX(selectOptions);
        if (GameManager.Instance.playerSO.vida == GameManager.Instance.playerSO.vidaMax)
        {
            GameManager.Instance.playerSO.vidaMax += 60;
            GameManager.Instance.playerSO.vida = GameManager.Instance.playerSO.vidaMax;
        }
        else
        {
            GameManager.Instance.playerSO.vida += 80;
            if (GameManager.Instance.playerSO.vida >= GameManager.Instance.playerSO.vidaMax)
            {
                GameManager.Instance.playerSO.vida = GameManager.Instance.playerSO.vidaMax;
            }
        }
        GameManager.Instance.LoadScene("CombateTurnos");
    }

    private void currentNextLvl()
    {
        nextLvL.text = "NEXT LVL " + GameManager.Instance.nivelAct.ToString();
    }

}
