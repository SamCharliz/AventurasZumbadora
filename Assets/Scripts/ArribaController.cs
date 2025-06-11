using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArribaController : MonoBehaviour
{
    private bool estaColisionando = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("hand"))
        {
            estaColisionando = true;
            // Haz algo cuando el GameObject colisiona con "OtroObjeto"
            Debug.Log("El GameObject ha colisionado con el otro objeto.");
        }
        if (collision.gameObject.CompareTag("arriba"))
        {
            estaColisionando = true;
            // Haz algo cuando el GameObject colisiona con "OtroObjeto"
            Debug.Log("El GameObject ha colisionado con el otro objeto.");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("hand"))
        {
            estaColisionando = false;
            // Haz algo cuando el GameObject deja de colisionar con "OtroObjeto"
            Debug.Log("El GameObject ha dejado de colisionar con el otro objeto.");
        }
        
        if (collision.gameObject.CompareTag("arriba"))
        {
            estaColisionando = false;
            // Haz algo cuando el GameObject deja de colisionar con "OtroObjeto"
            Debug.Log("El GameObject ha dejado de colisionar con el otro objeto.");
        }
    }

    private void Update()
    {
        if (estaColisionando)
        {
            // Realiza acciones adicionales mientras los objetos están colisionando
            Debug.Log("Los objetos siguen colisionando.");
        }
    }
}
