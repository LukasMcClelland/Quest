using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiPlayer : AbstractPlayer
{
    //Ai player Instance Vars
    public int[] lv = { 5, 7, 10 };
    //5

// Abstract Player overide
    public override void Player(int id){
        imgSet = new List<Sprite>();
        PlayerRank = new Rank();
        BP = 0;
        playerID = id;
        HandleTextFile.WriteLog("AI Log: AI Player " + (playerID + 1) + " Becomes Squire", GameControler.SName);
        PlayerRank.Squire();
    }
//

//Core Abstract Methods To be implemented by Types Must be Implemented for Stratgy too run
    public abstract void JoinTournament(List<GameObject> pl, int reward, TournementControler Tc);
    public abstract void ExecuteTournament(List<GameObject> pl, int reward, TournementControler Tc);
    public abstract void TournamentBehavior(int Case, TournementControler Tc);
    public abstract void SetupQuest(int stages, QuestController Qc);
    public abstract void joinQuest(int stage, QuestController controller);
    public abstract void playStage(QuestController qc);
//

    //Universal Ai Helper Methods 
    public void Join(TournementControler Tc, bool choice){Tc.join(choice);}
    public void PlayCard(int UID, TournementControler Tc){
        Dropzone Dropzone = Tc.dzones[playerID];
        Hand THand = Tc.hands[Tc.turn];
        HandleTextFile.WriteLog("AI Log: Player " + (playerID + 1) + " Plays Card" + THand.cards[UID].name, GameControler.SName);
        Card temp = Instantiate(THand.cards[UID]);
        Dropzone.AddCard(temp);
        temp.transform.SetParent(Dropzone.transform);
        Destroy(THand.transform.GetChild(UID).gameObject);
    }
    public void PlayCard(int UID, QuestController Qc, bool Sponsor, int Stage)
    {
        Debug.Log("StageNumber: " + Stage);
        if (!Sponsor)
        {
            Dropzone Dropzone = Qc.dzones[playerID];
            Hand THand = Qc.hands[Qc.turn];
            HandleTextFile.WriteLog("AI Log: Player " + (playerID + 1) + " Plays Card" + THand.cards[UID].name, GameControler.SName);
            Card temp = Instantiate(THand.cards[UID]);
            Dropzone.AddCard(temp);
            temp.transform.SetParent(Dropzone.transform);
            Destroy(THand.transform.GetChild(UID).gameObject);
        }
        else
        {
            Dropzone Dropzone = Qc.qzones[Stage];
            Hand THand = Qc.hands[Qc.turn];
            HandleTextFile.WriteLog("AI Log: Player " + (playerID + 1) + " Plays Card" + THand.cards[UID].name, GameControler.SName);
            Card temp = Instantiate(THand.cards[UID]);
            Dropzone.AddCard(temp);
            temp.transform.SetParent(Dropzone.transform);
            Destroy(THand.transform.GetChild(UID).gameObject);
        }
    }

    public void RemoveCards(List<int> HIndex, Controler GC){
        List<Card> newHand = new List<Card>();
        for (int i = 0; i <HIndex.Count; i++)
        {
            GC.hands[this.playerID].cards[HIndex[i]] = null;
        }
        for (int j = 0; j < GC.hands[this.playerID].cards.Count; j++)
        {
            if(GC.hands[this.playerID].cards[j]!=null){
                newHand.Add(GC.hands[this.playerID].cards[j]);
            }
        }
        GC.hands[playerID].cards = newHand;
    }

    //Sponsor Logic is the same for both Ai variants this method returns a false value if it will not sponsor the quest and a true if it will
    // Ai will only sponsor if it Can Sponsor which is a bool i asumed is determined by Quest and if case 1 is false Ai will often sponsor a quest if able
    public bool SponserQuest(List<GameObject> pl, int reward, QuestController Qc, bool CanSponsor){
        int r = 0;
        if(CanSponsor){
            for (int i = 0; i < pl.Count; i++){
                AbstractPlayer tempPlayer = pl[i].GetComponent<AbstractPlayer>();
                r = tempPlayer.PlayerRank.rank;
                if (reward + tempPlayer.PlayerRank.Sheild >= lv[r]){return false;}
            }
            return true;
        }
        else{return false;}
    }
    public int SetNthStage(int total, int stageindex, QuestController Qc, bool EquipmentAllowed)
    {
        int nthTotal = 0;
        int ID = this.playerID;
        List<int> Indexes = new List<int>();
        int index = StrongestFoe(Qc);
        FoeCard FC = (FoeCard)Qc.hands[playerID].cards[index];
        nthTotal += FC.BattlePoints;
        PlayCard(index, Qc, true, stageindex);
        Indexes.Add(index);

        if (EquipmentAllowed == true)
        {
            for (int i = 0; i < Qc.hands[ID].cards.Count; i++){
                if (Qc.hands[ID].cards[i].Type == "Equipment")
                {
                    EquipmentCard EC = (EquipmentCard)Qc.hands[ID].cards[i];
                    if (EC.BattlePoints + nthTotal < total)
                    {
                        index = i;
                        nthTotal += EC.BattlePoints;
                        PlayCard(i, Qc, true, stageindex);
                        Indexes.Add(index);
                        break;
                    }
                }
            }
        }
        RemoveCards(Indexes,Qc);
        return nthTotal;
    }

    public void ExecutetPhase(List<int> play, TournementControler Tc){

        for (int j = 0; j < play.Count; j++)
        {
            PlayCard(j, Tc);
            Tc.hands[playerID].cards[play[j]] = null;
        }

        List<Card> replace = new List<Card>();

        for (int i = 0; i < Tc.hands[playerID].cards.Count; i++)
        {
            if (Tc.hands[playerID].cards[i] != null) { replace.Add(Tc.hands[playerID].cards[i]); }
        }

        Tc.hands[playerID].cards.Clear();
        Tc.hands[playerID].cards = replace;
        foreach (Transform child in Tc.hands[playerID].gameObject.transform) { GameObject.Destroy(child.gameObject); }
        foreach (Card c in Tc.hands[playerID].cards) { Instantiate(c).transform.SetParent(Tc.hands[playerID].gameObject.transform); }
    }

    public int StrongestFoe(QuestController QC){
        int Highest = 0;
        int temp = 0;
        int HighestIndex = -1;
        int ID = this.playerID;
        for (int i = 0; i <QC.hands[ID].cards.Count; i++)
        {
            if(QC.hands[ID].cards[i].Type == "Foe"){
                temp = ((FoeCard)QC.hands[ID].cards[i]).BattlePoints;
                if(temp > Highest){
                    Highest = temp;
                    HighestIndex = i;
                }
            }            
        }
        if(HighestIndex != -1){return HighestIndex;}
        else{
            Debug.Log("if this happened then we have a serrious Problem");
            return 0;
        }
    }


//
}