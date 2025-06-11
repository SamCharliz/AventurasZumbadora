using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;
using UnityEngine.SceneManagement;

public class AdminEstadisticas : MonoBehaviour {

	public delegate void Audio ();
	public static event Audio IniciaMusicaExito;

	public GameObject generalGraphPanel;

	public Button graphic_button; //este boton se instanciara sobre la grafica por cada partida, al darle click se mostraran mas detalles de la partida
	public GameObject data_window2; //La ventana donde se mostraran los detalles especificos de cada partida
	public Image grid3; // es la imagen con la cuadricula, debe cambiar de tamaño tamaño en proporcion al workSpace
	public UILineTextureRenderer graphic3; //el line renderer que dibuja la grafica
	public GameObject info_window; //La ventana donde se mostraran los detalles especificos de cada partida
	public Text noPartidas_text;
	public Text nombrePaciente;


	private PlayerData dataForRead;
	int _noPartidasJugadas;

	List<Button> botonesxNivel;
	List<GameObject> cuadrosDeInfo;	
	
	ManejadorXMLs miXml;

	// Use this for initialization
	void Start () {
		if(IniciaMusicaExito!=null){
			IniciaMusicaExito ();
		}
		dataForRead = new PlayerData ();
		miXml = new ManejadorXMLs ();

		noPartidas_text.gameObject.SetActive (false);
		nombrePaciente.text="Avance general de "+GameMaster.NombrePaciente;

		//if(miXml.BuscaArchivoXML(GameMaster.rutaDeArchivos+"\\"+"1_Data.xml")==false){   //<--------------Solo poner esto si no está implementada la escena de Login, de lo contrario poner la linea de abajo
		if(miXml.BuscaArchivoXML(GameMaster.rutaDeArchivos+"\\"+GameMaster.IdPaciente+"_Data.xml")==false){
			noPartidas_text.gameObject.SetActive (true);
			generalGraphPanel.SetActive (false);
			return;
		}

        //en el objeto dataForRead se almacenara la informacion que se lea del XML del paciente
        //dataForRead = miXml.CargaHistorialPartidas (GameMaster.rutaDeArchivos+"\\"+"1_Data.xml");   //<--------------Solo poner esto si no está implementada la escena de Login, de lo contrario poner la linea de abajo
        dataForRead = miXml.CargaHistorialPartidas(GameMaster.rutaDeArchivos + "\\" + GameMaster.IdPaciente + "_Data.xml");



        //lista que almacenara los botones creados en la grafica por nivel para despues remover los listeners de cada boton
        botonesxNivel = new List<Button> ();
		//lista que almacena las ventanas que contienen la informacion de cada partida cuando se presiona algun boton de la lista botonesxNivel 
		cuadrosDeInfo = new List<GameObject> ();

		//Primero tenemos que saber cual fue la ultima partida jugada
		_noPartidasJugadas = dataForRead.HistorialPartidas.Count;
		Debug.Log("Número de partidas: " + dataForRead.HistorialPartidas.Count);
		DibujaGraficaGeneral();

	}
		

	/*El punto de origen de la grafica es en 0,0 y va aumentando cada 27.5 tanto X como Y,
	* por lo tanto el maximo valor en Y para un punto del line renderer es 275
	*/
	void DibujaGraficaGeneral(){


		if (_noPartidasJugadas == 0) {
			//no se dibuja grafica
			return;
		}

		int NoPuntos=(_noPartidasJugadas>15)?15:_noPartidasJugadas;
		int indexListPartidasPorNivel=_noPartidasJugadas-1;
		List<string> indexList=new List<string>();

		graphic3.Points =new Vector2[NoPuntos];

		//llenando el diccionario de las ultimas 15 partidas

		for (int i=0; i<NoPuntos; i++) {
			indexList.Add (indexListPartidasPorNivel+"n"); //n indica que la partida pertenece a la lista de partidas por nivel
			indexListPartidasPorNivel--;

		}

		//Graficando las ultimas 15 partidas(maximo) que jugó el paciente
		int noIteracion=0;
		for(int i=NoPuntos-1;i>=0;i--){
			int indexToGraph=0;
			indexToGraph=int.Parse(indexList[i].Substring(0,indexList[i].Length-1));

			float _indiceComplejidad = 0f;
			_indiceComplejidad = dataForRead.HistorialPartidas [indexToGraph].IC;
			//Debug.Log("INDICE DE COMPLEJIDAD"+_indiceComplejidad);

			if(NoPuntos>1)
				graphic3.Points[noIteracion].Set(27.5f*noIteracion,2.75f*_indiceComplejidad);
			else{
				graphic3.gameObject.SetActive(false);
			}

			//Se crea un boton por cada partida jugada
			Button partida_button=Instantiate(graphic_button);
			partida_button.transform.SetParent(grid3.transform);
			Vector2 button_position=new Vector2(27.5f*noIteracion, 2.75f*_indiceComplejidad);
			partida_button.GetComponent<RectTransform>().anchoredPosition=button_position;
			partida_button.name=indexToGraph.ToString();

			//importante quitar los listener cuando el GameObject se destruya o deshabilite con DestroyListener
			//A cada boton que se ha creado se le agrega un listener para poder llamar a la funcion MoreInfoPartida tomando como argumento
			//el nombre del boton para saber el indice de la partida y mostrar sus respectiva info.

			partida_button.onClick.AddListener(()=>MoreInfoPartidapersonalizada(int.Parse(partida_button.name),"GeneralGraphPanel",button_position));
			botonesxNivel.Add(partida_button);
			Debug.Log("iteracion"+noIteracion);
			noIteracion++;
		}

	}

	//TODO
	//Editar esta parte para mostrar los datos que se midieron durante la partida en las ventanitas que salen al dar click en los botones
	//
	public void MoreInfoPartidapersonalizada(int index, string nameOfParentPanel, Vector2 button_position)
	{
		//Se instancia la pequeña ventana de datos y se agrega a la lista de ventanas
		//Se crea una ventana cada vez que se presiona el boton corresopndiente a la partida jugada
		if (data_window2)
		{
			GameObject window = (GameObject)Instantiate(data_window2, transform.position + new Vector3(button_position.x - 150f, button_position.y - 200f, 0f), transform.rotation);
			window.transform.SetParent(GameObject.Find(nameOfParentPanel).transform);
			window.transform.localScale = new Vector3(1f, 1f, 1f);
			cuadrosDeInfo.Add(window);
			//Si se necesitara ahondar mas niveles de jerarquía sería Find("NameOfGameObject/NameOfGameObject/.../NameOfTarget")
			window.transform.Find("Fecha").GetComponent<Text>().text = dataForRead.HistorialPartidas[index].fecha.ToString();
			string datosDescriptivos = "\n\n";
			datosDescriptivos = dataForRead.HistorialPartidas[index].fecha.ToString();
			//datosDescriptivos += "\nNombre de la rutina: " + dataForRead.HistorialPartidas[index].nombreDeLaRutina.ToString();
			datosDescriptivos += "\nNombre de la rutina: " + dataForRead.HistorialPartidas[index].nombreMovimiento.ToString();
			//datosDescriptivos += "\nNivel: " + dataForRead.HistorialPartidas[index].nivel.ToString();
			datosDescriptivos += "\nNo. de sets: " + dataForRead.HistorialPartidas[index].numeroDeFloresTotales.ToString();
			datosDescriptivos += "\nMano: " + dataForRead.HistorialPartidas[index].mano.ToString();
			datosDescriptivos += "\nIC: " + dataForRead.HistorialPartidas[index].IC.ToString();

			float _tiempoReal = 0.0f;
			if (dataForRead.HistorialPartidas[index].duracionPartida > 60f)
			{
				_tiempoReal = (int)(dataForRead.HistorialPartidas[index].duracionPartida / 60) + ((dataForRead.HistorialPartidas[index].duracionPartida % 60) / 100f);
				datosDescriptivos += "\nTiempo total de la partida: " + _tiempoReal.ToString() + "min";
			}
			else
			{
				datosDescriptivos += "\nTiempo total de la partida: " + dataForRead.HistorialPartidas[index].duracionPartida.ToString() + " s";
			}

			window.transform.Find("Base/DetailedDescription").GetComponent<Text>().text = datosDescriptivos;
		}
	}

	//
	//TODO
	//Editar esta parte para continuar con la escena correcta
	//
	public void Continuar()
	{
		//SceneManager.LoadScene(GameMaster.escenaDeDondeVengo);
		
		if (GameMaster.Modo.Equals (GameMaster.ModoLogin.Clinica)) {
			SceneManager.LoadScene ("Menu");
		}
		if (GameMaster.Modo.Equals (GameMaster.ModoLogin.Paciente)) {
			SceneManager.LoadScene ("Paciente");
		}
		if (GameMaster.Modo.Equals (GameMaster.ModoLogin.Terapeuta)) {
			SceneManager.LoadScene ("Terapeuta"); //tiene que regresar a la selección de paciente
		}
		

	}

	//===De aquí para abajo no es necesario modicar el script======================================================================================

	public void ShowLittleInfoWindow()
	{
		info_window.SetActive(true);
	}

	float CalculaTiempoHHMMSS(string date)
	{
		int time = 0;
		//Debug.Log(date + "LALALA");
		if (date.Length > 1)
		{
			Debug.Log(date.Substring(0, 2));
			Debug.Log(date.Substring(3, 2));
			Debug.Log(date.Substring(6, 2));
			time += int.Parse(date.Substring(0, 2)) * 3600;
			time += int.Parse(date.Substring(3, 2)) * 60;
			time += int.Parse(date.Substring(6, 2));
		}
		return time / 86400f;
	}

	//Devuelve true si la partida mas reciente es la Personal, false si la partida mas reciente es la que se jugo por Nivel
	bool ComparaFechasEntrePartidas(string fechaPartidaPersonal, string fechaPartidaNivel)
	{
		float diasPersonal = 0;
		float diasNivel = 0;
		if (!fechaPartidaPersonal.Equals(""))
		{
			diasPersonal += (float)CalculaDiasTranscurridos(fechaPartidaPersonal.Substring(0, 10));
			diasPersonal += CalculaTiempoHHMMSS(fechaPartidaPersonal.Substring(11, 8));
		}
		if (!fechaPartidaNivel.Equals(""))
		{
			diasNivel += (float)CalculaDiasTranscurridos(fechaPartidaNivel.Substring(0, 10));
			diasNivel += CalculaTiempoHHMMSS(fechaPartidaNivel.Substring(11, 8));
		}
		Debug.Log("dias personal" + diasPersonal + " y dias nivel" + diasNivel);
		if (diasPersonal > diasNivel)
			return true;
		else
			return false;
	}

	//Esta funcion devuelve cuantos dias han pasado en el año hasta el dia que se encuentra en el argumento date  yyyy-MM-dd  dd/MM/yyyy
	int CalculaDiasTranscurridos(string date){	
		int año=int.Parse(date.Substring(2,2));
		int dia=int.Parse(date.Substring(date.LastIndexOf("-")+1,2)); 
		int dias_transcurridos = dia + DiasMes(date.Substring(date.IndexOf("-")+1,2)) + (año* 365);
		return dias_transcurridos;

	}

	//Esta funcion devuelve cuantos dias han pasado en el año hasta el mes indicado en el argumento mes
	int DiasMes(string mes){

		int enero = 31;
		int febrero = enero + 29;
		int marzo = febrero + 31;
		int abril = marzo + 30;
		int mayo = abril + 31;
		int junio = mayo + 30;
		int julio = junio + 31;
		int agosto = julio + 31;
		int septiembre = agosto + 30;
		int octubre = septiembre + 31;
		int noviembre = octubre + 30;
		int diciembre = noviembre + 31;

		switch(mes){
			case "01":
				return enero;
			case "02":
				return febrero;
			case "03":
				return marzo;
			case "04":
				return abril;
			case "05":
				return mayo;
			case "06":
				return junio;
			case "07":
				return julio;
			case "08":
				return agosto;
			case "09":
				return septiembre;
			case "10":
				return octubre;
			case "11":
				return noviembre;
			case "12":
				return diciembre;
			default:
				return 0; 
		}

	}

	


	void RemoveInfoWindows(){
		foreach (GameObject cuadro in cuadrosDeInfo)
			Destroy (cuadro);
		cuadrosDeInfo.Clear ();
	}

	void RemoveListeners(List<Button> botonesDePartidas){
		if (botonesDePartidas.Count != 0) {
			foreach(Button p in botonesDePartidas){
				p.onClick.RemoveAllListeners();
				Destroy(p.gameObject);
			} 
		}
		botonesDePartidas.Clear ();
	}

	bool IsFirstDateOlder(string Date1, string Date2){
		Debug.Log (Date1);
		Debug.Log (Date2);
		return true;
	}

	

}
