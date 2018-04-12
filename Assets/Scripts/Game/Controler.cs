using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Controler : Photon.MonoBehaviour
{
    public Popup pop;
    public int turn;
    public List<Hand> hands;
    public List<GameObject> players;
    public List<Dropzone> dzones;
    public List<GameObject> glows;
    public abstract void EndTurn();
    public abstract void join(bool choice);
    public InfoDisplay ifDisplay;
    public Deck Discard;
    public static string SName;
    public static int LogLine;
    public bool gameState, cheating;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

    public List<List<int>> UpdateInfo(int AddSheilds)
    {
        HandleTextFile.WriteLog((LogLine += 1) + " UI Log: Update Player Info Zone: Cards, BP, Shields, Bids" + "#SNG-2, #BNF-8, #BNF-11, #BNF-14, #BNF-20, #BNF-23, #BNF-27", SName);
        List<List<int>> returnList = new List<List<int>>();
        for (int i = 0; i < 4; i++)
        {
            AbstractPlayer temp = players[i].GetComponent<AbstractPlayer>();
            returnList.Add(temp.UpdateAll(AddSheilds, dzones[i].UpdateBP("!"), dzones[i].UpdateBids(), hands[i].cards.Count));
            players[i].gameObject.GetComponent<Image>().sprite = temp.imgSet[temp.PlayerRank.rank];
        }
        return returnList;
    }

    public void SetGlow(int turn)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == turn)
            {
                glows[i].SetActive(true);
                HandleTextFile.WriteLog((LogLine += 1) + " UI Log: Update UI For Player " + (turn + 1) + " Glow " + "#SNG-2", SName);
            }
            else { glows[i].SetActive(false); }
        }
    }

    public void FlipDropzone(Dropzone d, bool faceUp)
    {
        if (faceUp)
        {
            foreach (Transform child in d.transform)
            {
                child.gameObject.GetComponent<Image>().sprite = child.gameObject.GetComponent<AdventureCard>().cardBack;
            }
        }
        else
        {
            foreach (Transform child in d.transform)
            {
                child.gameObject.GetComponent<Image>().sprite = child.gameObject.GetComponent<AdventureCard>().img;

            }
        }
    }
    public void DiscardCard(int UID)
    {
        Deck DPile = this.Discard;
        Hand THand = this.hands[this.turn];
        Card temp = Instantiate(THand.cards[UID]);
        temp.gameObject.GetComponent<Image>().sprite = temp.gameObject.GetComponent<AdventureCard>().cardBack;
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Discard Card " + THand.cards[UID].name, GameControler.SName);
        DPile.cards.Add(temp);
        temp.transform.SetParent(DPile.transform);
        Destroy(THand.transform.GetChild(UID).gameObject);
        THand.cards.RemoveAt(UID);
        ifDisplay.setText(UpdateInfo(0));
    }


}
