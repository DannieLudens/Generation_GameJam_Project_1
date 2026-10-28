using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// AudioManager de Never-Ending Monday (T26).
/// - Música por modo: menú (loop), juego (playlist aleatoria sin cortes) y stinger de Game Over.
/// - Capa drone que sube cuando queda poco tiempo (solo en el modo juego).
/// - SFX y UI con una línea: AudioManager.Instance.PlaySFX(clip);
/// - Volúmenes de música y efectos para el panel de pausa/configuración (se guardan en PlayerPrefs).
/// Requiere un Audio Mixer con grupos Music, SFX y UI, y los parámetros expuestos
/// "MusicVolume", "SFXVolume" y "UIVolume".
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mixer;
    [Tooltip("Subgrupo donde suena el loop de música (ej. Music/BGM).")]
    public AudioMixerGroup musicGroup;
    [Tooltip("Subgrupo donde suena el drone (ej. Music/Drone). Si lo dejas vacío usa musicGroup.")]
    public AudioMixerGroup droneGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup uiGroup;

    public enum MusicMode { None, Menu, Gameplay, Stinger, Leaderboard }

    [Header("Música por modo")]
    [Tooltip("Música del menú principal (en loop). Ej: la Slower Version.")]
    public AudioClip menuMusic;
    [Tooltip("Música del leaderboard (en loop). Si la dejas vacía, usa la del menú.")]
    public AudioClip leaderboardMusic;
    [Tooltip("Pistas del juego: suenan una tras otra en orden aleatorio, sin cortes y sin repetir la misma seguida.")]
    public AudioClip[] gameplayPlaylist;
    [Tooltip("Respaldo: si la playlist está vacía, el juego usa este loop.")]
    public AudioClip musicLoop;
    [Tooltip("Pieza que cierra la música al ser despedido (3er memorando).")]
    public AudioClip gameOverStinger;
    [Tooltip("Qué música arranca al dar Play en esta escena (solo si Auto Music By Scene está apagado).")]
    public MusicMode startMode = MusicMode.Gameplay;

    [Header("Música automática por escena")]
    [Tooltip("Elige la música según el nombre de la escena que se carga. Cualquier otra escena = juego.")]
    public bool autoMusicByScene = true;
    public string menuSceneName = "Titulo";
    public string leaderboardSceneName = "Leaderboard";

    [Header("Sonidos de UI automáticos")]
    [Tooltip("Suena al moverse entre botones (selección con teclado o mouse).")]
    public AudioClip uiHover;
    [Tooltip("Suena al presionar cualquier botón (Button) de la escena.")]
    public AudioClip uiClick;

    [Header("Tensión")]
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

    // Mismas claves que usa PantallaConfiguracion (Johana): sus sliders controlan este audio
    const string MusicKey = "volumenMusica";
    const string SfxKey = "volumenEfectos";
    const float DefaultMusic = 0.7f, DefaultSfx = 0.8f;
    float appliedMusic = -1f, appliedSfx = -1f, nextPrefsCheck;
    GameObject lastSelected;
    float ignoreHoverUntil;

    AudioSource[] musicSources;   // 2 para encadenar pistas sin cortes
    AudioSource droneSource, uiSource;
    public MusicMode Mode { get; private set; } = MusicMode.None;
    int playingSrc, lastTrack = -1;
    double nextSwitchDsp;         // momento exacto (reloj de audio) en que entra la siguiente pista
    Coroutine stingerRoutine;
    AudioSource[] sfxSources;
    int nextSfx;
    float tensionTarget, tension;
    bool paused;
    // Nivel de mezcla que dejaste a mano en el Audio Mixer (se lee al iniciar).
    // El slider del jugador solo baja desde ese nivel, nunca lo sube por encima.
    float baseMusicDb, baseSfxDb, baseUiDb;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSources = new[] { NewSource("Music_A", musicGroup, false), NewSource("Music_B", musicGroup, false) };
        droneSource = NewSource("Drone", droneGroup != null ? droneGroup : musicGroup, true);
        uiSource = NewSource("UI", uiGroup, false);
        sfxSources = new AudioSource[sfxVoices];
        for (int i = 0; i < sfxVoices; i++) sfxSources[i] = NewSource("SFX_" + i, sfxGroup, false);
    }

    void Start()
    {
        // Guarda la mezcla manual del Audio Mixer (por ejemplo Music en -15 dB)
        mixer.GetFloat("MusicVolume", out baseMusicDb);
        mixer.GetFloat("SFXVolume", out baseSfxDb);
        mixer.GetFloat("UIVolume", out baseUiDb);

        // Aplica los volúmenes guardados del jugador encima de esa mezcla
        SyncVolumesFromPrefs();

        if (autoMusicByScene) ApplySceneMusic(SceneManager.GetActiveScene().name);
        else switch (startMode)
        {
            case MusicMode.Menu: PlayMenuMusic(); break;
            case MusicMode.Leaderboard: PlayLeaderboardMusic(); break;
            case MusicMode.Gameplay: PlayGameplayMusic(); break;
        }
        HookButtons();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (autoMusicByScene) ApplySceneMusic(scene.name);
        HookButtons();
    }

    void ApplySceneMusic(string sceneName)
    {
        SetPausedMuffle(false);
        if (sceneName == menuSceneName) PlayMenuMusic();
        else if (sceneName == leaderboardSceneName) PlayLeaderboardMusic();
        else PlayGameplayMusic(); // escena de juego (también al Reintentar)
    }

    // Agrega el clic a todos los botones de la escena (incluye paneles ocultos como pausa o Game Over)
    void HookButtons()
    {
        lastSelected = null;
        ignoreHoverUntil = Time.unscaledTime + 0.3f; // no suena el hover del botón que se selecciona solo al abrir
        foreach (var b in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            b.onClick.AddListener(() => PlayUI(uiClick));
    }

    void SyncVolumesFromPrefs()
    {
        float m = PlayerPrefs.GetFloat(MusicKey, DefaultMusic);
        float s = PlayerPrefs.GetFloat(SfxKey, DefaultSfx);
        if (!Mathf.Approximately(m, appliedMusic)) { ApplyMusicVolume(m); appliedMusic = m; }
        if (!Mathf.Approximately(s, appliedSfx)) { ApplySfxVolume(s); appliedSfx = s; }
    }

    void Update()
    {
        // Crossfade: la tensión se mueve suave hacia su objetivo;
        // el drone sube y la música baja al mismo tiempo.
        tension = Mathf.MoveTowards(tension, tensionTarget, tensionFadeSpeed * Time.unscaledDeltaTime);
        droneSource.volume = tension * droneMaxVolume;
        float music = Mode == MusicMode.Stinger ? musicVolume : musicVolume * (1f - tension * musicDuck);
        if (paused) music *= 0.4f;
        foreach (var src in musicSources) src.volume = music;

        // Sliders de Johana: revisa cada 0,25 s si cambiaron (funciona también en pausa)
        if (Time.unscaledTime >= nextPrefsCheck)
        {
            nextPrefsCheck = Time.unscaledTime + 0.25f;
            SyncVolumesFromPrefs();
        }

        // Hover: suena cuando cambia el botón seleccionado
        var es = EventSystem.current;
        var sel = es != null ? es.currentSelectedGameObject : null;
        if (sel != lastSelected)
        {
            if (sel != null && lastSelected != null && Time.unscaledTime > ignoreHoverUntil) PlayUI(uiHover);
            lastSelected = sel;
        }

        // Playlist: cuando entra la pista ya programada, se programa la siguiente en la otra fuente
        if (Mode == MusicMode.Gameplay && gameplayPlaylist != null && gameplayPlaylist.Length > 0
            && AudioSettings.dspTime >= nextSwitchDsp)
        {
            playingSrc = 1 - playingSrc;                                   // ya suena la que estaba programada
            double end = nextSwitchDsp + Length(musicSources[playingSrc].clip);
            ScheduleNextTrack(1 - playingSrc, end);                        // la siguiente, en la fuente libre
            nextSwitchDsp = end;                                           // próximo cambio (antes faltaba esto)
        }
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

    /// Menú principal: loop de la versión lenta, sin drone.
    public void PlayMenuMusic() => PlayLoop(MusicMode.Menu, menuMusic);

    /// Leaderboard: su propia pista en loop (o la del menú si no hay).
    public void PlayLeaderboardMusic() =>
        PlayLoop(MusicMode.Leaderboard, leaderboardMusic != null ? leaderboardMusic : menuMusic);

    void PlayLoop(MusicMode mode, AudioClip clip)
    {
        if (Mode == mode) return; // ya está sonando: no reinicia la canción
        StopMusic();
        Mode = mode;
        if (clip == null) return;
        musicSources[0].clip = clip;
        musicSources[0].loop = true;
        musicSources[0].Play();
        Debug.Log($"[AUDIO] {mode} → {clip.name}");
    }

    /// Partida: playlist aleatoria encadenada sin cortes + drone en silencio listo para subir.
    public void PlayGameplayMusic()
    {
        StopMusic();
        Mode = MusicMode.Gameplay;
        Debug.Log($"[AUDIO] Gameplay (playlist de {(gameplayPlaylist == null ? 0 : gameplayPlaylist.Length)} pistas)");

        if (gameplayPlaylist != null && gameplayPlaylist.Length > 0)
        {
            double start = AudioSettings.dspTime + 0.1;
            playingSrc = 0;
            ScheduleNextTrack(0, start);                                   // primera pista
            ScheduleNextTrack(1, start + Length(musicSources[0].clip));    // la que sigue
            nextSwitchDsp = start + Length(musicSources[0].clip);
        }
        else if (musicLoop != null)
        {
            musicSources[0].clip = musicLoop;
            musicSources[0].loop = true;
            musicSources[0].Play();
        }

        if (droneLoop != null)
        {
            droneSource.clip = droneLoop;
            droneSource.volume = 0f;
            droneSource.Play(); // suena siempre en silencio y sube con SetTension
        }
    }

    /// Por compatibilidad con código anterior.
    public void PlayMusic() => PlayGameplayMusic();

    /// 3er memorando: corta la música, suena el stinger una vez y avisa cuando termina.
    /// Ejemplo: AudioManager.Instance.PlayGameOverStinger(() => SceneManager.LoadScene("GameOver"));
    /// Devuelve la duración del stinger (por si quieren sincronizar una animación).
    public float PlayGameOverStinger(Action onFinished = null)
    {
        StopMusic();
        Mode = MusicMode.Stinger;
        if (gameOverStinger == null) { onFinished?.Invoke(); return 0f; }

        musicSources[0].clip = gameOverStinger;
        musicSources[0].loop = false;
        musicSources[0].Play();
        float len = (float)Length(gameOverStinger);
        stingerRoutine = StartCoroutine(AfterStinger(len, onFinished));
        return len;
    }

    IEnumerator AfterStinger(float seconds, Action onFinished)
    {
        yield return new WaitForSecondsRealtime(seconds); // funciona aunque el juego esté en timeScale 0
        stingerRoutine = null;
        onFinished?.Invoke();
    }

    public void StopMusic()
    {
        if (stingerRoutine != null) { StopCoroutine(stingerRoutine); stingerRoutine = null; }
        foreach (var src in musicSources) { src.Stop(); src.loop = false; }
        droneSource.Stop();
        tensionTarget = tension = 0f;
        Mode = MusicMode.None;
    }

    void ScheduleNextTrack(int src, double startDsp)
    {
        int i = PickTrack();
        musicSources[src].clip = gameplayPlaylist[i];
        musicSources[src].loop = false;
        musicSources[src].PlayScheduled(startDsp);
        Debug.Log($"[AUDIO] Playlist → {gameplayPlaylist[i].name} en {musicSources[src].name}");
    }

    int PickTrack()
    {
        int n = gameplayPlaylist.Length;
        int i = UnityEngine.Random.Range(0, n);
        if (n > 1) while (i == lastTrack) i = UnityEngine.Random.Range(0, n);
        lastTrack = i;
        return i;
    }

    // Duración exacta en segundos (más precisa que clip.length para encadenar)
    static double Length(AudioClip c) => c == null ? 0 : (double)c.samples / c.frequency;

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
        PlaySFX(clip, volume, pitchVariation, null);
    }

    /// Igual que PlaySFX pero por un subgrupo del mixer (ej. SFX/Obstacles) para mezclarlo aparte.
    /// Ejemplo: AudioManager.Instance.PlaySFX(clip, 1f, sfxObstaclesGroup);
    public void PlaySFX(AudioClip clip, float volume, AudioMixerGroup group)
    {
        PlaySFX(clip, volume, defaultPitchVariation, group);
    }

    public void PlaySFX(AudioClip clip, float volume, float pitchVariation, AudioMixerGroup group)
    {
        if (clip == null) return;
        var s = sfxSources[nextSfx];
        nextSfx = (nextSfx + 1) % sfxSources.Length;
        s.outputAudioMixerGroup = group != null ? group : sfxGroup;
        s.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
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
        ApplyMusicVolume(value01);
        appliedMusic = value01;
        PlayerPrefs.SetFloat(MusicKey, value01);
    }

    public void SetSFXVolume(float value01)
    {
        ApplySfxVolume(value01);
        appliedSfx = value01;
        PlayerPrefs.SetFloat(SfxKey, value01);
    }

    void ApplyMusicVolume(float v) => mixer.SetFloat("MusicVolume", Mix(baseMusicDb, v));

    void ApplySfxVolume(float v)
    {
        // Los efectos y la UI comparten el mismo slider
        mixer.SetFloat("SFXVolume", Mix(baseSfxDb, v));
        mixer.SetFloat("UIVolume", Mix(baseUiDb, v));
    }

    public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicKey, DefaultMusic);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SfxKey, DefaultSfx);

    // 0..1 lineal → -80..0 dB (el slider se siente natural)
    static float ToDecibels(float v) => v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;

    // Mezcla manual + slider del jugador (slider en 1 = tu mezcla tal cual)
    static float Mix(float baseDb, float v) => v <= 0.0001f ? -80f : Mathf.Max(-80f, baseDb + ToDecibels(v));
}