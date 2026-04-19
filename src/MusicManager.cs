using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource bgLayer1;
    public AudioSource bgLayer2;

    public AudioSource introLayer1;
    public AudioSource introLayer2;
    
    public float maxVolume = 0.85f;

    public float fadeDuration = 3f;

    void Start()
    {
        bgLayer1.Play();
        bgLayer2.Play();
        introLayer1.volume = 0;
        introLayer2.volume = 0;
        introLayer1.Play();
        introLayer2.Play();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInGroup(new AudioSource[] { introLayer1, introLayer2 }, maxVolume));
    }

    IEnumerator FadeInGroup(AudioSource[] sources, float targetVolume)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(0f, targetVolume, t / fadeDuration);

            foreach (var s in sources)
                s.volume = v;

            yield return null;
        }

        foreach (var s in sources)
            s.volume = targetVolume;
    }

    public void Stop()
    {
        bgLayer1.Stop();
        bgLayer2.Stop();
        introLayer1.Stop();
        introLayer2.Stop();
    }
}