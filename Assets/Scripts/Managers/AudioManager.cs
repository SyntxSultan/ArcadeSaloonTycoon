using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MusicTrack
{
    None = 0,
    MainTheme = 1
}

public enum SFX
{
    None = 0, 
    UIClick = 1,
    GainMoney = 2,
    LoseMoney = 3,
    PlaceItem = 4,
    CollectReward = 5,
    GainCoin = 6
}

[System.Serializable]
public class MusicItem {
    public MusicTrack id;
    public AudioClip clip;
    [Range(0f,1f)] public float baseVolume = 1f;
    public bool loop = true;
}

[System.Serializable]
public class SfxItem {
    public SFX id;
    public AudioClip clip;
    [Range(0f,1f)] public float baseVolume = 1f;
    public bool loop = false;
}

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private List<MusicItem> musicItems = new List<MusicItem>();

    [Header("SFX")]
    [SerializeField] private List<SfxItem> sfxItems = new List<SfxItem>();
    [SerializeField] private int sfxPoolSize = 5;

    private const string PREF_MUSIC = "AM_Music";
    private const string PREF_SFX = "AM_SFX";

    private AudioSource musicSource;
    private Dictionary<MusicTrack, MusicItem> musicDict;
    private Dictionary<SFX, SfxItem> sfxDict;
    private List<AudioSource> sfxPool;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake() 
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build dictionaries for fast enum lookup
        musicDict = new Dictionary<MusicTrack, MusicItem>();
        foreach (var m in musicItems) if (m != null && m.id != MusicTrack.None && m.clip != null) musicDict[m.id] = m;

        sfxDict = new Dictionary<SFX, SfxItem>();
        foreach (var s in sfxItems) if (s != null && s.id != SFX.None && s.clip != null) sfxDict[s.id] = s;

        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        // SFX pool
        sfxPool = new List<AudioSource>(sfxPoolSize);
        for (int i = 0; i < sfxPoolSize; i++) {
            var go = new GameObject("SFX_Source_" + i);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f; // 2D only
            sfxPool.Add(src);
        }

        LoadVolumes();
    }

    private void Start()
    {
        PlayMusic(MusicTrack.MainTheme, 3.5f);
    }

    #region Music
    public void PlayMusic(MusicTrack track, float crossfadeTime = 0f) {
        if (track == MusicTrack.None || !musicDict.ContainsKey(track)) {
            Debug.LogWarning($"AudioManager: Music {track} not configured.");
            return;
        }
        var item = musicDict[track];
        if (musicSource.clip == item.clip && musicSource.isPlaying) return;

        if (crossfadeTime <= 0f) {
            musicSource.clip = item.clip;
            musicSource.loop = item.loop;
            musicSource.volume = item.baseVolume * musicVolume;
            musicSource.Play();
        } else {
            StartCoroutine(CrossfadeTo(item, crossfadeTime));
        }
    }

    public void StopMusic(float fadeOut = 0f) {
        if (fadeOut <= 0f) {
            musicSource.Stop();
            musicSource.clip = null;
        } else {
            StartCoroutine(FadeOutMusic(fadeOut));
        }
    }

    IEnumerator CrossfadeTo(MusicItem newItem, float time) {
        float startVol = musicSource.isPlaying ? musicSource.volume : 0f;
        // Start new clip on same source but ramp from 0 -> target (simple)
        musicSource.clip = newItem.clip;
        musicSource.loop = newItem.loop;
        musicSource.volume = 0f;
        musicSource.Play();

        float elapsed = 0f;
        float target = newItem.baseVolume * musicVolume;
        while (elapsed < time) {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, target, elapsed / time);
            yield return null;
        }
        musicSource.volume = target;
    }

    IEnumerator FadeOutMusic(float time) {
        if (!musicSource.isPlaying) yield break;
        float start = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < time) {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(start, 0f, elapsed / time);
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = null;
    }
    #endregion

    #region SFX
    public void PlaySFX(SFX id, float volumeScale = 1f, float pitch = 1f) {
        if (id == SFX.None || !sfxDict.ContainsKey(id)) {
            Debug.LogWarning($"AudioManager: SFX {id} not configured.");
            return;
        }
        var item = sfxDict[id];
        var src = GetFreeSFXSource();
        if (src == null) {
            // Eğer pool doluysa en eskisini kullan
            src = sfxPool[0];
        }
        src.clip = item.clip;
        src.loop = item.loop;
        src.pitch = pitch;
        src.volume = Mathf.Clamp01(item.baseVolume * sfxVolume * volumeScale);
        src.Play();
    }

    public void StopAllSFX() {
        foreach (var a in sfxPool) a.Stop();
    }

    AudioSource GetFreeSFXSource() {
        foreach (var a in sfxPool) if (!a.isPlaying) return a;
        return null;
    }
    #endregion

    public void SetMusicVolume(float linear) {
        musicVolume = Mathf.Clamp01(linear);
        if (musicSource != null && musicSource.clip != null) {
            // try get base volume
            float baseVol = 1f;
            foreach (var m in musicItems) if (m.clip == musicSource.clip) { baseVol = m.baseVolume; break; }
            musicSource.volume = baseVol * musicVolume;
        }
        PlayerPrefs.SetFloat(PREF_MUSIC, musicVolume);
    }
    public void SetSFXVolume(float linear) {
        sfxVolume = Mathf.Clamp01(linear);
        PlayerPrefs.SetFloat(PREF_SFX, sfxVolume);
    }
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    void LoadVolumes() {
        musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
        sfxVolume = PlayerPrefs.GetFloat(PREF_SFX, 1f);
    }
}
