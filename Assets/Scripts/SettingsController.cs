using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Models.SettingsModel;

public class SettingsController : MonoBehaviour
{
    private static SettingsModelRoot settingsGame = new SettingsModelRoot();

    public SettingsModelRoot GetSettingsGame()
    {
        return settingsGame;
    }

    public void SetSettingsGame(SettingsModelRoot settings)
    {
        settingsGame = settings;
    }

    // Time movement setting
    [SerializeField] private Dropdown positionTimeDropdown;
    
    // Time movement setting
    [SerializeField] private Dropdown spawnOrderDropdown;
    
    // Number od sets setting
    [SerializeField] private Dropdown numberSetsDropdown;
    
    // Rest time setting
    [SerializeField] private Dropdown restTimeDropdown;
    
    // Reaction time setting
    [SerializeField] private Dropdown reactionTimeDropdown;
    
    // Distractors setting
    [SerializeField] private Dropdown distractorsDropdown;
    [SerializeField] private Text distractorsText;
    
    // Frequency spawn distractors setting
    [SerializeField] private Dropdown frequencyDistractorsDropdown;

    [SerializeField] private Button botonSiguiente;

    #region Eventos que le van a avisar al SceneController que se agrego/cambio alguna configuración

    public delegate void SettingHandAction(string setting, string type);
    public static event SettingHandAction SetSetting;

    public delegate void SettingMovementAction(SettingsModelValuesRoot.Movimiento setting);
    public static event SettingMovementAction SetSettingMovement;

    #endregion

    void Start()
    {
        SaveDropdownSetting("time");
        SaveDropdownSetting("order");
        SaveDropdownSetting("sets");
        SaveDropdownSetting("rest");
        SaveDropdownSetting("reaction");
        SaveDropdownSetting("distractors");
    }

    public void SaveHandSetting(string hand)
    {
        settingsGame.mano = hand;
        if (SetSetting != null)
            SetSetting(settingsGame.mano, "hand");
        Debug.Log("Se guardo configuración de mano " + settingsGame.mano);
    }

    public void SaveMovementTypeSetting(string movementType)
    {
        // Aquí activamos el botón de continuar
        botonSiguiente.gameObject.SetActive(true);

        switch (movementType)
        {
            case "De pronación a supinación":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.PS;
                distractorsDropdown.gameObject.SetActive(true);
                distractorsText.gameObject.SetActive(true);
                break;
            case "Únicamente flexión de muñeca en posición neutra":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.UFMPN;
                distractorsDropdown.gameObject.SetActive(false);
                distractorsText.gameObject.SetActive(false);
                break;
            case "Únicamente extensión de muñeca en posición neutra":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.UEMPN;
                distractorsDropdown.gameObject.SetActive(false);
                distractorsText.gameObject.SetActive(false);
                break;
            case "Únicamente flexión de muñeca en pronación":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.UFMP;
                distractorsDropdown.gameObject.SetActive(false);
                distractorsText.gameObject.SetActive(false);
                break;
            case "Únicamente extensión de muñeca en pronación":
                settingsGame.nombreMovimiento =SettingsModelValuesRoot.Movimiento.UEMP;
                distractorsDropdown.gameObject.SetActive(false);
                distractorsText.gameObject.SetActive(false);
                break;
            case "Combinación de flexión y extensión en posición neutra":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.CFEPN;
                distractorsDropdown.gameObject.SetActive(true);
                distractorsText.gameObject.SetActive(true);
                break;
            case "Combinación de flexión y extensión en pronación":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.CFEP;
                distractorsDropdown.gameObject.SetActive(true);
                distractorsText.gameObject.SetActive(true);
                break;
            case "Combinación de todos los movimientos":
                settingsGame.nombreMovimiento = SettingsModelValuesRoot.Movimiento.CTM;
                distractorsDropdown.gameObject.SetActive(true);
                distractorsText.gameObject.SetActive(true);
                break;
        }

        //if (SetSetting != null)
        //    SetSetting(movementType, "movementType");
        
        if (SetSettingMovement != null)
            SetSettingMovement(settingsGame.nombreMovimiento);

        if (settingsGame.nombreMovimiento != SettingsModelValuesRoot.Movimiento.CFEPN &&
            settingsGame.nombreMovimiento != SettingsModelValuesRoot.Movimiento.CFEP &&
            settingsGame.nombreMovimiento != SettingsModelValuesRoot.Movimiento.CTM)
        {
            spawnOrderDropdown.gameObject.SetActive(false);
            numberSetsDropdown.gameObject.transform.localPosition = new Vector3(200, 120, 0);
            restTimeDropdown.gameObject.transform.localPosition = new Vector3(-200, -30, 0);
        }
        else
        {
            spawnOrderDropdown.gameObject.SetActive(true);
            numberSetsDropdown.gameObject.transform.localPosition = new Vector3(-200, -30, 0);
            restTimeDropdown.gameObject.transform.localPosition = new Vector3(200, -30, 0);

        }
        Debug.Log("Se guardo configuración de tipo de movimiento: " + movementType);
    }

    public void SaveDropdownSetting(string setting)
    {
        Debug.Log("Setting: " + setting);

        switch (setting)
        {
            case "time":
                settingsGame.tiempoEnPosicion = positionTimeDropdown.options[positionTimeDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.tiempoEnPosicion, "positionTime");
                Debug.Log("Se guardo tiempo que el jugador debe de mantener cada posición: " + settingsGame.tiempoEnPosicion);
                break;
            case "order":
                settingsGame.ordenAparacion = spawnOrderDropdown.options[spawnOrderDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.ordenAparacion, "spawnOrder");
                Debug.Log("Se guardo el orden de aparición: " + settingsGame.ordenAparacion);
                break;
            case "sets":
                settingsGame.numeroSets = numberSetsDropdown.options[numberSetsDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.numeroSets, "numberSets");
                Debug.Log("Se guardo el número de sets: " + settingsGame.numeroSets);
                break;
            case "rest":
                settingsGame.tiempoDescanso = restTimeDropdown.options[restTimeDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.tiempoDescanso, "restTime");
                Debug.Log("Se guardo el tiempo de descanso: " + settingsGame.tiempoDescanso);
                break;
            case "reaction":
                settingsGame.tiempoReaccion = reactionTimeDropdown.options[reactionTimeDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.tiempoReaccion, "reaction");
                Debug.Log("Se guardo el tiempo de reacción: " + settingsGame.tiempoReaccion);
                break;
            case "distractors":
                settingsGame.conDistractores = distractorsDropdown.options[distractorsDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.conDistractores, "distractors");
                if (settingsGame.conDistractores == "Si")
                {
                    frequencyDistractorsDropdown.gameObject.SetActive(true); 
                }
                else
                {
                    frequencyDistractorsDropdown.gameObject.SetActive(false);
                    if (SetSetting != null)
                        SetSetting("", "frequencyDistractors");
                }
                Debug.Log("Se guardo si habrá distractores: " + settingsGame.conDistractores);
                break;
            case "frequencyDistractors":
                settingsGame.frecuenciaDistractores = frequencyDistractorsDropdown.options[frequencyDistractorsDropdown.value].text;
                if (SetSetting != null)
                    SetSetting(settingsGame.frecuenciaDistractores, "frequencyDistractors");
                Debug.Log("Se guardo la frecuencia de los distractores: " + settingsGame.frecuenciaDistractores);
                break;
        }
    }
}
