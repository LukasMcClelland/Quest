using UnityEngine;

public class ButtonScript : Photon.MonoBehaviour
{
    public Controler TempGameScript;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

    // Refactored Button Handlers
    public void ButtonHandlerBlockScreen(){
        if (PhotonNetwork.player.ID == TempGameScript.turn + 1){
            TempGameScript.pop.DisableWaitScreen();
            TempGameScript.pop.DisableBlockScreenPopup();
        }
        else {TempGameScript.pop.DisableBlockScreenPopup();}
    }
    public void ButtonHandlerEventScreen(){
        if (PhotonNetwork.player.ID == TempGameScript.turn + 1){
            TempGameScript.pop.DisableWaitScreen();
            TempGameScript.pop.DisableEventScreenPopup();
        } 
        else { TempGameScript.pop.DisableEventScreenPopup(); }
    }
    public void ButtonHandlerTournamentBlockScren(){
        if (PhotonNetwork.player.ID ==  TempGameScript.turn + 1){
            TempGameScript.pop.DisableWaitScreen();
            TempGameScript.pop.DisableTournamentBlockScreenPopup();
        }
        else { TempGameScript.pop.DisableTournamentBlockScreenPopup(); }
    }
    public void ButtonHandlerTournamentStartScren(){
        if (PhotonNetwork.player.ID ==  TempGameScript.turn + 1){TempGameScript.pop.DisableTournamentStartPopup();}
        else { TempGameScript.pop.DisableTournamentStartPopup(); }
    }
    public void ButtonHandlerQuestBlockScren(){
        if (PhotonNetwork.player.ID == TempGameScript.turn + 1) {
            TempGameScript.pop.DisableWaitScreen();
            TempGameScript.pop.DisableQuestBlockScreenPopup();
        }
        else { TempGameScript.pop.DisableQuestBlockScreenPopup();}
    }
    public void ButtonHandlerQuestStartScreen(){
        if (PhotonNetwork.player.ID == TempGameScript.turn + 1) {TempGameScript.pop.DisableQuestStartPopup(); }
        else { TempGameScript.pop.DisableQuestStartPopup(); }
    }
    public void ButtonHandlerJoinScreen(){
        if (PhotonNetwork.player.ID ==  TempGameScript.turn + 1){
            TempGameScript.pop.DisablejoinScreenPopup();}
        else { TempGameScript.pop.DisablejoinScreenPopup();}
    }
    public void ButtonHandlerSponsorScreen(){
        if (PhotonNetwork.player.ID ==  TempGameScript.turn + 1){TempGameScript.pop.DisableSponsorScreenPopup();}
        else {TempGameScript.pop.DisableSponsorScreenPopup();}
    }
    public void RW(){
        HandleTextFile.WriteLog("UI log: Congrats You Found Our Captain...GOOD MORNING VIETNAM", GameControler.SName);
        TempGameScript.pop.EnableRW();
    }
    public void ButtonHandlerCheat()
    {
        ((QuestController)TempGameScript).Cheat();
    }
    public void ButtonHandlerDiscard(){TempGameScript.pop.DisableDiscardPopup();}
    public void ButtonHandlerSponsorIneligibleScreen(){TempGameScript.pop.DisableSponsorIneligiblePopup();}
    public void ButtonHandlerTournamentWinnerScreen(){TempGameScript.pop.DisableTournamentWinnerPopup();}
    public void ButtonHandlerQuestWinnerScreen() { TempGameScript.pop.DisableQuestWinnerPopup(); }
    public void Discard(){TempGameScript.pop.DisableDiscardPopup();}
    public void DisableRW(){TempGameScript.pop.DisableRWPopup();}

//

// RPC Wrappers 
    public void end(){
        HandleTextFile.WriteLog("UI log: Pass Turn Button Presed", GameControler.SName);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedEndTurn", PhotonTargets.All);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedWait", PhotonTargets.All);
    }
    public void Join(){
        HandleTextFile.WriteLog("UI log: Join Button Presed", GameControler.SName);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedJoin", PhotonTargets.All);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedWait", PhotonTargets.All);
    }
    public void Decline(){
        HandleTextFile.WriteLog("UI log: Decline Button Presed", GameControler.SName);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedDecline", PhotonTargets.All);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedWait", PhotonTargets.All);
    }
    public void Sponsor(){
        HandleTextFile.WriteLog("UI log: Sponsor Button Presed", GameControler.SName);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedSponsor", PhotonTargets.All);
        this.gameObject.GetComponent<PhotonView>().RPC("NetworkedWait", PhotonTargets.All);
    }
//
    public void DisableResolveStageScreen(){TempGameScript.pop.DisableResolveStagePopup();}

    public void DisableQuestWinnerScreen(){TempGameScript.pop.DisableQuestWinnerPopup();}

//RPC Calls
    [PunRPC]
    public void NetworkedEndTurn(){
        HandleTextFile.WriteLog("Networking log: Game State Posted", GameControler.SName);
        TempGameScript.EndTurn();
    }
    [PunRPC]
    public void NetworkedJoin(){
        HandleTextFile.WriteLog("Networking log: Game State Posted", GameControler.SName);
        TempGameScript.join(true);
    }
    [PunRPC]
    public void NetworkedDecline(){
        HandleTextFile.WriteLog("Networking log: Game State Posted", GameControler.SName);
        TempGameScript.join(false);    
    }
    [PunRPC]
    public void NetworkedWait(){
        int PID = PhotonNetwork.player.ID;
        int TN = TempGameScript.turn + 1;
        if (PID == TN)
        {
            //TempGameScript.pop.DisableWaitScreen();
        }
        else {//TempGameScript.pop.GameWaitScreen.gameObject.SetActive(true); 
        }
    }
    [PunRPC]
    public void NetworkedSponsor(){
        if(((QuestController)TempGameScript).CanPlayerSponsor() == true){
            ((QuestController)TempGameScript).SponsorQuest();
        }
    }
//


}