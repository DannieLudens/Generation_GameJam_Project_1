using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaGameOver : MonoBehaviour
{
    public Text textoMetros;
    public Text textoDias;
    public GameObject bloqueNombre;
    public InputField campoNombre;
    public Button botonGuardar;
    public Button botonReintentar;
    public Button botonMenu;

    private int metros = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        botonGuardar.onClick.AddListener(Guardar);
        botonReintentar.onClick.AddListener(Reintentar);
        botonMenu.onClick.AddListener(IrAlMenu);
        campoNombre.onValueChanged.AddListener(Mayusculas);
        campoNombre.onSubmit.AddListener(GuardarConEnter);
    }

    void OnEnable()
    {
        campoNombre.text = PlayerPrefs.GetString("ultimoNombre", "");
        Revisar();

        if (bloqueNombre.activeSelf)
        {
            campoNombre.Select();
        }
        else
        {
            botonReintentar.Select();
        }
    }

    // si entra al top 10 pido el nombre, si no solo reintentar o menu
    void Revisar()
    {
        if (EntraEnTop10())
        {
            bloqueNombre.SetActive(true);
            botonGuardar.gameObject.SetActive(true);
            botonMenu.gameObject.SetActive(false);
        }
        else
        {
            bloqueNombre.SetActive(false);
            botonGuardar.gameObject.SetActive(false);
            botonMenu.gameObject.SetActive(true);
        }
    }

    public void PonerDatos(int metrosRecorridos, int dias)
    {
        metros = metrosRecorridos;
        textoMetros.text = metros.ToString("N0") + " m";
        textoDias.text = dias + " días";
        Revisar();
    }

    void GuardarConEnter(string texto)
    {
        Guardar();
    }

    void Mayusculas(string texto)
    {
        campoNombre.text = texto.ToUpper();
    }

    bool EntraEnTop10()
    {
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetString("nombre" + i, "") == "" || metros > PlayerPrefs.GetInt("metros" + i, 0))
            {
                return true;
            }
        }
        return false;
    }

    public void Guardar()
    {
        string nombre = campoNombre.text;
        if (nombre == "")
        {
            nombre = "JOHN";
        }
        PlayerPrefs.SetString("ultimoNombre", nombre);

        // busco en que puesto queda (si hay empate se queda abajo)
        int puesto = -1;
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetString("nombre" + i, "") == "" || metros > PlayerPrefs.GetInt("metros" + i, 0))
            {
                puesto = i;
                break;
            }
        }

        if (puesto == -1)
        {
            return;
        }

        // corro los de abajo una fila
        for (int i = 9; i > puesto; i--)
        {
            PlayerPrefs.SetString("nombre" + i, PlayerPrefs.GetString("nombre" + (i - 1), ""));
            PlayerPrefs.SetInt("metros" + i, PlayerPrefs.GetInt("metros" + (i - 1), 0));
        }

        PlayerPrefs.SetString("nombre" + puesto, nombre);
        PlayerPrefs.SetInt("metros" + puesto, metros);
        PlayerPrefs.SetInt("filaNueva", puesto);

        Time.timeScale = 1;
        SceneManager.LoadScene("Leaderboard");
    }

    public void Reintentar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Titulo");
    }
}
