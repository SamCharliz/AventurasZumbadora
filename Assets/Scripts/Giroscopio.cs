using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Scripts.Models.SettingsModel;

public class Giroscopio : MonoBehaviour
{

    //public GameObject cuboRojo;
    //public GameObject cuboAzul;
    public GameObject pivoteCuboRojo;
    public GameObject pivoteCuboAzul;
    public GameObject pivoteCuboAzulEnCuboRojo;
    public GameObject padreCajas;

    // Texto que indica si se pudo conectar el sensor
    public Text sensorConcetado;
    public Text sensorConcetadoCorrecto;
    public Text textoConectar;
    // Promedio calibracion
    public Text promedioCalibracion;
    // Boton para continuar a la calibracion de la mano
    public Button continueButton;
    public Button connectButton;
    // Panel de resumen donde se pregunta si se desean calibrar los sensores
    public GameObject modalConectarrSensor;
    // Panel en el que se pregunta si esta bien la calibracion o se desea volver a hacer
    public GameObject modalCalibrarNuevamente;

    private static string _devicePort = "";
    public static string DevicePort
    {
        get
        {
            return _devicePort;
        }
    }
    public static bool DeviceConnected = false;

    [Tooltip("Texto que aparece cuando comienza la escena y que se elimina cuando se presiona el botón Conectar ")]
    public Text calibration_text;
    [Tooltip("Texto que aparece cuando se conecta el Dispositivo con exito")]
    public Text exito_text;
    [Tooltip("Texto que aparece cuando aparecen errores de tipo el puerto esta ocupado y no se puede conectar el dispositivo")]
    public Text reintentarConectarElGuante_text;
    public Image cargando_Image;
    public Button conectarGuante_Button; //con este boton vamos a comenzar a calibrar 
    public Button reintentar_Button, ayuda_Button;
    public Button jugar_button; //este boton es para comenzar a jugar 
    public GameObject Interfaz_Regresar;

    public Text Indautor_mensaje;


    public Text promedioCalibracionArribaText;
    public Text promedioCalibracionAbajoText;
    public Text promedioCalibracionIzquierdaText;
    public Text promedioCalibracionDerechaText;

    private bool _sigueEjecutandoHilo;
    public static SerialPort sp;
    Thread myThread;
    private Animator _animacionCargando;
    Finger _finger_pressed;



    public delegate void GiroscopioAction();
    //public static event GiroscopioAction HandRotation;
    //public static event GiroscopioAction HandNeutralPosition;

    public static event GiroscopioAction HandUp;
    public static event GiroscopioAction HandLeft;
    public static event GiroscopioAction HandDown;
    public static event GiroscopioAction HandRigth;
    public static event GiroscopioAction HandNeutral;

    public enum Finger
    {
        None,
        Index,
        Middle,
        Ring,
        Pinky,
    }

    private void OnEnable()
    {
        hand.EnterUp += BeeEnterUp;
        hand.EnterDown += BeeEnterDown;
        hand.Exit += BeeExit;

    }


    //Muy importante simpre descativar los hilos cuando se cambia de escena
    void OnDisable()
    {
        _sigueEjecutandoHilo = false;
        hand.EnterUp -= BeeEnterUp;
        hand.EnterDown -= BeeEnterDown;
        hand.Exit -= BeeExit;
    }

    bool isUp = false;
    bool isDown = false;
    bool isRotated = false;

    void BeeEnterUp()
    {
        isUp = true;
    }
    void BeeEnterDown()
    {
        isDown = true;
    }
    void BeeExit()
    {
        isUp = false;
        isDown = false;
    }

    void BeeRotated()
    {
        isRotated = true;
    }

    void BeeNeutral()
    {
        isRotated = false;
    }

    public void ContinueButton()
    {
        modalConectarrSensor.gameObject.SetActive(false);
    }

    Quaternion angR = new Quaternion();
    Quaternion angB = new Quaternion();
    Quaternion angR2 = new Quaternion();
    Quaternion angB2 = new Quaternion();
    Quaternion angR3 = new Quaternion();
    Quaternion angB3 = new Quaternion();
    Quaternion angR4 = new Quaternion();
    Quaternion angB4 = new Quaternion();
    float q1, q2, q3, q4;
    float qu1, qu2, qu3, qu4;

    #region Configuraciones del juego

    public GameObject manoDerecha;
    public GameObject manoIzquierda;

    public Transform manoDerechaTransform;
    public Transform manoIzquierdaTransform;
    public Transform brazoTransform;



    private SettingsModelRoot configuracionJuego = new SettingsModelRoot()
    {
        mano = "Izquierda",
        nombreMovimiento = SettingsModelValuesRoot.Movimiento.CFEP,
        tiempoEnPosicion = "10 segundos",
        ordenAparacion = "Aleatorio",
        numeroSets = "3 sets (30 ejercicios)",
        tiempoDescanso = "10 segundos",
        tiempoReaccion = "20 segundos (fácil)",
        conDistractores = "Si",
        frecuenciaDistractores = "Media",
        anguloArriba = 0,
        anguloAbajo = 0,
        anguloIzquierda = 0,
        anguloDerecha = 0,
    };

    #endregion

    void Awake()
    {
        _finger_pressed = Finger.None;
        DeviceConnected = false;
        _sigueEjecutandoHilo = false;
        angR.Set(2f, 2f, 2f, 2f);
        //DontDestroyOnLoad(this.gameObject); // prueba para ver que se siga ejecutando el hilo
    }

    int numeroDeCalibraciones;
    void Start()
    {
        var settingsController = new SettingsController();
        configuracionJuego = settingsController.GetSettingsGame();

        orientaciones2 = SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento];

        //Debug.Log("Mano con la que se va a jugar");

        if (configuracionJuego.mano == "Derecha")
        {
            manoDerecha.gameObject.SetActive(true);
            manoIzquierda.gameObject.SetActive(false);
        }
        else
        {
            manoDerecha.gameObject.SetActive(false);
            manoIzquierda.gameObject.SetActive(true);
        }



        //ConnectGlove();

        // Primera calibración
        //ComenzarCalibracion();
        MuestraMovimiento();
    }

    public void PreConnectGlove()
    {
        reintentarConectarElGuante_text.gameObject.SetActive(false);
        reintentar_Button.gameObject.SetActive(false);
        ayuda_Button.gameObject.SetActive(false);

        StartCoroutine(ActivaBarraCargandoyLuegoConectaGuante());

    }

    IEnumerator ActivaBarraCargandoyLuegoConectaGuante()
    {
        _animacionCargando.SetTrigger("reset");
        cargando_Image.gameObject.SetActive(true);
        conectarGuante_Button.gameObject.SetActive(false);
        calibration_text.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);

        ConnectGlove();

    }


    //Esta es la función que realizará la conexión con el guante
    public void ConnectGlove()
    {
        numeroDeCalibraciones = SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento].Count;

        //		foreach (string puertoSerial in SerialPort.GetPortNames()) {	//Se intentara conectar con cada uno de los puertos disponibles
        //			sp = new SerialPort (puertoSerial, 9600); //9600 12000 -
        //			Debug.Log (sp.PortName);
        //		}

        //		sp = new SerialPort ("\\\\.\\COM10", 9600); //9600 12000
        //		sp.Open ();
        //		sp.ReadTimeout = 2000; //estaba en 20ms  2000

        if (_devicePort != "")
        { //Ya sabemos en que COM se encuentra el guante
            try
            {
                DeviceConnected = true;
                _sigueEjecutandoHilo = true;
                myThread = new Thread(new ThreadStart(ReadArduinoData));
                myThread.Start();

                return;
            }
            catch (Exception ex)
            {
                Debug.Log("time out 1");
                //Si esto no funciona si intentará con los demás puertos
            }
        }

        foreach (string puertoSerial in SerialPort.GetPortNames())
        {   //Se intentara conectar con cada uno de los puertos disponibles 
            if (puertoSerial.Length > 4)
            {
                //sp = new SerialPort ("\\\\.\\" + puertoSerial, 9600);
                sp = new SerialPort("\\\\.\\" + puertoSerial, 115200);
            }
            else
            {
                sp = new SerialPort(puertoSerial, 115200); //9600  
            }
            try
            {
                if (!sp.IsOpen)
                {
                    sp.Open();
                }
                sp.ReadTimeout = 1000; //estaba en 20ms  2000
                //Debug.Log(sp.PortName);

                try
                {
                    string test = sp.ReadLine(); // original
                    //if (test.Contains("58:bf:25:31:c1:f8"))
                    if (test.Length > 25)
                    {   //si el puerto se pudo abrir, se leera la cadena de datos que recibe el puerto, sabremos que se trata del guante si la cadena recibida tiene un tamaño de 20
                        _devicePort = sp.PortName;
                        DeviceConnected = true;
                        _sigueEjecutandoHilo = true;
                        // Mandar mensaje de que se conecto el dispositivo
                        sensorConcetadoCorrecto.text = "Haz clic en 'Continuar' para proceder con la calibración.\n" +
                                               "Por favor, ultilza el brazo con el que realizarás la terapia.";
                        sensorConcetado.text = "";
                        textoConectar.text = "";
                        continueButton.gameObject.SetActive(true);
                        connectButton.gameObject.SetActive(false);
                        myThread = new Thread(new ThreadStart(ReadArduinoData));
                        myThread.Start();


                        break;
                    }
                    sp.Close();
                }
                catch (Exception ex)
                {
                    Debug.Log("time out 2");
                    sensorConcetado.text = "No se pudo conectar. Haz clic en 'Conectar' para volver a intentarlo.";
                }
            }
            catch (Exception e)
            {
                Debug.Log("el puerto esta ocupado");
                sensorConcetado.text = "No se pudo conectar. Haz clic en 'Conectar' para volver a intentarlo.";

            }
        }
    }


    private void ReadArduinoData()
    {

        while (_sigueEjecutandoHilo)
        {
            if (sp.IsOpen && DeviceConnected)
            {
                try
                {
                    SerialDataProccessing(sp.ReadLine());
                    //Debug.Log(sp.ReadLine());
                }
                catch (System.Exception) { }
            }

        }
    }


    void SerialDataProccessing(string data)
    {
        string[] data_array = data.Split('/');  //es un arreglo de un solo elemento: data_array[0]=0000 58:bf:25:31:c1:f8/0.997864/0.013245/-0.030090/0.056213/0.999268/-0.005188/0.034668/-0.012207
        //Debug.Log("Primer Quaternion"+ data_array[1]+" , " + data_array[2] + " , " + data_array[3] + " , " + data_array[4]);
        //Debug.Log("Segundo Quaternion" + data_array[5] + " , " + data_array[6] + " , " + data_array[7] + " , " + data_array[8]);

        q1 = float.Parse(data_array[1]);
        q2 = float.Parse(data_array[2]);
        q3 = float.Parse(data_array[3]);
        q4 = float.Parse(data_array[4]);

        qu1 = float.Parse(data_array[5]);
        qu2 = float.Parse(data_array[6]);
        qu3 = float.Parse(data_array[7]);
        qu4 = float.Parse(data_array[8]);

        //angB.Set(float.Parse(data_array[5]), float.Parse(data_array[6]), float.Parse(data_array[7]), float.Parse(data_array[8]));
        //cuboAzul.transform.rotation = angB;
        //cuboRojo.transform.rotation = angR;
    }

    Vector3 eulerBlue;
    float angle4;
    private void Update()
    {
        //angR.Set(q1,q2,q3,q4);
        //angB.Set(qu1,qu2,qu3,qu4);
        //cuboAzul.transform.rotation = angB;
        //cuboRojo.transform.rotation = angR;

        angR2.Set(q1, q2, q4, q3);
        angB2.Set(qu1, qu2, qu4, qu3);
        pivoteCuboAzul.transform.localRotation = angB2; // está es la mano


        //Por una extraña razon debo invertir Y y Z, pero esto depende del dispositivo, no es normal
        /*eulerBlue = pivoteCuboAzul.transform.localRotation.eulerAngles;
        pivoteCuboAzul.transform.localRotation = Quaternion.Euler(eulerBlue.x, eulerBlue.y*-1f, eulerBlue.z*-1f);  
        */

        //pivoteCuboRojo.transform.rotation = angR2; // este es el brazo

        padreCajas.transform.rotation = Quaternion.Euler(angR2.eulerAngles.x * -1, 0f, 0f); // Saber rotación de cubos
        var anguloMinimoCalibracion = -10;
        var anguloMaximoCalibracion = 10;
        if (angR2.eulerAngles.x * -1 < anguloMinimoCalibracion || 
            angR2.eulerAngles.x * -1 > anguloMaximoCalibracion)
        {
            BeeRotated();
        }
        else
        {
            BeeNeutral();
        }

        pivoteCuboAzul.transform.position = pivoteCuboAzulEnCuboRojo.transform.position;
        //cuboAzul2.transform.rotation = angB2;
        //cuboRojo2.transform.rotation = angR2;

        Vector3 mano3 = manoIzquierdaTransform.up;
        Vector3 brazo3 = brazoTransform.up;
        float angle3 = Vector3.Angle(mano3, brazo3);
        angle4 = angle3;
        //Debug.Log("Up " + (angle3));

        //if (Input.GetKey(KeyCode.A))
        //{
        //    banderaSiguienteMovimiento = false;
        //}


        if (isUp)
        {
            if (isRotated && configuracionJuego.mano == "Derecha")
            {
                // derecha
                ManoMovimiento(SettingsModelValuesRoot.Orientacion.DERECHA);
            }
            else if (isRotated && configuracionJuego.mano == "Izquierda")
            {
                // izquierda
                ManoMovimiento(SettingsModelValuesRoot.Orientacion.IZQUIERDA);
            }
            else
            {
                // arriba
                ManoMovimiento(SettingsModelValuesRoot.Orientacion.ARRIBA);
            }
        }
        else if (isDown)
        {
            if (isRotated && configuracionJuego.mano == "Derecha")
            {
                // izquierda
                ManoMovimiento(SettingsModelValuesRoot.Orientacion.IZQUIERDA);
            }
            else if (isRotated && configuracionJuego.mano == "Izquierda")
            {
                // derecha
                ManoMovimiento(SettingsModelValuesRoot.Orientacion.DERECHA);
            }
            else
            {
                // abajo
                ManoMovimiento(SettingsModelValuesRoot.Orientacion.ABAJO);
            }
        }
        else
        {
            // neutral
            ManoMovimiento(SettingsModelValuesRoot.Orientacion.NEUTRAL);
        }

        //Debug.Log("Movimientos totales: " + numeroDeCalibraciones);
        //Debug.Log("Movimientos calibrados: " + contadorDeMovimietos2);
        if (contadorDeMovimietos2 - 1 < numeroDeCalibraciones && contadorDeMovimietos2 - 1 >= 0)
        {

            // Esperamos a que se oprima la barra espaciadora para
            // calcular el promedio y mostrar la siguiente calibracion
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Cambio de movimiento");

                MuestraMovimiento();
                GuardarCalibracion();
            } 
        }
        
    }

    List<float> calibrationValues = new List<float>();
    int counterCalibration = 0;
    int numeroDeCalibracionesPorMovimiento = 3;
    public Text indicacionDeMovimientoText;
    public Text manoEnMovimiento;
    public void SaveCalibration()
    {
        var movimientosDeMano = SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento];

        if (counterCalibration < (movimientosDeMano.Count * numeroDeCalibracionesPorMovimiento))
        {
            Debug.Log("Calibrando sensores: " + counterCalibration);

            var direccion = string.Empty;

            var direccionAux = movimientosDeMano[counterCalibration % numeroDeCalibracionesPorMovimiento];

            switch (direccionAux)
            {
                case SettingsModelValuesRoot.Orientacion.ABAJO:
                    direccion = "abajo";
                    break;
                case SettingsModelValuesRoot.Orientacion.ARRIBA:
                    direccion = "arriba";
                    break;
                case SettingsModelValuesRoot.Orientacion.DERECHA:
                    direccion = "derecha";
                    break;
                case SettingsModelValuesRoot.Orientacion.IZQUIERDA:
                    direccion = "izquierda";
                    break;
            }

            indicacionDeMovimientoText.text = "Mover la mano a la " + direccion;

            Vector3 mano3 = manoIzquierdaTransform.up;
            Vector3 brazo3 = brazoTransform.up;
            float angle3 = Vector3.Angle(mano3, brazo3);
            calibrationValues.Add(angle3);
            // Cada posición de la mano debe calibrarse tres veces y de ahí se saca el promedioArriba
            counterCalibration++;
        }
    }

    public void PrintCalibration()
    {
        Debug.Log("A continuación se mostraran las calibraciones:");
        foreach (var item in calibrationValues)
        {
            Debug.Log("Angulo: " + (item));
        }
    }


    //bool banderaSiguienteMovimiento = true;
    //List<SettingsModelValuesRoot.Orientacion> orientaciones2 = SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento];
    List<SettingsModelValuesRoot.Orientacion> orientaciones2;
    int countOrientaciones = 0;


    int contadorDeMovimietos2 = 0;
    public void MuestraMovimiento()
    {
        var orientaciones = SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento];
        orientaciones.AddRange(SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento]);
        orientaciones.AddRange(SettingsModelValuesRoot.nombreMovimientoValores[configuracionJuego.nombreMovimiento]);
        orientaciones2 = orientaciones;

        var orientacion = orientaciones[contadorDeMovimietos2];
        Debug.Log("Muestra movimiento: " + orientacion);

        switch (orientacion)
        {
            case SettingsModelValuesRoot.Orientacion.ABAJO:
                indicacionDeMovimientoText.text = "Mover la mano hacia abajo " + contadorDeMovimietos2;
                break;
            case SettingsModelValuesRoot.Orientacion.ARRIBA:
                indicacionDeMovimientoText.text = "Mover la mano hacia arriba " + contadorDeMovimietos2;
                break;
            case SettingsModelValuesRoot.Orientacion.DERECHA:
                indicacionDeMovimientoText.text = "Mover la mano hacia la derecha " + contadorDeMovimietos2;
                break;
            case SettingsModelValuesRoot.Orientacion.IZQUIERDA:
                indicacionDeMovimientoText.text = "Mover la mano hacia izquierda " + contadorDeMovimietos2;
                break;
            case SettingsModelValuesRoot.Orientacion.NEUTRAL:
                indicacionDeMovimientoText.text = "Posicion neutral " + contadorDeMovimietos2;
                break;
        }

        contadorDeMovimietos2++;

    }


    public void CalibrarNuevamente()
    {
        listCalibracionesArriba.Clear();
        listCalibracionesAbajo.Clear();
        listCalibracionesIzquierda.Clear();
        listCalibracionesDerecha.Clear();
        contadorDeMovimietos2 = 0;
        MuestraMovimiento();
        modalCalibrarNuevamente.gameObject.SetActive(false);
    }

    public void GuardarCalibracion()
    {
        var orientacion = orientaciones2[contadorDeMovimietos2];

        switch (orientacion)
        {
            case SettingsModelValuesRoot.Orientacion.ABAJO:
                listCalibracionesAbajo.Add(angle4);
                break;
            case SettingsModelValuesRoot.Orientacion.ARRIBA:
                listCalibracionesArriba.Add(angle4);
                break;
            case SettingsModelValuesRoot.Orientacion.IZQUIERDA:
                listCalibracionesIzquierda.Add(angle4);
                break;
            case SettingsModelValuesRoot.Orientacion.DERECHA:
                listCalibracionesDerecha.Add(angle4);
                break;
        }

        CalcularPromedioCalibracion();

        if (contadorDeMovimietos2 - 1 >= numeroDeCalibraciones)
        {
            Debug.Log("Cambio de panel");

            modalCalibrarNuevamente.gameObject.SetActive(true);
        }
    }

    List<float> listCalibracionesArriba = new List<float>();
    List<float> listCalibracionesAbajo = new List<float>();
    List<float> listCalibracionesIzquierda = new List<float>();
    List<float> listCalibracionesDerecha = new List<float>();
    public void CalcularPromedioCalibracion()
    {
        var orientacion = orientaciones2[contadorDeMovimietos2];

        switch (orientacion)
        {
            case SettingsModelValuesRoot.Orientacion.ABAJO:
                configuracionJuego.anguloAbajo = Promedio(listCalibracionesArriba, orientacion);
                promedioCalibracionAbajoText.text = "Abajo: " + configuracionJuego.anguloAbajo;
                break;
            case SettingsModelValuesRoot.Orientacion.ARRIBA:
                configuracionJuego.anguloArriba = Promedio(listCalibracionesAbajo, orientacion);
                promedioCalibracionArribaText.text = "Arriba:  " + configuracionJuego.anguloArriba;
                break;
            case SettingsModelValuesRoot.Orientacion.IZQUIERDA:
                configuracionJuego.anguloIzquierda = Promedio(listCalibracionesIzquierda, orientacion);
                promedioCalibracionIzquierdaText.text = "Izquierda: " + configuracionJuego.anguloIzquierda;
                break;
            case SettingsModelValuesRoot.Orientacion.DERECHA:
                configuracionJuego.anguloDerecha = Promedio(listCalibracionesDerecha, orientacion);
                promedioCalibracionDerechaText.text = "Derecha: " + configuracionJuego.anguloDerecha;
                break;
        }
    }

    public float Promedio(List<float> calibraciones, SettingsModelValuesRoot.Orientacion orientacion)
    {

        float promedio = 0;

        foreach (var item in calibraciones)
        {
            promedio += item;
        }

        promedio = promedio / calibraciones.Count;

        return promedio;
    }
    public void ManoMovimiento(SettingsModelValuesRoot.Orientacion orientacion)
    {
        switch (orientacion)
        {
            case SettingsModelValuesRoot.Orientacion.ABAJO:
                if (HandDown != null)
                    HandDown();
                manoEnMovimiento.text = "Mano: abajo";
                break;
            case SettingsModelValuesRoot.Orientacion.ARRIBA:
                if (HandUp != null)
                    HandUp();
                manoEnMovimiento.text = "Mano: arriba";
                break;
            case SettingsModelValuesRoot.Orientacion.DERECHA:
                if (HandRigth != null)
                    HandRigth();
                manoEnMovimiento.text = "Mano: derecha";
                break;
            case SettingsModelValuesRoot.Orientacion.IZQUIERDA:
                if (HandLeft != null)
                    HandLeft();
                manoEnMovimiento.text = "Mano: izquierda";
                break;
            case SettingsModelValuesRoot.Orientacion.NEUTRAL:
                if (HandNeutral != null)
                    HandNeutral();
                manoEnMovimiento.text = "Mano: neutral";
                break;
        }
    }

}
