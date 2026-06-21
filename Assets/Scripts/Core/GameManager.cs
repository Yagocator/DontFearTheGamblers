using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI pjVida;
    public TextMeshProUGUI pjShild;

    public static event Action<bool> Shakecamara;

    public static GameManager Instance;
    public Animator transition;

    public PJSO playerSO;

    [SerializeField] private float duracionFeedback = 0.4f;
    [SerializeField] private float fuerzaShake = 5f;
    private Color colorOriginalVida;
    private Color colorOriginalShild;
    private Vector3 posOriginalVida;
    private Vector3 posOriginalShild;

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

        if (playerSO.shild > 0)
        {
            int dañoAlEscudo = Mathf.Min(playerSO.shild, dañoRestante);
            playerSO.shild -= dañoAlEscudo;
            dañoRestante -= dañoAlEscudo;
            huboDAño = true;
        }

        if (dañoRestante > 0)
        {
            playerSO.vida -= dañoRestante;
            huboDAño = true;
            if (playerSO.vida <= 0)
            {
                playerSO.vida = 0;
                Morir();
            }
        }

        if (huboDAño)
        {
            StartCoroutine(FeedbackDaño());
        }

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

    private void Morir()
    {
        // TODO hay que hacer que te puedas morir
    }

    private void UpdateUI()
    {
        pjVida.text = playerSO.vida.ToString();
        pjShild.text = playerSO.shild.ToString();
    }

    public void LoadScene(string name)
    {
        StartCoroutine(Transition(name));
    }
    IEnumerator Transition(string name)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(name);
    }


}

