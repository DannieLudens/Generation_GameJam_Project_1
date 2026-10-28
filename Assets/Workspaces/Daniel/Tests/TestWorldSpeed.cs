using UnityEngine;

/// SOLO PARA PRUEBAS (escena de Daniel). No va en el juego final:
/// el real es GameManager.worldSpeed (Ana) y la animación la ajusta T27 (Zian).
/// Simula que pasan los días: sube la velocidad del mundo y acelera la animación de John,
/// así los pasos (Animation Events) suenan más rápido.
///   N → siguiente día (+0,5 m/s)    M → volver al día 1
/// Ponlo en un GameObject vacío "TEST_WorldSpeed".
public class TestWorldSpeed : MonoBehaviour
{
    public static TestWorldSpeed Instance { get; private set; }

    [Header("Valores del GDD")]
    public float baseSpeed = 12f;      // día 1
    public float speedPerDay = 0.5f;
    public float maxSpeed = 18f;

    [Header("Animación de John")]
    public Animator playerAnimator;    // arrastra player Variant_Audio

    [Header("Calendario (UI)")]
    [Tooltip("sfx_calendar_tear: hoja del calendario rasgándose al pasar de día.")]
    public AudioClip calendarTear;
    [Range(0f, 1f)] public float tearVolume = 1f;

    [Header("Distancia (valores del GDD)")]
    public float baseDayDistance = 600f;   // día 1
    public float distancePerDay = 50f;
    [Tooltip("Pasa de día solo al completar la distancia del día (simula cruzar al jefe).")]
    public bool autoDayByDistance = true;

    [Header("Avance automático por tiempo (opcional)")]
    [Tooltip("Pasa de día solo cada X segundos. 0 = desactivado.")]
    public float autoDaySeconds = 0f;

    public float Speed { get; private set; }
    int day = 1;
    float timer;
    float tearAnim; // 1 → 0: sacude la hoja en pantalla al pasar de día
    float totalMeters;   // puntaje: metros de toda la partida
    float dayMeters;     // metros recorridos en el día actual
    float DayDistance => baseDayDistance + distancePerDay * (day - 1);
    public float TotalMeters => totalMeters;

    void Awake()
    {
        Instance = this;
        SetDay(1);
    }

    void Update()
    {
        if (tearAnim > 0f) tearAnim = Mathf.Max(0f, tearAnim - Time.deltaTime * 2f);
        // Distancia virtual: John no avanza, pero cuenta los metros que el mundo pasa
        float step = Speed * Time.deltaTime;
        totalMeters += step;
        dayMeters += step;
        if (autoDayByDistance && dayMeters >= DayDistance) SetDay(day + 1);

        if (Input.GetKeyDown(KeyCode.N)) SetDay(day + 1);
        if (Input.GetKeyDown(KeyCode.M)) { totalMeters = 0f; SetDay(1); }

        if (autoDaySeconds > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= autoDaySeconds) { timer = 0f; SetDay(day + 1); }
        }
    }

    void SetDay(int d)
    {
        // Suena la hoja solo al avanzar de día (no al iniciar ni al volver al día 1)
        if (d > day && calendarTear && AudioManager.Instance)
        {
            AudioManager.Instance.PlayUI(calendarTear, tearVolume);
            tearAnim = 1f;
        }
        day = d;
        dayMeters = 0f;
        Speed = Mathf.Min(maxSpeed, baseSpeed + speedPerDay * (day - 1));
        // La línea clave que debe ir en el juego real (T27):
        if (playerAnimator) playerAnimator.speed = Speed / baseSpeed;
        Debug.Log($"[TEST] Día {day}: {Speed:0.0} m/s · animación {Speed / baseSpeed:0.00}×");
    }

    void OnGUI()
    {
        GUI.Box(new Rect(280, 10, 220, 70), "TEST VELOCIDAD");
        GUI.Label(new Rect(290, 35, 200, 20), $"Día {day} · {Speed:0.0} m/s · {Speed / baseSpeed:0.00}×");
        GUI.Label(new Rect(290, 55, 200, 20), "N siguiente día   M día 1");

        // Metros (debajo del reloj, arriba al centro, como en el HUD)
        var meters = new GUIStyle(GUI.skin.box) { fontSize = 22, alignment = TextAnchor.MiddleCenter };
        GUI.Box(new Rect(Screen.width / 2f - 90, 10, 180, 40), $"{totalMeters:N0} m", meters);
        GUI.Label(new Rect(Screen.width / 2f - 90, 52, 180, 20),
                  $"Jefe en {Mathf.Max(0f, DayDistance - dayMeters):0} m ({Mathf.Max(0f, DayDistance - dayMeters) / Speed:0} s)");

        // Calendario de mentira arriba a la derecha (la hoja se sacude al rasgarse)
        float shake = Mathf.Sin(tearAnim * 40f) * 8f * tearAnim;
        var r = new Rect(Screen.width - 130 + shake, 10 - tearAnim * 20f, 110, 80);
        GUI.Box(r, "");
        GUI.Box(new Rect(r.x, r.y, r.width, 22), "LUNES");
        var big = new GUIStyle(GUI.skin.label) { fontSize = 36, alignment = TextAnchor.MiddleCenter };
        GUI.Label(new Rect(r.x, r.y + 22, r.width, 58), day.ToString(), big);
    }
}