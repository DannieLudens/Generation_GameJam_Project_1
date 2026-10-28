using UnityEngine;
using UnityEngine.UI;

public class PantallaCreditos : MonoBehaviour
{
    public Button botonVolver;
    public Button botonCreditos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        botonVolver.onClick.AddListener(Cerrar);
    }

    void OnEnable()
    {
        botonVolver.Select();
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
        botonCreditos.Select();
    }
}
