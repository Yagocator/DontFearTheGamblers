using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PJController : MonoBehaviour
{
    public TextMeshProUGUI pjVida;
    public TextMeshProUGUI pjShild;

    private Color colorOriginalVida;
    private Color colorOriginalShild;
    private Vector3 posOriginalVida;
    private Vector3 posOriginalShild;

    [SerializeField] private float duracionFeedback = 0.4f;
    [SerializeField] private float fuerzaShake = 5f;

    public AudioClip playerDeathSFX;
    public static event Action<bool> Shakecamara;


    private void Start()
    {
        colorOriginalVida = pjVida.color;
        colorOriginalShild = pjShild.color;
        posOriginalVida = pjVida.transform.localPosition;
        posOriginalShild = pjShild.transform.localPosition;
    }
    public void AtacarPJ(int Damage)
    {
        Shakecamara?.Invoke(true);
        int dañoRestante = Damage;
        bool huboDAño = false;

        if (GameManager.Instance.playerSO.shild > 0)
        {
            int dañoAlEscudo = Mathf.Min(GameManager.Instance.playerSO.shild, dañoRestante);
            GameManager.Instance.playerSO.shild -= dañoAlEscudo;
            dañoRestante -= dañoAlEscudo;
            huboDAño = true;
        }

        if (dañoRestante > 0)
        {
            GameManager.Instance.playerSO.vida -= dañoRestante;
            huboDAño = true;
            if (GameManager.Instance.playerSO.vida <= 0)
            {
                GameManager.Instance.playerSO.vida = 0;
                Morir();
            }
        }

        if (huboDAño)
        {
            StartCoroutine(FeedbackDaño());
        }

        UpdateUI();
    }
    public void PJGetDefence(int cuant)
    {
        GameManager.Instance.playerSO.shild += cuant;
        StartCoroutine(FeedbackEscudo());
        UpdateUI();
    }
    public void PJGetHeald(int cuant)
    {
        GameManager.Instance.playerSO.vida += cuant;
        if (GameManager.Instance.playerSO.vidaMax <= GameManager.Instance.playerSO.vida)
        {
            GameManager.Instance.playerSO.vida = GameManager.Instance.playerSO.vidaMax;
        }
        StartCoroutine(FeedbackVida());
        UpdateUI();
    }

    private IEnumerator FeedbackDaño()
    {
        pjVida.color = Color.red;
        pjShild.color = Color.red;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFeedback)
        {
            Vector3 shakeOffset = new Vector3(UnityEngine.Random.Range(-fuerzaShake, fuerzaShake), UnityEngine.Random.Range(-fuerzaShake, fuerzaShake), 0);
            pjVida.transform.localPosition = posOriginalVida + shakeOffset;
            pjShild.transform.localPosition = posOriginalShild + shakeOffset;

            tiempoTranscurrido += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        pjVida.transform.localPosition = posOriginalVida;
        pjShild.transform.localPosition = posOriginalShild;
        pjVida.color = colorOriginalVida;
        pjShild.color = colorOriginalShild;
    }
    private IEnumerator FeedbackEscudo()
    {
        pjShild.color = Color.blue;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFeedback)
        {
            Vector3 shakeOffset = new Vector3(UnityEngine.Random.Range(-fuerzaShake, fuerzaShake), UnityEngine.Random.Range(-fuerzaShake, fuerzaShake), 0);
            pjShild.transform.localPosition = posOriginalShild + shakeOffset;

            tiempoTranscurrido += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        pjShild.transform.localPosition = posOriginalShild;
        pjShild.color = colorOriginalShild;
    }
    private IEnumerator FeedbackVida()
    {
        pjVida.color = Color.green;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFeedback)
        {
            Vector3 shakeOffset = new Vector3(UnityEngine.Random.Range(-fuerzaShake, fuerzaShake), UnityEngine.Random.Range(-fuerzaShake, fuerzaShake), 0);
            pjVida.transform.localPosition = posOriginalVida + shakeOffset;

            tiempoTranscurrido += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        pjVida.transform.localPosition = posOriginalVida;
        pjVida.color = colorOriginalVida;
    }

    private void Morir()
    {
        // TODO hacer una panatalla de muerta si me da tiempo
        GameManager.Instance.StopAllSFX();
        GameManager.Instance.StopMusic();
        GameManager.Instance.PlaySFX(playerDeathSFX);
        GameManager.Instance.LoadScene("Menu");
    }

    private void UpdateUI()
    {
        pjVida.text = GameManager.Instance.playerSO.vida.ToString();
        pjShild.text = GameManager.Instance.playerSO.shild.ToString();
    }
}
