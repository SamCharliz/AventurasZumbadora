using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Scripts.Models.SettingsModel;
using static PartidaAbejaModel;

public class SpawnController : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject blackFlowerPrefab;
    public GameObject player;
    public Transform  camara;

    private float _duracionPartida = 0f;

    // Configuracion del tiempo que debe de mantener el paciente cierta posicion
    // Contador del tiempo que se tiene para tomar la flor
    public Text   countText;
    private float timeRemaining;
    private bool timerIsRunning = false;
    
    // Contador del tiempo que se tiene para descansar
    public Text countRestText;
    private float timeRestRemaining;
    private bool  restTimerIsRunning = false;

    // Contador que indica el número de flores que han salido en el juego
    private int totalFlowers = 0;
    public Text   totalFlowerCountText;

    // Contador que indica el tiempo que el jugador debe de mantener la mano en cierta posición
    private float timePositionRemainig = 0;
    private float timePositionRemainigCopy = 0;
    public Text positionTimeCountText;
    private bool positionTimerIsRunning = false;

    // Nos indica el número de flores blancas que se tomaron pero se soltaron antes de tiempo
    public Text dropFlowersCountText;
    private int dropFlowersCount = 0;

    public Text PartidaTerminadaText;

    public delegate void SpawnAction();
    public static event SpawnAction TimeOver;
    public static event SpawnAction PartidaTerminada;
    public static event SpawnAction LostFlower;

    public delegate void SpawnGameOver(bool isGameOver);
    public static event SpawnGameOver CollectedFlowers;

    private GameObject monedaTemporal;
    private GameObject blackFlowerTemporal;

    // Settings
    private SettingsModelRoot settingsGame = new SettingsModelRoot()
    {
        mano = "Derecha",
        nombreMovimiento = SettingsModelValuesRoot.Movimiento.CFEPN,
        tiempoEnPosicion = "10 segundos",
        ordenAparacion = "Aleatorio",
        numeroSets = "3 sets (30 ejercicios)",
        tiempoDescanso = "10 segundos",
        tiempoReaccion = "20 segundos (fácil)",
        conDistractores = "Si",
        frecuenciaDistractores = "Media",
    };

    private static Partida resultadosPartida = new Partida();

    // Aqui se guarda la informacion de la partida
    private PartidaAbejaModelRoot resultadosDelJuego;

    private void OnEnable()
    {
        BeeController.CollisionWithFlower += disableCounters;
        BeeController.CollisionWithBlackFlower += disableCounters;
        BeeController.FlowerDestroyed += SummonFlower2;
        BeeController.CollisionWithFlower += AbejaChocaConFlor;
        BeeController.CollisionWithFlowerCounter += ActivateTimePositionCounter;
        BeeController.FlorNegra += SetFlorNegra;
        BeeController.FlorBlanca += SetFlorBlanca;
        BeeController.FlorPerdida += SetFlorPerdida;
        
    }

    private void OnDisable()
    {
        BeeController.CollisionWithFlower -= disableCounters;
        BeeController.CollisionWithBlackFlower -= disableCounters;
        BeeController.FlowerDestroyed -= SummonFlower2;
        BeeController.CollisionWithFlower -= AbejaChocaConFlor;
        BeeController.CollisionWithFlowerCounter -= ActivateTimePositionCounter;
        BeeController.FlorNegra -= SetFlorNegra;
        BeeController.FlorBlanca -= SetFlorBlanca;
        BeeController.FlorPerdida -= SetFlorPerdida;
    }

    private bool shouldRunCoroutine = false;
    List<int> orderFlower = new List<int>();
    List<Tuple<SettingsModelValuesRoot.Orientacion, int>> orderSpawnFlower = new List<Tuple<SettingsModelValuesRoot.Orientacion, int>>();
    bool partidaTerminada = false;

    public void SetFlorNegra(int value)
    {
        resultadosPartida.fallos = value;
    }
    
    public void SetFlorBlanca(int value)
    {
        resultadosPartida.aciertos = value;
    }

    public void SetFlorPerdida(int value)
    {
        resultadosPartida.falsosAciertos = value;
    }

    void Start()
    {
        Time.timeScale = 1;
        isWhitheFlower = false;
        var settingsController = new SettingsController();
        settingsGame = settingsController.GetSettingsGame();
        resultadosDelJuego = new PartidaAbejaModelRoot();
        resultadosDelJuego.settings = settingsGame;

        #region Creamos el orden en que saldrán las flores en el juego

        var numberOfSets = SettingsModelValuesRoot.numeroSetsValores[settingsGame.numeroSets];
        var isWithDistractor = SettingsModelValuesRoot.conDistractoresValores[settingsGame.conDistractores];

        if (isWithDistractor)
        {
            var frequencyDistractors = SettingsModelValuesRoot.frecuenciaDistractoresValores[settingsGame.frecuenciaDistractores];

            var percentflowersWithDistractor = ((double)numberOfSets / 100) * frequencyDistractors;
            var flowersWithDistractor = Math.Truncate(percentflowersWithDistractor);
            var flowers = numberOfSets - flowersWithDistractor;

            orderFlower.AddRange(Enumerable.Repeat(0, (int)flowers));
            orderFlower.AddRange(Enumerable.Repeat(1, (int)flowersWithDistractor));

            // Desordenar la lista
            System.Random rand = new System.Random();
            int n = orderFlower.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                int temp = orderFlower[i];
                orderFlower[i] = orderFlower[j];
                orderFlower[j] = temp;
            }
        }
        else
        {
            orderFlower.AddRange(Enumerable.Repeat(0, numberOfSets));
        }

        var orientaciones = SettingsModelValuesRoot.nombreMovimientoValores[settingsGame.nombreMovimiento];

        var aux = 0;

        //var orden = SettingsModelValuesRoot.ordenAparacionValores[/*configuracionJuego.ordenAparacion*/"Uno y uno"];
        var orden = SettingsModelValuesRoot.ordenAparacionValores[settingsGame?.ordenAparacion ?? "Uno y uno"];

        if (orden.Count == 1 && orden.Contains(1))
        {
            Debug.LogWarning("Caso de uno y uno");
            for (int i = 0; i < numberOfSets; i++)
            {
                orderSpawnFlower.Add(new Tuple<SettingsModelValuesRoot.Orientacion, int>(orientaciones.ElementAt(aux), orderFlower.ElementAt(i)));
            
                if (aux < orientaciones.Count() - 1)
                    aux++;
                else
                    aux = 0;
            }
        }
        else if (orden.Count == 1 && orden.Contains(2))
        {
            Debug.LogWarning("Caso de dos y dos");

            var repeatPositionList = new List<SettingsModelValuesRoot.Orientacion>(){ };

            var axu2 = 0;
            for (int i = 0; i < numberOfSets/2; i++)
            {
                
                repeatPositionList.Add(orientaciones.ElementAt(axu2));
                repeatPositionList.Add(orientaciones.ElementAt(axu2));
                
                if (axu2 < orientaciones.Count() - 1)
                {
                    axu2++;
                }
                else
                {
                    axu2 = 0;
                }
            }
            
            for (int i = 0; i < numberOfSets; i++)
            {
                orderSpawnFlower.Add(new Tuple<SettingsModelValuesRoot.Orientacion, int>(repeatPositionList.ElementAt(i), orderFlower.ElementAt(i)));
            }
        }
        else
        {
            Debug.LogWarning("Caso aleatorio");

            var elementsPerMovement = ((double)numberOfSets / orientaciones.Count());
            var repeatPositionList = new List<SettingsModelValuesRoot.Orientacion>() { };

            foreach (var item in orientaciones)
            {
                repeatPositionList.AddRange(Enumerable.Repeat(item, (int)Math.Truncate(elementsPerMovement))); 
            }

            // Desordenar la lista
            System.Random rand = new System.Random();
            int n = repeatPositionList.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                var temp = repeatPositionList[i];
                repeatPositionList[i] = repeatPositionList[j];
                repeatPositionList[j] = temp;
            }

            for (int i = 0; i < numberOfSets; i++)
            {
                orderSpawnFlower.Add(new Tuple<SettingsModelValuesRoot.Orientacion, int>(repeatPositionList.ElementAt(i), orderFlower.ElementAt(i)));
            }
        }

        //var posAux = 0;
        //foreach (var item in orderSpawnFlower)
        //{
        //    Debug.LogWarning("Pos " + posAux + ": " + item.Item1 + " " + item.Item2);
        //}

        // Imprimir la lista desordenada
        //var stringList = "Lista : ";
        //foreach (var num in orderFlower)
        //{
        //    stringList += num + " ";
        //}

        //Debug.LogWarning(stringList);

        #endregion

        //enableCounter("flor");
        //SpawnFlower(coinPrefab, SettingsModelValuesRoot.tiempoReaccionValores[configuracionJuego.tiempoReaccion], false);
        //totalFlowers++;
        //SetCountText("totalFlowers", totalFlowers);

        resultadosPartida.fecha = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        resultadosPartida.mano = settingsGame.mano;
        resultadosPartida.nombreMovimiento = SettingsModelValuesRoot.nombreMovimiento[settingsGame.nombreMovimiento.ToString()];
        resultadosPartida.tiempoEnPosicion = SettingsModelValuesRoot.tiempoEnPosicionValores[settingsGame.tiempoEnPosicion].FirstOrDefault();
        resultadosPartida.ordenAparacion = settingsGame.ordenAparacion;
        resultadosPartida.numeroDeFloresTotales = SettingsModelValuesRoot.numeroSetsValores[settingsGame.numeroSets];
        resultadosPartida.tiempoDescanso = SettingsModelValuesRoot.tiempoDescansoValores[settingsGame.tiempoDescanso];
        resultadosPartida.tiempoReaccion = SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion];
        //resultadosPartida.conDistractores = settingsGame.conDistractores;
        resultadosPartida.frecuenciaDistractores = settingsGame.conDistractores == "No" ? 0 : SettingsModelValuesRoot.frecuenciaDistractoresValores[settingsGame.frecuenciaDistractores];


        enableCounter("flor");
        var flowerSpawnSettings = orderFlower.ElementAt(0);
        SpawnFlower(coinPrefab, SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion], flowerSpawnSettings == 1 ? true : false, orderSpawnFlower.ElementAt(0).Item1);
        //totalFlowers++;
        //SetCountText("totalFlowers", 1);
    }
    private void Update()
    {
        if (!partidaTerminada)
        {
            _duracionPartida += Time.deltaTime; 
        }


        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                SetCountText("reaction", timeRemaining);
            }
            else
            {
                countText.gameObject.SetActive(false);
                timerIsRunning = false;
                if (LostFlower != null)
                    LostFlower();
            }
        }

        if (restTimerIsRunning)
        {
            if (timeRestRemaining > 0)
            {
                timeRestRemaining -= Time.deltaTime;
                SetCountText("restTime", timeRestRemaining);
            }
            else
            {
                countRestText.gameObject.SetActive(false);
                restTimerIsRunning = false;
            }
        }

        if (positionTimerIsRunning)
        {
            //Debug.Log("POSITION TIMER ACTIVE");

            if (manteniendoPos)
            {
                //Debug.Log("Estoy manteniendo pos");
                if (timePositionRemainig > 0)
                {
                    //Debug.Log("POSITION TIMER:" + timePositionRemainig);

                    timePositionRemainig -= Time.deltaTime;
                    SetCountText("position", timePositionRemainig);
                }
                else  //ya paso el tiempo que debe mantener la posición
                {
                    positionTimeCountText.gameObject.SetActive(false);
                    positionTimerIsRunning = false;
                    StartCoroutine(DestruyeFlor(0f));
                } 
            }
            else   //Dejo de mantener la posicion antes de tiempo
            {
                positionTimeCountText.gameObject.SetActive(false);
                positionTimerIsRunning = false;
                //Debug.Log("Deje de mantener pos antes de tiempo :(");
                StartCoroutine(DestruyeFlor(0f));

            }
        }

        //Debug.Log("partidaTerminada" + partidaTerminada);
        //Debug.Log("totalFlowers + 1 = " + (totalFlowers + 1));
        //Debug.Log("numeroSetsValores = " + SettingsModelValuesRoot.numeroSetsValores[settingsGame.numeroSets]);
        if (!partidaTerminada && (totalFlowers + 1) == SettingsModelValuesRoot.numeroSetsValores[settingsGame.numeroSets])
        {
            Debug.Log("Se va a mandar a llamar TerminaPartidas");
            partidaTerminada = true;
            StartCoroutine(TerminaPartida());
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            activatePositionTimer();
            var isActiveTimer = positionTimerIsRunning ? "true" : "false";
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            //Debug.Log("Space key is held down");
            positionTimeCountText.gameObject.SetActive(false);
            positionTimerIsRunning = false;
        }

        if ((Input.GetKeyUp(KeyCode.A) || 
            Input.GetKeyUp(KeyCode.D) || 
            Input.GetKeyUp(KeyCode.W) || 
            Input.GetKeyUp(KeyCode.S) || 
            Input.GetKeyUp(KeyCode.LeftArrow) || 
            Input.GetKeyUp(KeyCode.RightArrow) || 
            Input.GetKeyUp(KeyCode.UpArrow) || 
            Input.GetKeyUp(KeyCode.DownArrow)) && 
            positionTimerIsRunning == true)
        {
            Debug.Log("detecte que regrese al origen");   
            manteniendoPos = false;
            positionTimeCountText.gameObject.SetActive(false);
            //positionTimerIsRunning = false;
            dropFlowersCount++;
            SetCountText("dropFlowers", dropFlowersCount);
        }
    }

    public void activatePositionTimer()
    {
        if (positionTimerIsRunning == false)
        {
            enableCounter("position");
        }
    }

    IEnumerator TerminaPartida()
    {
        resultadosPartida.duracionPartida = _duracionPartida;
        yield return new WaitForSeconds(SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion]);
        //Time.timeScale = 0;
        PartidaTerminadaText.text = "Partida terminada. Bien hecho.";
        yield return new WaitForSeconds(3.0f);
        // Aquí mandamos a llamar al método que guardara la información de la partida
        GameMaster.GuardaResultadosDePartida(resultadosPartida);
        Debug.Log("Se manda a llamr el evento");
        if (PartidaTerminada != null)
            PartidaTerminada();
    }

    public void SummonFlower2() 
    {
        StartCoroutine(InvokeFlower());
    }

    Coroutine vidaDeFlor;
    bool manteniendoPos = false;
    public void SummonFlower() 
    {
        //Debug.Log("Entro en SummonFlower y va a DestruyeFlor con 10");
        vidaDeFlor = StartCoroutine(DestruyeFlor(SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion]));
    }

    public void AbejaChocaConFlor()
    {
        StopCoroutine(vidaDeFlor);

        var positionTimeList = SettingsModelValuesRoot.tiempoEnPosicionValores[settingsGame.tiempoEnPosicion];
        System.Random randomPositionTime = new System.Random();
        timePositionRemainig = timePositionRemainigCopy = positionTimeList.ElementAt(randomPositionTime.Next(0, positionTimeList.Count - 1));

        //Debug.Log("Tiempo de espera: " + timePositionRemainigCopy);

        if (positionTimeList.ElementAt(randomPositionTime.Next(0, positionTimeList.Count - 1)) > 1)
        {
            //Debug.Log("Se activa contador");
            //Debug.Log("Tiempo de espera: " + timePositionRemainigCopy);
            enableCounter("position");
            manteniendoPos = true;
        }
        else
        {
            StartCoroutine(DestruyeFlor(0f));
        }
    }

    IEnumerator DestruyeFlor(float time)
    {
        //Debug.Log("Entro en DestruyeFlor con " +time);
        yield return new WaitForSeconds(time);
        //Debug.Log("Termino de esperar el DestruyeFlor con " + time);
        if (TimeOver != null)
            TimeOver();
    }
    
    public void ActivateTimePositionCounter() 
    {
        //Debug.Log("Se activaraaaa contador");
        isWhitheFlower = true;
    }

    private int IteratorFlower = 0;
    private bool isWhitheFlower;

    IEnumerator InvokeFlower() 
    {
        //Debug.Log("Entro en el InvokeFlower");       

        // Tiempo para volver a la posición inicial
        yield return new WaitForSeconds(3);

        if (totalFlowers <= SettingsModelValuesRoot.numeroSetsValores[settingsGame.numeroSets] + 1)
        {
            Debug.Log("Invocando flor " + totalFlowers);


            totalFlowers++;

            SetCountText("totalFlowers", totalFlowers);

            var flowerSpawnSettings = orderFlower.ElementAt(IteratorFlower);
            //Debug.Log(flowerSpawnSettings);


            if (IteratorFlower % 10 == 0 && IteratorFlower != 0)
            {
                enableCounter("descanso");
                yield return new WaitForSeconds(SettingsModelValuesRoot.tiempoDescansoValores[settingsGame.tiempoDescanso]);
                
                enableCounter("flor");
                SpawnFlower(coinPrefab, SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion], flowerSpawnSettings == 1 ? true : false, orderSpawnFlower.ElementAt(IteratorFlower).Item1);
            }
            else
            {
                enableCounter("flor");
                SpawnFlower(coinPrefab, SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion], flowerSpawnSettings == 1 ? true : false, orderSpawnFlower.ElementAt(IteratorFlower).Item1);

                yield return new WaitForSeconds(SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion]);

            }


        }
        else {
            
        }

    }

    void SpawnFlower(GameObject flower, float timerDestroy, bool isWithDistractor, SettingsModelValuesRoot.Orientacion orientacion)
    {

        monedaTemporal = Instantiate(flower, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
        monedaTemporal.transform.SetParent(player.transform);
        //monedaTemporal.transform.Rotate(0f, 180f, 0f);

        if (isWithDistractor)
        {
            blackFlowerTemporal = Instantiate(blackFlowerPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
            blackFlowerTemporal.transform.SetParent(player.transform);
            //blackFlowerTemporal.transform.Rotate(0f, 180f, 0f);
            //Destroy(blackFlowerTemporal.gameObject, 3); // Aquí dice que dura tres segundos la flor negra
            Destroy(blackFlowerTemporal.gameObject, SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion]);
        }
        
        float spawnDirection = UnityEngine.Random.Range(0, 4);
        if (SettingsModelValuesRoot.Orientacion.IZQUIERDA == orientacion)
        {
            monedaTemporal.transform.localPosition = new Vector3(-2f, 0.23f, 3f);
        }
        else if (SettingsModelValuesRoot.Orientacion.DERECHA == orientacion)
        {
            monedaTemporal.transform.localPosition = new Vector3(2f, 0.23f, 3f);
        }
        else if (SettingsModelValuesRoot.Orientacion.ARRIBA == orientacion)
        {
            monedaTemporal.transform.localPosition = new Vector3(0f, 1f, 3f);
        }
        else if (SettingsModelValuesRoot.Orientacion.ABAJO == orientacion)
        {
            monedaTemporal.transform.localPosition = new Vector3(0f, -1f, 3f);
        }

        if (isWithDistractor)
        {
            var listPosiciones = new List<SettingsModelValuesRoot.Orientacion>()
            {
                SettingsModelValuesRoot.Orientacion.ARRIBA,
                SettingsModelValuesRoot.Orientacion.DERECHA,
                SettingsModelValuesRoot.Orientacion.ABAJO,
                SettingsModelValuesRoot.Orientacion.IZQUIERDA
            };

            //var listPosiciones = SettingsModelValuesRoot.nombreMovimientoValores[settingsGame.nombreMovimiento];

            var spawnDirection2 = UnityEngine.Random.Range(0, 4);
            var posicion2 = listPosiciones.ElementAt(spawnDirection2);
            do
            {
                spawnDirection2 = UnityEngine.Random.Range(0, 4);
                posicion2 = listPosiciones.ElementAt(spawnDirection2);

            } while (posicion2 == orientacion);

            //if (SettingsModelValuesRoot.Orientacion.IZQUIERDA == posicion2)
            if (SettingsModelValuesRoot.Orientacion.IZQUIERDA == orientacion)
            {
                blackFlowerTemporal.transform.localPosition = new Vector3(2f, 0.23f, 3f); // derecha
            }
            //else if (SettingsModelValuesRoot.Orientacion.DERECHA == posicion2)
            else if (SettingsModelValuesRoot.Orientacion.DERECHA == orientacion)
            {
                blackFlowerTemporal.transform.localPosition = new Vector3(-2f, 0.23f, 3f); // izquierda
            }
            //else if (SettingsModelValuesRoot.Orientacion.ARRIBA == posicion2)
            else if (SettingsModelValuesRoot.Orientacion.ARRIBA == orientacion)
            {
                blackFlowerTemporal.transform.localPosition = new Vector3(0f, -1f, 3f); // abajo
            }
            //else if (SettingsModelValuesRoot.Orientacion.ABAJO == posicion2)
            else if (SettingsModelValuesRoot.Orientacion.ABAJO == orientacion)
            {
                blackFlowerTemporal.transform.localPosition = new Vector3(0f, 1f, 3f); // arriba
            }

            blackFlowerTemporal.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        }

        monedaTemporal.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        vidaDeFlor = StartCoroutine(DestruyeFlor(SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion]));
        IteratorFlower++;
    }

    void SetCountText(string counterType, float value)
    {
        switch (counterType)
        {
            case "reaction":
                countText.text = Math.Truncate(value + 1).ToString() + " segundos...";
                break;
            case "restTime":
                countRestText.text = "Descansa " + Math.Truncate(value + 1).ToString() + " segundos...";
                break;
            case "totalFlowers":
                totalFlowerCountText.text = "Flor " + Math.Truncate(value + 1).ToString() + "/" + SettingsModelValuesRoot.numeroSetsValores[settingsGame.numeroSets];
                break;
            case "position":
                positionTimeCountText.text = Math.Truncate(value + 1).ToString() + "s";
                break;
            case "dropFlowers":
                dropFlowersCountText.text = "Flores soltadas:" + Math.Truncate(value).ToString();
                break;
        }
    }

    void disableCounters()
    {
        countRestText.gameObject.SetActive(false);
        restTimerIsRunning = false;

        countText.gameObject.SetActive(false);
        timerIsRunning = false;

        shouldRunCoroutine = false;
    }

    void enableCounter(string counter)
    {
        switch (counter)
        {
            case "flor":
                #region FLOR
                countText.gameObject.SetActive(true);
                timerIsRunning = true;
                timeRemaining = SettingsModelValuesRoot.tiempoReaccionValores[settingsGame.tiempoReaccion];

                countRestText.gameObject.SetActive(false);
                restTimerIsRunning = false;
                positionTimeCountText.gameObject.SetActive(false);
                positionTimerIsRunning = false; 
                #endregion
                break;
            case "descanso":
                #region DESCANSO
                countRestText.gameObject.SetActive(true);
                restTimerIsRunning = true;
                timeRestRemaining = SettingsModelValuesRoot.tiempoDescansoValores[settingsGame.tiempoDescanso];

                countText.gameObject.SetActive(false);
                timerIsRunning = false;
                positionTimeCountText.gameObject.SetActive(false);
                positionTimerIsRunning = false; 
                #endregion
                break;
            case "position":
                #region POSITION
                positionTimeCountText.gameObject.SetActive(true);
                positionTimerIsRunning = true;
                var positionTimeList = SettingsModelValuesRoot.tiempoEnPosicionValores[settingsGame.tiempoEnPosicion];
                System.Random randomPositionTime = new System.Random();
                timePositionRemainig = timePositionRemainigCopy = positionTimeList.ElementAt(randomPositionTime.Next(0, positionTimeList.Count - 1));

                countText.gameObject.SetActive(false);
                timerIsRunning = false;
                countRestText.gameObject.SetActive(false);
                restTimerIsRunning = false;
                #endregion
                break;
        }
    }
}