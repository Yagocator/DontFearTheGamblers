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


    public static GameManager Instance;
    public Animator transition;

    public PJSO playerSO;

    public List<PinchoSO> posibles_pinchos = new List<PinchoSO>();



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

    public void LoadScene(string name)
    {
        StartCoroutine(Transition(name));
    }
    IEnumerator Transition(string name)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        transition.SetTrigger("Return");
        SceneManager.LoadScene(name);
    }

    public void createHand()
    {
        playerSO.baraja.Clear();
        List<PinchoSO> baraja = new List<PinchoSO>();
        baraja.Add(Instantiate(posibles_pinchos[0]));
        baraja.Add(Instantiate(posibles_pinchos[0]));
        baraja.Add(Instantiate(posibles_pinchos[1]));
        baraja.Add(Instantiate(posibles_pinchos[2]));
        for (int i = 0; i < 3; i++)
        {
            int j = UnityEngine.Random.Range(0, posibles_pinchos.Count);
            baraja.Add(Instantiate(posibles_pinchos[j]));
        }
        playerSO.baraja = baraja;
    }
}

