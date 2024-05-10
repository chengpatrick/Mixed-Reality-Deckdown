using System.Collections;
using System.Collections.Generic;
using Meta.Utilities;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // SFX Audio Clip
    [SerializeField]
    List<AudioClip> SFXClip_List;

    Dictionary<string, AudioClip> SFXClip_Dic;

    // Audio Sources
    [SerializeField]
    AudioSource BGM_AudioSource;

    [SerializeField]
    GameObject SFX_AudioSources;

    [SerializeField]
    List<AudioSource> SFXAS_List;

    [SerializeField]
    AudioClip BGM;

    private void Awake()
    {
        SetUp_SFXClip_Dic();

/*        if (BGM != null)
        {
            BGM_AudioSource.clip = BGM;
            BGM_AudioSource.Play();
        }*/
    }

    public void PlaySFXClip(string name)
    {
        // If the SFX want to play not in the Dic
        if (!SFXClip_Dic.ContainsKey(name))
        {
            Debug.LogWarning("The SFX " + name + " does not exist");
            return;
        }

        // Check if any SFX Audio Source Available
        AudioSource ac = GetAvailableAudioSource();

        if (ac != null)
        {
            ac.PlayOneShot(SFXClip_Dic[name]);
        }
    }

    public void Play2DSFXInRandom(string name, int totalRandomNum)
    {
        int randomResult = Random.Range(1, totalRandomNum + 1);

        PlaySFXClip(name + randomResult);
    }

    public void PlaySFXAtPosition(string name, Vector3 position)
    {
        // If the SFX want to play not in the Dic
        if (!SFXClip_Dic.ContainsKey(name))
        {
            Debug.LogWarning("The SFX " + name + " does not exist");
            return;
        }

        // Check if any SFX Audio Source Available
        AudioSource ac = GetAvailableAudioSource();

        if (ac != null)
        {
            ac.transform.position = position;
            ac.PlayOneShot(SFXClip_Dic[name]);
        }
    }

    // Play SFX Clip in random pitch range
    public void PlaySFXClipInVary(string name, float lowPitch, float highPitch)
    {
        // If the SFX want to play not in the Dic
        if (!SFXClip_Dic.ContainsKey(name))
        {
            Debug.LogWarning("The SFX " + name + " does not exist");
            return;
        }

        // Check if any SFX Audio Source Available
        AudioSource ac = GetAvailableAudioSource();

        if (ac != null)
        {
            ac.pitch = Random.Range(lowPitch, highPitch);
            ac.clip = SFXClip_Dic[name];
            StartCoroutine(SFXVary_Coroutine(ac));
        }
    }

    IEnumerator SFXVary_Coroutine(AudioSource ac)
    {
        ac.Play();
        yield return new WaitForSeconds(ac.clip.length);
        ac.pitch = 1;
        ac.clip = null;
    }

    AudioSource GetAvailableAudioSource()
    {
        foreach(AudioSource ac in SFXAS_List)
        {
            if (!ac.isPlaying) return ac;
        }

        return null;
    }

    [ContextMenu("Menu-SetUp Audio Source")]
    void MenuAudioSourceSetUp()
    {
        if (SFX_AudioSources == null)
        {
            Debug.Log("Please Drag In the SFX_AudioSources Object");
            return;
        }

        BGM_AudioSource = GetComponentInChildren<AudioSource>();

        SFXAS_List.Clear();

        foreach (AudioSource ac in SFX_AudioSources.GetComponentsInChildren<AudioSource>())
        {
            SFXAS_List.Add(ac);
        }
    }

    void SetUp_SFXClip_Dic()
    {
        SFXClip_Dic = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in SFXClip_List)
        {
            SFXClip_Dic.Add(clip.name, clip);
        }

        SFXClip_List.Clear();
    }

    public void SetBGMVolume(float x)
    {
        BGM_AudioSource.volume = Mathf.Clamp01(x);
    }

    public void SetSFXVolume(float x)
    {
        foreach(AudioSource source in SFXAS_List)
        {
            source.volume = Mathf.Clamp01(x);
        }
    }

}
