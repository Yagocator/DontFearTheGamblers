using System;
using UnityEngine;
using System.Collections;

public class RuletaController : MonoBehaviour
{
    public GameObject ruleta;
    public GameObject SlootTemplate;

    private RuletaScript RuletaScript;
    private GameObject[] sloots;
    private SlootScript[] slootsScripts;

    public static event Action<int> ResultRuleta;
    public static event Action<bool> ButtonPressed;

    private void createAllSloots()
    {
        sloots = new GameObject[37];
        slootsScripts = new SlootScript[37];
        for (int i = 0; i < 37; i++)
        {
            sloots[i] = Instantiate(SlootTemplate, ruleta.transform);
            sloots[i].transform.localPosition = new Vector3(-0.00200000009f, 2.84699988f, 0);
            sloots[i].transform.RotateAround(ruleta.transform.position, new Vector3(0.0f, 0.0f, 1.0f), -i * 9.72972972972f);
            sloots[i].SetActive(true);
            slootsScripts[i] = sloots[i].GetComponent<SlootScript>();
            slootsScripts[i].setValor(i);
            if (i == 0) slootsScripts[i].setColor(0);
            else if (i % 2 == 0) slootsScripts[i].setColor(1);
            else slootsScripts[i].setColor(2);

            slootsScripts[i].initSloot();

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createAllSloots();
        RuletaScript = ruleta.GetComponent<RuletaScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RuletaButtonPressed()
    {
        StartCoroutine(RuletaButtonPressed_WithBarrier());
        ButtonPressed.Invoke(true);
    }

    private IEnumerator RuletaButtonPressed_WithBarrier()
    {
        RuletaScript.RotateRuleta();
        yield return new WaitUntil(() => RuletaScript.finishRotate);
        int temp = RuletaScript.casillaPrincipal();
        int valor = slootsScripts[temp].getValor();
        Debug.Log("Valor: " + valor);
        ResultRuleta?.Invoke(valor);// TODO de momento es un numero mas adelante tendremos que hacer una lista con el tipo de pico y el id en el que ha tocado
    }

}
