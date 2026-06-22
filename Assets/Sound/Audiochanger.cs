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

        OnchangeMasterVolumen(sliders[0].value);
        OnchangeMusicVolumen(sliders[1].value);
        OnchangeSFXVolumen(sliders[2].value);
    }


    public void OnchangeMasterVolumen(float value)
    {
        GameManager.Instance.maseterVolumen = value;
        audioMixer.SetFloat("Master", SliderToDb(value));    
    }
    public void OnchangeMusicVolumen(float value)
    {
        GameManager.Instance.musicVolumen = value;
        audioMixer.SetFloat("Music", SliderToDb(value));
    }
    public void OnchangeSFXVolumen(float value)
    {
        GameManager.Instance.SFXVolumen = value;
        audioMixer.SetFloat("SFX", SliderToDb(value));
    }

    private float SliderToDb(float value)
    {
        value = Mathf.Clamp01(value);

        if (value <= 0.0001f)
        {
            return -80f;
        }

        return Mathf.Log10(value) * 20f;
    }

}
