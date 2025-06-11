using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Models.SettingsModel;

public class BeeController : MonoBehaviour
{
    public float speed = 1f;
    public Rigidbody bee;

    // Nos indica el número de flores blancas que ha tocado la abeja
    public Text countText;
    private int collisionCount = 0;

    // Nos indica el número de flores negras que ha tocado la abeja
    public Text countText2;
    private int collisionCount2 = 0;

    // Nos indica el número de flores blancas que la abeja no alcanzó a tocar
    public Text lostFlowersText;
    private int lostFlowersCount = 0;

    public delegate void BeeAction();
    public static event BeeAction CollisionWithFlower;
    public static event BeeAction CollisionWithFlowerCounter;
    public static event BeeAction CollisionWithBlackFlower;
    public static event BeeAction FlowerDestroyed;

    public delegate void BeeCounter(int count);
    public static event BeeCounter FlorNegra;
    public static event BeeCounter FlorBlanca;
    public static event BeeCounter FlorPerdida;

    public GameObject manoDerecha;
    public GameObject manoIzquierda;
    public GameObject manoRotacion;

    public enum Counter
    {
        BlackFlower = 1,
        WhiteFlower = 2,
        LostFlower = 3
    }

    // Settings
    //private SettingsModelRoot configuracionJuego = new SettingsModelRoot();

    private SettingsModelRoot settingsGame = new SettingsModelRoot()
    {
        mano = "Izquierda",
        nombreMovimiento = SettingsModelValuesRoot.Movimiento.PS,
        tiempoEnPosicion = "5 segundos",
        ordenAparacion = "Uno y uno",
        numeroSets = "1 set (10 ejercicios)",
        tiempoDescanso = "10 segundos",
        tiempoReaccion = "20 segundos (fácil)",
        conDistractores = "No",
        frecuenciaDistractores = "Baja",
        anguloArriba = 0,
        anguloAbajo = 0,
        anguloIzquierda = 0,
        anguloDerecha = 0,
    };

    private void OnEnable()
    {
        SpawnController.TimeOver += DestroyFlower;
        SpawnController.LostFlower += LostFlower;

        Giroscopio.HandUp += BeeUp;
        Giroscopio.HandLeft += BeeLeft;
        Giroscopio.HandDown += BeeDown;
        Giroscopio.HandRigth += BeeRigth;
        Giroscopio.HandNeutral += BeeNeutralPosition;

    }

    private void OnDisable()
    {
        SpawnController.TimeOver -= DestroyFlower;
        SpawnController.LostFlower -= LostFlower;

        Giroscopio.HandUp -= BeeUp;
        Giroscopio.HandLeft -= BeeLeft;
        Giroscopio.HandDown -= BeeDown;
        Giroscopio.HandRigth -= BeeRigth;
        Giroscopio.HandNeutral -= BeeNeutralPosition;

    }

    private void Start()
    {
        var settingsController = new SettingsController();
        settingsGame = settingsController.GetSettingsGame();

        if (settingsGame.mano == "Derecha")
        {
            manoDerecha.gameObject.SetActive(true);
            manoIzquierda.gameObject.SetActive(false);
        }
        else
        {
            manoDerecha.gameObject.SetActive(false);
            manoIzquierda.gameObject.SetActive(true);
        }
    }

    void LostFlower()
    {
        lostFlowersCount++;
        SetCounterText(Counter.LostFlower, lostFlowersCount, lostFlowersText);
    }

    void DestroyFlower()
    {
        GameObject[] objectsWithTag2 = GameObject.FindGameObjectsWithTag("coin");
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("BlackFlower");

        foreach (GameObject obj in objectsWithTag2)
        {
            Destroy(obj);
        }
        
        foreach (GameObject obj in objectsWithTag)
        {
            Destroy(obj);
        }
        if (FlowerDestroyed != null)
            FlowerDestroyed();

    }

    bool handUp = false;
    bool handDown = false;
    bool handLeft = false;
    bool handRigth = false;
    bool handNeutral = false;

    void BeeUp()
    {
        handUp = true;

        handNeutral = false;

        //Debug.LogWarning("Entre en BeeUp: " + handUp);
    }

    void BeeDown()
    {
        handDown = true;

        handNeutral = false;

        //Debug.LogWarning("Entre en BeeUp: " + handDown);
    }

    void BeeLeft()
    {
        handLeft = true;

        handNeutral = false;

        //Debug.LogWarning("Entre en BeeLeft: " + handLeft);
    }
    
    void BeeRigth()
    {
        handRigth = true;

        handNeutral = false;

        //Debug.LogWarning("Entre en BeeLeft: " + handRigth);
    }

    void BeeNeutralPosition()
    {
        handUp = false;
        handDown = false;
        handLeft = false;
        handRigth = false;

        handNeutral = true;
    }

    void FixedUpdate()
    {

        if (settingsGame.mano == "Derecha" && manoRotacion.transform.rotation.x > 135f)
        {
            manoRotacion.transform.rotation = new Quaternion(90, 180, 90, 0);
        }


        if (handUp && settingsGame.anguloAbajo >= 0)
        {
            //Debug.LogWarning("Entro en el handUp");
            this.transform.localPosition = new Vector3(0f, 1f, 3f);
        }
        else if (handDown)
        {
            //Debug.LogWarning("Entro en el handDown");
            this.transform.localPosition = new Vector3(0f, -1f, 3f);
        }
        else if(handLeft)
        {
            //Debug.LogWarning("Entro en el handLeft");
            this.transform.localPosition = new Vector3(-2f, 0.23f, 3f);
        }
        else if (handRigth)
        {
            //Debug.LogWarning("Entro en el handRigth");
            this.transform.localPosition = new Vector3(2f, 0.23f, 3f);
        }
        else if (handNeutral)
        {
            //Debug.LogWarning("Entro en el handNeutral");
            //this.transform.localPosition = new Vector3(0f, 0.23f, 3f);
            this.transform.localPosition = new Vector3(0f, -0.25f, 3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("coin"))
        {


            other.gameObject.SetActive(false);

            collisionCount++;
            SetCounterText(Counter.WhiteFlower, collisionCount, countText);
            if (CollisionWithFlower != null)
                CollisionWithFlower();
            if (CollisionWithFlowerCounter != null)
                CollisionWithFlowerCounter();

            GameObject[] objectsWithTag2 = GameObject.FindGameObjectsWithTag("BlackFlower");
            foreach (GameObject obj in objectsWithTag2)
            {
                Destroy(obj);
            }
        }

        if (other.gameObject.CompareTag("BlackFlower"))
        {
            other.gameObject.SetActive(false);

            GameObject[] objectsWithTag2 = GameObject.FindGameObjectsWithTag("coin");
            foreach (GameObject obj in objectsWithTag2)
            {
                Destroy(obj);
            }

            collisionCount2++;
            SetCounterText(Counter.BlackFlower, collisionCount2, countText2);
            if (CollisionWithBlackFlower != null)
                CollisionWithBlackFlower();

            if (FlowerDestroyed != null)
                FlowerDestroyed();
        }

    }

    void BeeMovement()
    {
        if (settingsGame.mano.ToLower() == "izquierda")
        {
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.localPosition = new Vector3(-2f, 0.23f, 3f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                this.transform.localPosition = new Vector3(2f, 0.23f, 3f);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                this.transform.localPosition = new Vector3(0f, 1f, 3f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                this.transform.localPosition = new Vector3(0f, -1f, 3f);
            }
            else
            {
                this.transform.localPosition = new Vector3(0f, 0.23f, 3f);
            } 
        }
        else if (settingsGame.mano.ToLower() == "derecha")
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.localPosition = new Vector3(-2f, 0.23f, 3f);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                this.transform.localPosition = new Vector3(2f, 0.23f, 3f);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                this.transform.localPosition = new Vector3(0f, 1f, 3f);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                this.transform.localPosition = new Vector3(0f, -1f, 3f);
            }
            else
            {
                //this.transform.localPosition = new Vector3(0f, 0.23f, 3f);
                this.transform.localPosition = new Vector3(0f, -0.23f, 3f);
            }
        }
    }

    void SetCounterText(Counter flower, int counter, Text text)
    {
        switch (flower)
        {
            case Counter.BlackFlower:
                text.text = "Flores negras: " + counter.ToString();
                if (FlorNegra != null)
                    FlorNegra(counter);
                break;
            case Counter.WhiteFlower:
                text.text = "Flores blancas: " + counter.ToString();
                if (FlorBlanca != null)
                    FlorBlanca(counter);
                break;
            case Counter.LostFlower:
                text.text = "Flores perdidas: " + counter.ToString();
                if (FlorPerdida != null)
                    FlorPerdida(counter);
                break;
        }
    }
}
