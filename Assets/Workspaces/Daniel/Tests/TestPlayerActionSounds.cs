using UnityEngine;
using UnityEngine.Audio;

/// SOLO PARA PRUEBAS DE AUDIO (escena de Daniel). No va en el juego final.
/// Simula los sonidos de las acciones de John con el teclado, sin moverlo:
///   Espacio / W / ↑  → salto (grupo Jump)
///   A / D / ← / →    → cambio de carril (grupo Move)
/// En el juego real, el PlayerController de Ana hace estas mismas llamadas
/// justo cuando John salta o cambia de carril.
/// Ponlo en player Variant_Audio.
public class TestPlayerActionSounds : MonoBehaviour
{
    [Header("Salto")]
    public AudioClip[] jumpClips;          // sfx_jump_1, sfx_jump_2
    public AudioMixerGroup jumpGroup;      // SFX/Jump
    [Range(0f, 1f)] public float jumpVolume = 1f;

    [Header("Cambio de carril")]
    public AudioClip[] laneClips;          // sfx_laneChange_1
    public AudioMixerGroup moveGroup;      // SFX/Move
    [Range(0f, 1f)] public float laneVolume = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Play(jumpClips, jumpVolume, jumpGroup, "salto");

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            Play(laneClips, laneVolume, moveGroup, "cambio de carril");
    }

    void Play(AudioClip[] clips, float volume, AudioMixerGroup group, string label)
    {
        if (clips == null || clips.Length == 0 || AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySFX(clips[Random.Range(0, clips.Length)], volume, group);
        Debug.Log($"[TEST] Sonido de {label}");
    }
}