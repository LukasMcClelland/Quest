using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;



public class Deck : MonoBehaviour
{
    public List<Card> cards = new List<Card>();

    public void DeckIsEmpty(Deck Discard)
    {
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " UI log: Deck is Empty", GameControler.SName);
        for (int i = 0; i < Discard.cards.Count; i++)
        {
            this.cards.Add(Discard.cards[i]);
        }
        Discard.cards.Clear();
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " UI log: Discard Suffled In to Deck ", GameControler.SName);
    }

    public void shuffle()
    {
        HandleTextFile.WriteLog((GameControler.LogLine+=1) + " UI log: Deck Shuffled", GameControler.SName);
        List<Card> NewCards = new List<Card>();
        System.Random rand = new System.Random();
        int index = 0;
        while (cards.Count > 0)
        {
            index = rand.Next(0, cards.Count);
            NewCards.Add(cards[index]);
            cards.RemoveAt(index);
        }

        cards = NewCards;
    }



}