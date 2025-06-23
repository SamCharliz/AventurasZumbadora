using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;


//calibracion

public class Calibracion : MonoBehaviour
{

    int panelActual = 0;
    // z variable que nos ayuda a contar las iteracioens
    // c variable que nos ayuda a saber si esta activa o no la corrutina
    int z = 0, yAnterior = 0, yAnteriorAnterior = 0, c = 0;
    public GameObject[] panel,hand; //Elementos de la interfaz grafica
    public GameObject barraCalibracionController, representacionMano;
    //Boton para pasar al siguiente 
    public Button[] botonSiguiente; 
    public static int valorMaximo = 0;
    public static int[] valoresMedios = new int[10];
    public static int[] valoresMaximos = new int[10];
    public static string data, hand, pivoteCuboAzul;
    public static bool Right, Left, up, down, neutral;//Configuración de la mano 
    public Toggle handRigth, handLeft, handDown, handUp, handNeutral; //Elementos de la interfaz de usuario
    public Text[] mensajesErrorMano,  mensajes; //Etiquetas para indicarle al usuario que esta usando la mano equivocada
    //Contador para indicarle al usuario cuanto tiempo le falta
    private Text textoReloj;
    private Text Mensaje; 
    private int Segundos;
    private int Intervalo = 1000;
    private bool pausa; 
    private Timer Reloj; 
    private string contenidoReloj; 
    private IEnumerator _coroutine,controlarBoton; 
    private bool[] zonas = { false, false, false, false, false, false, false, false, false, false };
    private bool[] zonasAnteriores = {false,false,false,false,false,false,false,false,false,false}; 
    

    void Awake()
    {
        valorMaximo = 0;
        derecha = true;
        izquierda = false;
        abierta = true;
        down = false;
        
 
 