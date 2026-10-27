using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDJuego : MonoBehaviour
{
    [Header("Tiempo")]
    public Image panelTiempo;
    public Text textoTiempo;
    public Text textoTarde;
    public Sprite spritePanelNormal;
    public Sprite spritePanelTarde;
    public Color colorTiempoNormal;
    public Color colorTiempoCritico;

    [Header("Distancia")]
    public RectTransform barraFill;
    public RectTransform barraBrillo;
    public Text textoDistanciaJefe;
    public Text textoRecorrido;

    [Header("Memorandos")]
    public Image[] memos;
    public Sprite spriteMemoVacio;
    public Sprite spriteMemoSellado;

    [Header("Calendario")]
    public Text textoNumeroDia;

    [Header("Popups")]
    public GameObject plantillaPopup;
    public Sprite iconoSuma;
    public Sprite iconoResta;
    public Color colorSuma;
    public Color colorResta;

    private float anchoBarra = 440;
    private int memosActuales = 0;

    public void MostrarTiempo(float tiempo)
    {
        int minutos = (int)tiempo / 60;
        int segundos = (int)tiempo % 60;
        textoTiempo.text = minutos.ToString("00") + ":" + segundos.ToString("00");

        if (tiempo <= 0)
        {
            textoTiempo.gameObject.SetActive(false);
            textoTarde.gameObject.SetActive(true);
            panelTiempo.sprite = spritePanelTarde;
        }
        else
        {
            textoTiempo.gameObject.SetActive(true);
            textoTarde.gameObject.SetActive(false);
            panelTiempo.sprite = spritePanelNormal;

            if (tiempo < 10)
            {
                textoTiempo.color = colorTiempoCritico;
            }
            else
            {
                textoTiempo.color = colorTiempoNormal;
            }
        }
    }

    // progreso va de 0 a 1
    public void MostrarProgreso(float metrosAlJefe, float progreso)
    {
        textoDistanciaJefe.text = (int)metrosAlJefe + " m";

        float ancho = anchoBarra * progreso;
        barraFill.sizeDelta = new Vector2(ancho, 28);
        barraBrillo.sizeDelta = new Vector2(ancho - 12, 6);

        barraFill.gameObject.SetActive(ancho > 16);
        barraBrillo.gameObject.SetActive(ancho > 24);
    }

    public void MostrarRecorrido(float metros)
    {
        textoRecorrido.text = ((int)metros).ToString("N0") + " m";
    }

    public void MostrarMemorandos(int cantidad)
    {
        for (int i = 0; i < memos.Length; i++)
        {
            if (i < cantidad)
            {
                memos[i].sprite = spriteMemoSellado;
            }
            else
            {
                memos[i].sprite = spriteMemoVacio;
            }
        }

        if (cantidad > memosActuales)
        {
            StartCoroutine(AnimarSello(memos[cantidad - 1].transform));
        }
        memosActuales = cantidad;
    }

    public void MostrarDia(int numero)
    {
        textoNumeroDia.text = numero.ToString();
    }

    public void MostrarPopupTiempo(int segundos)
    {
        GameObject popup = Instantiate(plantillaPopup, plantillaPopup.transform.parent);
        popup.SetActive(true);

        Image icono = popup.GetComponentInChildren<Image>();
        Text texto = popup.GetComponentInChildren<Text>();

        if (segundos > 0)
        {
            icono.sprite = iconoSuma;
            texto.text = "+" + segundos + "s";
            texto.color = colorSuma;
        }
        else
        {
            icono.sprite = iconoResta;
            texto.text = segundos + "s";
            texto.color = colorResta;
        }

        StartCoroutine(SubirPopup(popup));
    }

    IEnumerator AnimarSello(Transform memo)
    {
        memo.localScale = new Vector3(1.4f, 1.4f, 1);
        yield return new WaitForSeconds(0.15f);
        memo.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator SubirPopup(GameObject popup)
    {
        for (int i = 0; i < 10; i++)
        {
            popup.transform.Translate(Vector3.up * 6);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(popup);
    }
}
