using UnityEngine;

public static class StarlightAudio
{
    static AudioSource source;
    static AudioClip button;
    static AudioClip impact;
#if UNITY_ANDROID && !UNITY_EDITOR
    static float lastVibration = float.NegativeInfinity;
#endif

    public static void PlayButton()
    {
        if (button == null) button = Resources.Load<AudioClip>("Starlight/Crystal Tap");
        Play(button);
    }

    public static void PlayImpact(bool playerWasHit)
    {
        if (impact == null) impact = Resources.Load<AudioClip>("Starlight/Crystal Impact");
        Play(impact);
#if UNITY_ANDROID && !UNITY_EDITOR
        var settings = SettingsManager.Instance;
        if (playerWasHit && settings != null && settings.VibrationEnabled
            && Time.unscaledTime - lastVibration >= .8f)
        {
            lastVibration = Time.unscaledTime;
            Handheld.Vibrate();
        }
#endif
    }

    static void Play(AudioClip clip)
    {
        var settings = SettingsManager.Instance;
        if (clip == null || settings == null || settings.SfxVolume <= 0)
            return;

        if (source == null)
        {
            source = settings.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        source.PlayOneShot(clip, settings.SfxVolume);
    }
}
