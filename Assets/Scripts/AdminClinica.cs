using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdminClinica : MonoBehaviour
{
	public delegate void Audio();
	public static event Audio MusicaSeleccion;


	public GameObject panel_config;
	public Text nombrePaciente;
	
	public GameObject cerrarSesion_panel;
	public GameObject panelConfirmacion;
	public Text detallesRutina_text;


	private Mano _manoElegida = Mano.Ninguna;
	ManejadorXMLs _miXml;
	float vol_ini;
	RutinaData especificacionesRutina;


	void OnEnable()
	{

		AduanaCITAN.partidasDescargadasDeCITAN += GuardaXML;
		VerificadorRed.noHayConexionConCITAN += RegistraQueNoHayInternetAntesDeContinuar;
		VerificadorRed.tenemosConexionConCITAN += RegistraQueSIHayInternetAntesDeContinuar;
	}

	void OnDisable()
	{

		AduanaCITAN.partidasDescargadasDeCITAN -= GuardaXML;
		VerificadorRed.noHayConexionConCITAN -= RegistraQueNoHayInternetAntesDeContinuar;
		VerificadorRed.tenemosConexionConCITAN -= RegistraQueSIHayInternetAntesDeContinuar;
	}

	void Awake()
	{
		
		_miXml = new ManejadorXMLs();
        if (cerrarSesion_panel != null)
        {
			cerrarSesion_panel.SetActive(false);
		}
        if (panelConfirmacion != null)
        {
			panelConfirmacion.SetActive(false);
		}
#if UNITY_EDITOR
		Debug.Log("Siempre que regresamos a esta escena verificamos si hay conexión con el servidor Citan");
#endif
		StartCoroutine(VerificadorRed.VerificaConexionConCITAN());
	}


	void Start()
	{
		if (GameMaster.ModoSinConexion)
		{
			nombrePaciente.text = "Bienvenido su Id es: " + GameMaster.IdPaciente + " sin conexión con el servidor.";
		}
		else
		{
			nombrePaciente.text = "Bienvenido " + GameMaster.NombrePaciente;
		}

	}

	void RegistraQueNoHayInternetAntesDeContinuar()
	{
		GameMaster.RegistraQueEstoyEnModoSINConexion();
#if UNITY_EDITOR
		Debug.Log("No tenemos Internet. Vamos a ver si ya tiene una rutina en la PC para jugar.Llamamos una corutina que busca en el servidor porque esa llamará a la función de buscar una rutina en la PC");
#endif
		StartCoroutine(AduanaCITAN.RevisaSiTieneRutinaAsignada(GameMaster.IdPaciente, this, GameMaster.rutaDeArchivos, 17));
	}

	void RegistraQueSIHayInternetAntesDeContinuar()
	{
		GameMaster.RegistraQueEstoyEnModoConConexion();
#if UNITY_EDITOR
		Debug.Log("Vamos a subir al servidor partidas que no se hayan guardado en el servidor, si es que existen");
#endif
		if (_miXml.VerificaPartidasNoSubidas(GameMaster.RutaHistorialPartidasPaciente))
		{
			_miXml.GuardaLasPartidasPendientes(this, GameMaster.RutaHistorialPartidasPaciente, GameMaster.IdPaciente);
		}
#if UNITY_EDITOR
		Debug.Log("En este punto tenemos conexión con CITAN, en caso de que ya existiera un XML en la PC, ya revisamos que todas las partidas están en la BD, pero quizá el paciente jugó en otra PC y ahora en la BD hay más partidas.");
		Debug.Log("Por eso vamos a borrar el XML de la PC y volver a descargar las partidas de la BD para asegurarnos que tenemos los datos más actuales.");
#endif
		_miXml.BorraHistarialPartidasXML(GameMaster.RutaHistorialPartidasPaciente);
#if UNITY_EDITOR
		Debug.Log("Hemos borrado el Xml del paciente de la PC.");
#endif
		StartCoroutine(AduanaCITAN.DescargaPartidasDeCITAN(GameMaster.IdPaciente));
		StartCoroutine(AduanaCITAN.RevisaSiTieneRutinaAsignada(GameMaster.IdPaciente, this, GameMaster.rutaDeArchivos, 17));
	}

	void GuardaXML(string[] partidas)
	{
		GameMaster.RegistraQueSiHayXMLdelPacienteEnPC();
		_miXml.CreaContenedorDelHistorialPartidas(partidas, GameMaster.RutaHistorialPartidasPaciente, GameMaster.IdPaciente, GameMaster.NombrePaciente);
#if UNITY_EDITOR
		Debug.Log("Hemos creado un nuevo XML.");
#endif
	}

	

	//=======================================================================================================================================================================================
	//==================Jugar y tutoriales
	//=======================================================================================================================================================================================


	
	//Este panel muestra un resumen de  todos los parámetros de la partida que se está a punto de jugar
	public void MuestraPanelConfirmar()
	{

		panelConfirmacion.gameObject.SetActive(true);

		//detallesRutina_text.text = "Movimiento: " + especificacionesRutina.tipoMov.ToString() + "\n" + "Sensibilidad: " + _sensibilidadTemp + "\n" + "Nivel: " + especificacionesRutina.nivel.ToString() + "\n" + "Debe dar click: " + ((especificacionesRutina.conClick == true) ? "Sí" : "No") + "\n" + "Mano: " + _manoElegida.ToString()+"\n"+especificacionesRutina.mensajeParaPaciente;

	}

	//Esta función se manda llamar para cargar la Escena de Juego, para ello se cargan todos los parámetros de la partida
	public void Comenzar()
	{
		/*
		GameMaster.AsignaParametrosParaJugar(especificacionesRutina.NombreDeRutina, especificacionesRutina.tipoMov, _sensibilidadReal, especificacionesRutina.conClick, especificacionesRutina.nivel, _tiempoEspera, _manoElegida, _idCarnivoro, _idHerbivoro);
		if (_jugaraConMuneca)
			SceneManager.LoadScene("Muneca");
		else
			SceneManager.LoadScene("InvasionJurasica");
		*/

	}

	public void LoadScene(string nameScene)
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(nameScene);
	}

	//=======================================================================================================================================================================================
	//==================Cerrar sesión
	//=======================================================================================================================================================================================


	public void LogOutYes()
	{
		SceneManager.LoadScene(0);
	}
	//=======================================================================================================================================================================================
	//==================Preguntas frecuentes
	//=======================================================================================================================================================================================

	public void HelpButton()
	{
		SceneManager.LoadScene(17);
	}

	//=======================================================================================================================================================================================
	//==================Resultados
	//=======================================================================================================================================================================================

	public void VeAResultados()
	{
		SceneManager.LoadScene("Estadisticas1");
	}

	public void VeACreditos()
	{
		GameMaster.escenaDeDondeVengo = "Paciente";
		SceneManager.LoadScene("Creditos");
	}

	//=======================================================================================================================================================================================
	//==================Salir juego
	//=======================================================================================================================================================================================


}
