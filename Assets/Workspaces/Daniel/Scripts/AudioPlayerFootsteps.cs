using UnityEngine;

/// Pasos de John con variación de tono y volumen para que no suenen repetidos.
/// Dos formas de dispararlo:
///   A) Animation Events: en los frames donde cada pie toca el piso, agrega un evento
///      que llame a la función "Footstep" (sin parámetros).
///   B) Sin eventos: activa "useTimer" y ajusta "stepInterval" a la cadencia de la animación.
[RequireComponent(typeof(AudioSource))]
public class AudioPlayerFootsteps : MonoBehaviour
{
    [Header("Sonidos")]
    [Tooltip("Pon 1 clip o varios. Con varios, nunca repite el mismo dos veces seguidas.")]
    public AudioClip[] footstepClips;
 
    [Header("Variación")]
    [Range(0.5f, 1.5f)] public float minPitch = 0.85f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.15f;
    [Tooltip("Diferencia mínima de tono entre un paso y el siguiente, para que siempre se note el cambio.")]
    [Range(0f, 0.3f)] public float minPitchJump = 0.08f;
    [Tooltip("Un pie suena un poco más grave que el otro (izquierdo/derecho). 0 = desactivado.")]
    [Range(0f, 0.2f)] public float footPitchOffset = 0.05f;
    [Range(0f, 1f)] public float minVolume = 0.75f;
    [Range(0f, 1f)] public float maxVolume = 1f;
 
    [Header("Modo temporizador (si no usas Animation Events)")]
    public bool useTimer = false;
    [Tooltip("Segundos entre pasos. Para trotar ~0.3–0.35 s.")]
    public float stepInterval = 0.32f;
 
    [Header("Solo suena si está en el piso")]
    [Tooltip("Opcional: si lo dejas vacío, siempre suena.")]
    public CharacterController characterController;
 
    AudioSource source;
    int lastIndex = -1;
    float lastPitch = 1f;
    bool leftFoot;
    float timer;
 
    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D: la cámara siempre va pegada a John
    }
 
    void Update()
    {
        if (!useTimer) return;
        timer += Time.deltaTime;
        if (timer >= stepInterval)
        {
            timer = 0f;
            Footstep();
        }
    }
 
    // Llamado por Animation Event o por el temporizador
    public void Footstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        if (characterController != null && !characterController.isGrounded) return;
 
        leftFoot = !leftFoot;
        source.pitch = PickPitch() + (leftFoot ? -footPitchOffset : footPitchOffset);
        source.PlayOneShot(PickClip(), Random.Range(minVolume, maxVolume));
    }
 
    float PickPitch()
    {
        // Busca un tono que se aleje al menos minPitchJump del anterior (máx. 10 intentos)
        float p = Random.Range(minPitch, maxPitch);
        for (int i = 0; i < 10 && Mathf.Abs(p - lastPitch) < minPitchJump; i++)
            p = Random.Range(minPitch, maxPitch);
        lastPitch = p;
        return p;
    }
 
    AudioClip PickClip()
    {
        if (footstepClips.Length == 1) return footstepClips[0];
        int i;
        do { i = Random.Range(0, footstepClips.Length); } while (i == lastIndex);
        lastIndex = i;
        return footstepClips[i];
    }
}