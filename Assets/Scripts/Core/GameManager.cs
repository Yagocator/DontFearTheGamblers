using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI pjVida;
    public TextMeshProUGUI pjShild;

    public static event Action<bool> Shakecamara;

    public static GameManager Instance;
    public Animator transition;

    public PJSO playerSO;

    public List<PinchoSO> posibles_pinchos = new List<PinchoSO>();

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
    public void PJGetDefence(int cuant)
    {
        playerSO.shild += cuant;
        StartCoroutine(FeedbackEscudo());
        UpdateUI();
    }
    public void PJGetHeald(int cuant)
    {
        playerSO.vida += cuant;
        if (playerSO.vidaMax <= playerSO.vida)
        {
            playerSO.vida = playerSO.vidaMax;
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

    public void createHand()
    {
        playerSO.baraja.Clear();
        List<PinchoSO> baraja = new List<PinchoSO>();
        baraja.Add(Instantiate(posibles_pinchos[0]));
        baraja.Add(Instantiate(posibles_pinchos[1]));
        baraja.Add(Instantiate(posibles_pinchos[2]));
        for (int i = 0; i < 4; i++)
        {
            int j = UnityEngine.Random.Range(0, posibles_pinchos.Count);
            baraja.Add(Instantiate(posibles_pinchos[j]));
        }
        playerSO.baraja = baraja;
    }
}

