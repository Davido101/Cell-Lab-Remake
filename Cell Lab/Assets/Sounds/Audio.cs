using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public bool destroyOnFinish = false;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void PlayAudio()
    {
        audioSource.Play();
        StartCoroutine(WaitForSound());
    }

    public void SceneUpdate(bool destroyAfter)
    {
        destroyOnFinish = true;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator WaitForSound()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        if (destroyOnFinish)
            Destroy(gameObject);
    }
}
