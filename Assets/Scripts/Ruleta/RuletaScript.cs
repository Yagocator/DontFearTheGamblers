using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RuletaScript : MonoBehaviour
{
    public bool finishRotate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finishRotate = false; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateRuleta()
    {
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
        float tmp = 0; 
        while (tmp < rotationTime)
        {
            float t = tmp / rotationTime; 
            float factor; 
            if (t > 0.05f && t < 0.65f)
            {
                if (EaseAccelerated(t) > EaseDerivate(t)) factor = EaseAccelerated(t);
                else factor = EaseDerivate(t);
            
            }
            else factor = EaseDerivate(t);
            tmp += 0.01f;
            transform.Rotate(0.0f, 0.0f, 0.8f * factor); 
            yield return new WaitForSeconds(0.004f);
        }
        finishRotate = true; 
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
