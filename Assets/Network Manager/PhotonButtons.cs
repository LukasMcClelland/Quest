using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonButtons : MonoBehaviour {
    public Color[] c = {Color.red, Color.blue, Color.cyan, Color.yellow};

    public void OnClickJoinRoom(){
        PhotonNetwork.JoinRoom("Game");
    } 
    public void OnClick(){
        int arg = PhotonNetwork.player.ID;
        this.gameObject.GetComponent<PhotonView>().RPC("ChangeColor", PhotonTargets.All, arg);
    
    }
    public void OnClickPlay(){
        PhotonNetwork.LoadLevel(1);
    }
	
    [PunRPC]public void ChangeColor(int Clicked){
        this.gameObject.GetComponent<Image>().color = c[Clicked-1];
    }

}
