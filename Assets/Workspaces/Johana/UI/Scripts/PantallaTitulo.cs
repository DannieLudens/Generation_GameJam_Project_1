using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaTitulo : MonoBehaviour
{
    public Button botonJugar;
    public Button botonLeaderboard;
    public Button botonConfiguracion;
    public Button botonSalir;
    public GameObject panelConfiguracion;
    public string escenaJuego;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        botonJugar.onClick.AddListener(Jugar);
        botonLeaderboard.onClick.AddListener(VerLeaderboard);
        botonConfiguracion.onClick.AddListener(AbrirConfiguracion);
        botonSalir.onClick.AddListener(Salir);

        panelConfiguracion.SetActive(false);
        botonJugar.Select();
    }

    public void Jugar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    public void VerLeaderboard()
    {
        // desde el titulo no se resalta ninguna fila
        PlayerPrefs.SetInt("filaNueva", -1);
        SceneManager.LoadScene("Leaderboard");
    }

    public void AbrirConfiguracion()
    {
        panelConfiguracion.SetActive(true);
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
