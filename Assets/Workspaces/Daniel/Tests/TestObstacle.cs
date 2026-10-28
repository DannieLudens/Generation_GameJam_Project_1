using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// SOLO PARA PRUEBAS DE AUDIO (escena de Daniel). No va en el juego final:
/// el movimiento real lo hace MoveTowardsPlayer (Ana) y el spawner (Zian).
/// Un cubo que viene hacia John como si fuera obstáculo o reloj, y al tocarlo suena su SFX.
/// Setup del cubo: Box Collider con "Is Trigger" ✔ + Rigidbody con "Is Kinematic" ✔.
/// Setup de John: un Collider (por ejemplo Capsule Collider) y Tag = "Player".
public class TestObstacle : MonoBehaviour
{
    public enum Kind { Obstaculo, Reloj }

    [Header("Qué representa")]
    public Kind kind = Kind.Obstaculo;
    [Tooltip("Uno o varios sonidos; con varios elige uno al azar.")]
    public AudioClip[] hitSounds;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Subgrupo del mixer (ej. SFX/Obstacles). Vacío = grupo SFX general.")]
    public AudioMixerGroup mixerGroup;
    [Header("Sonido después del golpe (solo obstáculos)")]
    [Tooltip("sfx_slowed_1/2/3: suena uno al azar justo después del golpe.")]
    public AudioClip[] afterHitSounds;
    [Tooltip("Segundos entre el golpe y el slowed.")]
    public float afterHitDelay = 0.1f;
    [Range(0f, 1f)] public float afterHitVolume = 1f;
    [Tooltip("Subgrupo del mixer para el slowed. Vacío = el mismo de arriba.")]
    public AudioMixerGroup afterHitGroup;

    [Tooltip("Solo informativo en la consola: segundos que quitaría (obstáculo) o daría (reloj).")]
    public float seconds = -2f;

    [Header("Movimiento hacia John (eje Z negativo)")]
    public float speed = 12f;
    [Tooltip("Z donde reaparece el cubo.")]
    public float spawnZ = 60f;
    [Tooltip("Cuando pasa esta Z (detrás de la cámara) vuelve a spawnZ.")]
    public float despawnZ = -10f;

    Renderer rend;

    void Awake() => rend = GetComponent<Renderer>();

    void Update()
    {
        // Si hay un TEST_WorldSpeed en la escena, usa su velocidad (simula los días)
        float v = TestWorldSpeed.Instance != null ? TestWorldSpeed.Instance.Speed : speed;
        transform.position += Vector3.back * v * Time.deltaTime;
        if (transform.position.z < despawnZ) Respawn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (hitSounds != null && hitSounds.Length > 0 && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(hitSounds[Random.Range(0, hitSounds.Length)], volume, mixerGroup);

        if (kind == Kind.Obstaculo && afterHitSounds != null && afterHitSounds.Length > 0)
            StartCoroutine(PlayAfterHit());

        Debug.Log($"[TEST] {name} ({kind}) tocó a John → {seconds:+0;-0} s");

        // El reloj desaparece al tomarlo; el obstáculo se oculta un momento
        if (rend) rend.enabled = false;
        Invoke(nameof(Respawn), 0.5f);
    }

    IEnumerator PlayAfterHit()
    {
        yield return new WaitForSeconds(afterHitDelay);
        if (AudioManager.Instance == null) yield break;
        var clip = afterHitSounds[Random.Range(0, afterHitSounds.Length)];
        AudioManager.Instance.PlaySFX(clip, afterHitVolume, afterHitGroup != null ? afterHitGroup : mixerGroup);
    }

    void Respawn()
    {
        CancelInvoke();
        var p = transform.position;
        transform.position = new Vector3(p.x, p.y, spawnZ);
        if (rend) rend.enabled = true;
    }
}