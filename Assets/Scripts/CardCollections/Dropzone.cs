using System.Collections.Generic;
using UnityEngine;

public class Dropzone : Photon.MonoBehaviour
{


    public string Type;
    public List<AllyCard> Allies;
    public List<EquipmentCard> Equipment;
    public List<FoeCard> ControledFoes;
    public List<AmourCard> SpecificUseCards; //PlaceHolder till we make a amour card type 

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }


    public int UpdateBP(string foename)
    {
        int PlayerBattlePoints = 0;
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action log: Update Battle Points in Players Dropzone", GameControler.SName);
        for (int i = 0; i < Allies.Count; i++)
        {
            PlayerBattlePoints += Allies[i].BattlePoints;
        }
        for (int i = 0; i < Equipment.Count; i++)
        {
            PlayerBattlePoints += Equipment[i].BattlePoints;
        }
        for (int i = 0; i < ControledFoes.Count; i++)
        {
            if (string.Compare(foename, "All") == 0)
            {
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action log: Foe Card Special BP Used", GameControler.SName);
                PlayerBattlePoints += (ControledFoes[i].BattlePoints + ControledFoes[i].QuestBattlePoints);
            }
            else if(string.Compare(foename, ControledFoes[i].name) == 0)
            {
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action log: Foe Card Special BP Used", GameControler.SName);
                PlayerBattlePoints += (ControledFoes[i].BattlePoints + ControledFoes[i].QuestBattlePoints);
            }
            else
            {
                PlayerBattlePoints += ControledFoes[i].BattlePoints;
            }
        }
        for (int i = 0; i < SpecificUseCards.Count; i++)
        {
            PlayerBattlePoints += SpecificUseCards[i].BattlePoints;
        }

        return PlayerBattlePoints;
    }
    public int UpdateBids()
    {
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action log: Update Bids", GameControler.SName);
        int Bids = 0;
        for (int i = 0; i < Allies.Count; i++)
        {
            Bids += Allies[i].Bids;
        }

        for (int i = 0; i < SpecificUseCards.Count; i++)
        {
            Bids += SpecificUseCards[i].Bids;
        }

        return Bids;
    }

    public void AddCard(Card NewCard)
    {
        string temp = NewCard.Type;
        switch (temp)
        {
            case "Ally":
                Allies.Add((AllyCard)NewCard);
                break;

            case "Foe":
                ControledFoes.Add((FoeCard)NewCard);
                break;
            case "Equipment":
                Equipment.Add((EquipmentCard)NewCard);
                break;
            case "Amour":
                SpecificUseCards.Add((AmourCard)NewCard);
                break;
            default:
                break;
        }

    }


}