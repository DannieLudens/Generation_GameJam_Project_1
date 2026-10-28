using System.Collections;
using UnityEngine;

/// SOLO PARA PRUEBAS DE AUDIO (escena de Daniel). No va en el juego final.
/// Simula el reloj del día para probar el crossfade música → drone,
/// y dispara los sonidos que todavía no tienen un objeto (jefe, memo, calendario, UI) con teclas.
/// Ponlo en un GameObject vacío "TEST_AudioPanel".
public class TestAudioPanel : MonoBehaviour
{
    public static TestAudioPanel Instance { get; private set; }

    [Header("Reloj simulado")]
    public float startTime = 20f;
    public float maxTime = 90f;
    public bool running = true;

    [Header("Sonidos (arrástralos cuando los tengas)")]
    public AudioClip clockGood;     // C
    public AudioClip bossGreen;     // B (con tiempo)
    public AudioClip bossRed;       // B (en 0: luz roja)
    public AudioClip memoStamp;     // B (en 0, después de la luz roja) o S
    [Tooltip("Segundos entre la luz roja y el sello del memorando.")]
    public float memoDelay = 0.4f;
    public AudioClip calendarTear;  // B
    public AudioClip uiHover;       // ↑ / ↓
    public AudioClip uiClick;       // Enter

    [Header("Memorandos")]
    public int maxMemos = 3;

    float timeLeft;
    int memos;
    float stampAnim; // 1 → 0: el sello "golpea" la casilla en pantalla

    void Awake() => Instance = this;
    void Start() => timeLeft = startTime;

    void Update()
    {
        if (running) timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
        if (stampAnim > 0f) stampAnim = Mathf.Max(0f, stampAnim - Time.deltaTime * 3f);

        // Igual que lo haría el TimeManager real
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetTension(timeLeft < 10f ? 1f - timeLeft / 10f : 0f);

        if (Input.GetKeyDown(KeyCode.C)) { Add(3f); Play(clockGood); }
        if (Input.GetKeyDown(KeyCode.X)) timeLeft = Mathf.Max(0f, timeLeft - 2f);   // choque
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Si hay TEST_WorldSpeed, B pasa de día allá (y eso llama a CrossBoss)
            if (TestWorldSpeed.Instance != null) TestWorldSpeed.Instance.NextDay();
            else CrossBoss();
        }
        if (Input.GetKeyDown(KeyCode.S)) GiveMemo();                 // probar solo el sello
        if (Input.GetKeyDown(KeyCode.R))
        {
            timeLeft = startTime; memos = 0; running = true;
            if (AudioManager.Instance) AudioManager.Instance.PlayGameplayMusic();
        }
        // Probar cada modo de música por separado
        if (Input.GetKeyDown(KeyCode.Alpha1) && AudioManager.Instance) AudioManager.Instance.PlayMenuMusic();
        if (Input.GetKeyDown(KeyCode.Alpha2) && AudioManager.Instance) AudioManager.Instance.PlayGameplayMusic();
        if (Input.GetKeyDown(KeyCode.Alpha4) && AudioManager.Instance) AudioManager.Instance.PlayLeaderboardMusic();
        if (Input.GetKeyDown(KeyCode.Alpha3)) memos = maxMemos - 1; // el próximo memo despide a John
        if (Input.GetKeyDown(KeyCode.P)) running = !running;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) PlayUI(uiHover);
        if (Input.GetKeyDown(KeyCode.Return)) PlayUI(uiClick);
    }

    /// Llamado al cruzar la oficina del jefe (por B o al completar la distancia del día)
    public void CrossBoss()
    {
        bool onTime = timeLeft > 0f;
        if (onTime) Play(bossGreen);
        else StartCoroutine(LateCross());
        // La hoja del calendario la pone TEST_WorldSpeed; aquí solo si no existe
        if (TestWorldSpeed.Instance == null) Play(calendarTear);
        Add(60f);
        Debug.Log($"[TEST] Cruce del jefe: {(onTime ? "VERDE" : "ROJA + memorando")}");
    }

    IEnumerator LateCross()
    {
        Play(bossRed);
        yield return new WaitForSeconds(memoDelay);
        GiveMemo();
    }

    void GiveMemo()
    {
        memos++;
        stampAnim = 1f;
        Play(memoStamp);
        Debug.Log($"[TEST] Memorando {memos}/{maxMemos}");
        if (memos >= maxMemos)
        {
            Debug.Log("[TEST] ¡DESPEDIDO! Suena el stinger…");
            running = false;
            if (AudioManager.Instance)
                AudioManager.Instance.PlayGameOverStinger(() =>
                {
                    // Aquí el juego real cambiaría a la escena de Game Over / leaderboard
                    Debug.Log("[TEST] Stinger terminado → cambiar a Game Over / leaderboard. R para reiniciar");
                    AudioManager.Instance.PlayLeaderboardMusic();
                });
        }
    }

    void Add(float s) => timeLeft = Mathf.Min(maxTime, timeLeft + s);
    void Play(AudioClip c) { if (c && AudioManager.Instance) AudioManager.Instance.PlaySFX(c); }
    void PlayUI(AudioClip c) { if (c && AudioManager.Instance) AudioManager.Instance.PlayUI(c); }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 260, 190), "TEST AUDIO");
        GUI.Label(new Rect(20, 160, 240, 20), "1 menú 2 juego 3 casi despedido 4 leaderb.");
        GUI.Label(new Rect(20, 35, 240, 20), $"Reloj: {timeLeft:0.0} s {(timeLeft <= 0 ? "¡TARDE!" : "")}");
        GUI.Label(new Rect(20, 55, 240, 20), "C reloj +3   X choque −2");
        GUI.Label(new Rect(20, 75, 240, 20), "B cruzar jefe   R reiniciar");
        GUI.Label(new Rect(20, 95, 240, 20), "P pausar reloj   S sello de memo");
        GUI.Label(new Rect(20, 115, 240, 20), "↑/↓ hover UI   Enter click UI");

        // Casillas de memorandos (la última recibida "golpea" al sellarse)
        GUI.Label(new Rect(20, 140, 90, 20), "Memos:");
        for (int i = 0; i < maxMemos; i++)
        {
            bool stamped = i < memos;
            float grow = (stamped && i == memos - 1) ? stampAnim * 10f : 0f;
            var r = new Rect(80 + i * 40 - grow / 2f, 138 - grow / 2f, 32 + grow, 22 + grow);
            GUI.Box(r, stamped ? "✕" : "");
        }
        if (memos >= maxMemos)
            GUI.Box(new Rect(Screen.width / 2f - 120, Screen.height / 2f - 30, 240, 60), "¡DESPEDIDO!\nR para reiniciar");
    }
}