using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventCard : StoryCard
{
	public abstract void Run(Deck SomeDeck, List<GameObject> pl, List<Hand> h, List<Dropzone> dz, int theTurn, GameControler g);
}