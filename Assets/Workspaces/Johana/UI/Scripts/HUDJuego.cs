using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDJuego : MonoBehaviour
{
    public Image panelTiempo;
    public Text textoTiempo;
    public Text textoTarde;
    public Sprite spritePanelNormal;
    public Sprite spritePanelTarde;
    public Color colorTiempoNormal;
    public Color colorTiempoCritico;
    public GameObject vinetaRoja;

    public Text textoRecorrido;

    public Image[] memos;
    public Sprite spriteMemoVacio;
    public Sprite spriteMemoSellado;

    public Text textoNumeroDia;

    public GameObject plantillaPopup;
    public Sprite iconoSuma;
    public Sprite iconoResta;
    public Color colorSuma;
    public Color colorResta;

    public GameObject avisoATiempo;
    public GameObject vinetaVerde;
    public Text textoATiempo;

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
            vinetaRoja.SetActive(true);
        }
        else
        {
            textoTiempo.gameObject.SetActive(true);
            textoTarde.gameObject.SetActive(false);
            panelTiempo.sprite = spritePanelNormal;

            if (tiempo < 10)
            {
                textoTiempo.color = colorTiempoCritico;
                vinetaRoja.SetActive(true);
            }
            else
            {
                textoTiempo.color = colorTiempoNormal;
                vinetaRoja.SetActive(false);
            }
        }
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

    // cuando llega a tiempo donde el jefe
    public void MostrarATiempo(int diaNuevo)
    {
        textoATiempo.text = "+1:00  ·  Día " + diaNuevo;
        StartCoroutine(ATiempo());
    }

    IEnumerator ATiempo()
    {
        avisoATiempo.SetActive(true);
        vinetaVerde.SetActive(true);
        yield return new WaitForSeconds(2);
        avisoATiempo.SetActive(false);
        vinetaVerde.SetActive(false);
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
