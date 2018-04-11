using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour {
	public string[] Teams = {"Red", "Blue", "Green", "Black"};
	public GameObject PlayerPrefab;

	public void CreatePlayer(int id)
	{
		LobbyPlayer temp = PlayerPrefab.GetComponent<LobbyPlayer>();
		temp.PlayerName = Teams[id];
		temp.Pid = id;
		temp.DisplayText.text = "Player" + (id+1).ToString() + " Ready";
		GameObject PL = Instantiate(PlayerPrefab);
		PL.transform.SetParent(transform,false);	
		Debug.Log("player Created");
	}
}