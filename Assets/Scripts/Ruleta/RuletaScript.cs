using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RuletaScript : MonoBehaviour
{
    public bool finishRotate;

    [Header("Audio giro ruleta")]
    public AudioClip girarRuletaSFX;
    public float volumenGiroRuleta = 1f;
    public float pitchMinGiro = 1f;
    public float pitchMaxGiro = 1.2f;
    public float gradosPorFrameParaPitchMax = 1.2f;

    private AudioSource giroRuletaSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finishRotate = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RotateRuleta()
    {
        if (!finishRotate) return;
        finishRotate = false;

        float rotationTime = UnityEngine.Random.Range(2f, 6f);
        StartCoroutine(girar(rotationTime));
    }

    private float Ease(float t)
    {
        return -2 * t * t * t + 3 * t * t;
    }

    private float EaseDerivate(float t)
    {
        return -6 * t * t + 6 * t;
    }

    private float EaseAccelerated(float t)
    {
        return -6 * 0.05f * 0.05f + 6 * 0.05f + t / 0.65f * 0.9f - (-6 * 0.05f * 0.05f + 6 * 0.05f);
    }
    private IEnumerator girar(float rotationTime)
    {
        giroRuletaSource = GameManager.Instance.PlayLoopSFX(
            girarRuletaSFX,
            volumenGiroRuleta,
            pitchMinGiro
        );

        float elapsed = 0f;

        while (elapsed < rotationTime)
        {
            float t = elapsed / rotationTime;
            float factor;

            if (t > 0.05f && t < 0.65f)
            {
                factor = Mathf.Max(EaseAccelerated(t), EaseDerivate(t));
            }
            else
            {
                factor = EaseDerivate(t);
            }

            float gradosEsteFrame = 200f * factor * Time.deltaTime;

            transform.Rotate(0.0f, 0.0f, gradosEsteFrame);

            ActualizarSonidoGiro(Mathf.Abs(gradosEsteFrame));

            elapsed += Time.deltaTime;

            yield return null;
        }

        DetenerSonidoGiro();

        finishRotate = true;
    }
    private void ActualizarSonidoGiro(float gradosEsteFrame)
    {
        if (giroRuletaSource == null) return;

        float velocidadNormalizada = Mathf.Clamp01(
            gradosEsteFrame / gradosPorFrameParaPitchMax
        );

        giroRuletaSource.pitch = Mathf.Lerp(
            pitchMinGiro,
            pitchMaxGiro,
            velocidadNormalizada
        );

        giroRuletaSource.volume = volumenGiroRuleta * Mathf.Lerp(
            0.55f,
            1f,
            velocidadNormalizada
        );
    }

    private void DetenerSonidoGiro()
    {
        if (giroRuletaSource == null) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopSFX(giroRuletaSource);
        }

        giroRuletaSource = null;
    }

    private void OnDisable()
    {
        DetenerSonidoGiro();
    }
    public int casillaPrincipal()
    {
        float parte = 9.72972972972f;
        float halfParte = parte / 2;

        float z = transform.localEulerAngles.z;
        int valor = (int)(((z + halfParte) / parte) % 37);
        return valor;
    }

    public int casillaWithRotation(float rotation)
    {
        float parte = 9.72972972972f;
        float halfParte = parte / 2;

        float z = transform.localEulerAngles.z;
        int valor = ((int)((z + rotation + halfParte) / parte) % 37);
        return valor;
    }
}
