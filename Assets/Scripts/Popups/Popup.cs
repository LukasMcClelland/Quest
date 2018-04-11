using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public BlockScreen BlockScreen, TournamentBlockScreen, QuestBlockScreen;
    public QuestStartPopup QuestStartPopup;
    public TournamentStartPopup TournyStartPopup;
    public DiscardWindow DiscardWindow;
    public EventPopup EventPopup;
    public TournamentWinnerPopup TournyWinnerPopup;
    public GameWinningPopup GameWinningPopup;
    public WaitScreen GameWaitScreen;
    public RobinW rw;
    public SponsorIneligiblePopup SponsorIneligiblePopup;
    public ResolveStagePopup ResolveStagePopup;
    public QuestWinnerPopup QuestWinnerPopup;
    public JoinScreen JoinScreenPopup, SponsorScreenPopup;

    // quest resolve popup (winner and shields won)
    // kings call to arms (2 foes discard, or 1 weapon discard)
    // MAAYBE improve discard?
    // popup is block screen (with raycast)
    
    //Enablers 
    public void EnableWaitScreen(){this.GameWaitScreen.gameObject.SetActive(true);}
    public void EnableBlockScreen(GameObject player, int number){
        this.BlockScreen.gameObject.SetActive(true);
        this.BlockScreen.playerNumber.gameObject.SetActive(true);
        this.BlockScreen.playerNumber.text = "Player " + (number + 1).ToString() + "'s turn";
        this.BlockScreen.player.gameObject.GetComponent<Image>().sprite = player.GetComponent<Image>().sprite;
    }
    public void EnableEventCardPopup(Card StoryCard){
        this.EventPopup.gameObject.SetActive(true);
        this.EventPopup.Card.gameObject.GetComponent<Image>().sprite = StoryCard.GetComponent<Image>().sprite;
    }
    public void EnableTournamentStartPopup(Card StoryCard){
        this.TournyStartPopup.gameObject.SetActive(true);
        this.TournyStartPopup.card.gameObject.GetComponent<Image>().sprite = StoryCard.GetComponent<Image>().sprite;
    }
    public void EnableTournamentBlockScreen(GameObject player, int number){
        this.TournamentBlockScreen.gameObject.SetActive(true);
        this.TournamentBlockScreen.playerNumber.gameObject.SetActive(true);
        this.TournamentBlockScreen.playerNumber.text = "Player " + (number + 1).ToString() + "'s Tournament Phase";
        this.TournamentBlockScreen.player.gameObject.GetComponent<Image>().sprite = player.GetComponent<Image>().sprite;
    }
    public void EnableQuestBlockScreen(GameObject player, int number){
        this.QuestBlockScreen.gameObject.SetActive(true);
        this.QuestBlockScreen.playerNumber.gameObject.SetActive(true);
        this.QuestBlockScreen.playerNumber.text = "Player " + (number + 1).ToString() + "'s Quest Phase";
        this.QuestBlockScreen.player.gameObject.GetComponent<Image>().sprite = player.GetComponent<Image>().sprite;
    }
    public void EnableJoinBlockScreen(int player, bool joined){
        this. JoinScreenPopup.gameObject.SetActive(true);
        if(joined==true){
            JoinScreenPopup.join.gameObject.SetActive(true);
            JoinScreenPopup.status.text = "Player " + (player + 1) + " Has Joined The Tournament"; 
        }
        else{
            JoinScreenPopup.decline.gameObject.SetActive(true);
            JoinScreenPopup.status.text = "Player " + (player + 1) + " Has Fled The Tournament"; 
        }
    }
    public void EnableSponsorBlockScreen(int player, bool joined){
        this.SponsorScreenPopup.gameObject.SetActive(true);
        if(joined==true){
            SponsorScreenPopup.join.gameObject.SetActive(true);
            SponsorScreenPopup.status.text = "Player " + (player + 1) + " Has Sponsored the Horde"; 
        }
    }
    public void EnableTournamentWinnerPopup(GameObject PlayerThatWon, int NumShields, int PlayerNum){
        TournyWinnerPopup.gameObject.SetActive(true);
        TournyWinnerPopup.text.text = "Player " + (PlayerNum + 1).ToString() + " wins the Tournament and is rewarded with " + NumShields.ToString() + " shields!";
        TournyWinnerPopup.card.gameObject.GetComponent<Image>().sprite = PlayerThatWon.GetComponent<Image>().sprite;
    }
    public void ActivateDiscard(int CardsToDiscard){
        DiscardWindow.gameObject.SetActive(true);
        DiscardWindow.Prompt.gameObject.SetActive(true);
        DiscardWindow.Prompt.text = "You have more than 12 cards. Discard " + CardsToDiscard.ToString() + " in order to continue";
        DiscardWindow.CardsToDiscard = CardsToDiscard;
    }
    public void EnableRW(){
        this.gameObject.SetActive(true);
        rw.gameObject.SetActive(true);
    }
    public void EnableQuestStartPopup(Card StoryCard){
        QuestStartPopup.gameObject.SetActive(true);
        QuestStartPopup.card.gameObject.GetComponent<Image>().sprite = StoryCard.GetComponent<Image>().sprite;
    }
    public void EnableSponsorIneligiblePopup(){
        this.gameObject.SetActive(true);
        this.SponsorIneligiblePopup.gameObject.SetActive(true);
    }

    //Disablers
    public void DisableBlockScreenPopup(){this.BlockScreen.gameObject.SetActive(false);}
    public void DisableWaitScreen(){this.GameWaitScreen.gameObject.SetActive(false);}
    public void DisableEventScreenPopup(){this.EventPopup.gameObject.SetActive(false);}
    public void DisableTournamentBlockScreenPopup(){TournamentBlockScreen.gameObject.SetActive(false);}
    public void DisableTournamentStartPopup(){TournyStartPopup.gameObject.SetActive(false);}
    public void DisableTournamentWinnerPopup(){TournyWinnerPopup.gameObject.SetActive(false);}
    public void DisableDiscardPopup(){DiscardWindow.gameObject.SetActive(false);}
    public void DisableRWPopup(){rw.gameObject.SetActive(false);}
    public void DisableQuestStartPopup(){QuestStartPopup.gameObject.SetActive(false);}
    public void DisableSponsorIneligiblePopup(){SponsorIneligiblePopup.gameObject.SetActive(false);}
    public void DisableQuestBlockScreenPopup(){QuestBlockScreen.gameObject.SetActive(false);}
    public void DisableQuestWinnerPopup(){QuestWinnerPopup.gameObject.SetActive(false);}
    public void DisablejoinScreenPopup(){
        JoinScreenPopup.decline.gameObject.SetActive(false);
        JoinScreenPopup.join.gameObject.SetActive(false);
        JoinScreenPopup.gameObject.SetActive(false);
    }
    public void DisableSponsorScreenPopup(){
        SponsorScreenPopup.decline.gameObject.SetActive(false);
        SponsorScreenPopup.join.gameObject.SetActive(false);
        SponsorScreenPopup.gameObject.SetActive(false);
    }


//TO DO
    public void EnableGameWinningPopup(GameObject PlayerWhoWon, int PlayerNum)
    {
        this.gameObject.SetActive(true);
        GameWinningPopup.gameObject.SetActive(true);
        GameWinningPopup.card.gameObject.GetComponent<Image>().sprite = PlayerWhoWon.GetComponent<Image>().sprite;
        GameWinningPopup.message.text = "Player " + (PlayerNum + 1).ToString() + " wins and has become a knight of the Round Table!";
    }

    public void EnableResolveStagePopup(int stage, List<bool> questingPlayers, int sponsorPlayer, List<Dropzone> dzones, List<Dropzone> qzones, List<GameObject> players)
    {
        foreach(Transform Child in  qzones[stage].transform){
            GameObject temp = Instantiate(ResolveStagePopup.tempIMG);
            temp.transform.SetParent(ResolveStagePopup.SponsorZone.transform);
            temp.GetComponent<Image>().sprite = Child.GetComponent<Card>().img;
        }
        for (int i = 0; i <questingPlayers.Count; i++)
        {
            ResolveStagePopup.playerPics[i].GetComponent<Image>().sprite = players[i].GetComponent<Image>().sprite;
            foreach(Transform Child in dzones[i].transform)
            {
                GameObject temp = Instantiate(ResolveStagePopup.tempIMG);
                temp.transform.SetParent(ResolveStagePopup.PlayerZones[i].transform);
                temp.GetComponent<Image>().sprite = Child.GetComponent<Card>().img;
            }

            if (questingPlayers[i])
            {
                ResolveStagePopup.Checks[i].SetActive(true);
            }
            else
            {
                if(i == sponsorPlayer)
                {
                    ResolveStagePopup.sponsorIndicators[i].SetActive(true);
                }
                else
                {
                    ResolveStagePopup.Xs[i].SetActive(true);
                }
            }       
        }
        this.gameObject.SetActive(true);
        this.ResolveStagePopup.gameObject.SetActive(true);
    }
  
    public void EnableQuestWinnerPopup(List<bool> qplayers, int shields, int sponsorPlayer, List<GameObject> players)
    {
        for (int i = 0; i < qplayers.Count; i++)
        {
            QuestWinnerPopup.playerPics[i].GetComponent<Image>().sprite = players[i].GetComponent<Image>().sprite;
            if (i == sponsorPlayer)
            {
                QuestWinnerPopup.texts[i].text = "Player " + (i + 1) + " was the sponsor";
            }
            else
            {
                if (qplayers[i])
                {
                    QuestWinnerPopup.texts[i].text = "Player " + (i+1) + " is rewarded with " + shields + " shields";
                }
                else
                {
                    QuestWinnerPopup.texts[i].text = "Player " + (i+1) + " receives no reward";
                }
            }
        }
    }

    public void DisableResolveStagePopup()
    {
        foreach (Transform Child in ResolveStagePopup.SponsorZone.transform)
        {
            Destroy(Child.gameObject);
        }

        for (int i = 0; i < ResolveStagePopup.PlayerZones.Count; i++)
        {
            foreach (Transform Child in ResolveStagePopup.PlayerZones[i].transform)
            {
                Destroy(Child.gameObject);
            }
            ResolveStagePopup.Checks[i].SetActive(false);
            ResolveStagePopup.sponsorIndicators[i].SetActive(false);
            ResolveStagePopup.Xs[i].SetActive(false);
        }

        ResolveStagePopup.gameObject.SetActive(false);
    }
//

}
