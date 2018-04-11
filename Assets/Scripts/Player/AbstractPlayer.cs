using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayer : MonoBehaviour
{
    public int playerID;
    public int BP;
    public int Bids;
    public Rank PlayerRank;
    public List<Sprite> imgSet;
    public int NumCards;

    public abstract void Player(int id);

    public int SetBP(int points)
    {
        BP = points + PlayerRank.BattlePoints;
        return BP;
    }

    public int SetSheilds(int addedSheilds)
    {
        HandleTextFile.WriteLog("Action Log: Player " + (playerID + 1) + " Gets Sheilds" + addedSheilds, GameControler.SName);
        PlayerRank.Sheild += addedSheilds;
        if (PlayerRank.Sheild >= 5 && PlayerRank.rank == 0)
        {
            PlayerRank.knight();
            HandleTextFile.WriteLog("Action Log: Player " + (playerID + 1) + " Becomes Knight", GameControler.SName);
        }
        else if (PlayerRank.Sheild >= 7 && PlayerRank.rank == 1)
        {
            PlayerRank.Cknight();
            HandleTextFile.WriteLog("Action Log: Player " + (playerID + 1) + " Become Champion Knight", GameControler.SName);
        }
        else if (PlayerRank.Sheild >= 10 && PlayerRank.rank == 2)
        {
            PlayerRank.Cknight();
            HandleTextFile.WriteLog("Action Log: Player " + (playerID + 1) + " Becomes Knight Of The Round", GameControler.SName);
        }
        return PlayerRank.Sheild;
    }

    public int SetBids(int NewBids)
    {
        Bids = NewBids;
        return Bids;
    }

    public int SetNumCards(int num)
    {
        NumCards = num;
        return NumCards;
    }
    public List<int> UpdateAll(int s, int points, int bids, int NumCards)
    {
        List<int> RList = new List<int>();
        RList.Add(SetBP(points));
        RList.Add(SetSheilds(s));
        RList.Add(SetBids(bids));
        RList.Add(SetNumCards(NumCards));
        return RList;

    }


}