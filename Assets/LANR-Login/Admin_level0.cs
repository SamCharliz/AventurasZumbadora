using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml; 
using System.Xml.Serialization; 
using System.Text;
using System.IO;
using UnityEngine.UI;
using Ionic.Zip;
//using UnityEngine.Networking;
//using NUnit.Framework.Internal.Filters;

public class Admin_level0 : MonoBehaviour {

	public InputField nombrePaciente_input;
	public InputField passwordPaciente_input;
	public InputField nombreTerapeuta_input;
	public InputField contraseniaTerapeuta_input;
	[Tooltip("Aquí debemos poner el texto que le dice al paciente que debe ingresar una contraseña, o que la contraseña que ingresó fue incorrecta.")]
	public Text warning_text;
	//[Tooltip("Aquí debemos poner el texto que le dice al terapeuta que ingresó una contraseña incorrecta o que se deben ingresar las credenciales correspondientes cuando no hay Internet")]
	//public Text warningText_clinica;
	//[Tooltip("Aquí debe ir el panel que contiene al texto warningText_clinica")]
	//public GameObject warningPanel_clinica;
	[Tooltip("Aquí va el panel que le dice al paciente que no tienen Internet y de si esta seguro que desea jugar sin conexión")]
	public GameObject panelAdvertenciaSinInternet;

    [Tooltip("Aquí va el panel de fondo (Opcional)")]
    public GameObject panelFondo;
    [Tooltip("Aquí va el panel donde el paciente ingresa el ID y su contraseña")]
    public GameObject panelAutenticaPaciente;
    [Tooltip("Aquí va el panel donde se le pregunta al paciente si lo esta asistiendo un terapeuta")]
	public GameObject panelPreguntaSiEstaAsistido;
	[Tooltip("Aquí va el panel donde el terapeuta tiene que ingresar su correo y su contraseña")]
	public GameObject panelAutenticaTerapeuta;

	string _clave;
	string _alfabeto = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	ManejadorXMLs _miXML;

	void OnEnable(){
		VerificadorRed.noHayConexionConCITAN += RegistraQueNoHayInternetAntesDeContinuar;
		VerificadorRed.tenemosConexionConCITAN += RegistraQueSIHayInternetAntesDeContinuar;
	}

	void OnDisable(){
		VerificadorRed.noHayConexionConCITAN -= RegistraQueNoHayInternetAntesDeContinuar;
		VerificadorRed.tenemosConexionConCITAN -= RegistraQueSIHayInternetAntesDeContinuar;
	}

	void Awake () {
        GameMaster.CreaDirectorioDelJuego();
		_miXML = new ManejadorXMLs ();
		_clave = "";       
		GameMaster.LimpiaNombreyIdPaciente ();
		GameMaster.LimpiaNombreyIdTerapeuta ();
		GameMaster.RegistraQueNoEstoyLogeado ();
        if (panelFondo != null)
        {
            panelFondo.SetActive(false);
        }
        panelAutenticaPaciente.SetActive(true);
        panelAutenticaTerapeuta.SetActive (false);
		panelPreguntaSiEstaAsistido.SetActive (false);
		warning_text.text = "";
		panelAdvertenciaSinInternet.SetActive (false);

		StartCoroutine(VerificadorRed.VerificaConexionConCITAN());


	}

	void RegistraQueNoHayInternetAntesDeContinuar(){
		#if UNITY_EDITOR
		Debug.Log("No hay conexión con el servidor. Registramos que estamos en modo SIN conexión y le damos su clave al paciente");
		#endif
		GameMaster.RegistraQueEstoyEnModoSINConexion ();
	}

	void RegistraQueSIHayInternetAntesDeContinuar(){
		#if UNITY_EDITOR
		Debug.Log("Registramos que estamos en modo CON conexión.");
		#endif
		GameMaster.RegistraQueEstoyEnModoConConexion ();
		//StartCoroutine(LoadName());
	}

    public void MuestraPanelLogIn() {
        if (panelFondo != null) {
            panelFondo.SetActive(true);
        }
        panelAutenticaPaciente.SetActive(true);
    }

	public void CompruebaUsuario (){
		string _usuario = nombrePaciente_input.text.ToString().Trim();
		if (_usuario.Length == 0)
		{
			warning_text.text = "Debe ingresar un ID de usuario para continuar.";
			return;
		}
        #if UNITY_EDITOR
        Debug.Log ("Con Internet:  "+GameMaster.ModoSinConexion);
        #endif
        int i = 0;
		bool _idNumerico = int.TryParse (nombrePaciente_input.text.ToString (), out i);
		if(_idNumerico == true){			
			RevisaDatosPaciente();
		} else {
			if (GameMaster.ModoSinConexion) {
				warning_text.text=" ";
				warning_text.text = "No hay conexión con el servidor, revise su conexión a Internet e intente más tarde.";
			} else {
				StartCoroutine (VerifyTherapist (nombrePaciente_input.text.ToString (), passwordPaciente_input.text.ToString (), false));
			}
		}
	} 

	void RevisaDatosPaciente(){
        warning_text.text = "";
        string _idPaciente = nombrePaciente_input.text.ToString().Trim();
		#if UNITY_EDITOR
		Debug.Log("Tengamos o no Internet asignamos 3 variables, el id del paciente, el nombre del archivo XML que almacenará las partidas y la ruta completa de donde se encuentra ese archivo.");
		#endif
		GameMaster.SetFileName (_idPaciente.ToString());
		GameMaster.SetIdPaciente (_idPaciente.ToString());
		GameMaster.SetRutaHistorialPartidasPaciente (_idPaciente.ToString());
		if (GameMaster.ModoSinConexion) {
			panelAdvertenciaSinInternet.gameObject.SetActive(true);
		} else {
			StartCoroutine (LoadName ());
		}

	}

	public void GeneraClaveParaJugar(){
		panelAdvertenciaSinInternet.SetActive(false);
		panelPreguntaSiEstaAsistido.SetActive (true);
	}

	public void VeACreditos(){
		GameMaster.escenaDeDondeVengo = "Inicia Sesion";
		SceneManager.LoadScene ("Creditos");
	}

//	public void PreguntaSiEstaAsistidoAunqueSinInternet()
//	{
//		#if UNITY_EDITOR
//		Debug.Log("Guardamos en GameMaster la clave que se generó");
//		#endif
//		//GameMaster.clave = _clave;
//		panelPreguntaSiEstaAsistido.SetActive (true);
//	}	

	public void CancelarIngreso()
	{
		panelAdvertenciaSinInternet.gameObject.SetActive(false);
	}



	public void NoGuardar()
	{
		#if UNITY_EDITOR
		Debug.Log("El paciente ha elegido no subir las partidas que jugó sin Internet a CITAN, se eliminarán. Preguntamos si el paciente esta asistido por un terapeuta.");
		#endif
		_miXML.NoGuardesLasPartidasPendientes(GameMaster.RutaHistorialPartidasPaciente);
		panelAutenticaTerapeuta.SetActive(true);
	}

	public void Guardar()
	{
		#if UNITY_EDITOR
		Debug.Log("El usuario ha elegido subir a CITAN las partidas que se jugaron sin Internet. Se pedirá la clave.");
		#endif
		//PlayerData _myData = _miXML.CargaHistorialPartidas(GameMaster.RutaHistorialPartidasPaciente);
		//if (clave_input.text == _myData.HistorialPartidas[_myData.HistorialPartidas.Count - 1].clave)
		//{
			#if UNITY_EDITOR
			Debug.Log("Se ha ingresado una clave válida, se subirán las partidas a CITAN. Preguntamos si el paciente esta asistido por un terapeuta.");
			#endif
			//claveCorrectaIncorrecta_text.text = "Clave válida, espera a que se guarden los datos.";
			_miXML.GuardaLasPartidasPendientes(this,GameMaster.RutaHistorialPartidasPaciente,GameMaster.IdPaciente);
			//panelActualizaPartidas.SetActive (false);
			panelPreguntaSiEstaAsistido.SetActive(true);
		//}
		//else
		//{
		//	claveCorrectaIncorrecta_text.text = "Clave no válida, vuelve a intentarlo.";
		//}
	}

	void GenerarClave()
	{
		_clave = Random.Range(0, 9) + _alfabeto[Random.Range(0, _alfabeto.Length)].ToString() + Random.Range(0, 9) + _alfabeto[Random.Range(0, _alfabeto.Length)].ToString();
		//clave_text.text = _clave;
	}
		
	private IEnumerator LoadName()
	{    	
		#if UNITY_EDITOR
		Debug.Log("Tenemos Internet, verificando que el ID que se ingresó corresponda a un paciente.");
		#endif
		warning_text.text=" ";
		
		string urlString1 = DireccionesURL.Id_NombrePaciente + WWW.EscapeURL(GameMaster.IdPaciente);
		WWW postName1 = new WWW(urlString1);
        yield return postName1;
		//string urlString1 = DireccionesURL.Id_NombrePaciente + UnityWebRequest.EscapeURL(GameMaster.IdPaciente);
		//UnityWebRequest postName1 = new UnityWebRequest(urlString1);
		//postName1.downloadHandler = new DownloadHandlerBuffer();
		//yield return postName1.SendWebRequest();
		//if (postName1.downloadHandler.text == "Inexistente")
		if (postName1.text == "Inexistente")
		{
			#if UNITY_EDITOR
			Debug.Log("El ID que se ingresó no existe en la BD");
			#endif
			warning_text.text = "ID de usuario inválido";
			yield break;
		}
		//GameMaster.SetNombrePaciente(postName1.downloadHandler.text);
		GameMaster.SetNombrePaciente(postName1.text);

#if UNITY_EDITOR
		Debug.Log("Hemos obtenido el nombre completo del paciente"+GameMaster.NombrePaciente);
		#endif

		if (passwordPaciente_input.text.Length == 0)
		{
			warning_text.text = "Ingrese su contraseña.";
			yield break;
		}

		#if UNITY_EDITOR
		Debug.Log("Vamos a obtener la contraseña del paciente...");
#endif

		
		string urlString2 = DireccionesURL.Id_ContraseniaPaciente + "?id=" + WWW.EscapeURL(GameMaster.IdPaciente);
		WWW postName2 = new WWW(urlString2);
        yield return postName2;
        string _password = postName2.text.ToString();
		//string urlString2 = DireccionesURL.Id_ContraseniaPaciente + "?id=" + UnityWebRequest.EscapeURL(GameMaster.IdPaciente);
		//UnityWebRequest postName2 = new UnityWebRequest(urlString2);
		//postName2.downloadHandler = new DownloadHandlerBuffer();
		//yield return postName2.SendWebRequest();        
		//string _password = postName2.downloadHandler.text.ToString();

#if UNITY_EDITOR
		Debug.Log("Contraseña"+_password);
		#endif

		if (!passwordPaciente_input.text.Equals (_password)) {
			warning_text.text = "Contraseña no válida.";
			yield break;
		}

		#if UNITY_EDITOR
		Debug.Log("Vemos si hay un archivo XML del paciente en la PC, si lo hay vemos si hay partidas sin subir, si no lo hay en la sig. escena intentaremos descargarlo de CITAN");
		#endif

		if (_miXML.BuscaArchivoXML(GameMaster.RutaHistorialPartidasPaciente))
		{          
			#if UNITY_EDITOR
			Debug.Log("El paciente cuenta con partidas en esta PC. Vamos a ver si hay partidas sin guardar.");
			#endif
			GameMaster.RegistraQueSiHayXMLdelPacienteEnPC ();
			bool _necesitoActualizarPartidas = _miXML.VerificaPartidasNoSubidas (GameMaster.RutaHistorialPartidasPaciente);
			if (_necesitoActualizarPartidas) {
				#if UNITY_EDITOR
				Debug.Log("Hay partidas que no están guardadas en CITAN");
				#endif
				//panelActualizaPartidas.SetActive (true);
				Guardar();
			} else {
				#if UNITY_EDITOR
				Debug.Log("Todas las partidas están guardadas en CITAN");
				#endif
				panelPreguntaSiEstaAsistido.SetActive(true);
			}
		}
		else // Cuando no existe el archivo XML de la persona hay que tomar datos del servidor
		{    
			#if UNITY_EDITOR
			Debug.Log("No se encontró un archivo XML. Se intentarán descargar el historial de partidas del servidor.");
			#endif
			GameMaster.RegistraQueNoHayXMLdelPacienteEnPC (); 
			panelPreguntaSiEstaAsistido.SetActive(true);
		}

	}

	public void Independiente(){
		#if UNITY_EDITOR
		Debug.Log("Entramos al juego como Paciente únicamente. Registramos que NO estamos en modo clínica");
		#endif
		GameMaster.RegistraQueEstoyEnModoPaciente();
		SceneManager.LoadScene ("Paciente");
	}

	public void RevisaDatosTerapeuta(){
		if (nombreTerapeuta_input.text.Equals ("") || contraseniaTerapeuta_input.text.Equals ("")) {
			//warningPanel_clinica.SetActive (true);
			warning_text.text = "Debe de ingresar un nombre de usuario y una contraseña para poder continuar.";
			return;
		}
		if(!GameMaster.ModoSinConexion){  //Hay conexión con el servidor
			#if UNITY_EDITOR
			Debug.Log("Hay conexión con el servidor, comprobaremos que el correo existe en CITAN");
			#endif
			//warningPanel_clinica.SetActive (false);
			StartCoroutine(VerifyTherapist(nombreTerapeuta_input.text.ToString (), contraseniaTerapeuta_input.text.ToString (),true));  //se revisa en la tabla Doctor si este existe
		}
		else{ //No hay conexion con el servidor
			#if UNITY_EDITOR
			Debug.Log("No hay conexión con el servidor, se deben ingresar las credenciales clinica/clinica. Registramos que estamos en modo clinica");
			#endif
			if (nombreTerapeuta_input.text.Equals (GameMaster.TerapeutaCuandoNoHayConexion) && contraseniaTerapeuta_input.text.Equals (GameMaster.ContraseniaDeTerapeutaCuandoNoHayConexion)){
				//Iniciamos sesion en clinica sin conexion
				GameMaster.RegistraQueEstoyEnModoClinica();	
				SceneManager.LoadScene("Menu");
			}else{
				//warningPanel_clinica.SetActive (true);
				warning_text.text="Error. Por favor ingrese el nombre de usuario y contraseña\ncorrespondientes cuando no hay conexión.";
			}
		}
	}

	private IEnumerator VerifyTherapist(string email,string password, bool quieroEntrarAModoClinica){
		string urlStringDoctor = DireccionesURL.Email_NombreTerapeutaID + WWW.EscapeURL(email);		
		WWW postNameDoctor = new WWW(urlStringDoctor);
		yield return postNameDoctor;
		//string urlStringDoctor = DireccionesURL.Email_NombreTerapeutaID + UnityWebRequest.EscapeURL(email);
		//UnityWebRequest postNameDoctor = new UnityWebRequest(urlStringDoctor);
		//postNameDoctor.downloadHandler = new DownloadHandlerBuffer();
		//yield return postNameDoctor.SendWebRequest();
		if (postNameDoctor.text.Equals("Inexistente"))
		//if (postNameDoctor.downloadHandler.text.Equals("Inexistente"))
		{
			//warningPanel_clinica.SetActive (true);
			warning_text.text = "Correo inválido";
			yield break;
		}

		//string nombreTerapeuta = postNameDoctor.downloadHandler.text.ToString().Substring(0, postNameDoctor.downloadHandler.text.ToString().LastIndexOf(" "));
		//string idTerapeuta = postNameDoctor.downloadHandler.text.ToString().Substring(postNameDoctor.downloadHandler.text.ToString().LastIndexOf(" ")+1);

		string nombreTerapeuta = postNameDoctor.text.ToString().Substring(0, postNameDoctor.text.ToString().LastIndexOf(" "));
		string idTerapeuta = postNameDoctor.text.ToString().Substring(postNameDoctor.text.ToString().LastIndexOf(" ")+1);

		GameMaster.AsignaNombreyIdTerapeuta (nombreTerapeuta,idTerapeuta);

		
		string urlString1 = DireccionesURL.EmailPassword_TrueFalse + WWW.EscapeURL(email) + "/" + WWW.EscapeURL(password);
		WWW postName1 = new WWW(urlString1);
        yield return postName1;
		//string urlString1 = DireccionesURL.EmailPassword_TrueFalse + UnityWebRequest.EscapeURL(email) + "/" + UnityWebRequest.EscapeURL(password);
		//UnityWebRequest postName1 = new UnityWebRequest(urlString1);
		//postName1.downloadHandler = new DownloadHandlerBuffer();
		//yield return postName1.SendWebRequest();
		if (postName1.text.Equals("true"))
		//if (postName1.downloadHandler.text.Equals("true"))
		{				
			if(quieroEntrarAModoClinica){
				#if UNITY_EDITOR
				Debug.Log("Las credenciales coinciden. Registramos que estamos en modo clinica");
				#endif
				GameMaster.RegistraQueEstoyEnModoClinica ();
				SceneManager.LoadScene("Menu");		
			}else{
				#if UNITY_EDITOR
				Debug.Log("Las credenciales coinciden. Registramos que estamos en modo terapeuta");
				#endif
				GameMaster.RegistraQueEstoyEnModoTerapeuta ();
				SceneManager.LoadScene("Terapeuta");
			}
		}
		else
		{
			//warningPanel_clinica.SetActive (true);
			warning_text.text = "Contraseña incorrecta. Por favor verifique e intente nuevamente.";
			yield break;
		}
	}





	void Update(){
		if (Input.GetKeyDown (KeyCode.Tab) && !panelAutenticaTerapeuta.activeSelf) {
			if (!nombrePaciente_input.isFocused)
				nombrePaciente_input.Select ();
			else
				passwordPaciente_input.Select ();
		}

		if(Input.GetKeyDown (KeyCode.Return) && !panelAutenticaTerapeuta.activeSelf){
			CompruebaUsuario ();
		}

		if (Input.GetKeyDown (KeyCode.Tab) && panelAutenticaTerapeuta.activeSelf) {
			if (!nombreTerapeuta_input.isFocused)
				nombreTerapeuta_input.Select ();
			else
				contraseniaTerapeuta_input.Select ();
		}

		if(Input.GetKeyDown (KeyCode.Return) && panelAutenticaTerapeuta.activeSelf){
			RevisaDatosTerapeuta ();
		}
	}


	//============================================================================================================================================
//	public static InfoPartida datos;
//	public static TerapeutaData terapeuta;
//	public static Nivel2 datosNivel2;
//	//public static string nombreRutinaAJugar;
//
//	private static string nombreRutinaTemp;
//	public static string NombreRutinaTemp{
//		get { return nombreRutinaTemp; }
//	}
//
//	private static bool asistidoPorTerapeuta;
//	public static bool AsistidoPorTerapeuta{
//		get {return asistidoPorTerapeuta;}
//	}
//
//	private static bool rutinaAsignada;
//	public static bool RutinaAsignada{
//		set { rutinaAsignada = value; }
//		get { return rutinaAsignada; }
//	}
		
}