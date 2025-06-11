using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Models.SettingsModel;

public class hand : MonoBehaviour
{
    bool isUp = false;
    bool isDown = false;

    public delegate void HandAction();
    public static event HandAction EnterUp;
    public static event HandAction EnterDown;
    public static event HandAction Exit;

    

    private void Update()
    {
        if (!(isDown && isUp))
        {
            //Debug.Log("La mano está en posición neutra.");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("OnTriggerEnter");

        if (collision.gameObject.CompareTag("arriba"))
        {
            // Haz algo cuando el GameObject colisiona con "OtroObjeto"
            Debug.LogWarning("La mano está tocando arriba.");
            isUp = true;
            if (EnterUp != null)
                EnterUp();
            
        }
        
        if (collision.gameObject.CompareTag("abajo"))
        {
            // Haz algo cuando el GameObject colisiona con "OtroObjeto"
            Debug.LogWarning("La mano está tocando abajo.");
            isDown = true;
            if (EnterDown != null)
                EnterDown();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("OnTriggerExit");
        if (collision.gameObject.CompareTag("arriba"))
        {
            // Haz algo cuando el GameObject deja de colisionar con "OtroObjeto"
            Debug.LogWarning("La mano dejo de tocar arriba.");
            isUp = false;
            if (Exit != null)
                Exit();
        }

        if (collision.gameObject.CompareTag("abajo"))
        {
            // Haz algo cuando el GameObject colisiona con "OtroObjeto"
            Debug.LogWarning("La mano dejo de tocar abajo.");
            isDown = false;
            if (Exit != null)
                Exit();
        }
    }
}
