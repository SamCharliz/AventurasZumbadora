using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;


public class PartidasTerminator : MonoBehaviour {

	//====================================================================================
	//Justo cuando se termine una partida se debe mandar a llamar a la función IntentaSubirLaPartidaA_CITAN a través de un evento desde el script que administre
	//la escena de juego
	//====================================================================================


	//Si se detectan partidas que no se han subido a CITAN indicarle al usuario con un panel que se van a subir las partidas
	//Este panel debe tener un boton que diga continuar que mandará llamar a la función Guardar de este script
	//public GameObject avisoPartidasPendiantesPanel; 

	private PlayerData _bitacoraDelJugador;
	Partida _nuevaPartida;
	private ManejadorXMLs _miXML;
	
	private string _PlayerName; 
	private bool _hayPartidasPendientesPorSubir;
	private bool _tengoConexionConCITAN;
	private bool _yaSubiLaPartidaQueAcaboDeJugar;

	void OnEnable(){
		/*
		AdminNivel1.TerminaElNivel1 += IntentaSubirLaPartidaA_CITAN;
		AdminNivel2SinRutina.TerminaElNivel2 += IntentaSubirLaPartidaA_CITAN;
		AdminNivel3.TerminaElNivel3 += IntentaSubirLaPartidaA_CITAN;
		*/
		SpawnController.PartidaTerminada += IntentaSubirLaPartidaA_CITAN;
		AduanaCITAN.partidaSubidaConExito += MarcaLaPartidaComoSubidaYArchivala;
		AduanaCITAN.partidaNoSePudoSubir += MarcaLaPartidaComoNOSubidaYArchivala;
	}

	void OnDisable(){
		/*
		AdminNivel1.TerminaElNivel1 -= IntentaSubirLaPartidaA_CITAN;
		AdminNivel2SinRutina.TerminaElNivel2 -= IntentaSubirLaPartidaA_CITAN;
		AdminNivel3.TerminaElNivel3 -= IntentaSubirLaPartidaA_CITAN;
		*/
		SpawnController.PartidaTerminada -= IntentaSubirLaPartidaA_CITAN;
		AduanaCITAN.partidaSubidaConExito -= MarcaLaPartidaComoSubidaYArchivala;
		AduanaCITAN.partidaNoSePudoSubir -= MarcaLaPartidaComoNOSubidaYArchivala;
	}


	/*En la clase Partida tambien se tienen arreglos que almacenan No de aciertos, fallos y falsos aciertos, sin embargo, primero se guardaran dichos datos de manera "local"
	 * y al final del juego se guardaran formalmente en el archivo. En una relacion 1 a 1 con los elementos de la clase Partida
	 */
	void Awake() {
		_yaSubiLaPartidaQueAcaboDeJugar = false;
		_bitacoraDelJugador = new PlayerData ();
		_bitacoraDelJugador.id = GameMaster.IdPaciente;
		_bitacoraDelJugador.nombre = GameMaster.NombrePaciente;
		_nuevaPartida = new Partida ();
		_miXML=new ManejadorXMLs();
		//avisoPartidasPendiantesPanel.SetActive(false);
	}

	void IntentaSubirLaPartidaA_CITAN(){
		Debug.Log("Entro en el evento");
		Debug.Log("Creando MLX");
		GameMaster.CreaDirectorioDelJuego();
		GameMaster.SetRutaHistorialPartidasPaciente(GameMaster.IdPaciente);
		_nuevaPartida = CreaContenedorDeLaPartidaJugada ();
	}


	Partida CreaContenedorDeLaPartidaJugada() {
		Partida _partidaRecienJugada = new Partida ();
		//OBTEN DE GAMEMASTER UN OBJETO DE TIPO PARTIDA EN VEZ DE OBTENER PARÁMETRO POR PARÁMETRO
		_partidaRecienJugada = GameMaster.CargaResultadospartida();
		
		//completa la información que falta sobre la partida como el IC u otros detalles

		/*
		_partidaRecienJugada.erroresTotales = _partidaRecienJugada.dedoIndice_errores + _partidaRecienJugada.dedoMedio_errores + _partidaRecienJugada.dedoAnular_errores + _partidaRecienJugada.dedoMenique_errores;
		_partidaRecienJugada.tiempoDeCadaRepeticion = GameMaster.TiempoDeCadaRepeticion;
		_partidaRecienJugada.tiempoTotal = 0f;
		for(int i=0;i<_partidaRecienJugada.tiempoDeCadaRepeticion.Count;i++){
			_partidaRecienJugada.tiempoTotal += GameMaster.TiempoDeCadaRepeticion [i];
		}
		_partidaRecienJugada.tiempoPromedio = _partidaRecienJugada.tiempoTotal / _partidaRecienJugada.tiempoDeCadaRepeticion.Count;
		_partidaRecienJugada.tipoDeEjercicioSeleccionado = GameMaster.EjercicioElegidoNivel2;
		_partidaRecienJugada.nombreDeLaRutina = GameMaster.NombreRutina;
		_partidaRecienJugada.IC = 0f;
		float _numeroDeEjerciciosEnTotal = _partidaRecienJugada.numeroDeRepeticiones * _partidaRecienJugada.numeroDeMovimientosTotalesPorRepeticion;
		float _relacionTiempo=(_numeroDeEjerciciosEnTotal/_partidaRecienJugada.tiempoTotal)*(30f/7f); //en un nivel que tenga 7 ejercicios por repeticion una persona normal se tarda en promedio 30 segundos
		_relacionTiempo=(_relacionTiempo>=1f)?1f:_relacionTiempo;
		float _relacionEjercicios=(_numeroDeEjerciciosEnTotal/150f); //como el número máximo de repeticiones es 10 y el número maximo de ingredientes en el nivel 3 es 15 , entonces el número máximo de movimientos en una repetición es 
		float _relacionErrores=_partidaRecienJugada.erroresTotales/_numeroDeEjerciciosEnTotal;
		_partidaRecienJugada.IC=(_relacionTiempo*50f)+(_relacionEjercicios*50f)-(_relacionErrores*10f);
		_partidaRecienJugada.IC=(_partidaRecienJugada.IC<=0f)?0f:_partidaRecienJugada.IC;
		Debug.Log ("El Ic es"+_partidaRecienJugada.IC+" relacion tiempo"+_relacionTiempo+"relacionejercicios"+_relacionEjercicios+"relacion errores"+_relacionErrores);
		Debug.Log ("tiempos"+_partidaRecienJugada.tiempoDeCadaRepeticion.Count);
		*/
		StartCoroutine (AduanaCITAN.SubePartidasA_CITAN (_partidaRecienJugada,GameMaster.IdPaciente.ToString()));
		return _partidaRecienJugada;
	}

	void MarcaLaPartidaComoSubidaYArchivala(){
		if (_yaSubiLaPartidaQueAcaboDeJugar)
			return;
		#if UNITY_EDITOR
		Debug.Log("Solo debo entrar a este evento una sola vez, que es cuando subo a la BD la partida que acabo de jugar. Pero puede volver a lanzarse si es que hay partidas sin guardar, pero evitamos eso con una bandera.");
		#endif
		_yaSubiLaPartidaQueAcaboDeJugar = true;
		_nuevaPartida.guardado = 1;

		_bitacoraDelJugador.HistorialPartidas.Add(_nuevaPartida);
		_miXML.CreaHistorialPartidasXML(_bitacoraDelJugador,GameMaster.RutaHistorialPartidasPaciente);
		#if UNITY_EDITOR
		Debug.Log("En caso de que hubiera iniciado sin Internet y durante la partida se haya restablecido la conexión voy a ver si hay partidas sin guardar en la BD. Quien se encargará de cambiar el estado del ModoSinConexion es Paciente.cs o SeleccionDispo.cs");
		#endif
		_hayPartidasPendientesPorSubir=_miXML.VerificaPartidasNoSubidas(GameMaster.RutaHistorialPartidasPaciente);
		if (_hayPartidasPendientesPorSubir)
		{
			#if UNITY_EDITOR
			Debug.Log("Hay partidas que no se han subido a la BD ");
#endif
			//avisoPartidasPendiantesPanel.SetActive(true);
			_miXML.GuardaLasPartidasPendientes(this,GameMaster.RutaHistorialPartidasPaciente, GameMaster.IdPaciente);
		}
		else
		{
			#if UNITY_EDITOR
			Debug.Log("Todas las partidas estan en la BD avanzando a a escena de Resultados");
			#endif
			
		}
		SceneManager.LoadScene("Estadisticas1");
	}

	void MarcaLaPartidaComoNOSubidaYArchivala(){

		_nuevaPartida.guardado = 0;
		_bitacoraDelJugador.HistorialPartidas.Add(_nuevaPartida);
		_miXML.CreaHistorialPartidasXML(_bitacoraDelJugador,GameMaster.RutaHistorialPartidasPaciente);		
		#if UNITY_EDITOR
		Debug.Log("Nunca tuve Internet de todas formas. No mostramos ningun panel. Continuamos con la escena de Resultados.");
		#endif
		SceneManager.LoadScene ("Estadisticas1");
		 
	}

	public void Guardar()
	{
		StartCoroutine(GuardaPartidasPendientes());
	}

	private IEnumerator GuardaPartidasPendientes()
	{

		PlayerData _myData = _miXML.CargaHistorialPartidas(GameMaster.RutaHistorialPartidasPaciente);		
		_miXML.GuardaLasPartidasPendientes(this, GameMaster.RutaHistorialPartidasPaciente, GameMaster.IdPaciente);
		yield return new WaitForSeconds(2.0f);
		SceneManager.LoadScene("Estadisticas1");		
		yield return null;
	}

}
