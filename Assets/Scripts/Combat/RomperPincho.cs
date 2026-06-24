using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RomperPincho : MonoBehaviour
{
    public PinchoSO PinchoSO;
    public Animator animator;
    public Button boton;

    public static event Action<bool> EliminadoPincho;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (boton == null)
            boton = GetComponentInChildren<Button>();
        // o: boton = GetComponentInChildren<Button>();
    }

    public void init()
    {
        if (boton == null)
        {
            Debug.LogError("boton es NULL en " + gameObject.name);
            return;
        }
        boton.onClick.AddListener(RomperPinchoFunc);
    }

    public void RomperPinchoFunc()
    {
        EliminadoPincho?.Invoke(true);
        GameManager.Instance.playerSO.baraja.Remove(PinchoSO);
        StartCoroutine(esperarAnim());
    }

    private IEnumerator esperarAnim()
    {
        animator.SetTrigger("Morir");
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.LoadScene("NextState");
    }
}
