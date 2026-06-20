using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI pjVida;
    public TextMeshProUGUI pjShild;

    public static event Action<bool> Shakecamara;


    public static GameManager Instance;

    public PJSO playerSO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

    }

    public void AtacarPJ(int Damage)
    {
        Shakecamara?.Invoke(true);
        int dañoRestante = Damage;
        if (playerSO.shild > 0)
        {
            int dañoAlEscudo = Mathf.Min(playerSO.shild, dañoRestante);
            playerSO.shild -= dañoAlEscudo;
            dañoRestante -= dañoAlEscudo;
        }

        if (dañoRestante > 0)
        {
            playerSO.vida -= dañoRestante;
            if (playerSO.vida <= 0)
            {
                playerSO.vida = 0;
                Morir();
            }
        }
        UpdateUI();
    }

    private void Morir()
    {

    }
    private void UpdateUI()
    {
        pjVida.text = playerSO.vida.ToString();
        pjShild.text = playerSO.shild.ToString();

    }
}
