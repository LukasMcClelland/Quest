using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defensive : AiPlayer
{
    public override void JoinTournament(List<GameObject> pl, int reward, TournementControler Tc){
        int r = 0;
        bool join = false;
        for (int i = 0; i < pl.Count; i++)
        {
            AbstractPlayer tempPlayer = pl[i].GetComponent<AbstractPlayer>();
            r = tempPlayer.PlayerRank.rank;
            if (reward + tempPlayer.PlayerRank.Sheild >= lv[r]){join = true;}
        }
        Join(Tc, join);
    }
    public override void ExecuteTournament(List<GameObject> pl, int reward, TournementControler Tc)
    {
        TournamentBehavior(1, Tc);
    }
    public override void TournamentBehavior(int Case, TournementControler Tc)
    {
        int ID = this.playerID;
        switch (Case)
        {
            case 1:
                bool play = false;
                for (int i = 0; i < Tc.hands[ID].cards.Count; i++)
                {
                    switch(Tc.hands[ID].cards[i].Type)
                    {
                        case "Foe":
                            break;
                        case "Amour":
                            if (Tc.dzones[playerID].SpecificUseCards.Count == 0){
                                PlayCard(i,Tc);
                            }
                            break;
                        case "Ally":
                            PlayCard(i,Tc);
                            break;
                        case "Equipment":
                            play = true;
                            EquipmentCard EC1 = ((EquipmentCard)Tc.hands[ID].cards[i]);
                            foreach(EquipmentCard EC in Tc.dzones[playerID].Equipment){
                                if(EC1.TypeOfEquipment == EC.TypeOfEquipment){
                                    play = false;
                                    break;
                                }
                            }
                            if(play == true){PlayCard(i,Tc);}
                            break;    
                        default:
                            break;
                    }
                }
                break;

            case 2:
                bool play2 = false;
                List<int> Equip = new List<int>();
                for (int i = 0; i < Tc.hands[ID].cards.Count; i++)
                {
                    if (Tc.hands[ID].cards[i].Type == "Equipment")
                    {
                        for (int j = i + 1; j < Tc.hands[ID].cards.Count; j++)
                        {
                            if (Tc.hands[ID].cards[j].Type == "Equipment")
                            {
                                if (((EquipmentCard)Tc.hands[ID].cards[i]).TypeOfEquipment == ((EquipmentCard)Tc.hands[ID].cards[j]).TypeOfEquipment)
                                {
                                    Equip.Add(i);
                                    play2 = true;
                                }
                            }
                        }
                    }
                }
                if (play2 == true) { ExecutetPhase(Equip, Tc); }
                break;

            default:
                break;

        }
    }

    public override void SetupQuest(int stages, QuestController Qc)
    {
        int StageMax = 51;

        StageMax = SetNthStage(StageMax, stages - 1, Qc, true);
        for (int i = 0; i < stages - 1; ++i)
        {
            StageMax = SetNthStage(StageMax, i, Qc, true);
        }
    }

    public override void joinQuest(int numberOfStages, QuestController qc)
    {
        List<string> typesOfEquipment = new List<string> { "Dagger", "Sword", "Mount", "Lance", "LSword", "Axe"};
        List<int> weaponTotals = new List<int>();
        for(int i=0; i < 6; i++) { weaponTotals.Add(0); }
        int cardCount = 0, foeCounter = 0;
        List<Card> playerCards = qc.hands[playerID].cards;
        
        foreach (Card C in playerCards)
        {
            if(C.Type == "Foe")
            {
                if(((FoeCard)C).BattlePoints < 20)
                    foeCounter++;
            }
            if (C.Type == "Ally")
            {
                cardCount++;
            }
            if (C.Type == "Equipment")
            {
                EquipmentCard EQ = (EquipmentCard)C;
                for (int i = 0; i < 6; i++)
                {
                    if(EQ.TypeOfEquipment == typesOfEquipment[i] && weaponTotals[i] <= numberOfStages){
                        weaponTotals[i]++;
                        cardCount++;
                    }
                }
            }
        }
        if(cardCount >= numberOfStages * 2) { qc.join(true); }
        else { qc.join(false); }
    }
    public override void playStage(QuestController qc)
    {
        int cardsPlayed = 0;
        List<int> HIndex = new List<int>();
        bool AmourCardInDZ = false;
        if (qc.dzones[playerID].SpecificUseCards.Count > 0) {AmourCardInDZ = true; }
        List<Card> playerCards = qc.hands[playerID].cards;
        bool CanPlayCard = true;

        //play up to two cards, amour or ally
        for (int i = 0; i < playerCards.Count; i++)
        {
            Card tempCard = qc.hands[playerID].cards[i];

            if (string.Compare(tempCard.Type, "Amour") == 0 && !AmourCardInDZ)
            {
                PlayCard(i, qc, false, -1);
                HIndex.Add(i);
                cardsPlayed++;
                AmourCardInDZ = true;
            }
            if (string.Compare(tempCard.Type, "Ally") == 0)
            {
                if (cardsPlayed < 2 || qc.stageNumber == qc.questCard.numberOfStages - 1)
                {
                    PlayCard(i, qc, false, -1);
                    HIndex.Add(i);
                    cardsPlayed++;
                }
            }
        }

        //play the lowest equipment cards until 2 cards have been played 
        int squishyMuffins = 2;
        if (cardsPlayed < 2) {
            if (qc.stageNumber == qc.questCard.numberOfStages - 1) { squishyMuffins = qc.hands[playerID].cards.Count; }
            for (int j = 0; j < squishyMuffins; j++)
            {
                int lowest = 100, index = 0;
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

                        if (CanPlayCard && ((EquipmentCard)tempCard).BattlePoints < lowest)
                        {
                            lowest = ((EquipmentCard)tempCard).BattlePoints;
                            index = i;
                        }
                    }
                }
                Card tCard = qc.hands[playerID].cards[index];
                if (cardsPlayed < 2 || qc.stageNumber == qc.questCard.numberOfStages - 1 && string.Compare(tCard.Type, "Equipment") == 0)
                {
                    PlayCard(index, qc, false, -1);
                    HIndex.Add(index);
                    cardsPlayed++;
                }
            }
        }
        RemoveCards(HIndex, qc);
    }
}