using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournementControler : Controler
{
    public Deck ADeck;
    public int playerWhoDrewTheCard, turnsPassed, joinedPlayers;
    public List<bool> joined;
    public List<bool> joined2;
    public GameControler InatiatedBy;
    private int Reward;
    public Dropzone DiscardDz;
    public GameObject boardUI;
    public Button joinButton, declineButton, endTurnButton;

    public void CreateTournement(List<Hand> h, List<GameObject> p, Deck d, Deck DiscardDeck, List<Dropzone> dz, int t, GameControler master, int Bonus, Card Tc){
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Create New Tournament Started ", GameControler.SName);
        turn = t;
        for (int i = 0; i < 4; i++) { joined.Add(false); }
        for (int i = 0; i < 4; i++) { joined2.Add(false); }
        playerWhoDrewTheCard = t;
        Reward = Bonus;
        DisableAllDropZones();
        DisableAllHands();
        pop.EnableTournamentStartPopup((TournamentCard)Tc);
        ifDisplay.setText(UpdateInfo(0));
        turnsPassed = 0;
        gameState = true;
        boardUI.SetActive(true);
        SetGlow(t);
        for (int i = 0; i < dzones.Count; i++){dzones[i].GetComponent<Dropzone>().Type = "TZone";}
        if(PhotonNetwork.player.ID != turn+1){pop.EnableTournamentBlockScreen(players[turn], turn);}
        if (players[turn].GetComponent<AiPlayer>() != null)
        {
            AiPlayer temp = players[turn].GetComponent<AiPlayer>();
            temp.JoinTournament(players, Reward, this);
        }
    }

    public override void join(bool choice){
        joined[turn] = choice;
        if(PhotonNetwork.player.ID != turn+1){pop.EnableJoinBlockScreen(turn, choice);}
        else{pop.EnableWaitScreen();}
        HandleTextFile.WriteLog((GameControler.LogLine += 1) + " Player " + (turn+1) + "'s choice has been shown to other players, #BNF-30", GameControler.SName);
        turnsPassed++;
        TurnOrder();
        SetGlow(turn);
        pop.EnableTournamentBlockScreen(players[turn], turn);
        if (choice == true) { joinedPlayers++; }
        HandleTextFile.WriteLog((GameControler.LogLine += 1) + " Action Log: Player "+(turn+1)+" Joins the Tournament : " + choice + ", #BNF-30", GameControler.SName);

        if (turnsPassed > 3) { Start(); }
    //AI Logic
        if (players[turn].GetComponent<AiPlayer>() != null){
            AiPlayer temp = players[turn].GetComponent<AiPlayer>();
            temp.JoinTournament(players, Reward, this);
        }
    }
    public void Start(){
        if (joinedPlayers > 1)
        {
            EnableBtn(false);
            EnableEnd(true);
            for (int i = 0; i < 4; i++)
            {
                if (joined[i] == true) { hands[i].Draw(ADeck); }
            }
            turnsPassed = 0;
            GetNext();
        }
        else if (turnsPassed > 3)
        {
            boardUI.SetActive(false);
            InatiatedBy.ReturnControl(ADeck, Discard, hands, dzones, playerWhoDrewTheCard);
        }
    }

    public override void EndTurn(){
        HandleTextFile.WriteLog("Action Log: Tournament Turn End ", GameControler.SName);
        FlipDropzone(dzones[turn], true);
        if (hands[turn].cards.Count > 12){ 
            if(PhotonNetwork.player.ID == turn+1){pop.ActivateDiscard(hands[turn].cards.Count - 12);} 
        }
        else
        {
            int next = playerWhoDrewTheCard;
            if (playerWhoDrewTheCard + 1 > 3) { next = 0; }
            if (turnsPassed == joinedPlayers)
            {
                pop.EnableTournamentBlockScreen(players[next], (next));
                EndTournament();
            }
            else
            {

                glows[turn].SetActive(false);
                glows[turn + 1].SetActive(true);
                GetNext();
            }
        }
    }

    public void EndTournament(){
        HandleTextFile.WriteLog("Action Log: Tournament Turn End ", GameControler.SName);
        CalculateWinner();
        boardUI.SetActive(false);
        gameState = false;
        InatiatedBy.ReturnControl(ADeck, Discard, hands, dzones, playerWhoDrewTheCard);
    }

    public void GetNext(){
        HandleTextFile.WriteLog("Action Log: Get Next Joined Player ", GameControler.SName);
        DisableAllDropZones();
        DisableAllHands();
        for (int i = 0; i < joined.Count; i++)
        {
            //AI Logic
            if (players[i].GetComponent<AiPlayer>() != null)
            {
                if (joined[i] == true)
                {
                    turn = i;
                    SetGlow(turn);
                    joined[i] = false;
                    joined2[i] = true;
                    EnableI(turn);
                    EnableHand(turn);
                    players[i].GetComponent<AiPlayer>().ExecuteTournament(players, Reward, this);
                    if (hands[i].cards.Count >= 12)
                    {
                        HandleTextFile.WriteLog("AI Log: Force Ai Discard " + hands[i].cards[0].name, GameControler.SName);
                        while (hands[i].cards.Count >= 12) {DiscardCard(0); }
                    }
                    turnsPassed++;
                    EndTurn();
                    break;
                }
            }
            else
            {
                if (joined[i] == true)
                {
                    turn = i;
                    pop.EnableTournamentBlockScreen(players[turn], turn);
                    pop.EnableWaitScreen();
                    SetGlow(turn);
                    EnableI(turn);
                    EnableHand(turn);
                    joined[i] = false;
                    joined2[i] = true;
                    turnsPassed++;
                    break;
                }
            }
        }
    }

    public void EnableBtn(bool state)
    {
        joinButton.gameObject.SetActive(state);
        declineButton.gameObject.SetActive(state);
    }

    public void EnableEnd(bool state){endTurnButton.gameObject.SetActive(state);}

    public void DisableAllDropZones(){
        HandleTextFile.WriteLog("UI Log: Disable All Dropzones", GameControler.SName);
        for (int i = 0; i < dzones.Count; i++)
        {
            Dropable temp = (Dropable)dzones[i].GetComponent<Dropable>();
            temp.enabled = false;
        }
    }
    public void EnableI(int DZIndex){
        HandleTextFile.WriteLog("UI Log: Enable Players" + (DZIndex + 1) + " Dropzone", GameControler.SName);
        Dropable temp = (Dropable)dzones[DZIndex].GetComponent<Dropable>();
        temp.enabled = true;
    }

    public void EnableHand(int HIndex){
        HandleTextFile.WriteLog("UI Log: Enable Players" + (HIndex + 1) + " Hand", GameControler.SName);
        hands[HIndex].gameObject.SetActive(true);
    }
    public void DisableAllHands(){
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].gameObject.SetActive(false);
            Debug.Log("Disbaling hand at " + i);
        }
    }

    public void TurnOrder(){
        if (turn < 3) { turn++; }
        else turn = 0;
        HandleTextFile.WriteLog("Action Log: Player " + (turn + 1) + " Tournament Turn Start", GameControler.SName);
    }

    public void CalculateWinner(){
        HandleTextFile.WriteLog("Action Log: Calculate Tournament Winner", GameControler.SName);
        int TBP = 0;
        int winingPlayer;
        List<int> RoundTotal = new List<int>();
        for (int i = 0; i < players.Count; i++)
        {
            if (joined2[i] == true){
                AbstractPlayer LeadPlayer = players[i].GetComponent<AbstractPlayer>();
                TBP = LeadPlayer.SetBP(dzones[i].UpdateBP("!"));
                RoundTotal.Add(TBP);
            }

            else { RoundTotal.Add(0); }
        }
        winingPlayer = TieBreaker(RoundTotal);
        if (winingPlayer < 4)
        {
            AbstractPlayer WinPlayer = players[winingPlayer].GetComponent<AbstractPlayer>();
            HandleTextFile.WriteLog("Action Log: Player " + WinPlayer.playerID + " Wins", GameControler.SName);
            WinPlayer.SetSheilds(Reward + joinedPlayers);
            InatiatedBy.TournamentWinner = winingPlayer;
            InatiatedBy.TournamentWinnerShields = (Reward + joinedPlayers);
            pop.EnableTournamentWinnerPopup(players[winingPlayer], (Reward + joinedPlayers), winingPlayer);
        }

        for (int i = 0; i < 4; i++) { FlipDropzone(dzones[i], false); }
    }

    public void ResetTournament(){
        HandleTextFile.WriteLog("Action Log: Reset Tournament Data To Default", GameControler.SName);
        turn = 5;
        playerWhoDrewTheCard = 0;
        turnsPassed = 0;
        joinedPlayers = 0;
        joined.Clear();
        joined2.Clear();
        ClearDZ(1);
        EnableBtn(true);
        EnableEnd(false);
        pop.DisableTournamentBlockScreenPopup();
    }

    public int TieBreaker(List<int> RoundTotal){
        int high = 0;
        int leadIndex = 4;
        for (int i = 0; i < RoundTotal.Count; i++)
        {
            if (RoundTotal[i] > high)
            {
                leadIndex = i;
                high = RoundTotal[i];
            }
        }
        return leadIndex;
    }

// AI Method End
    public void ClearDZ(int Mode){
        for (int i = 0; i < hands.Count; i++)
        {
            if (Mode == 1)
            {
                foreach (AmourCard AC in dzones[i].SpecificUseCards)
                {
                    AC.GetComponent<Dragable>().returnParent = Discard.transform;
                    AC.transform.SetParent(Discard.transform);
                    AC.gameObject.GetComponent<Image>().sprite = AC.GetComponent<AdventureCard>().cardBack;
                    Discard.cards.Add(AC);

                }
                dzones[i].SpecificUseCards.Clear();
            }
            foreach (EquipmentCard EC in dzones[i].Equipment)
            {
                EC.GetComponent<Dragable>().returnParent = Discard.transform;
                EC.transform.SetParent(Discard.transform);
                EC.gameObject.GetComponent<Image>().sprite = EC.GetComponent<AdventureCard>().cardBack;
                Discard.cards.Add(EC);
            }
            dzones[i].Equipment.Clear();
        }
    }
}
