using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AvisoMemorando : MonoBehaviour
{
    public GameObject aviso;
    public Text textoContador;
    public Image[] memos;
    public Sprite memoVacio;
    public Sprite memoSellado;
    public GameObject pantallaGameOver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aviso.SetActive(false);
    }

    public void MostrarMemorando(int numero)
    {
        textoContador.text = "MEMORANDO " + numero + " DE 3";

        for (int i = 0; i < memos.Length; i++)
        {
            if (i < numero)
            {
                memos[i].sprite = memoSellado;
            }
            else
            {
                memos[i].sprite = memoVacio;
            }
        }

        StartCoroutine(Mostrar(numero));
    }

    IEnumerator Mostrar(int numero)
    {
        aviso.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        aviso.SetActive(false);

        // con el tercero se acaba
        if (numero >= 3)
        {
            pantallaGameOver.SetActive(true);
        }
    }
}
