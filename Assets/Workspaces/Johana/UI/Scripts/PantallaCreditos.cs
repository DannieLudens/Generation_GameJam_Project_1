using UnityEngine;
using UnityEngine.UI;

public class PantallaCreditos : MonoBehaviour
{
    public bool ocultarAlIniciar = true;
    public Button botonVolver;

    private bool mostrar = false;

    void Awake()
    {
        botonVolver.onClick.AddListener(Cerrar);

        if (ocultarAlIniciar && !mostrar)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        botonVolver.Select();
    }

    public void Mostrar()
    {
        mostrar = true;
        gameObject.SetActive(true);
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
    }
}
