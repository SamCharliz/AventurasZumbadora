using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Scripts.Models.SettingsModel;

public class SceneController : MonoBehaviour
{
    public delegate void FlowerSetting(int flowers);
    public static event FlowerSetting SendFlowers;

    private int countWhiteFlower;
    public InputField whiteflowerSetting;

    public GameObject modalWindow;

    // Panel de configuración de tipo de mano con la que se va a jugar
    public GameObject modalHand;
    
    // Panel de configuración de tipo de movimiento
    public GameObject modalMovement;
    
    // Panel de configuración de flores dentro del juego
    public GameObject modalSpawn;
    
    // Panel de configuración de flores distractoras dentro del juego
    public GameObject modalSpawn2;

    public GameObject panelMensajeTerapeuta;
    
    // Panel de resumen de las configuraciones
    public GameObject modalResumeSettings;
    
    // Panel de resumen donde se pregunta si se desean calibrar los sensores
    public GameObject modalCalibrarSensores;
    
    // Panel para elegir escena
    public GameObject modalSceneSettings;

    #region Texts de la sección de resumen de las configuraciones

    private void OnEnable()
    {
        AduanaCITAN.rutinaSubidaConExito += ActivaPanelesPartidaSubidaConExito;
        SettingsController.SetSetting += SetSettingText;
        SpawnController.CollectedFlowers += PauseGame;
        SettingsController.SetSettingMovement += SetSettingMovement;
    }

    private void OnDisable()
    {
        AduanaCITAN.rutinaSubidaConExito -= ActivaPanelesPartidaSubidaConExito;
        SettingsController.SetSetting -= SetSettingText;
        SpawnController.CollectedFlowers -= PauseGame;
        SettingsController.SetSettingMovement -= SetSettingMovement;
    }

    public Text handResume;
    public Text movementTypeResume;
    public Text positionTimeResume;
    public Text spawnOrderResume;
    private string spawnOrderSaved = "";
    public Text numberSetsResume;
    public Text restTimeResume;
    public Text reactionTimeResume;
    public Text distractorsResume;
    public Text distractorsFrequencyResume;
    public Text mensajeTerapeuta;
    public InputField comentarioPaciente_inputField;
    //public string _comentario;
    ManejadorXMLs miXml=new ManejadorXMLs();
    // Configuraciones
    private SettingsModelRoot settingsGame = new SettingsModelRoot();



    private void Start()
    {
        var settingsController = new SettingsController();
        settingsGame = settingsController.GetSettingsGame();

        if (GameMaster.Modo.Equals(GameMaster.ModoLogin.Terapeuta))
        {
            modalHand.SetActive(false);
            modalMovement.SetActive(true);
        }
        else {
            modalHand.SetActive(true);
            modalMovement.SetActive(false);
        }
    }



    void SetSettingText(string setting, string type)
    {
        switch (type)
        {
            case "hand":
                handResume.text = "Mano con la que se va a jugar: " + setting;
                // pasar en automatico a la siguiente configuracion despues de seleccionar la mano
                modalHand.SetActive(false);
                modalMovement.SetActive(true);
                break;
            //case "movementType":
            //    movementTypeResume.text = "Tipo de movimiento que se va a realizar: " + setting;
            //    if (!setting.Contains("Combinación"))
            //    {
            //        spawnOrderResume.text = "";
            //        numberSetsResume.gameObject.transform.localPosition = new Vector3(0, 50, 0);
            //        restTimeResume.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            //        reactionTimeResume.gameObject.transform.localPosition = new Vector3(0, -50, 0);
            //        distractorsResume.gameObject.transform.localPosition = new Vector3(0, -100, 0);
            //        distractorsFrequencyResume.gameObject.transform.localPosition = new Vector3(0, -150, 0);
            //    }
            //    else
            //    {
            //        spawnOrderResume.text = "Orden de aparición de los ejercicios: " + spawnOrderSaved;
            //        numberSetsResume.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            //        restTimeResume.gameObject.transform.localPosition = new Vector3(0, -50, 0);
            //        reactionTimeResume.gameObject.transform.localPosition = new Vector3(0, -100, 0);
            //        distractorsResume.gameObject.transform.localPosition = new Vector3(0, -150, 0);
            //        distractorsFrequencyResume.gameObject.transform.localPosition = new Vector3(0, -200, 0);
            //    }
            //    break;
            case "positionTime":
                positionTimeResume.text = "Tiempo que el jugador debe de mantener cada posición: " + setting;
                break;
            case "spawnOrder":
                spawnOrderResume.text = "Orden de aparición de los ejercicios: " + setting;
                spawnOrderSaved = setting;
                break;
            case "numberSets":
                numberSetsResume.text = "Número de sets: " + setting;
                break;
            case "restTime":
                restTimeResume.text = "Tiempo de descanso entre cada set: " + setting;
                break;
            case "reaction":
                reactionTimeResume.text = "Tiempo de reacción: " + setting;
                break;
            case "distractors":
                distractorsResume.text = "Aparecerán distractores: " + setting;
                break;
            case "frequencyDistractors":
                if (!string.IsNullOrEmpty(setting))
                {
                    distractorsFrequencyResume.text = "Frecuencia de aparición de los distractores: " + setting; 
                }
                else
                {
                    distractorsFrequencyResume.text = "";
                }
                break;
        }
    }

    void SetSettingMovement(SettingsModelValuesRoot.Movimiento setting)
    {

        switch (setting)
        {
            case SettingsModelValuesRoot.Movimiento.PS:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: De pronación a supinación";
                break;
            case SettingsModelValuesRoot.Movimiento.UFMPN:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Únicamente flexión de muñeca en posición neutra";
                break;
            case SettingsModelValuesRoot.Movimiento.UEMPN:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Únicamente extensión de muñeca en posición neutra";
                break;
            case SettingsModelValuesRoot.Movimiento.UFMP:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Únicamente flexión de muñeca en pronación";
                break;
            case SettingsModelValuesRoot.Movimiento.UEMP:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Únicamente extensión de muñeca en pronación";
                break;
            case SettingsModelValuesRoot.Movimiento.CFEPN:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Combinación de flexión y extensión en posición neutra";
                break;
            case SettingsModelValuesRoot.Movimiento.CFEP:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Combinación de flexión y extensión en pronación";
                break;
            case SettingsModelValuesRoot.Movimiento.CTM:
                movementTypeResume.text = "Tipo de movimiento que se va a realizar: Combinación de todos los movimientos";
                break;
        }

        //movementTypeResume.text = "Tipo de movimiento que se va a realizar: " + setting;
        if (setting != SettingsModelValuesRoot.Movimiento.CFEPN &&
            setting != SettingsModelValuesRoot.Movimiento.CFEP &&
            setting != SettingsModelValuesRoot.Movimiento.CTM)
        {
            spawnOrderResume.text = "";
            numberSetsResume.gameObject.transform.localPosition = new Vector3(0, 50, 0);
            restTimeResume.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            reactionTimeResume.gameObject.transform.localPosition = new Vector3(0, -50, 0);
            distractorsResume.gameObject.transform.localPosition = new Vector3(0, -100, 0);
            distractorsFrequencyResume.gameObject.transform.localPosition = new Vector3(0, -150, 0);
        }
        else
        {
            spawnOrderResume.text = "Orden de aparición de los ejercicios: " + spawnOrderSaved;
            numberSetsResume.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            restTimeResume.gameObject.transform.localPosition = new Vector3(0, -50, 0);
            reactionTimeResume.gameObject.transform.localPosition = new Vector3(0, -100, 0);
            distractorsResume.gameObject.transform.localPosition = new Vector3(0, -150, 0);
            distractorsFrequencyResume.gameObject.transform.localPosition = new Vector3(0, -200, 0);
        }

        if (GameMaster.Modo.Equals(GameMaster.ModoLogin.Terapeuta))
        {
            mensajeTerapeuta.text = settingsGame.comentario;
        }
    }

    #endregion

    public void LoadScene(string nameScene)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(nameScene);
    }

    public void PauseGame()
    {
        Time.timeScale = 1.0f;

        if (GameMaster.Modo.Equals(GameMaster.ModoLogin.Paciente))
        {
            SceneManager.LoadScene("Paciente");

        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void LoadHandMovementPanel(bool isNext)
    {
        modalHand.SetActive(isNext ? false : true);
        modalMovement.SetActive(isNext ? true : false);
    }

    public void LoadMovementSpawnPanel(bool isNext)
    {
        modalMovement.SetActive(isNext ? false : true);
        modalSpawn.SetActive(isNext ? true : false);
    }

    public void LoadSpawnSpawnPanel(bool isNext)
    {
        modalSpawn.SetActive(isNext ? false : true);
        modalSpawn2.SetActive(isNext ? true : false);
    }

    public void LoadSpawnResumePanel(bool isNext)
    {
        Debug.Log("Se va a carga el panel de resumen");
        if (GameMaster.Modo.Equals(GameMaster.ModoLogin.Terapeuta))
        {
            Debug.Log("Panel de mensaje");

            panelMensajeTerapeuta.SetActive(true);
            modalSpawn2.SetActive(false);
            modalResumeSettings.SetActive(false);

        }
        else
        {
            Debug.Log("Panel de resumen");

            modalSpawn2.SetActive(isNext ? false : true);
            modalResumeSettings.SetActive(isNext ? true : false);
        }
    }

    public void LoadTerapeutaEscenaAnterior()
    {
        panelMensajeTerapeuta.SetActive(false);
        modalSpawn2.SetActive(true);
    }

    public void LoadTerapeutaEscenaSiguiente()
    {
        settingsGame.comentario = comentarioPaciente_inputField.text.ToString();
        Debug.Log("Mafia"+ comentarioPaciente_inputField.text.ToString());
        panelMensajeTerapeuta.SetActive(false);
        modalResumeSettings.SetActive(true);
    }

    public void LoadResumeScenePanel(bool isNext)
    {
        if (GameMaster.Modo.Equals(GameMaster.ModoLogin.Terapeuta))
        {
            CreaXMLRutinaYEnvia();
            return;
        }
        var settingsController = new SettingsController();
        settingsGame = settingsController.GetSettingsGame();
        var orientaciones = SettingsModelValuesRoot.nombreMovimientoValores[settingsGame.nombreMovimiento];
        var faltaCalibracion = false;

        foreach (var direccion in orientaciones)
        {
            switch (direccion)
            {
                case SettingsModelValuesRoot.Orientacion.ARRIBA:
                    faltaCalibracion = settingsGame.anguloArriba != null && settingsGame.anguloArriba > 0 ? false : true;
                    break;
                case SettingsModelValuesRoot.Orientacion.ABAJO:
                    faltaCalibracion = settingsGame.anguloAbajo != null && settingsGame.anguloAbajo > 0 ? false : true;
                    break;
                case SettingsModelValuesRoot.Orientacion.DERECHA:
                    faltaCalibracion = settingsGame.anguloDerecha != null && settingsGame.anguloDerecha > 0 ? false : true;
                    break;
                case SettingsModelValuesRoot.Orientacion.IZQUIERDA:
                    faltaCalibracion = settingsGame.anguloIzquierda != null && settingsGame.anguloIzquierda > 0 ? false : true;
                    break;
            }
        }

        if (faltaCalibracion)
        {
            LoadScene("Calibrar");
        }
        else
        {
            modalResumeSettings.SetActive(isNext ? false : true);
            modalSceneSettings.SetActive(isNext ? true : false);
        }
    }

    public void CreaXMLRutinaYEnvia() 
    {
        GuardayAsignaRutina();
        //debes de activqr un panel que diga subeindo rutina...
    }




    public void LoadResumeConectPanel(bool isNext)
    {
        modalResumeSettings.SetActive(isNext ? false : true);
        modalCalibrarSensores.SetActive(isNext ? true : false);
    }

    public void LoadConectPanelResume(bool isNext)
    {
        modalCalibrarSensores.SetActive(isNext ? false : true);
        modalResumeSettings.SetActive(isNext ? true : false);
    }

    public void SaveSettings()
    {
        countWhiteFlower = int.Parse(whiteflowerSetting.text.ToString());
        //conutFlower = int.Parse(flowerSetting.GetComponentInChildren<Text>().ToString());

        print(whiteflowerSetting.text);
    }

    public void QuitGame()
    {
        Application.Quit();
    }   

    public void PauseGame(bool isGameOver)
    {
        Time.timeScale = 0;

        if (!isGameOver)
        {
            modalWindow.SetActive(true); 
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        modalWindow.SetActive(false);
    }

    public void GuardayAsignaRutina()
    {
        //_mensajePaciente=mensajeParaPaciente.GetComponentInChildren<Text>().text;
        Save();
        StartCoroutine(AduanaCITAN.SubeRutinaAlServidor(GameMaster.IdPaciente, GameMaster.rutaDeArchivos, "rutinaZumbadora"));
        StartCoroutine(ActualizaRutina());
    }


    void Save()
    {
        
        miXml.CreaXMLRutina(settingsGame, GameMaster.IdPaciente, GameMaster.rutaDeArchivos);
        return;
    }

    //SI TUVIMOS EXITO SUBIENDO SE LLAMA A ESTA FUNCION
    void ActivaPanelesPartidaSubidaConExito()
    {
        Debug.Log("LOGRAMOS subir la rutina");
        //panel_confirmacion.SetActive(false);
        //panelRutinaSubidayAsignada.SetActive(true);
    }

    IEnumerator ActualizaRutina()
    {
        //===================================================================================================================================================================================================
        //===============================================================================================
        
        
        string urlString = DireccionesURL.IdPacienteIdJuego_ActualizaRutina + "?id=" + WWW.EscapeURL(GameMaster.IdPaciente) + "&rutina=" + WWW.EscapeURL(GameMaster.IdPaciente + "_AZ.xml") + "&id_game=17"; //<----actualizar el ID del juego
                                                                                                                                                                                                                //===============================================================================================
                                                                                                                                                                                                                //====================================================================================================================================================================================================
#if UNITY_EDITOR
        Debug.Log("Vamos a actualizar la base de datos");
        Debug.Log("Estoy mandando" + urlString);
#endif
        WWW postName = new WWW(urlString);
        yield return postName;
#if UNITY_EDITOR
        Debug.Log("La asignacion de rutina retorno" + postName.text.ToString());
#endif

    }
}
