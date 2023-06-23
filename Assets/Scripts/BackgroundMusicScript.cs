using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicScript : MonoBehaviour
{
    // Private variable for the audio source
    private AudioSource _audioSource;

    // Public variable for the music object
    public GameObject ObjectMusic;

    // Private variable for the music volume
    private float _musicVolume = 1f;

    // Serialized field for the volume slider
    [SerializeField] private Slider _volumeSlider;

    // Called before the first frame update
    void Start()
    {
        // Find the music object and get its audio source
        ObjectMusic = GameObject.FindWithTag("BackgroundMusic");
        _audioSource = ObjectMusic.GetComponent<AudioSource>();

        // Get the saved music volume or use a default value
        _musicVolume = PlayerPrefs.GetFloat("backgroundVolume", 0.5f);

        // Set the audio source volume
        _audioSource.volume = _musicVolume;

        // Set the slider value if the slider exists - not every scene has a slider
        if (_volumeSlider != null)
        {
            _volumeSlider.value = _musicVolume;
        }
    }

    void Update()
    {
        // Update the audio source volume and save it
        _audioSource.volume = _musicVolume;
        PlayerPrefs.SetFloat("backgroundVolume", _musicVolume);
    }

    // Function to update the music volume
    public void UpdateVolume(float vol)
    {
        _musicVolume = vol;
    }
}
