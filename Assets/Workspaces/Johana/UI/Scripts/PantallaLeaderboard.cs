using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PantallaLeaderboard : MonoBehaviour
{
    public Image[] filas;
    public Text[] nombres;
    public Text[] metros;
    public Sprite filaNormal;
    public Sprite filaDestacada;
    public Button botonMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int filaNueva = PlayerPrefs.GetInt("filaNueva", -1);

        for (int i = 0; i < filas.Length; i++)
        {
            string nombre = PlayerPrefs.GetString("nombre" + i, "");

            if (nombre == "")
            {
                nombres[i].text = "—";
                metros[i].text = "—";
            }
            else
            {
                nombres[i].text = nombre;
                metros[i].text = PlayerPrefs.GetInt("metros" + i, 0).ToString("N0") + " m";
            }

            if (i == filaNueva)
            {
                filas[i].sprite = filaDestacada;
            }
            else
            {
                filas[i].sprite = filaNormal;
            }
        }

        PlayerPrefs.SetInt("filaNueva", -1);
        botonMenu.onClick.AddListener(VolverAlMenu);
        botonMenu.Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            VolverAlMenu();
        }
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Titulo");
    }
}
