using UnityEngine;

/// SOLO PARA PRUEBAS DE AUDIO (escena de Daniel). No va en el juego final.
/// Simula el reloj del día para probar el crossfade música → drone,
/// y dispara los sonidos que todavía no tienen un objeto (jefe, memo, calendario, UI) con teclas.
/// Ponlo en un GameObject vacío "TEST_AudioPanel".
public class TestAudioPanel : MonoBehaviour
{
    [Header("Reloj simulado")]
    public float startTime = 20f;
    public float maxTime = 90f;
    public bool running = true;

    [Header("Sonidos (arrástralos cuando los tengas)")]
    public AudioClip clockGood;     // C
    public AudioClip bossGreen;     // B (con tiempo)
    public AudioClip memoStamp;     // B (en 0)
    public AudioClip calendarTear;  // B
    public AudioClip uiHover;       // ↑ / ↓
    public AudioClip uiClick;       // Enter

    float timeLeft;

    void Start() => timeLeft = startTime;

    void Update()
    {
        if (running) timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);

        // Igual que lo haría el TimeManager real
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetTension(timeLeft < 10f ? 1f - timeLeft / 10f : 0f);

        if (Input.GetKeyDown(KeyCode.C)) { Add(3f); Play(clockGood); }
        if (Input.GetKeyDown(KeyCode.X)) timeLeft = Mathf.Max(0f, timeLeft - 2f);   // choque
        if (Input.GetKeyDown(KeyCode.B)) CrossBoss();
        if (Input.GetKeyDown(KeyCode.R)) timeLeft = startTime;
        if (Input.GetKeyDown(KeyCode.P)) running = !running;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) PlayUI(uiHover);
        if (Input.GetKeyDown(KeyCode.Return)) PlayUI(uiClick);
    }

    void CrossBoss()
    {
        bool onTime = timeLeft > 0f;
        Play(onTime ? bossGreen : memoStamp);
        Play(calendarTear);
        Add(60f);
        Debug.Log($"[TEST] Cruce del jefe: {(onTime ? "VERDE" : "ROJA + memorando")}");
    }

    void Add(float s) => timeLeft = Mathf.Min(maxTime, timeLeft + s);
    void Play(AudioClip c) { if (c && AudioManager.Instance) AudioManager.Instance.PlaySFX(c); }
    void PlayUI(AudioClip c) { if (c && AudioManager.Instance) AudioManager.Instance.PlayUI(c); }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 260, 150), "TEST AUDIO");
        GUI.Label(new Rect(20, 35, 240, 20), $"Reloj: {timeLeft:0.0} s {(timeLeft <= 0 ? "¡TARDE!" : "")}");
        GUI.Label(new Rect(20, 55, 240, 20), "C reloj +3   X choque −2");
        GUI.Label(new Rect(20, 75, 240, 20), "B cruzar jefe   R reiniciar");
        GUI.Label(new Rect(20, 95, 240, 20), "P pausar reloj");
        GUI.Label(new Rect(20, 115, 240, 20), "↑/↓ hover UI   Enter click UI");
    }
}