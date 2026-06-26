using UnityEngine;

public class notOPEnough : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    public void Win()
    {
        GameManager.Instance.LoadScene("Menu");
    }
    public void continueF()
    {
        GameManager.Instance.notOPEnough = true;
        GameManager.Instance.LoadScene("NextState");
    }
}
