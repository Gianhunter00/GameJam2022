using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAppearance : MonoBehaviour
{
    public GameObject MyCanvas;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MyCanvas.SetActive(true);
        Debug.Log("Enter");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MyCanvas.SetActive(false);

    }
}
