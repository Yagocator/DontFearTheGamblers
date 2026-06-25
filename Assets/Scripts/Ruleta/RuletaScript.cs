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

        DateTime d = DateTime.UtcNow;
        int seed = (d.Second + 213) * (d.Hour + 43) * (d.Day + 6456);
        float rotationTime = 8 + ((seed / 3.2f) % 10);

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
        return -6 * 0.05f * 0.05f + 6 * 0.05f +  t / 0.65f * 0.9f - (-6 * 0.05f * 0.05f + 6 * 0.05f); 
    }
    private IEnumerator girar(float rotationTime)
    {
        giroRuletaSource = GameManager.Instance.PlayLoopSFX(
            girarRuletaSFX,
            volumenGiroRuleta,
            pitchMinGiro
        );

        float tmp = 0f;

        while (tmp < rotationTime)
        {
            float t = tmp / rotationTime;
            float factor;

            if (t > 0.05f && t < 0.65f)
            {
                if (EaseAccelerated(t) > EaseDerivate(t))
                    factor = EaseAccelerated(t);
                else
                    factor = EaseDerivate(t);
            }
            else
            {
                factor = EaseDerivate(t);
            }

            float gradosEsteFrame = 0.8f * factor;

            transform.Rotate(0.0f, 0.0f, gradosEsteFrame);

            ActualizarSonidoGiro(Mathf.Abs(gradosEsteFrame));

            tmp += 0.01f;

            yield return new WaitForSeconds(0.004f);
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
        int valor = (int) (((z + halfParte) / parte) % 37);
        return valor; 
    }

    public int casillaWithRotation(float rotation)
    {
        float parte = 9.72972972972f; 
        float halfParte = parte / 2;

        float z = transform.localEulerAngles.z; 
        int valor = ((int) ((z + rotation + halfParte) / parte) % 37);
        return valor; 
    }
}
