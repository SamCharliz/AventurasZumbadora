using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VerificadorRed{
	
	public delegate void verificaConexionAction();
	public static event verificaConexionAction noHayConexionConCITAN;
	public static event verificaConexionAction tenemosConexionConCITAN;


	public static IEnumerator VerificaConexionConCITAN()
	{	
		#if UNITY_EDITOR
		Debug.Log("Verificando si hay conexion con Internet...");
		#endif
		WWW www = new WWW(DireccionesURL.LigaParaProbarSiHayInternet);
		yield return www;
		Debug.Log(www.error);
		if (www.error != null)
		{
			#if UNITY_EDITOR
			Debug.Log("No hay conexión con el servidor. Debemos registrar que estamos en modo SIN conexión");
			#endif
			if(noHayConexionConCITAN!=null){
				noHayConexionConCITAN ();
			}
		}
		else {
			#if UNITY_EDITOR
			Debug.Log("Si hay conexión con el servidor. Debemos registrar que estamos en modo CON conexión.");
			#endif
			if (tenemosConexionConCITAN != null) {
				tenemosConexionConCITAN ();
			}
		}

	}


}
