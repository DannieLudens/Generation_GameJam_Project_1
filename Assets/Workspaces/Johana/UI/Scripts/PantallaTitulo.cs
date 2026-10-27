using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaTitulo : MonoBehaviour
{
    public Button botonJugar;
    public Button botonOpciones;
    public Button botonSalir;
    public string escenaJuego;

    void Start()
    {
        botonJugar.onClick.AddListener(Jugar);
        botonOpciones.onClick.AddListener(Opciones);
        botonSalir.onClick.AddListener(Salir);
        botonJugar.Select();
    }

    public void Jugar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    public void Opciones()
    {
        Debug.Log("Opciones");
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
