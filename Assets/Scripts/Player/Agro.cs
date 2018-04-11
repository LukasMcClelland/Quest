using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agro : AiPlayer
{

// Tournament Start 
    public override void JoinTournament(List<GameObject> pl, int reward, TournementControler Tc)
    {
        HandleTextFile.WriteLog("Action Log: AI Player " + (playerID + 1) + "Joins Tournament", GameControler.SName);
        Join(Tc, true);
    }
    public override void TournamentBehavior(int Case, TournementControler Tc)
    {   
        int TBP = this.BP;
        List<int> HIndex = new List<int>(); 
        HandleTextFile.WriteLog("Action Log: AI Player " + (playerID + 1) + "Is Picking Cards To Play", GameControler.SName);
        for (int i = 0; i < Tc.hands[playerID].cards.Count; i++){
            bool CanPlayCard = false;
            int TempBP = 0;
            Card tempCard = Tc.hands[playerID].cards[i];
            switch(Tc.hands[playerID].cards[i].Type)
            {
                case "Foe":
                    break;
                case "Ally":
                    CanPlayCard = true;
                    TempBP += ((AllyCard)tempCard).BattlePoints;
                    PlayCard(i, Tc);
                    break;
                case "Equipment":
                    CanPlayCard = true;
                    EquipmentCard TempEC = (EquipmentCard)tempCard;
                    foreach(EquipmentCard EC in Tc.dzones[playerID].Equipment){
                        if (EC.TypeOfEquipment == TempEC.TypeOfEquipment){                        
                            CanPlayCard = false;
                            break;
                            }
                        }
                    if(CanPlayCard){
                        TempBP += TempEC.BattlePoints;
                        PlayCard(i, Tc);
                    }
                    break;
                case "Amour":
                    CanPlayCard = true;
                    TempBP += 10;
                    PlayCard(i, Tc);
                    break;
                default:
                    Debug.Log("I Can't  Let you Do that Dave");
                    break;
            }
            if(CanPlayCard){
                HIndex.Add(i);
                TBP += TempBP;
            }
            if (TBP > 50){break;}
        }
        RemoveCards(HIndex, Tc);
    }
    public override void ExecuteTournament(List<GameObject> pl, int reward, TournementControler Tc)
    {
        TournamentBehavior(0, Tc);
    }

// Tournament End 


    //Quest Setup Methods Not Tested but works simply Agro Ai will set up the last Stage to be at most 40 with 1 foe and equipment if posible
    //then then the Ai will set up each stage to be less than the total of the previous max

// Quest Start
    public override void SetupQuest(int stages, QuestController Qc)
    {
        Debug.Log(stages);
        int StageMax = 41;
        StageMax = SetNthStage(StageMax, stages - 1, Qc, true);
        for (int i = 0; i < stages - 1; ++i){
            StageMax = SetNthStage(StageMax, i, Qc, false);
        }
    }
    public override void joinQuest(int numberOfStages, QuestController qc)
    {
        int TempBP = 0;
        float count = 0f;
        int AmourFound = 0;
        foreach (Card c in qc.hands[playerID].cards){
            TempBP = 0;
            switch (c.Type){
                case "Ally":
                    TempBP += ((AllyCard)c).BattlePoints;
                    if (TempBP >= 10) { count++; }
                    else if (((AllyCard)c).name == "Sir Percival") { count += .5f; }
                    break;
                case "Equipment":
                    EquipmentCard TempEC = (EquipmentCard)c;
                    if (TempEC.BattlePoints >= 10) { count++; }
                    else if (TempEC.name == "Corsair Dagger") { count += .5f; }
                    break;
                case "Amour":
                    if (AmourFound < 1)
                    {
                        count++;
                        AmourFound++;
                    }
                    break;
                default:
                    break;
            }
        }
        Debug.Log(count);
        if (count >= numberOfStages) { qc.join(true); }
        else { qc.join(false); }
    }

    public override void playStage(QuestController qc)
    {
        int BP = 0;
        List<int> HIndex = new List<int>();
        bool AmourCardInDZ = false;
        if (qc.dzones[playerID].SpecificUseCards.Count > 0) { AmourCardInDZ = true; }
        List<Card> playerCards = qc.hands[playerID].cards;
        bool CanPlayCard = true;
        //first play amour in first stage if possible
        if(!AmourCardInDZ){
            for (int i=0; i < playerCards.Count;i++){
                if(string.Compare(playerCards[i].Type,  "Amour") == 0){
                    PlayCard(i, qc, false, -1);
                    HIndex.Add(i);
                    BP += 10;
                    break;
                }
            }
        }
        
            //then play allies
            for (int i = 0; i < playerCards.Count; i++)
            {
                Card tempCard = qc.hands[playerID].cards[i];
                if (string.Compare(tempCard.Type , "Ally") == 0)
                {
                    if(BP < 10 || qc.stageNumber == qc.questCard.numberOfStages - 1)
                    PlayCard(i, qc, false, -1);
                    HIndex.Add(i);
                    BP += tempCard.GetComponent<AllyCard>().BattlePoints;
                }
            }
            //then play equipment
            for (int i = 0; i < playerCards.Count; i++)
            {
                Card tempCard = qc.hands[playerID].cards[i];

                if (string.Compare(tempCard.Type, "Equipment") == 0)
                {
                    foreach (EquipmentCard EC in qc.dzones[playerID].Equipment)
                    {
                        if (EC.TypeOfEquipment == tempCard.GetComponent<EquipmentCard>().TypeOfEquipment)
                        {
                            CanPlayCard = false;
                        }
                    }

                    if (CanPlayCard && (BP < 10 || qc.stageNumber == qc.questCard.numberOfStages - 1))
                    {
                        PlayCard(i, qc, false, -1);
                        HIndex.Add(i);
                        BP += qc.hands[playerID].cards[i].GetComponent<EquipmentCard>().BattlePoints;
                    }
                }
            } 
        
           
        RemoveCards(HIndex, qc);
    }
// Quest End

}