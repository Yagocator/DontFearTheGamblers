using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public float duration = 1f;
    public AnimationCurve curve;

    void OnEnable()
    {
        PJController.Shakecamara += EmpezarShake;
    }
    void OnDisable()
    {
        PJController.Shakecamara -= EmpezarShake;
    }
    void OnDestroy()
    {
        PJController.Shakecamara -= EmpezarShake;
    }

    private void EmpezarShake(bool v)
    {
        StartCoroutine(shake());
    }

    IEnumerator shake()
    {
        Vector3 StartPostion = transform.position;
        float elpseShake = 0f;
        while (elpseShake < duration)
        {
            elpseShake += Time.deltaTime;
            float streng = curve.Evaluate(elpseShake / duration);
            transform.position = StartPostion + Random.insideUnitSphere * streng;
            yield return null;
        }
        transform.position = StartPostion;
    }
}
