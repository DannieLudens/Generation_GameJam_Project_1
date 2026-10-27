using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PantallaPausa : MonoBehaviour
{
    public GameObject panelPausa;
    public Button botonReanudar;
    public Button botonSalir;

    private bool pausado = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelPausa.SetActive(false);
        botonReanudar.onClick.AddListener(Reanudar);
        botonSalir.onClick.AddListener(SalirAlMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        pausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0;
        botonReanudar.Select();
    }

    public void Reanudar()
    {
        pausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1;
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Titulo");
    }
}
