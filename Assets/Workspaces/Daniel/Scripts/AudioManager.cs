using UnityEngine;
using UnityEngine.Audio;

/// AudioManager de Never-Ending Monday (T26).
/// - Música en loop + capa drone que sube cuando queda poco tiempo.
/// - SFX y UI con una línea: AudioManager.Instance.PlaySFX(clip);
/// - Volúmenes de música y efectos para el panel de pausa/configuración (se guardan en PlayerPrefs).
/// Requiere un Audio Mixer con grupos Music, SFX y UI, y los parámetros expuestos
/// "MusicVolume", "SFXVolume" y "UIVolume".
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sFXGroup;
    public AudioMixerGroup uIGroup;

    [Header("Música")]
    public AudioClip musicLoop;
    public AudioClip droneLoop;
    [Range(0f, 1f)] public float musicVolume = 0.8f;
    [Tooltip("Volumen máximo de la capa drone cuando la tensión es 1.")]
    [Range(0f, 1f)] public float droneMaxVolume = 0.7f;
    [Tooltip("Cuánto baja la música cuando el drone está al máximo (0.7 = la música queda al 30 %).")]
    [Range(0f, 1f)] public float musicDuck = 0.7f;
    [Tooltip("Qué tan rápido cambia la tensión (por segundo). 1 = la transición completa tarda 1 s.")]
    public float tensionFadeSpeed = 1f;

    [Header("SFX")]
    [Tooltip("Variación de tono por defecto para PlaySFX (0.05 = ±5 %).")]
    [Range(0f, 0.3f)] public float defaultPitchVariation = 0.05f;
    [Tooltip("Cuántos SFX pueden sonar al mismo tiempo.")]
    public int sfxVoices = 8;

    const string MusicKey = "vol_music";
    const string SfxKey = "vol_sfx";

    AudioSource musicSource, droneSource, uiSource;
    AudioSource[] sfxSources;
    int nextSfx;
    float tensionTarget, tension;
    bool paused;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = NewSource("Music", musicGroup, true);
        droneSource = NewSource("Drone", musicGroup, true);
        uiSource = NewSource("UI", uIGroup, false);
        sfxSources = new AudioSource[sfxVoices];
        for (int i = 0; i < sfxVoices; i++) sfxSources[i] = NewSource("SFX_" + i, sFXGroup, false);
    }

    void Start()
    {
        // Aplica los volúmenes guardados (el mixer solo acepta valores después de Awake)
        SetMusicVolume(PlayerPrefs.GetFloat(MusicKey, 0.8f));
        SetSFXVolume(PlayerPrefs.GetFloat(SfxKey, 0.8f));
        PlayMusic();
    }

    void Update()
    {
        // Crossfade: la tensión se mueve suave hacia su objetivo;
        // el drone sube y la música baja al mismo tiempo.
        tension = Mathf.MoveTowards(tension, tensionTarget, tensionFadeSpeed * Time.unscaledDeltaTime);
        droneSource.volume = tension * droneMaxVolume;
        float music = musicVolume * (1f - tension * musicDuck);
        musicSource.volume = paused ? music * 0.4f : music;
    }

    AudioSource NewSource(string name, AudioMixerGroup group, bool loop)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        var s = go.AddComponent<AudioSource>();
        s.outputAudioMixerGroup = group;
        s.loop = loop;
        s.playOnAwake = false;
        s.spatialBlend = 0f;
        return s;
    }

    // ---------- Música ----------

    public void PlayMusic()
    {
        if (musicLoop != null && !musicSource.isPlaying)
        {
            musicSource.clip = musicLoop;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
        if (droneLoop != null && !droneSource.isPlaying)
        {
            droneSource.clip = droneLoop;
            droneSource.volume = 0f;
            droneSource.Play(); // suena siempre en silencio y sube con SetTension
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        droneSource.Stop();
        tensionTarget = tension = 0f;
    }

    /// 0 = solo música, 1 = drone al máximo y música abajo.
    /// Quien sepa cuánto falta para el jefe (DayManager) lo llama cada frame, por ejemplo:
    /// float segundosAlJefe = metrosRestantes / GameManager.worldSpeed;
    /// AudioManager.Instance.SetTension(segundosAlJefe < 10f ? 1f - segundosAlJefe / 10f : 0f);
    /// Al cruzar el portal: SetTension(0) y la música vuelve sola.
    public void SetTension(float t)
    {
        tensionTarget = Mathf.Clamp01(t);
    }

    /// Para la pausa: baja la música sin detenerla (la pausa usa timeScale 0, esto no se ve afectado)
    public void SetPausedMuffle(bool isPaused)
    {
        paused = isPaused;
    }

    // ---------- SFX ----------

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        PlaySFX(clip, volume, defaultPitchVariation);
    }

    public void PlaySFX(AudioClip clip, float volume, float pitchVariation)
    {
        if (clip == null) return;
        var s = sfxSources[nextSfx];
        nextSfx = (nextSfx + 1) % sfxSources.Length;
        s.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        s.PlayOneShot(clip, volume);
    }

    public void PlayUI(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        uiSource.PlayOneShot(clip, volume);
    }

    // ---------- Volúmenes (sliders 0..1 del panel de pausa/configuración) ----------

    public void SetMusicVolume(float value01)
    {
        mixer.SetFloat("MusicVolume", ToDecibels(value01));
        PlayerPrefs.SetFloat(MusicKey, value01);
    }

    public void SetSFXVolume(float value01)
    {
        // Los efectos y la UI comparten el mismo slider
        mixer.SetFloat("SFXVolume", ToDecibels(value01));
        mixer.SetFloat("UIVolume", ToDecibels(value01));
        PlayerPrefs.SetFloat(SfxKey, value01);
    }

    public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicKey, 0.8f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SfxKey, 0.8f);

    // 0..1 lineal → -80..0 dB (el slider se siente natural)
    static float ToDecibels(float v) => v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;
}