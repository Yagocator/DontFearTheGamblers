using UnityEngine;

public class notOPEnough : MonoBehaviour
{
    public AudioClip winSFX;

    public AudioClip gigaChadContinueSFX; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    public void Win()
    {
        GameManager.Instance.StopAllSFX();
        GameManager.Instance.PlaySFX(winSFX);
        GameManager.Instance.LoadScene("Menu");
    }
    public void continueF()
    {
        GameManager.Instance.PlaySFX(gigaChadContinueSFX);
        GameManager.Instance.notOPEnough = true;
        GameManager.Instance.LoadScene("NextState");
    }
}
