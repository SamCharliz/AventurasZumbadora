using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Xml; 
using System.Xml.Serialization;  
using System.Text;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using Ionic.Zip;


public class InterfazMedico : MonoBehaviour {

	public delegate void Audio();
	public static event Audio MusicaTerapeuta;

	//===============================================
	//[Tooltip("En este panel sólo debe haber un botón que al darle click cierre la sesión del terapeuta")]
	//public GameObject panelSinInternet;
	public GameObject panel_config;
	public DropDownList pacientes_dropList;	
	public Text nombreTerapeuta;
	[Tooltip("Debe aparecer cuando se quiera asignar una rutina o ver los resultados de un paciente sin haber seleccionado un paciente de la lista")]
	public Text advertenciaSeleccion;
	//*********************************************************************************************
	//En el panel_confirmación deben estar contenidos el mensajeParaPaciente y descripciónRutina 
	//[Tooltip("Este es el mensaje que verá el paciente cuando se descargue la rutina")]
	//public InputField mensajeParaPaciente;
	//[Tooltip("Este es el texto que mostrará el resumen de todos los parámetros que se configuraron en la rutina personalizada")]
	//public Text descripcionRutina;
	//public GameObject panel_confirmacion;
	//********************************************************************************************
	//[Tooltip("Este es el panel que se muestra cuando la rutina se subió a CITAN con éxito")]
	//public GameObject panelRutinaSubidayAsignada;

	//================================================

	//OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
	//OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
	//Parámetros de la rutina a configurar
	private string _nombreRutina = "";
	private string _mensajePaciente = "";
	private int _nivel = 0;
	//OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
	//OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

	//De este objeto se leeran las caracteristicas y parametros de la rutina en la sig. escena
	public static RutinaData myRoutineData;

	private int repeticiones;
	private bool _rutinaSeleccionada = false;
	private bool _pacienteSeleccionado = false;
	private string _nombreRutinaSeleccionada;
	private string _idPacienteSeleccionado;
	private string _nombrePacienteSleccionado;
	private string namesP;


	private string _nombresPacientes;
	string _data, _routineData;
	ManejadorXMLs miXml;

	void OnEnable() {
		//AduanaCITAN.rutinaSubidaConExito += ActivaPanelesPartidaSubidaConExito;
		//AduanaCITAN.rutinasDescargadasExito += ObtieneRutinas;  //esto debe estar comentado si se trata de Interfaz Terapeuta v1
		VerificadorRed.noHayConexionConCITAN += RegistraQueNoHayInternetAntesDeContinuar;
		UnityEngine.UI.Extensions.DropDownList.pacienteSeleccionado += RegisterPatientId;
	}

	void OnDisable() {
		//AduanaCITAN.rutinaSubidaConExito -= ActivaPanelesPartidaSubidaConExito;
		//AduanaCITAN.rutinasDescargadasExito -= ObtieneRutinas;   //esto debe estar comentado si se trata de Interfaz Terapeuta v1
		VerificadorRed.noHayConexionConCITAN -= RegistraQueNoHayInternetAntesDeContinuar;
		UnityEngine.UI.Extensions.DropDownList.pacienteSeleccionado -= RegisterPatientId;
	}

	void Awake() {
		if (MusicaTerapeuta != null) {
			MusicaTerapeuta();
		}
		GameMaster.CreaDirectorioDelJuego();
		StartCoroutine(VerificadorRed.VerificaConexionConCITAN());
		//panelSinInternet.SetActive(false);
		miXml = new ManejadorXMLs();
		pacientes_dropList.Initialize(); //iniciamos las listas desplegables
										 //pacientesResultados_dropList.Initialize();
										 //rutinas.Initialize ();
		nombreTerapeuta.text = GameMaster.NombreTerapeuta;
		_nombresPacientes = "";
		LimpiaValores();
		StartCoroutine(GetPatientsFromDoc(GameMaster.IdTerapeuta));
	}

	void RegistraQueNoHayInternetAntesDeContinuar() {
		//panelSinInternet.SetActive(true);
	}

	void Start()
	{
		
	}

	//Obtenemos todos los pacientes que son responsabilidad del terapeuta
	private IEnumerator GetPatientsFromDoc(string id) {
#if UNITY_EDITOR
		Debug.Log("Obteniendo pacientes del Doc.");
#endif
		string urlString = DireccionesURL.IdTerapeuta_ListaPacientes + WWW.EscapeURL(id);
		WWW postName = new WWW(urlString);
		yield return postName;
#if UNITY_EDITOR
		Debug.Log("Pacientes:" + postName.text.ToString() + "FIN");
#endif
		string[] patients = postName.text.ToString().Split(';');
		foreach (string p in patients)
		{
			if (p.Length > 2) {
				DropDownListItem etemporal = new DropDownListItem();
				etemporal.Caption = p.ToString().Substring(0, p.LastIndexOf("_"));
				//aaqui tengo que poner el id del paceinte
				etemporal.ID = p.ToString().Substring(p.LastIndexOf("_") + 1);
				pacientes_dropList.Items.Add(etemporal);
				//pacientesResultados_dropList.Items.Add (etemporal);
			}
		}
		pacientes_dropList.RebuildPanel();
		//pacientesResultados_dropList.RebuildPanel();
	}

	//esta función se manda llamar desde el script DropDownList.cs al momento de seleccionar un paciente de la lista desplegable
	public void RegisterPatientId(String patientId, String patientName)
	{
		_pacienteSeleccionado = true;
		_idPacienteSeleccionado = patientId;
		_nombrePacienteSleccionado = patientName;
	}


	//==============================================================================================
	//Creación Rutina
	//==============================================================================================


	public void CreayAsignaRutinaBoton()
	{
		if (!_pacienteSeleccionado)
		{
			advertenciaSeleccion.text = "Por favor seleccione un paciente para continuar.";
			return;
		}
		advertenciaSeleccion.text = "";
		GameMaster.SetIdPaciente(_idPacienteSeleccionado);
		GameMaster.SetNombrePaciente(_nombrePacienteSleccionado);
		SceneManager.LoadScene("HandSetting");
		panel_config.SetActive(false);
	}


	public void SeleccionaNivel(int nivel)
	{
		_nivel = nivel;		
	}

	//Esta función se anda a llamar cuando se muestre el panel final de confirmación, con el fin de mostrarle al terapeuta un resumen de las características de la rutina
	void MuestraParametros() {
		
		//Poner aquí los parámetros que se puedan configurar
		//descripcionRutina.text = "Paciente: "+_nombrePacienteSleccionado+"\nTipo de movimiento: "+_nombreRutina+"\nNivel: "+_nivel.ToString()+"\nSensibilidad: "+_sensibilidadTemp+"\nDirección: "+_tipoMov.ToString()+"\nDebe dar click: "+click_text.text.ToString();
	}


//	public void GuardayAsignaRutina(){
//		//_mensajePaciente=mensajeParaPaciente.GetComponentInChildren<Text>().text;
//		Save ();		
//		StartCoroutine(AduanaCITAN.SubeRutinaAlServidor(_idPacienteSeleccionado, GameMaster.rutaDeArchivos,_nombreRutina));
//		StartCoroutine(ActualizaRutina());
//	}


//	void Save(){ 
//		RutinaData myData = new RutinaData();
//		myData.NombreDeRutina = _nombreRutina;// nombreDeRutina.text.ToString ();
//		myData.MensajeParaPacientes = _mensajePaciente;
//		myData.Nivel = _nivel;	
//		miXml.CreaXMLRutina (myData,_idPacienteSeleccionado,GameMaster.rutaDeArchivos);
//		return;
//	}

//	void ActivaPanelesPartidaSubidaConExito() {
//		//panel_confirmacion.SetActive(false);
//		//panelRutinaSubidayAsignada.SetActive(true);
//	}

//	IEnumerator ActualizaRutina()
//	{
//		//===================================================================================================================================================================================================
//		//===============================================================================================
//		string urlString = DireccionesURL.IdPacienteIdJuego_ActualizaRutina + "?id=" + WWW.EscapeURL(_idPacienteSeleccionado) + "&rutina=" + WWW.EscapeURL(_idPacienteSeleccionado + "_IJ.xml") + "&id_game=6"; //<----actualizar el ID del juego
//		//===============================================================================================
//		//====================================================================================================================================================================================================
//#if UNITY_EDITOR
//			Debug.Log("Vamos a actualizar la base de datos");
//			Debug.Log("Estoy mandando" + urlString);
//#endif
//			WWW postName = new WWW(urlString);
//			yield return postName;
//#if UNITY_EDITOR
//			Debug.Log("La asignacion de rutina retorno" + postName.text.ToString());
//#endif
		
//	}

	//==============================================================================================

	//============================================================================================
	//   Ver Resultados
	//============================================================================================

	public void VeResultadosDelPaciente(){
		if (!_pacienteSeleccionado) {
			advertenciaSeleccion.text = "Por favor seleccione un paciente para continuar.";
			return;
		}
		advertenciaSeleccion.text = "";
		GameMaster.AsignaNombreyIdPaciente (_nombrePacienteSleccionado,_idPacienteSeleccionado);
		Debug.Log ("Nombre paciente:"+ GameMaster.NombrePaciente +"   "+GameMaster.IdPaciente);
		StartCoroutine (AduanaCITAN.DescargaPartidasDeCITAN (GameMaster.IdPaciente));
		
		SceneManager.LoadScene ("Estadisticas1");
	}


	//=====================================================================================================


	//public void UltimosHijos(){
	//	panel [0].SetAsLastSibling ();
	//	panel [1].SetAsLastSibling ();
	//}

	public void GetPatients(){
		ActualizaDropList (namesP);

	}

	private void ActualizaDropList(string postName){

		string[] patients = postName.Split (';');
		foreach (string p in patients)
		{
			if(p.Length>2){
				Debug.Log(p+"paciente");
				DropDownListItem etem=new DropDownListItem();
				etem.Caption=p.ToString().Substring(0,p.LastIndexOf("_"));
				//aaqui tengo que poner el id del paceinte
				etem.ID=p.ToString().Substring(p.LastIndexOf("_")+1);
				pacientes_dropList.Items.Add(etem);
				//pacientesResultados_dropList.Items.Add(etem);
			}
		}
		Debug.Log (postName);
		Debug.Log ("termine");
		pacientes_dropList.RebuildPanel ();
		//pacientesResultados_dropList.RebuildPanel ();

	}

	public void LimpiaValores() {
		_nivel = 0;
		_mensajePaciente = "";
		//mensajeParaPaciente.GetComponentInChildren<Text>().text = "";
		_nombreRutina = "";
	}

	public void BotonSiCerraSesion(){
		SceneManager.LoadScene (0);
	}

	

}

