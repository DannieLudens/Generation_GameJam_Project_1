using UnityEngine;
using UnityEngine.EventSystems;

public class BotonFeedback : MonoBehaviour
{
    void Update()
    {
        // el boton seleccionado se ve un poco mas grande
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            transform.localScale = new Vector3(1.06f, 1.06f, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
