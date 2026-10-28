using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PantallaConfiguracion : MonoBehaviour
{
    public Slider sliderMusica;
    public Slider sliderEfectos;
    public Text textoMusica;
    public Text textoEfectos;
    public Button botonVolver;
    public Button botonCreditos;
    public Button botonSalir;
    public GameObject panelCreditos;
    public Button botonConfiguracionTitulo;
    public AudioSource musica;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sliderMusica.value = PlayerPrefs.GetFloat("volumenMusica", 0.7f);
        sliderEfectos.value = PlayerPrefs.GetFloat("volumenEfectos", 0.8f);

        sliderMusica.onValueChanged.AddListener(CambiarMusica);
        sliderEfectos.onValueChanged.AddListener(CambiarEfectos);
        botonVolver.onClick.AddListener(Volver);
        botonCreditos.onClick.AddListener(VerCreditos);
        botonSalir.onClick.AddListener(Salir);

        ActualizarTextos();
    }

    void OnEnable()
    {
        sliderMusica.Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // si estan los creditos abiertos cierro esos primero
            if (panelCreditos.activeSelf)
            {
                panelCreditos.SetActive(false);
                botonCreditos.Select();
            }
            else
            {
                Volver();
            }
        }
    }

    public void CambiarMusica(float valor)
    {
        PlayerPrefs.SetFloat("volumenMusica", valor);
        if (musica != null)
        {
            musica.volume = valor;
        }
        ActualizarTextos();
    }

    public void CambiarEfectos(float valor)
    {
        PlayerPrefs.SetFloat("volumenEfectos", valor);
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        textoMusica.text = Mathf.RoundToInt(sliderMusica.value * 100) + "%";
        textoEfectos.text = Mathf.RoundToInt(sliderEfectos.value * 100) + "%";
    }

    public void Volver()
    {
        gameObject.SetActive(false);
        botonConfiguracionTitulo.Select();
    }

    public void VerCreditos()
    {
        panelCreditos.SetActive(true);
    }

    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
