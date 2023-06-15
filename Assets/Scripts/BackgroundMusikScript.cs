using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusikScript : MonoBehaviour
{
    private AudioSource AudioSource;
    public GameObject ObjectMusic;
    private float musicVolume = 1f;
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        ObjectMusic = GameObject.FindWithTag("BackgroundMusic");
        AudioSource = ObjectMusic.GetComponent<AudioSource>();
        musicVolume = PlayerPrefs.GetFloat("backgroundVolume", 0.5f);
        AudioSource.volume = musicVolume;
        volumeSlider.value = musicVolume;
    }

    void Update()
    {
        AudioSource.volume = musicVolume;
        PlayerPrefs.SetFloat("backgroundVolume", musicVolume);
    }

    public void UpdateVolume(float vol)
    {
        musicVolume = vol;
    }

}
