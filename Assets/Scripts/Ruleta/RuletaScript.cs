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

        // Seleccionar una casilla destino aleatoria (0-36)
        int targetCasilla = UnityEngine.Random.Range(0, 37);

        // Calcular cuántos grados necesita rotar para llegar a esa casilla
        float gradosPorCasilla = 9.72972972972f;
        float targetRotation = targetCasilla * gradosPorCasilla;

        // Calcular cuántos grados necesita rotar desde la posición actual
        float currentZ = transform.localEulerAngles.z;
        float rotationNeeded = targetRotation - currentZ;

        // Si es negativo, agregar 360 para rotar hacia adelante
        if (rotationNeeded < 0)
            rotationNeeded += 360f;

        // Agregar vueltas extra (1-3) para que sea más emocionante
        int extraVueltas = UnityEngine.Random.Range(1, 4);
        rotationNeeded += 360f * extraVueltas;

        // Calcular tiempo de rotación basado en los grados (entre 4 y 8 segundos)
        float rotationTime = 4f + (rotationNeeded / 360f) * 0.5f;

        StartCoroutine(girar(rotationTime, rotationNeeded));
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
    private IEnumerator girar(float rotationTime, float targetRotation)
    {
        giroRuletaSource = GameManager.Instance.PlayLoopSFX(
            girarRuletaSFX,
            volumenGiroRuleta,
            pitchMinGiro
        );

        float elapsed = 0f;
        float rotationAccumulated = 0f;
        Vector3 startRotation = transform.localEulerAngles;

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

            float gradosEsteFrame = (targetRotation / rotationTime) * factor * Time.deltaTime;

            transform.Rotate(0.0f, 0.0f, gradosEsteFrame);
            rotationAccumulated += gradosEsteFrame;

            ActualizarSonidoGiro(Mathf.Abs(gradosEsteFrame));

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Asegurar que termina exactamente en la rotación destino
        float expectedZ = (startRotation.z + targetRotation) % 360f;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, expectedZ);

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
