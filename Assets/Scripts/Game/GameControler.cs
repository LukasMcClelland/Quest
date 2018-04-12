using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControler : Controler
{
    public Deck SDeck;
    public Deck ADeck;
    public TournementControler TournementControler;
    public QuestController QuestControl;
    public bool KingsRec;
    public int TournamentWinner;
    public int TournamentWinnerShields;
    public int NumHumanPlayers = MainMenuButtonManager.n;
    public GameObject boardUI;


    public void Awake()
    {
        SName = NetworkMannager.LName;
        LogLine = NetworkMannager.LobbyLineNumber;
        HandleTextFile.WriteLog((LogLine += 1) + " Network Log: Local Client Launched " + "#SNG-2", SName);
        turn = 0;
        TournementControler.boardUI.SetActive(false);
        QuestControl.boardUI.SetActive(false);
        boardUI.SetActive(true);
        NumHumanPlayers = MainMenuButtonManager.n;
        if (NumHumanPlayers == 0) { NumHumanPlayers = 4; }
        DealHands();
        CreatePlayers(PhotonNetwork.playerList.Length);
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Game Start", SName);
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Create " + NumHumanPlayers + "Human Players", SName);
        HandleTextFile.WriteLog((LogLine += 1) + " UI Log: Enable Block Screen", SName);
        ifDisplay.setText(UpdateInfo(0));
        pop.EnableBlockScreen(players[0], 0);
        StartTurn();
    }

    public void StartTurn()
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Player " + (turn + 1) + " turn start.", SName);
        ToggleBoard(turn, 0);
        for (int i = 0; i < 4; i++)
        {
            if (turn == i)
            {
                glows[i].SetActive(true);
                HandleTextFile.WriteLog((LogLine += 1) + " UI Log: Activate Player " + (turn + 1) + " Glow", SName);
            }
        }
        if (PhotonNetwork.player.ID != turn + 1) { pop.EnableWaitScreen(); }
    }

    public void ToggleBoard(int id, int State)
    {
        if (State == 0)
        {
            HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Disable Board Objects", SName);
            for (int i = 0; i < 4; i++)
            {
                dzones[i].GetComponent<Dropable>().enabled = false;
                hands[i].gameObject.SetActive(false);
                Dragable[] temphand = hands[i].GetComponentsInChildren<Dragable>();
                foreach (Dragable d in temphand) { d.enabled = false; }
            }
            hands[id].gameObject.SetActive(true);

        }
        else
        {
            HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Enable Board Objects", SName);
            for (int i = 0; i < 4; i++)
            {
                dzones[i].GetComponent<Dropable>().enabled = true;
                hands[i].gameObject.SetActive(true);
                Dragable[] temphand = hands[i].GetComponentsInChildren<Dragable>();
                foreach (Dragable d in temphand) { d.enabled = true; }
            }
        }

    }

    public override void EndTurn()
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Player " + (turn + 1) + " turn end.", SName);
        Card TemCard = DrawStoryCard();
        ifDisplay.setText(UpdateInfo(0));
        ToggleBoard(turn, 0);
        RunMode(CheckStoryCard(TemCard), TemCard);
        //TURN ORDER IS NO LONGER IN END TURN. 
        //If an event card is drawn, TurnOrder() is called in RunMode, otherwise it's called in the ReturnControl functions
        //This is done to maintain "seperate" turn structures for different boards
    }

    public void DealHands()
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Player Deal", SName);
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].dealHand(ADeck);
        }
    }
    public void CreatePlayers(int numberofHuman)
    {
        List<int> Ai = new List<int>();
        System.Random rnd = new System.Random();
        Ai.Add(rnd.Next(0, 100));
        Ai.Add(rnd.Next(0, 100));
        Ai.Add(rnd.Next(0, 100));
        Ai.Add(rnd.Next(0, 100));
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Create " + numberofHuman + " Players", SName);
        for (int i = 0; i < 4; i++)
        {
            if (i <= numberofHuman - 1)
            {
                players[i].gameObject.AddComponent<HumanPlayer>();
                players[i].gameObject.GetComponent<HumanPlayer>().Player(i);
            }
            else
            {
                HandleTextFile.WriteLog((LogLine += 1) + " AI Log: Create Random AI Player Type", SName);
                if (Ai[i] > 50)
                {
                    HandleTextFile.WriteLog((LogLine += 1) + " AI Log: AI Type Aggro Created", SName);
                    players[i].gameObject.AddComponent<Agro>();
                    players[i].gameObject.GetComponent<Agro>().Player(i);
                }
                else
                {
                    HandleTextFile.WriteLog((LogLine += 1) + " AI Log: AI Type Defensive Created", SName);
                    players[i].gameObject.AddComponent<Defensive>();
                    players[i].gameObject.GetComponent<Defensive>().Player(i);
                }
            }
            foreach (Sprite s in players[i].gameObject.GetComponent<imgSet>().images)
            {
                HandleTextFile.WriteLog((LogLine += 1) + " UI Log: AI Set Player Image", SName);
                players[i].gameObject.GetComponent<AbstractPlayer>().imgSet.Add(s);
            }
        }

    }
    public void TurnOrder()
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Turn Changes", SName);
        if (turn < 3) { turn++; }
        else turn = 0;
    }

    public Card DrawStoryCard()
    {
        Card TempCard = SDeck.cards[0].GetComponent<Card>();
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Story Card " + TempCard.name + " has Been Drawn " + "#BNF-1", SName);
        SDeck.cards.Remove(TempCard);
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Discard Story Card " + TempCard.name + "#BNF-1", SName);
        return TempCard;
    }

    public int CheckStoryCard(Card TemCard)
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Type of story card just drawn " + TemCard.Type, SName);
        Debug.Log((LogLine += 1) + " Type of story card just drawn: " + TemCard.Type);
        switch (TemCard.Type)
        {
            case "Event":
                return 1;
            case "Quest":
                return 2;
            case "Tournament":
                return 3;
            default:
                Debug.Log("OH FUCK, Somthing went Terribly Wrong");
                return 0;
        }
    }
    public void RunMode(int mode, Card Tc)
    {
        Debug.Log(Tc);
        int Bonus = 0;
        switch (mode)
        {
            case 1:
                HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Running Event", SName);
                pop.EnableEventCardPopup(Tc);
                pop.EnableBlockScreen(players[(turn + 1) % 4], ((turn + 1) % 4));
                RunEvent(ADeck, players, hands, dzones, ((EventQ)Tc));
                TurnOrder();
                if (PhotonNetwork.player.ID != turn + 1) { pop.EnableWaitScreen(); }
                CheckForWinner();
                ToggleBoard(turn, 0);
                SetGlow(turn);
                AiTurn();
                break;

            case 2:
                HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Running Quest", SName);
                QuestControl.CreateQuest(this, players, hands, turn, Tc, ADeck, Discard);
                break;

            case 3:
                Bonus = ((TournamentCard)Tc).bonusReward;
                ToggleBoard(turn, 1);
                TournementControler.CreateTournement(hands, players, ADeck, Discard, dzones, turn, this, Bonus, Tc);
                boardUI.SetActive(false);
                HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Running Tournament", SName);
                break;

            default:
                Debug.Log("we are dead");
                break;
        }
    }

    public void ReturnControl(Deck updateDeck, Deck updatedDiscard, List<Hand> uHand, List<Dropzone> dz, int playerWhoDrewTheCard)
    {
        //NEW PARAMETER playerWhoDrewTheCard
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Return Control T-Controller", SName);
        boardUI.SetActive(true);
        turn = playerWhoDrewTheCard;
        TurnOrder();
        SetGlow(turn);
        ifDisplay.setText(UpdateInfo(0));
        TournementControler.ResetTournament();
        pop.EnableBlockScreen(players[turn], turn);
        if (PhotonNetwork.player.ID != turn + 1) { pop.EnableWaitScreen(); }
        CheckForWinner();
        AiTurn();
    }

    //Rename
    public void ReturnControlFromQuest(bool used, int playerAfterSponsor)
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Return Control Q-Controller", SName);
        turn = playerAfterSponsor;
        boardUI.SetActive(true);
        if (KingsRec) KingsRec = !used;
        ifDisplay.setText(UpdateInfo(0));
        pop.EnableBlockScreen(players[turn], turn);
        ToggleBoard(turn, 0);
        SetGlow(turn);
        for (int i = 0; i < players.Count; i++)
        {
            FlipDropzone(dzones[i], false);
        }
        CheckForWinner();
        AiTurn();
    }

    public void CheckForWinner()
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Check For Winner", SName);
        for (int i = 0; i < 4; i++)
        {
            if (players[i].GetComponent<AbstractPlayer>().PlayerRank.rank >= 3 || (players[i].GetComponent<AbstractPlayer>().PlayerRank.rank == 2 && players[i].GetComponent<AbstractPlayer>().PlayerRank.Sheild > 9))
            {
                HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Player " + i + " won.", SName);
                pop.EnableGameWinningPopup(players[i], i);
            }
        }
    }

    public void RunEvent(Deck SomeDeck, List<GameObject> pl, List<Hand> h, List<Dropzone> dz, EventQ SCard)
    {
        HandleTextFile.WriteLog((LogLine += 1) + " Action Log: Execute Event Card" + SCard.name, SName);
        SCard.Run(SomeDeck, players, h, dz, turn, this);
        ifDisplay.setText(UpdateInfo(0));

    }

    public override void join(bool choice) { }

    public void AiTurn()
    {
        if (players[turn].GetComponent<AiPlayer>() != null)
        {
            HandleTextFile.WriteLog((LogLine += 1) + " AI Log: AI Player Draws Story Card", SName);
            EndTurn();
            ToggleBoard(turn, 0);
        }
    }
}


