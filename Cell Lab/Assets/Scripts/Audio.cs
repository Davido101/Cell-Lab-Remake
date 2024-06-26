using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    static Dictionary<string, AudioClip> loadedResources = new Dictionary<string, AudioClip>();
    private static Audio instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }


    /// <summary>
    /// Loads an AudioClip from the Resources folder
    /// </summary>
    /// <param name="resourcePath">The path to the audio file</param>
    /// <returns>The AudioClip loaded</returns>
    public static AudioClip LoadAudio(string resourcePath)
    {
        // If AudioClip is already loaded, return it
        if (loadedResources.ContainsKey(resourcePath))
        {
            return loadedResources[resourcePath];
        }

        // Otherwise, load it
        AudioClip audioClip = Resources.Load<AudioClip>(resourcePath);

        loadedResources.Add(resourcePath, audioClip);
        return audioClip;
    }

    /// <summary>
    /// Plays an AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip to be played</param>
    /// <param name="keepOnSceneUpdate">If the AudioClip should keep playing when changing scenes</param>
    /// <returns>The AudioSource being played</returns>
    public static AudioSource PlayAudio(AudioClip audioClip, bool keepOnSceneUpdate = false)
    {
        // Create the AudioSource and play it
        GameObject audioSourceObj = new GameObject("AudioSource");
        AudioSource audioSource = audioSourceObj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();

        if (keepOnSceneUpdate)
            DontDestroyOnLoad(audioSource.gameObject);
        instance.StartCoroutine(WaitForSound(audioSource));

        return audioSource;
    }

    static IEnumerator WaitForSound(AudioSource audioSource)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        if (audioSource != null)
            Destroy(audioSource.gameObject);
    }
}