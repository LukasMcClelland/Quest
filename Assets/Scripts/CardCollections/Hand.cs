using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : MonoBehaviour
{

    const int Size = 12;
    public List<Card> cards = new List<Card>();
    public int PlayerID;


    public void AssignPlayer(int ID) { PlayerID = ID; }

    public void dealHand(Deck InputDeck)
    {
        for (int i = 0; i < 12; i++)
        {
            HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action log: Deal Card " + InputDeck.cards[0].name, GameControler.SName);
            InputDeck.cards[0].gameObject.GetComponent<Dragable>().returnParent = this.transform;
            Instantiate(InputDeck.cards[0]).transform.SetParent(this.transform);
            this.cards.Add(InputDeck.cards[0]);
            InputDeck.cards.RemoveAt(0);
        }
    }

    public void Draw(Deck InputDeck)
    {
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " Action log: Draw Card " + InputDeck.cards[0].name, GameControler.SName);
        InputDeck.cards[0].gameObject.GetComponent<Dragable>().returnParent = this.transform;
        Instantiate(InputDeck.cards[0]).transform.SetParent(this.transform);
        this.cards.Add(InputDeck.cards[0]);
        InputDeck.cards.RemoveAt(0);
    }


}