//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Threading;
//using System;
//using System.IO.Ports;

//public class ESP32Controller : MonoBehaviour
//{
//    //public GameObject cuboRojo;
//    //public GameObject cuboAzul;
//    public GameObject pivoteCuboRojo;
//    public GameObject pivoteCuboAzul;
//    public GameObject pivoteCuboAzulEnCuboRojo;

//    private static string _devicePort = "";
//    public static string DevicePort
//    {
//        get
//        {
//            return _devicePort;
//        }

//    }
//    public static bool DeviceConnected = false;

//    [Tooltip("Texto que aparece cuando comienza la escena y que se elimina cuando se presiona el botón Conectar ")]
//    public Text calibration_text;
//    [Tooltip("Texto que aparece cuando se conecta el Dispositivo con exito")]
//    public Text exito_text;
//    [Tooltip("Texto que aparece cuando aparecen errores de tipo el puerto esta ocupado y no se puede conectar el dispositivo")]
//    public Text reintentarConectarElGuante_text;
//    public Image cargando_Image;
//    public Button conectarGuante_Button; //con este boton vamos a comenzar a calibrar 
//    public Button reintentar_Button, ayuda_Button;
//    public Button jugar_button; //este boton es para comenzar a jugar 
//    public GameObject Interfaz_Regresar;

//    public Text Indautor_mensaje;

//    private bool _sigueEjecutandoHilo;
//    public static SerialPort sp;
//    Thread myThread;
//    private Animator _animacionCargando;
//    Finger _finger_pressed;


//    public enum Finger
//    {
//        None,
//        Index,
//        Middle,
//        Ring,
//        Pinky,
//    }


//    //Muy importante simpre descativar los hilos cuando se cambia de escena
//    void OnDisable()
//    {
//        _sigueEjecutandoHilo = false;
//    }

//    Quaternion redAngle = new Quaternion(); // R de caja roja, ang de angulo
//    Quaternion redAngle2 = new Quaternion();
//    Quaternion blueAngle2 = new Quaternion(); // B de caja azul
//    //Quaternion angB = new Quaternion(); 
//    //Quaternion angR3 = new Quaternion();
//    //Quaternion angB3 = new Quaternion();
//    //Quaternion angR4 = new Quaternion();
//    //Quaternion angB4 = new Quaternion();
//    float redQuaternion1, redQuaternion2, redQuaternion3, redQuaternion4; // quaterniones rojos
//    float blueQuaternion1, blueQuaternion2, blueQuaternion3, blueQuaternion4; // quaterniones azules

//    void Awake()
//    {
//        _finger_pressed = Finger.None;
//        DeviceConnected = false;
//        _sigueEjecutandoHilo = false;
//        redAngle.Set(2f, 2f, 2f, 2f);

//    }

//    public void PreConnectGlove()
//    {
//        reintentarConectarElGuante_text.gameObject.SetActive(false);
//        reintentar_Button.gameObject.SetActive(false);
//        ayuda_Button.gameObject.SetActive(false);

//        StartCoroutine(ActivaBarraCargandoyLuegoConectaGuante());

//    }

//    IEnumerator ActivaBarraCargandoyLuegoConectaGuante()
//    {
//        _animacionCargando.SetTrigger("reset");
//        cargando_Image.gameObject.SetActive(true);
//        conectarGuante_Button.gameObject.SetActive(false);
//        calibration_text.gameObject.SetActive(false);
//        yield return new WaitForSeconds(2.0f);

//        ConnectGlove();

//    }


//    //Esta es la función que realizará la conexión con el guante
//    public void ConnectGlove()
//    {
//        //		foreach (string puertoSerial in SerialPort.GetPortNames()) {	//Se intentara conectar con cada uno de los puertos disponibles
//        //			sp = new SerialPort (puertoSerial, 9600); //9600 12000 -
//        //			Debug.Log (sp.PortName);
//        //		}

//        //		sp = new SerialPort ("\\\\.\\COM10", 9600); //9600 12000
//        //		sp.Open ();
//        //		sp.ReadTimeout = 2000; //estaba en 20ms  2000

//        if (_devicePort != "")
//        { //Ya sabemos en que COM se encuentra el guante
//            try
//            {
//                DeviceConnected = true;
//                _sigueEjecutandoHilo = true;
//                myThread = new Thread(new ThreadStart(ReadArduinoData));
//                myThread.Start();
//                //myThread.Join();

//                return;
//            }
//            catch (Exception ex)
//            {
//                Debug.Log("time out");
//                //Si esto no funciona si intentará con los demás puertos
//            }
//        }

//        foreach (string puertoSerial in SerialPort.GetPortNames())
//        {   //Se intentara conectar con cada uno de los puertos disponibles 
//            if (puertoSerial.Length > 4)
//            {
//                //sp = new SerialPort ("\\\\.\\" + puertoSerial, 9600);
//                sp = new SerialPort("\\\\.\\" + puertoSerial, 115200);
//            }
//            else
//            {
//                sp = new SerialPort(puertoSerial, 115200); //9600  
//            }
//            try
//            {
//                if (!sp.IsOpen)
//                {
//                    sp.Open();
//                }
//                sp.ReadTimeout = 1000; //estaba en 20ms  2000
//                Debug.Log(sp.PortName);

//                try
//                {
//                    string test = sp.ReadLine(); // original
//                    //if (test.Contains("58:bf:25:31:c1:f8"))
//                    if (test.Length > 25)
//                    {   //si el puerto se pudo abrir, se leera la cadena de datos que recibe el puerto, sabremos que se trata del guante si la cadena recibida tiene un tamaño de 20
//                        _devicePort = sp.PortName;
//                        DeviceConnected = true;
//                        _sigueEjecutandoHilo = true;
//                        myThread = new Thread(new ThreadStart(ReadArduinoData));
//                        myThread.Start();

//                        break;
//                    }
//                    sp.Close();
//                }
//                catch (Exception ex)
//                {
//                    Debug.Log("time out");

//                }
//            }
//            catch (Exception e)
//            {
//                Debug.Log("el puerto esta ocupado");

//            }
//        }
//    }




//    private void ReadArduinoData()
//    {

//        while (_sigueEjecutandoHilo)
//        {
//            if (sp.IsOpen && DeviceConnected)
//            {
//                try
//                {
//                    SerialDataProccessing(sp.ReadLine());
//                    Debug.Log(sp.ReadLine());
//                }
//                catch (System.Exception) { }
//            }

//        }
//    }


//    void SerialDataProccessing(string data)
//    {
//        string[] data_array = data.Split('/');  //es un arreglo de un solo elemento: data_array[0]=0000 58:bf:25:31:c1:f8/0.997864/0.013245/-0.030090/0.056213/0.999268/-0.005188/0.034668/-0.012207
//        //Debug.Log("Primer Quaternion"+ data_array[1]+" , " + data_array[2] + " , " + data_array[3] + " , " + data_array[4]);
//        //Debug.Log("Segundo Quaternion" + data_array[5] + " , " + data_array[6] + " , " + data_array[7] + " , " + data_array[8]);

//        //redQuaternion1 = float.Parse(data_array[1]);
//        //redQuaternion2 = float.Parse(data_array[2]);
//        //redQuaternion3 = float.Parse(data_array[3]);
//        //redQuaternion4 = float.Parse(data_array[4]);

//        //blueQuaternion1 = float.Parse(data_array[5]);
//        //blueQuaternion2 = float.Parse(data_array[6]);
//        //blueQuaternion3 = float.Parse(data_array[7]);
//        //blueQuaternion4 = float.Parse(data_array[8]);

//        //angB.Set(float.Parse(data_array[5]), float.Parse(data_array[6]), float.Parse(data_array[7]), float.Parse(data_array[8]));
//        //cuboAzul.transform.rotation = angB;
//        //cuboRojo.transform.rotation = redAngle;
//    }
//    Vector3 eulerBlue;
//    private void Update()
//    {
//        //redAngle.Set(redQuaternion1,redQuaternion2,redQuaternion3,redQuaternion4);
//        //angB.Set(blueQuaternion1,blueQuaternion2,blueQuaternion3,blueQuaternion4);
//        //cuboAzul.transform.rotation = angB;
//        //cuboRojo.transform.rotation = redAngle;

//        //redAngle2.Set(redQuaternion1, redQuaternion2, redQuaternion4, redQuaternion3); // quaterniones rojos
//        //redAngle2.Set(0, 0, 0, 0); // quaterniones azules
//        //blueAngle2.Set(blueQuaternion1, blueQuaternion2, blueQuaternion4, blueQuaternion3); // quaterniones azules
//        //blueAngle2.Set(0, 0, 0, 0); // quaterniones azules
//        pivoteCuboAzul.transform.localRotation = blueAngle2;
//        //Por una extraña razon debo invertir Y y Z, pero esto depende del dispositivo, no es normal
//        /*eulerBlue = pivoteCuboAzul.transform.localRotation.eulerAngles;
//        pivoteCuboAzul.transform.localRotation = Quaternion.Euler(eulerBlue.x, eulerBlue.y*-1f, eulerBlue.z*-1f);  
//        */
//        pivoteCuboRojo.transform.rotation = redAngle2;
//        pivoteCuboAzul.transform.position = pivoteCuboAzulEnCuboRojo.transform.position;
//        //cuboAzul2.transform.rotation = blueAngle2;
//        //cuboRojo2.transform.rotation = redAngle2;


//    }
//}
