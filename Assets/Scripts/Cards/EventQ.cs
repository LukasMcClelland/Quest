using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventQ : EventCard
{
    public override void Run(Deck SomeDeck, List<GameObject> pl, List<Hand> h, List<Dropzone> dz, int theTurn, GameControler g)
    {
        Debug.Log("Event card name: " + name);
        switch (this.name)
        {
            case "Prosperity":
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Prosperity Event Trigered", GameControler.SName);
                //All players immediately draw two Adventure Cards
                for (int i = 0; i < 4; i++)
                {
                    HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Player Draws 2 Cards", GameControler.SName);
                    h[i].Draw(SomeDeck);
                    h[i].Draw(SomeDeck);
                }
                break;
            case "Queen's Favour":
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Queens Favor Event Trigered", GameControler.SName);
                //The lowest ranked player(s) immediately receive 2 adventure cards
                int lowestRank = 5;
                List<int> lowestPlayers = new List<int>();
                List<int> ranks = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    AbstractPlayer Player3 = pl[i].GetComponent<AbstractPlayer>();
                    ranks.Add(Player3.PlayerRank.rank);
                }
                for (int i = 0; i < 4; i++)
                {
                    if (ranks[i] < lowestRank)
                    {
                        lowestPlayers.Clear();
                        lowestPlayers.Add(i);
                        lowestRank = ranks[i];
                    }
                    else if (ranks[i] == lowestRank) { lowestPlayers.Add(i); }
                }
                for (int t = 0; t < lowestPlayers.Count; t++)
                {
                    h[lowestPlayers[t]].Draw(SomeDeck);
                    h[lowestPlayers[t]].Draw(SomeDeck);
                }
                break;

            case "Camelot":
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Camelot Event Trigered", GameControler.SName);
                //All allies in play must be discarded
                for (int i = 0; i < 4; i++)
                {
                    dz[i].Allies.Clear();
                    foreach (Transform child in dz[i].gameObject.transform) { GameObject.Destroy(child.gameObject); }
                }
                break;
            case "Plague":
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Plague Event Trigered", GameControler.SName);
                //Drawer loses two shields if possible
                AbstractPlayer Player = pl[theTurn].GetComponent<AbstractPlayer>();
                if (Player.PlayerRank.Sheild >= 2) { Player.PlayerRank.Sheild -= 2; }
                break;
            case "King's Recognition":
                //The next player(s) to complete a quest receive 2 extra shields
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: King's Recognition Event Activated", GameControler.SName);
                g.KingsRec = true;
                break;
            case "Pox":
                HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action Log: Pox Event Trigered", GameControler.SName);
                //All players except the player drawing this card lose 1 shield
                for (int i = 0; i < 4; i++)
                {
                    AbstractPlayer Player1 = pl[i].GetComponent<AbstractPlayer>();
                    if (Player1.PlayerRank.Sheild >= 1 && i != theTurn) {Player1.PlayerRank.Sheild -= 1; }
                }
                break;
            case "Chivalrous Deed":
                int LowRank = 5;
                int LowShelds = 100;
                for (int i = 0; i < 4; i++){
                    AbstractPlayer Player2 = pl[i].GetComponent<AbstractPlayer>();
                    if(Player2.PlayerRank.rank<LowRank){LowRank = Player2.PlayerRank.rank;}
                }
                for (int i = 0; i < 4; i++){
                    AbstractPlayer Player2 = pl[i].GetComponent<AbstractPlayer>();
                    if(Player2.PlayerRank.rank == LowRank){
                        if(Player2.PlayerRank.Sheild < LowShelds){
                            LowShelds = Player2.PlayerRank.Sheild;
                        }
                    }
                }
                for (int i = 0; i < 4; i++){
                    AbstractPlayer Player2 = pl[i].GetComponent<AbstractPlayer>();
                    if(Player2.PlayerRank.rank==LowRank && Player2.PlayerRank.Sheild==LowShelds){
                        pl[i].GetComponent<AbstractPlayer>().SetSheilds(3);
                    }
                }
                break;

            default:
                Debug.Log("EVENTASTIC FAILURE!!!!");
                break;
        }
    }


}