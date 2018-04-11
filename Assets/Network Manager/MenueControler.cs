using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenueControler : MonoBehaviour {
	public PhotonButtons Play;
	public PhotonButtons Join;
	public PlayerList PlayerZone; 


	public void DisableJoin(){
		Join.gameObject.SetActive(false);
	}

	public void EnablePlay(){
		Play.gameObject.SetActive(true);
	}

	public void CreatePlayer(int id){
		PlayerZone.CreatePlayer(id);
	}

	public void CleanList(){
		foreach (Transform child in PlayerZone.transform){Destroy(child.gameObject);}
	}
}
