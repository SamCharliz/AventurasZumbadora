using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Assets.Scripts.Models.SettingsModel;
using System;

public static class GameMaster  {

	//En esta variable es donde se almacena la dirección donde se giardarán TODOS los resultados
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	public static string rutaDeArchivos="C:\\LANR\\AventuraZumbadora";
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	public static bool INDAUTOR=false;

	//creando el directorio donde se guardaran los resultados del juego 
	public static void CreaDirectorioDelJuego(){
		if (!Directory.Exists (rutaDeArchivos)) {
			Directory.CreateDirectory (rutaDeArchivos);
		}
	}

	private static string _fileName="";
	public static string FileName{
		get{ 
			return _fileName;
		}
	}

	public static void SetFileName(string idPaciente){
		_fileName = idPaciente + "_Data.xml";
	}

	private static string rutaHistorialPartidasPaciente = "";
	public static string RutaHistorialPartidasPaciente{
		get{ 
			return rutaHistorialPartidasPaciente;
		}
	}

	public static void SetRutaHistorialPartidasPaciente(string idPaciente){
		SetFileName (idPaciente);
		rutaHistorialPartidasPaciente = rutaDeArchivos + "\\" + _fileName;
	}


	//Clinica sin Internet-----------------------------------------------------------------------
	private static string terapeutaCuandoNoHayConexion="clinica";
	public static string TerapeutaCuandoNoHayConexion{
		get{
			return terapeutaCuandoNoHayConexion;
		}
	}
	private static string contraseniaDeTerapeutaCuandoNoHayConexion="clinica";
	public static string ContraseniaDeTerapeutaCuandoNoHayConexion{
		get{ 
			return contraseniaDeTerapeutaCuandoNoHayConexion;
		}
	}
	//------------------------------------------------------------------------------


	private static string nombrePaciente;
	public static string NombrePaciente{
		get{
			return nombrePaciente;
		}
	}
	private static string idPaciente = "1";
	public static string IdPaciente{
		get{
			return idPaciente;
		}
	}

	public static void SetNombrePaciente(string nombre){
		nombrePaciente = nombre;
	}

	public static void SetIdPaciente(string id){
		idPaciente = id;	
	}

	public static void AsignaNombreyIdPaciente(string _nombre,string _id){
		nombrePaciente = _nombre;
		idPaciente = _id;
	}

	public static void LimpiaNombreyIdPaciente(){
		nombrePaciente = "";
		idPaciente = "";
	}

	private static int partidasJugadas;
	public static int PartidasJugadas {
		get {
			return partidasJugadas;
		}
		
	}

	public static void RegistraPartidasJugadas(int partidas) {
		partidasJugadas = partidas;
	}

	//--------------------------------------------------------------------------
	private static string nombreTerapeuta;
	public static string NombreTerapeuta{
		get{
			return nombreTerapeuta;
		}
	}
	private static string idTerapeuta;
	public static string IdTerapeuta{
		get{
			return idTerapeuta;
		}
	}

	public static void AsignaNombreyIdTerapeuta(string _nombre,string _id){
		nombreTerapeuta = _nombre;
		idTerapeuta = _id;
	}

	public static void LimpiaNombreyIdTerapeuta(){
		nombreTerapeuta = "";
		idTerapeuta = "";
	}
	// --------------------------------------------------------------------------


	private static bool noHayXMLdelPacienteEnPC;
	public static bool NoHayXMLdelPacienteEnPC{
		get{ 
			return noHayXMLdelPacienteEnPC;
		}
	}

	public static void RegistraQueNoHayXMLdelPacienteEnPC(){
		noHayXMLdelPacienteEnPC = true;
	}


	public static void RegistraQueSiHayXMLdelPacienteEnPC(){
		noHayXMLdelPacienteEnPC = false;
	}

	//---------------------------------------------------------------------------

	private static bool modoSinConexion;   
	public static bool ModoSinConexion{
		get{ 
			return modoSinConexion;
		}
	}

	public static void RegistraQueEstoyEnModoConConexion(){
		modoSinConexion = false;
	}

	public static void RegistraQueEstoyEnModoSINConexion(){		
		modoSinConexion = true;
	}

	//--------------------------------------------------------------------------

	public enum ModoLogin
	{
		Paciente, Terapeuta, Clinica, Ninguno
	}
	private static ModoLogin modo;
	public static ModoLogin Modo{
		get{ 
			return modo;
		}
	}

	public static void RegistraQueEstoyEnModoClinica(){
		modo = ModoLogin.Clinica;
	}
		
	public static void RegistraQueEstoyEnModoPaciente(){
		modo = ModoLogin.Paciente;
	}

	public static void RegistraQueEstoyEnModoTerapeuta(){
		modo = ModoLogin.Terapeuta;
	}

	public static void RegistraQueNoEstoyLogeado(){
		modo = ModoLogin.Ninguno;
	}

	//-----------------------------------------------------------------------------

	//=============================================================================

	//Poner aqui el nombre de la escena inicial del juego
	public static string escenaDeDondeVengo="Inicia Sesion";

	public static bool vengoDeLaEscenaDeCalibracion=false;

	private static int escenarioElegido = 0;
	public static int EscenarioElegido {
		get {
			return escenarioElegido;
		}
	}

	private static Mano manoElegida;
	public static Mano ManoElegida {
		get {
			return manoElegida;
		}
	}
	
	private static RutinaData parametros=new RutinaData();

	
	public static void AsignaParametrosParaJugar(int _nivel, Mano _manoElegida, int _noRepeticiones, bool _jugaraRutinaPersonalizada, string _nombreRutina, int _tiempoReaccion, int _escenario){
		parametros.Nivel = _nivel;
		parametros.NumeroDeRepeticiones = _noRepeticiones;
		parametros.NombreDeRutina = _nombreRutina;
		//NOTA: Los  cosas que se puedan configurar desde la interfaz paciente, como el escenario, la mano con la que se jugará o el personaje deberían guardarse en variables aparte, no en el objeto parámetros
		escenarioElegido = _escenario;		
		manoElegida = _manoElegida;
					
	}

	//Esta es la función que se debe mandar llamar desde la escena de juego en el Awake() para cargar todos los parámetros con los que se va a jugar
	public static RutinaData CargaParametrosParaJugar() {
		return parametros;
	}


	//=======================================================================
	
	private static string fechaDeJuego;
	public static string FechaDeJuego{
		get{ 
			return fechaDeJuego;
		}
	}

	private static Partida resultados = new Partida();
	//Esta es la función que se debe mandar llamar desde la escena de juego cuando se termine la partida, en ese momento guardamos toda la información sobre los aciertos, errores, tiempos, etc. en GameMaster
	//esa información después se la pasaremos al script PartidasTerminator
	public static void GuardaResultadosDePartida(Partida resultadosPartida/*string _fecha, int aciertos, int falsos_Aciertos, int errores, float duracion, int nivel*/){
		//fechaDeJuego = _fecha;
		//resultados.fecha = _fecha;
		//resultados.aciertos = aciertos;
		//resultados.falsos_aciertos = falsos_Aciertos;
		//resultados.tiempoTotal = duracion;
		//resultados.nivel = nivel;

		resultados = resultadosPartida;
	}

	public static Partida CargaResultadospartida()
	{
		return resultados;
	}

}



public enum Mano {Derecha, Izquierda, Ninguna};

public class PlayerData {
	public string nombre;
	public string id;
	public List<Partida> HistorialPartidas = new List<Partida> ();
}

public class TerapeutaData{
	public string Nombre;
	public int Id;
}

public class Partida{
    public int guardado;
    public float duracionPartida;
    public float IC; // indice de complejidad
    public string fecha { get; set; }
	public int aciertos { get; set; }
	public int falsosAciertos { get; set; }
	public int floresNegras { get; set; }
	public int fallos { get; set; }
	public string mano { get; set; }
	public string nombreMovimiento { get; set; }
	public int tiempoEnPosicion { get; set; }
	public string ordenAparacion { get; set; }
	public int numeroDeFloresTotales { get; set; }
	public int tiempoDescanso { get; set; }
	public int tiempoReaccion { get; set; }
	public string conDistractores { get; set; }
	public int frecuenciaDistractores { get; set; }
	public float anguloArriba { get; set; }
	public float anguloAbajo { get; set; }
	public float anguloIzquierda { get; set; }
	public float anguloDerecha { get; set; }

}

public class RutinaData{
	public string NombreDeRutina;
	public string DescripcionDeRutina; // Este campo solo debe habilitarse si tenemos una versión 2 del Terapeuta
	public int Nivel;
	public int NumeroDeRepeticiones;
	public string MensajeParaPacientes;


	public string mano { get; set; }
	public SettingsModelValuesRoot.Movimiento nombreMovimiento { get; set; }
	public int tiempoEnPosicion { get; set; }
	public string ordenAparacion { get; set; }
	public int numeroSets { get; set; }
	public int tiempoDescanso { get; set; }
	public int tiempoReaccion { get; set; }
	public string conDistractores { get; set; }
	public string frecuenciaDistractores { get; set; }
	public float anguloArriba { get; set; }
	public float anguloAbajo { get; set; }
	public float anguloIzquierda { get; set; }
	public float anguloDerecha { get; set; }
}
