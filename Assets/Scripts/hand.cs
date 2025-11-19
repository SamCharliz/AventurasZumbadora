using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Models.SettingsModel;

public class hand : MonoBehaviour
{
    public Text angulo_text;

    public GameObject azulPivote;
    // Umbrales de detección
    public float umbralArriba = 30f;
    public float umbralAbajo = -30f;
    public float umbralNeutro = 10f;

    // Estado actual
    private bool estaArriba = false;
    private bool estaAbajo = false;
    private bool estaNeutra = true;

    // Evento para otros scripts
    public delegate void HandAction();
    public static event HandAction EnterUp;
    public static event HandAction EnterDown;
    public static event HandAction Exit;

    // Ángulo actual y temporizador para mostrarlo
    private float anguloActual = 0f;
    private float tiempoDeMuestra = 10f; // segundos que se muestra el ángulo
    private float temporizador = 0f;
    private bool mostrarAngulo = false;
    private float anguloOffset = 0f; // se usará para calibrar el 0
                                     // UI para mostrar instrucciones
    public Text mensajeCalibracionText;
    // Configuración
    public float tiempoParaConfirmar = 5f; // Tiempo que debe mantener la posición
    private float tiempoQuieto = 0f;
    private bool enCalibracion = false;
    private float anguloMaximoDetectado = 0f;
    private bool yaSeGuardo = false;



    void Update()
    {
        anguloActual = NormalizarAngulo(azulPivote.transform.localEulerAngles.x) - anguloOffset;
        Debug.Log(anguloActual);
        // Si se está mostrando el ángulo, cuenta regresiva
        if (mostrarAngulo)
        {
            temporizador -= Time.deltaTime;
            if (temporizador <= 0f)
            {
                mostrarAngulo = false;
            }
        }

        if (enCalibracion)
{
    if (Mathf.Abs(anguloActual) > umbralNeutro)
    {
        tiempoQuieto += Time.deltaTime;

        // Mostrar texto durante la espera
        mensajeCalibracionText.text = "Mantenga la posición: " + (tiempoParaConfirmar - tiempoQuieto).ToString("F1") + "s";

        // Guardar el ángulo más alto detectado
        if (Mathf.Abs(anguloActual) > Mathf.Abs(anguloMaximoDetectado))
        {
            anguloMaximoDetectado = anguloActual;
        }

        if (tiempoQuieto >= tiempoParaConfirmar && !yaSeGuardo)
        {
            yaSeGuardo = true;
            mensajeCalibracionText.text = "Ángulo guardado: " + anguloMaximoDetectado.ToString("F2") + "°";
            Debug.Log("Ángulo máximo registrado: " + anguloMaximoDetectado);
            
        }
    }
    else
    {
        mensajeCalibracionText.text = "Mueva la mano hasta la posición deseada.";
        tiempoQuieto = 0f;
    }
}


        // Detectar estado según el ángulo
        if (anguloActual > umbralArriba)
        {
            if (!estaArriba)
            {
                estaArriba = true;
                estaAbajo = false;
                estaNeutra = false;
                Debug.Log("Muñeca extendida (arriba)");
                EnterUp?.Invoke();
                IniciarMostrarAngulo();
            }
        }
        else if (anguloActual < umbralAbajo)
        {
            if (!estaAbajo)
            {
                estaArriba = false;
                estaAbajo = true;
                estaNeutra = false;
                Debug.Log("Muñeca flexionada (abajo)");
                EnterDown?.Invoke();
                IniciarMostrarAngulo();
            }
        }
        else if (Mathf.Abs(anguloActual) <= umbralNeutro)
        {
            if (!estaNeutra)
            {
                estaArriba = false;
                estaAbajo = false;
                estaNeutra = true;
                Debug.Log("Muñeca en posición neutra");
                Exit?.Invoke();
                IniciarMostrarAngulo();
            }
        }

        //if (mostrarAngulo)
        angulo_text.text = "Ángulo muñeca: " + anguloActual.ToString("F2") + "°";

    }

    // Mostrar el ángulo durante cierto tiempo
    void IniciarMostrarAngulo()
    {
        mostrarAngulo = true;
        temporizador = tiempoDeMuestra;
    }

    // Convertir 0–360 a -180 a 180
    private float NormalizarAngulo(float angulo)
    {
        if (angulo > 180f)
            angulo -= 360f;
        return angulo;
    }

    public void CalibrarCentro()
    {
        anguloOffset = NormalizarAngulo(azulPivote.transform.localEulerAngles.x);
        Debug.Log("Calibración realizada. Nuevo centro en: " + anguloOffset);
    }
    
    public void IniciarCalibracion()
{
    enCalibracion = true;
    tiempoQuieto = 0f;
    yaSeGuardo = false;
    anguloMaximoDetectado = 0f;
    mensajeCalibracionText.text = "Coloque la mano en posición y mantenga...";
}



}
