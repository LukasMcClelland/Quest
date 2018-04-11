using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : AbstractPlayer
{
    public override void Player(int id)
    {
        imgSet = new List<Sprite>();
        PlayerRank = new Rank();
        BP = 0;
        playerID = id;
        HandleTextFile.WriteLog("Action Log: Player " + (playerID + 1) + " Becomes Squire", GameControler.SName);
        PlayerRank.Squire();
    }





}