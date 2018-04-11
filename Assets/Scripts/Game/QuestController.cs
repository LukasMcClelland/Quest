using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : Controler
{
    private const int FIND_SPONSOR = 0;
    private const int BUILD_QUEST = 1;
    private const int JOIN_QUEST = 2;
    private const int QUESTING = 3;
    private const int STAGE_RESOLVE = 4;
    private const int QUEST_RESOLVE = 5;

    public GameControler GameController;
    public QuestCard questCard;
    public List<Dropzone> qzones;
    public List<GameObject> StageNames;
    public Deck ADeck;
    public int numQuestingPlayers, turnsPassed;
    public int phase, sponsorPlayer, stageNumber, totalNumStages, playerWhoDrewTheCard, playersWhoDidntSponsor, remainingPlayerTurnsInAStage;
    public GameObject AcceptBtn, DeclineBtn, EndTurnBtn, boardUI, qzonePlaceholder, SponsorButton, qzoneTextPlaceHolder;
    public int[] BPArray;
    public List<bool> questingPlayers;

    public void CreateQuest(GameControler g, List<GameObject> p, List<Hand> h, int t, Card card, Deck d, Deck disc){
        HandleTextFile.WriteLog("Action Log: " + p.Count + " player entering quest", GameControler.SName);
        boardUI.SetActive(true);
        DisableAllDropZones();
        DisableAllHands();
        pop.EnableQuestStartPopup(card);
        questCard = (QuestCard)card;
        for (int i = 0; i < 4; i++) { questingPlayers.Add(true); }
        playerWhoDrewTheCard = turn = t;
        phase = FIND_SPONSOR;
        stageNumber = 0;
        BPArray = new int[5];
        ifDisplay.setText(UpdateInfo(0));
        gameState = true;
        if(PhotonNetwork.player.ID != turn+1){
            pop.EnableQuestBlockScreen(players[turn], turn);
            pop.EnableWaitScreen();
        }
        hands[turn].gameObject.SetActive(true);
        numQuestingPlayers = players.Count;
        foreach(Dropzone DZ in dzones) { DZ.Type = "PlayerQZone"; }
        ToggleBoard(turn, 0);
        playersWhoDidntSponsor = turnsPassed = 0;
        sponsorPlayer = -1;
        if (players[turn].GetComponent<AiPlayer>() != null){AiBehavior("Start");}

    }
    public override void EndTurn(){
        if (hands[turn].cards.Count > 12)
        {
            if (PhotonNetwork.player.ID == turn + 1) { pop.ActivateDiscard(hands[turn].cards.Count - 12); }
        }
        else
        {
            if (phase == BUILD_QUEST)
            {
                if (CheckSponsorQuestZones()){
                    //sponsor has set up a valid quest
                    for (int i = 0; i < questCard.numberOfStages; i++)
                    {
                        FlipDropzone(qzones[i], true);
                        qzones[i].transform.position = qzonePlaceholder.transform.position;
                        StageNames[i].transform.position = qzoneTextPlaceHolder.transform.position;
                        Dragable[] qzone = qzones[i].GetComponentsInChildren<Dragable>();
                        foreach (Dragable thing in qzone) { thing.enabled = false; }
                        qzones[i].GetComponent<Dropable>().enabled = false;
                    }
                    EndTurnBtn.SetActive(false);
                    SponsorButton.SetActive(false);
                    phase = JOIN_QUEST;
                    AdvanceTurn();
                    Dragable[] TempHand = hands[turn].GetComponentsInChildren<Dragable>();
                    foreach (Dragable thing in TempHand) { thing.enabled = false; }
                    if (players[turn].GetComponent<AiPlayer>() != null){AiBehavior("Join");}
                }
            }

            else if (phase == QUESTING)
            {
                remainingPlayerTurnsInAStage--;
                if (remainingPlayerTurnsInAStage == 0)
                {
                    ResolveStage();
                }
                else
                {
                    AdvanceTurn();
                    if (players[turn].GetComponent<AiPlayer>() != null){AiBehavior("Play");}
                }
            }

            else
            {
                Debug.Log("DEATH. THIS SHOULD NEVER PRINT");
            }
        }
                
    }
    public override void join(bool choice){
        Debug.Log("Join/Decline Pressed");
        if(phase == FIND_SPONSOR)
        {
            if(!choice)
            {
                playersWhoDidntSponsor++;

                if(playersWhoDidntSponsor == players.Count)
                {
                    EndQuest(false, (playerWhoDrewTheCard + 1) % 4);
                }
                else
                {
                    AdvanceTurn();
                    if (players[turn].GetComponent<AiPlayer>() != null){AiBehavior("Start");}
                }
            }
        }
        if(phase == JOIN_QUEST)
        {
            if(!choice)
            {
                questingPlayers[turn] = false;
                numQuestingPlayers--;
            }
            if ((turn + 1) % 4 == sponsorPlayer)
            {
                if (numQuestingPlayers != 0)
                {
                    Debug.Log("phase is now QUESTING");
                    phase = QUESTING;
                    remainingPlayerTurnsInAStage = numQuestingPlayers;
                    EndTurnBtn.SetActive(true);
                    for (int i = 0; i < 4; i++)
                    {
                        if (questingPlayers[i])
                        {
                            hands[i].Draw(ADeck);
                        }
                    }
                    AdvanceTurn();
                    if (players[turn].GetComponent<AiPlayer>() != null){AiBehavior("Play");}
                }
                else
                {
                    EndQuest(true, (sponsorPlayer + 1) % 4);
                }
            }
            else
            {
                AdvanceTurn();
                if (players[turn].GetComponent<AiPlayer>() != null){AiBehavior("Join");}
            }
        }
    }
    //like TurnOrder() but does glows and hand switches too
    public void AdvanceTurn(){
        TurnOrder();
        SetGlow(turn);
        ToggleBoard(turn, 0);
        pop.EnableQuestBlockScreen(players[turn],turn);
        pop.EnableWaitScreen();
        Debug.Log("Player"+(turn+1)+"turn being set in AdvanceTurn");
        //when taking out toggle board, do the flipzone in here with a loop and ifs
    }
    public void DisableAllDropZones(){
        HandleTextFile.WriteLog("UI Log: Disable All Dropzones", GameControler.SName);
        for (int i = 0; i < dzones.Count; i++)
        {
            Dropable temp = (Dropable)dzones[i].GetComponent<Dropable>();
            temp.enabled = false;
        }
    }

    public void DisableAllHands(){
        for (int i = 0; i < hands.Count; i++){
            hands[i].gameObject.SetActive(false);
        }
    }
    public void TurnOrder(){
        if (numQuestingPlayers != 0)
        {
            HandleTextFile.WriteLog("Action Log: Turn Changes", GameControler.SName);
            bool playerFound = false;
            while (!playerFound)
            {
                turn = (turn + 1) % 4;
                if (questingPlayers[turn] == true)
                {
                    playerFound = true;
                }
            }
        }
    }

    public void ToggleBoard(int id, int State){
        if (State == 0)
        {
            HandleTextFile.WriteLog("Action Log: Disable Board Objects For All Except Current Player", GameControler.SName);
            for (int i = 0; i < 4; i++)
            {
                Dragable[] temphand = hands[i].GetComponentsInChildren<Dragable>();
                if (i == id)
                {
                    hands[i].gameObject.SetActive(true);
                    foreach (Dragable d in temphand) { d.enabled = true; }
                    if(phase != JOIN_QUEST && phase != FIND_SPONSOR)
                        dzones[i].GetComponent<Dropable>().enabled = true;
                    FlipDropzone(dzones[i], false);
                }
                else
                {
                    dzones[i].GetComponent<Dropable>().enabled = false;
                    hands[i].gameObject.SetActive(false);
                    foreach (Dragable d in temphand) { d.enabled = false; }
                    FlipDropzone(dzones[i], true);
                }
            }
        }
        else
        {
            HandleTextFile.WriteLog("Action Log: Enable Board Objects", GameControler.SName);
            for (int i = 0; i < 4; i++)
            {
                dzones[i].GetComponent<Dropable>().enabled = true;
                hands[i].gameObject.SetActive(true);
                Dragable[] temphand = hands[i].GetComponentsInChildren<Dragable>();
                foreach (Dragable d in temphand) { d.enabled = true; }
            }
        }
    }
    //keep these 2 functions paired
    //looks at a player's hand and allows them to sponsor if they have the right cards
    public bool CanPlayerSponsor(){
        int Stages = questCard.numberOfStages;
        int counter = 0;
        List<Card> cards = hands[base.turn].cards;
        List<int> FCards = new List<int>();
        foreach (Card FC in cards){
            if (FC.Type == "Foe"){
                counter++;
                FCards.Add(((FoeCard)FC).BattlePoints);
            }
        }
        if (Stages > counter){
            Debug.Log("Player Ineligible to sponsor");
            return false;
        }
        else{
            FCards.Sort();
            int last = 0;
            int count = 0;
            foreach (int i in FCards)
            {
                if (i > last)
                {
                    last = i;
                    count++;
                }
            }
            if (count < Stages)
            {
                Debug.Log("Player Ineligible to sponsor, Foe Cards do not increase in Battle Points");
                pop.EnableSponsorIneligiblePopup();
                return false;
            }
            else
            {
                Debug.Log("Player is eligible");
                return true;
            }
        }
    }
    public void SponsorQuest(){
        phase = BUILD_QUEST;
        questingPlayers[turn] = false;
        numQuestingPlayers--;
        EndTurnBtn.SetActive(true);
        sponsorPlayer = turn;
        for(int i=0; i < questCard.numberOfStages; i++){
            StageNames[i].gameObject.SetActive(true);
            qzones[i].gameObject.SetActive(true);   
        }
    }

    //checks sponsor's quest setup, returns true if valid
    public bool CheckSponsorQuestZones()
    {
        int last = 0;
        int tempBP;
        for (int i = 0; i < questCard.numberOfStages; i++)
        {
            tempBP = qzones[i].UpdateBP(questCard.specialFoeId);
            if (tempBP > last)
            {
                last = tempBP;
            }
            else {
                this.gameObject.GetComponent<PhotonView>().RPC("ResetSponsorQzones", PhotonTargets.All);
                return false;
            }

            if(qzones[i].ControledFoes.Count == 0)
            {
                this.gameObject.GetComponent<PhotonView>().RPC("ResetSponsorQzones", PhotonTargets.All);
                return false;
            }
        }
        pop.EnableSponsorBlockScreen(turn,true);
        return true;
    }

    //clears the sponsors cards in all quest zones because of invalid quest setup
    //cards go back in sponsors hand
    //called by CheckQuestZones()
    [PunRPC]
    public void ResetSponsorQzones()
    {
        for (int i = 0; i < questCard.numberOfStages; i++)
        {
            foreach (FoeCard FC in qzones[i].ControledFoes)
            {
                FC.GetComponent<Dragable>().returnParent = hands[turn].transform;
                FC.transform.SetParent(hands[turn].transform);
                hands[turn].cards.Add(FC);
            }
            foreach (EquipmentCard EC in qzones[i].Equipment)
            {
                EC.GetComponent<Dragable>().returnParent = hands[turn].transform;
                EC.transform.SetParent(hands[turn].transform);
                hands[turn].cards.Add(EC);
            }
            qzones[i].ControledFoes.Clear();
            qzones[i].Equipment.Clear();
        }
        Debug.Log("Not a Valid Quest Set Up!!!!");
    }

    //removes players that didnt win a stage
    //calls EndQuest() if applicable
    public void ResolveStage()
    {
        Debug.Log("resolving stage");
        bool stageWon = false;

        for (int i = 0; i < questingPlayers.Count; i++)
        {
            if (questingPlayers[i])
            {
                //Not sure if this should be less than or less than or equals to
                if (players[i].GetComponent<AbstractPlayer>().BP < qzones[stageNumber].UpdateBP(questCard.specialFoeId))
                {
                    questingPlayers[i] = false;
                }
                else
                {
                    hands[i].Draw(ADeck);
                    remainingPlayerTurnsInAStage++;
                }
            }
            if (questingPlayers[i])
                stageWon = true;
        }
        pop.EnableResolveStagePopup(stageNumber, questingPlayers, sponsorPlayer, dzones, qzones, players);
        stageNumber++;
        if (!stageWon || stageNumber == questCard.numberOfStages)
        {
            EndQuest(true, (sponsorPlayer + 1) % 4);
        }
        else
        {
            ClearDropZones(0);
            qzones[stageNumber - 1].gameObject.SetActive(false);
            StageNames[stageNumber - 1].SetActive(false);
            AdvanceTurn();
            if(players[turn].GetComponent<AiPlayer>() != null)
            {
                AiBehavior("Play");
                pop.DisableResolveStagePopup();
            }
        }
    }

    //clears drop zones between stages
    //allies remain from stage to stage
    // pass in a 1 it clears everything, pass in 0 it only clears equipment
    public void ClearDropZones(int Mode)
    {
        for (int i = 0; i < dzones.Count; i++)
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

    //clears the quest zone after a quest is over so it's ready to use for another quest
    public void ClearQzones()
    {
        for (int i = 0; i < qzones.Count; i++)
        {
            foreach (FoeCard FC in qzones[i].ControledFoes)
            {
                FC.GetComponent<Dragable>().returnParent = hands[turn].transform;
                FC.transform.SetParent(Discard.transform);
                Discard.cards.Add(FC);
            }
            qzones[i].ControledFoes.Clear();
            foreach (EquipmentCard EC in qzones[i].Equipment)
            {
                EC.GetComponent<Dragable>().returnParent = hands[turn].transform;
                EC.transform.SetParent(Discard.transform);
                Discard.cards.Add(EC);
            }
            qzones[i].Equipment.Clear();
        }
    }

    //give rewards to players that beat the quest
    public void RewardPlayers()
    {
        for (int i = 0; i < questingPlayers.Count; i++)
        {
            if (questingPlayers[i] == true) {
                players[i].GetComponent<AbstractPlayer>().SetSheilds(questCard.numberOfStages);
                if (GameController.KingsRec == true)
                {
                    players[i].GetComponent<AbstractPlayer>().SetSheilds(2);
                }
            }
        }
        GameController.KingsRec = false;
        ifDisplay.setText(UpdateInfo(0));
    }

    //give reward to sponsor, no players beat the quest
    public void RewardSponsor()
    {
        int cardCount = questCard.numberOfStages;
        foreach (Dropzone q in qzones) { cardCount += (q.ControledFoes.Count + q.Equipment.Count); }
        HandleTextFile.WriteLog("Action Log: Sponsor Gets a Reward of " + cardCount, GameControler.SName);
        for (int i = 0; i < cardCount; ++i)
        {
            hands[sponsorPlayer].Draw(ADeck);
        }
        Debug.Log("Rewarding sponsor. #ofstages" + questCard.numberOfStages + " total rewarded cards"+cardCount);
        ifDisplay.setText(UpdateInfo(0));
    }

    //if state == false : no one sponsored. quest ends
    //if state == true  : quest was sponsored so give rewards and adjust turn order to be player after sponsor
    public void EndQuest(bool state, int turnToSetToAfterQuest)
    {
        SponsorButton.SetActive(true);
        for (int i = 0; i < questCard.numberOfStages; i++)
        {
            StageNames[i].SetActive(false);
            qzones[i].gameObject.SetActive(false);
            FlipDropzone(dzones[i], true);
        }
        boardUI.SetActive(false);
        EndTurnBtn.SetActive(false);
        SponsorButton.SetActive(true);
        if (state)
        {
            RewardPlayers();
            RewardSponsor();
        }
        questingPlayers.Clear();
        ClearQzones();
        ClearDropZones(1);
        gameState = false;
        //pop.EnableQuestWinnerPopup(questingPlayers, questCard.numberOfStages, sponsorPlayer, players);
        GameController.ReturnControlFromQuest(false, turnToSetToAfterQuest);
    }

    public void AiBehavior(string Stage){
        AiPlayer temp = players[turn].GetComponent<AiPlayer>();
        switch(Stage){
            case "Start":
                if(temp.SponserQuest(players,questCard.numberOfStages,this,CanPlayerSponsor())== true){SponsorQuest();}
                temp.SetupQuest(questCard.numberOfStages,this);
                if (hands[turn].cards.Count >= 12){
                    HandleTextFile.WriteLog("AI Log: Force Ai Discard " + hands[turn].cards[0].name, GameControler.SName);
                    while (hands[turn].cards.Count >= 12) {DiscardCard(0);}
                }
                EndTurn();
                break;
            case "Join":
                temp.joinQuest(questCard.numberOfStages,this);
                break;
            case "Play":
                temp.playStage(this);
                if (hands[turn].cards.Count >= 12){
                    HandleTextFile.WriteLog("AI Log: Force Ai Discard " + hands[turn].cards[0].name, GameControler.SName);
                    while (hands[turn].cards.Count >= 12) {DiscardCard(0);}
                }
                EndTurn();
                break;

            default:
                    break;
        }
    }

}
