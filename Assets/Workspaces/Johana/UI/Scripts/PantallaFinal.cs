using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaFinal : MonoBehaviour
{
    public bool ocultarAlIniciar = true;
    public RectTransform sello;
    public CanvasGroup grupoContenido;
    public Text textoMetros;
    public Text textoDias;
    public Text textoMemorandos;
    public int diasTotales;
    public Button botonPrincipal;
    public Button botonMenu;
    public string escenaMenu;

    private bool mostrar = false;

    void Awake()
    {
        botonPrincipal.onClick.AddListener(Reintentar);
        botonMenu.onClick.AddListener(IrAlMenu);

        if (ocultarAlIniciar && !mostrar)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        StartCoroutine(Animar());
    }

    public void Mostrar(float metros, int dias, int memorandos)
    {
        textoMetros.text = ((int)metros).ToString("N0") + " m";
        textoDias.text = dias.ToString();

        // solo la pantalla de aprobado tiene memorandos y dias totales
        if (diasTotales > 0)
        {
            textoDias.text = dias + " / " + diasTotales;
        }
        if (textoMemorandos != null)
        {
            textoMemorandos.text = memorandos + " / 3";
        }

        mostrar = true;
        gameObject.SetActive(true);
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }

    IEnumerator Animar()
    {
        grupoContenido.alpha = 0;

        // el sello cae
        sello.localScale = new Vector3(2, 2, 1);
        for (int i = 0; i < 10; i++)
        {
            sello.localScale = sello.localScale - new Vector3(0.1f, 0.1f, 0);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.4f);
        grupoContenido.alpha = 1;
        botonPrincipal.Select();
    }
}
