using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Audiochanger : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider[] sliders; // 0 Master,1Music,2SFX


    private void Start()
    {
        sliders[0].value = GameManager.Instance.maseterVolumen;
        sliders[1].value = GameManager.Instance.musicVolumen;
        sliders[2].value = GameManager.Instance.SFXVolumen;
    }


    public void OnchangeMasterVolumen(float value)
    {
        GameManager.Instance.maseterVolumen = value;
        audioMixer.SetFloat("Master", Mathf.Lerp(-80f, 0f, value));
    }
    public void OnchangeMusicVolumen(float value)
    {
        GameManager.Instance.musicVolumen = value;
        audioMixer.SetFloat("Music", Mathf.Lerp(-80f, 0f, value));
    }
    public void OnchangeSFXVolumen(float value)
    {
        GameManager.Instance.SFXVolumen = value;
        audioMixer.SetFloat("SFX", Mathf.Lerp(-80f, 0f, value));
    }


}
