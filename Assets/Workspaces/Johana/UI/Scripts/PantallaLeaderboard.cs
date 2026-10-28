using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PantallaLeaderboard : MonoBehaviour
{
    public Text[] nombres;
    public Text[] metros;
    public Button botonMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < nombres.Length; i++)
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
        }

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
