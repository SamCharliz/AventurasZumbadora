using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class AduanaCITAN : MonoBehaviour {

	public delegate void descargaRutinaAction(string nombreRutina);
	public static event descargaRutinaAction rutinaDescargadaConExito;
	public static event descargaRutinaAction rutinaNoEncontrada;

	public delegate void descargaPartidasAction(string[] partidas);
	public static event descargaPartidasAction partidasDescargadasDeCITAN;

	public delegate void descargaRutinasDocAction();
	public static event descargaRutinasDocAction rutinasDescargadasExito;

	public delegate void subePartidaA_CitanAction();
	public static event subePartidaA_CitanAction partidaSubidaConExito;
	public static event subePartidaA_CitanAction partidaNoSePudoSubir;

	public delegate void subeRutinaA_CitanAction();
	public static event subeRutinaA_CitanAction rutinaSubidaConExito;

	public static IEnumerator SubePartidasA_CITAN(Partida p, string idPaciente)
	{
		#if UNITY_EDITOR
		Debug.Log("Vamos a subir los datos de una partida directamente a la Base de Datos");
#endif

		//string urlString = "Nada por el momento";
		//cambiar esto por los parámetros que se guarden en la clase Partida

		Debug.Log("orden aparición flores: " + p.ordenAparacion.ToString());


		string urlString = DireccionesURL.InsertaPartidasEnBD + "?" +
			"id=" + WWW.EscapeURL(idPaciente) + "&" +
			"fecha=" + "'" + WWW.EscapeURL(p.fecha.ToString()) + "'" + "&" +
			"aciertos=" + WWW.EscapeURL(p.aciertos.ToString()) + "&" +
			"duracionPartida=" + WWW.EscapeURL(p.duracionPartida.ToString()) + "&" +
			"floresNegras=" + WWW.EscapeURL(p.floresNegras.ToString()) + "&" +
			"fallos=" + WWW.EscapeURL(p.fallos.ToString()) + "&" +
			"falsosAciertos=" + WWW.EscapeURL(p.falsosAciertos.ToString()) + "&" +
			"mano=" + WWW.EscapeURL(p.mano.ToString()) + "&" +
			"nombreMovimiento=" + WWW.EscapeURL(p.nombreMovimiento.ToString()) + "&" +
			"tiempoEnPosicion=" + WWW.EscapeURL(p.tiempoEnPosicion.ToString()) + "&" +
			"ordenAparicion=" + WWW.EscapeURL(p.ordenAparacion.ToString()) + "&" +
            "numeroDeFloresTotales=" + WWW.EscapeURL(p.numeroDeFloresTotales.ToString()) + "&" +
            "tiempoDescanso=" + WWW.EscapeURL(p.tiempoDescanso.ToString()) + "&" +
            "tiempoReaccion=" + WWW.EscapeURL(p.tiempoReaccion.ToString()) + "&" +
			"frecuenciaDistractores=" + WWW.EscapeURL(p.frecuenciaDistractores.ToString()) + "&" +
			"anguloArriba=" + WWW.EscapeURL(p.anguloArriba.ToString()) + "&" +
			"anguloAbajo=" + WWW.EscapeURL(p.anguloAbajo.ToString()) + "&" +
			"anguloIzquierda=" + WWW.EscapeURL(p.anguloIzquierda.ToString()) + "&" +
			"anguloDerecha=" + WWW.EscapeURL(p.anguloDerecha.ToString()) + "&" +
            "duracionPartida=" + WWW.EscapeURL(p.duracionPartida.ToString()) + "&" +
            "IC=" + WWW.EscapeURL(p.IC.ToString());

#if UNITY_EDITOR
        Debug.Log("Esto es lo que vamos a subir:"+urlString);
		#endif

		WWW postName = new WWW(urlString);
		yield return postName;
		if (!postName.text.Equals("Exito al subir datos Bere."))
		{
			#if UNITY_EDITOR
			Debug.Log("No pudimos subir la partida");
			#endif
			if (partidaNoSePudoSubir != null) {
				partidaNoSePudoSubir ();
			}
		}
		else
		{
			#if UNITY_EDITOR
			Debug.Log("La partida se subió con éxito");
			#endif
			if (partidaSubidaConExito != null) {
				partidaSubidaConExito ();
			}
		} 

	}

	public static IEnumerator DescargaPartidasDeCITAN(string idPaciente)
	{
		string[] Datos_S;
		string urlString = DireccionesURL.ObtenResultadosJuego + "?" + "id=" + WWW.EscapeURL(idPaciente);
		WWW postName = new WWW(urlString);
		yield return postName;
		if (!postName.text.Equals("")) //si no entramos en esta condición significa que no hay partidas en la BD
		{
			#if UNITY_EDITOR
			Debug.Log("Tenemos partidas almacenadas en la BD");
			Debug.Log(postName.text);
			#endif
			Datos_S = postName.text.Split(';');
			if (partidasDescargadasDeCITAN != null) {
				partidasDescargadasDeCITAN (Datos_S);
			}
		}
		yield return null;
	}


	public static IEnumerator DescargaRutina(string nombreXML, string rutaDeArchivos, string idPaciente)
	{
		#if UNITY_EDITOR			
		Debug.Log("Descargando rutina personalizada...");
		#endif
		string[] idTerapeuta = nombreXML.Split ('_');
		string url = DireccionesURL.DirectorioRutinasJuego + "/" + nombreXML;
		WWWForm form = new WWWForm();
		#if UNITY_EDITOR
		Debug.Log("Esto es lo que tenemos que descargar"+url);
		#endif
		WWW ww = new WWW(url);
		yield return ww;
		if (ww.error == null)
		{
			ManejadorXMLs miXml = new ManejadorXMLs ();
			miXml.BorraRutinasPrevias (idPaciente,rutaDeArchivos);
			string fullPath = rutaDeArchivos + "\\" + nombreXML;
			string nuevoNombre = "-"+idPaciente +"_AZ.xml";  //<-----------------------------------Cambiar esto por la sintáxis que corresponda al juego
			File.WriteAllBytes(fullPath, ww.bytes);
			#if UNITY_EDITOR			
			Debug.Log("Rutina descargada con Exito");
			Debug.Log("Renombramos la rutina descargada para que en vez de ser idTerapeuta_NombreRutina_AZ.xml se llame -idPaciente_NombreRutina_SandwichRutina.xml");
			#endif
			File.Move (fullPath, rutaDeArchivos + "\\" + nuevoNombre); 

			if(rutinaDescargadaConExito!=null){
				rutinaDescargadaConExito (nuevoNombre);				
			}

		}
		else
		{
			#if UNITY_EDITOR			
			Debug.Log("No tenemos rutina en CITAN.");
			#endif
			if(rutinaNoEncontrada!=null){
				rutinaNoEncontrada (" ");
			}
		}

	}

	public static IEnumerator RevisaSiTieneRutinaAsignada(string id_paciente, MonoBehaviour instanciaMono, string rutaDeArchivos){
		#if UNITY_EDITOR			
		Debug.Log("Vamos a ver si el paciente tiene una rutina asignada en CITAN");
		#endif
		string url = DireccionesURL.IdPacienteIdJuego_RevisaSiTieneRutina  + "?id=" + WWW.EscapeURL(id_paciente) + "&game_id=" + WWW.EscapeURL("17");  //<------Cambiar el 17 por el Id del juego
		WWWForm form = new WWWForm();
		WWW ww = new WWW(url);
		yield return ww;
		Debug.Log (ww.text.ToString());
		if (ww.error == null && !ww.text.ToString().Equals("Ninguna"))
		{
			string _nombreRutina = ww.text.ToString ();
			#if UNITY_EDITOR			
			Debug.Log("Existe una rutina para el paciente. Procedemos a descargarla...");
			#endif
			instanciaMono.StartCoroutine (DescargaRutina(_nombreRutina,rutaDeArchivos,id_paciente));

		}
		else
		{
			#if UNITY_EDITOR			
			Debug.Log("No tenemos rutina en CITAN.");
			#endif
			if(rutinaNoEncontrada!=null){
				rutinaNoEncontrada (" ");
			}
		}
	}

	public static IEnumerator RevisaSiTieneRutinaAsignada(string id_paciente, MonoBehaviour instanciaMono, string rutaDeArchivos, int idJuego)
	{
#if UNITY_EDITOR
		Debug.Log("Vamos a ver si el paciente tiene una rutina asignada en CITAN");
#endif
		string url = DireccionesURL.IdPacienteIdJuego_RevisaSiTieneRutina + "?id=" + WWW.EscapeURL(id_paciente) + "&game_id=" + WWW.EscapeURL(idJuego.ToString());  //<------Cambiar el 4 por el Id del juego
		WWWForm form = new WWWForm();
		WWW ww = new WWW(url);
		yield return ww;
		Debug.Log(ww.text.ToString());
		if (ww.error == null && !ww.text.ToString().Equals("Ninguna"))
		{
			string _nombreRutina = ww.text.ToString();
#if UNITY_EDITOR
			Debug.Log("Existe una rutina para el paciente. Procedemos a descargarla...");
#endif
			instanciaMono.StartCoroutine(DescargaRutina(_nombreRutina, rutaDeArchivos, id_paciente));

		}
		else
		{
#if UNITY_EDITOR
			Debug.Log("No tenemos rutina en CITAN.");
#endif
			if (rutinaNoEncontrada != null)
			{
				rutinaNoEncontrada(" ");
			}
		}
	}

	//NOTA:Hay algunos juego en los que este método no es necesario
	public static IEnumerator DescargaRutinasTerapeuta(string doc_id){
		string url = DireccionesURL.IdTerapeutaNombreJuego_CreaZipRutinas+ doc_id+"&juego='sandwich'";  //<-------CAMBIAR EL NOMBRE DEL JUEGO
		WWW postName = new WWW (url);
		yield return postName;		//creamos el ZIP de las rutinas que ha creado el terapeuta se llama id_doc.zip ej. 13.zip
		#if UNITY_EDITOR
		Debug.Log("Vamos a intentar descargar del servidor todas las rutinas que ha creado el terapeuta");
		Debug.Log ("Esto es lo que recibimos  "+postName.text.ToString());
		#endif

		if (postName.text.ToString().Contains ("ZIPcreated")) { //si existen rutinas creadas por el terapeuta descargamos el zip de esas rutinas
			url = "http://lanr.ifc.unam.mx/unity/"+doc_id+".zip";
			WWW ww = new WWW(url);
			yield return ww; //aqui ya descargamos el zip
			if (ww.error == null)
			{	
				#if UNITY_EDITOR
				Debug.Log("Hemos descargado el ZIP de rutinas del servidor");
				#endif
				string fullPath = GameMaster.rutaDeArchivos+"\\"+doc_id+".zip"; //ruta donde guardamos el ZIP que descargamos
				File.WriteAllBytes (fullPath, ww.bytes);
				string exportLocation = GameMaster.rutaDeArchivos+"\\"; 	//ruta donde extraemos el contenido del ZIP ej. "C:/Users/Yoás/Desktop//"	
				ZipUtil.Unzip ( fullPath, exportLocation);
				#if UNITY_EDITOR
				Debug.Log("Hemos descomprimido el ZIP de rutinas y estan ahora en la PC");
				#endif
				File.Delete(fullPath); //borramos el ZIP
				//borramos el ZIP del servidor
				url = DireccionesURL.IdTerapeuta_BorraZip+doc_id;
				WWW postName2 = new WWW (url);
				yield return postName2;
				if (rutinasDescargadasExito != null)
					rutinasDescargadasExito ();
				#if UNITY_EDITOR
				Debug.Log("Hemos borrado el ZIP de rutinas que se creó en el servidor");
				#endif
			}
		}
	}

	public static IEnumerator SubeRutinaAlServidor(string idTerapeuta, string DireccionRutina, string nombreRutina){
		yield return new WaitForSeconds (5f); 
		string filePath = DireccionRutina + "\\" +idTerapeuta +"_AZ.xml";   //<-------CAMBIAR por la sintaxis correcta
		// Create a Web Form
		#if UNITY_EDITOR
		Debug.Log("Vamos a intentar subir la partida al servidor");
		Debug.Log(filePath);
		#endif
		WWWForm form = new WWWForm();
		if (File.Exists (filePath)) {
			StreamReader r = File.OpenText (filePath); 
			string _info = r.ReadToEnd (); 
			r.Close (); 
			Debug.Log ("File Read");
			byte[] levelData =Encoding.UTF8.GetBytes(_info);
			string fileName = idTerapeuta+"_AZ.xml";    //<-------CAMBIAR por la sintaxis correcta
			form.AddField("file","file");
			form.AddBinaryData ( "file", levelData, fileName,"text/xml");
			#if UNITY_EDITOR
			Debug.Log("Se creo el form de la rutina, sin errores hasta ahora");
			#endif
			WWW w = new WWW(DireccionesURL.SubeRutinaACITAN,form);
			yield return w;
			yield return new WaitForSeconds (5f);
			#if UNITY_EDITOR
			Debug.Log("La partida se subió");
			Debug.Log("Me responde"+w.text.ToString());
			#endif
		} else {
			#if UNITY_EDITOR
			Debug.Log("La rutina no existe");
			#endif

		}
		if (rutinaSubidaConExito != null) {
			rutinaSubidaConExito ();
		}

	}
}
