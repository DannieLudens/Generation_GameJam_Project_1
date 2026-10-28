using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// Conecta el audio al juego SIN modificar los scripts de Ana ni de Johana.
/// Va en el jugador (el mismo GameObject que tiene PlayerController), solo en la escena de Daniel
/// (no aplicar al prefab del jugador). Escucha lo que pasa y hace sonar lo que corresponde:
///   · Choques (silla, colega, cafetera) + "slowed" después del golpe
///   · Reloj recogido
///   · Salto y cambio de carril
///   · Drone de tensión con el tiempo restante
///   · Cambio de día (luz verde + hoja del calendario)
///   · Memorando (sello) y despido (stinger → música de leaderboard)
///   · Pausa (la música baja) y silencio de pasos en el aire / Game Over
[RequireComponent(typeof(PlayerController))]
public class GameAudioHooks : MonoBehaviour
{
    [Header("Choques (los nombres son los que revisa PlayerController)")]
    public string chairName = "Chair";
    public AudioClip[] chairHit;
    public string loverName = "Lover";
    public AudioClip[] loverHit;
    public string coffeeName = "coffee_machine";
    public AudioClip[] coffeeHit;
    public AudioMixerGroup obstaclesGroup;

    [Header("Después del golpe")]
    public AudioClip[] slowed;
    public float slowedDelay = 0.1f;
    public AudioMixerGroup slowedGroup;

    [Header("Reloj")]
    public AudioClip[] clockGood;
    public AudioMixerGroup clockGroup;

    [Header("Movimiento")]
    public AudioClip[] jump;
    public AudioMixerGroup jumpGroup;
    public AudioClip[] laneChange;
    public AudioMixerGroup moveGroup;

    [Header("Día y memorandos")]
    public AudioClip bossGreen;
    public AudioClip calendarTear;
    public float calendarDelay = 0.3f;
    [Tooltip("Opcional: suena antes del sello (llegó tarde).")]
    public AudioClip bossRed;
    public AudioClip memoStamp;
    public float memoDelay = 0.4f;
    public AudioMixerGroup bossGroup;
    public int memosForGameOver = 3;

    [Header("Tensión")]
    [Tooltip("Segundos de reloj desde los que sube el drone.")]
    public float tensionSeconds = 10f;

    [Header("Pasos")]
    [Tooltip("Si lo dejas vacío lo busca en el jugador o sus hijos.")]
    public AudioPlayerFootsteps footsteps;

    PlayerController player;
    float lastX;
    bool lastOnFloor = true;
    int lastCicle, lastMemos;
    bool gameOverHandled, wasPaused;

    AudioManager AM => AudioManager.Instance;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        if (footsteps == null) footsteps = GetComponentInChildren<AudioPlayerFootsteps>();
    }

    void Start()
    {
        lastX = transform.position.x;
        lastOnFloor = player.isOnFloor;
        lastCicle = player.cicle;
        lastMemos = player.memos;
    }

    void Update()
    {
        if (AM == null) return;
        bool over = PlayerController.gameOver || gameOverHandled;

        // Pausa (PantallaPausa pone timeScale en 0)
        bool paused = Time.timeScale == 0f && !over;
        if (paused != wasPaused) { AM.SetPausedMuffle(paused); wasPaused = paused; }
        if (paused) return;

        // Pasos: no suenan en el aire ni después del despido
        if (footsteps != null) footsteps.muted = !player.isOnFloor || over;

        if (over) return;

        // Cambio de carril: la X del jugador salta de golpe
        float x = transform.position.x;
        if (Mathf.Abs(x - lastX) > 0.5f) Play(laneChange, moveGroup);
        lastX = x;

        // Salto: isOnFloor pasa de true a false
        if (lastOnFloor && !player.isOnFloor) Play(jump, jumpGroup);
        lastOnFloor = player.isOnFloor;

        // Drone con el tiempo restante
        float t = Mathf.Max(0f, player.timer);
        AM.SetTension(t < tensionSeconds ? 1f - t / tensionSeconds : 0f);

        // Nuevo día: llegó donde el jefe
        if (player.cicle > lastCicle)
        {
            lastCicle = player.cicle;
            PlayOne(bossGreen, bossGroup);
            StartCoroutine(After(calendarDelay, () => AM.PlayUI(calendarTear)));
        }

        // Memorando
        if (player.memos > lastMemos)
        {
            lastMemos = player.memos;
            StartCoroutine(Memo(player.memos >= memosForGameOver));
        }
    }

    IEnumerator Memo(bool fired)
    {
        if (fired) gameOverHandled = true;
        if (bossRed != null)
        {
            PlayOne(bossRed, bossGroup);
            yield return new WaitForSecondsRealtime(memoDelay);
        }
        PlayOne(memoStamp, bossGroup);

        if (fired)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            AM.PlayGameOverStinger(() => AM.PlayLeaderboardMusic());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (AM == null || PlayerController.gameOver) return;

        if (other.TryGetComponent<Rewinds>(out _)) { Play(clockGood, clockGroup); return; }

        string n = other.gameObject.name;
        AudioClip[] hit = null;
        if (n.Contains(chairName)) hit = chairHit;
        else if (n.Contains(loverName)) hit = loverHit;
        else if (n.Contains(coffeeName)) hit = coffeeHit;
        if (hit == null) return;

        Play(hit, obstaclesGroup);
        if (slowed != null && slowed.Length > 0)
            StartCoroutine(After(slowedDelay, () => Play(slowed, slowedGroup != null ? slowedGroup : obstaclesGroup)));
    }

    IEnumerator After(float seconds, System.Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    void Play(AudioClip[] clips, AudioMixerGroup group)
    {
        if (clips == null || clips.Length == 0) return;
        PlayOne(clips[Random.Range(0, clips.Length)], group);
    }

    void PlayOne(AudioClip clip, AudioMixerGroup group)
    {
        if (clip != null && AM != null) AM.PlaySFX(clip, 1f, group);
    }
}