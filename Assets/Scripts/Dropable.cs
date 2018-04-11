using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dropable : Photon.MonoBehaviour, IDropHandler
{
    public TournementControler tournamentControler;
    public QuestController questControler;

    static string[] TzoneTypes = { "Ally", "Equipment", "Amour" };
    static string[] PlayerQzoneTypes = { "Ally", "Equipment", "Amour" };
    static string[] QzoneTypes = { "Foe", "Equipment" };

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

    //##3
    public void OnDrop(PointerEventData eventData)
    {
        Dragable dObject = eventData.pointerDrag.GetComponent<Dragable>();
        HandleTextFile.WriteLog("UI Log: Dragable Object Selected", GameControler.SName);
        //Debug.Log("*** DROP HAPPENED");
        if (dObject != null)
        {
            //Debug.Log("***" + this);
            HandleTextFile.WriteLog("UI Log: Dragable Object Not Null", GameControler.SName);
            if (this.GetComponent("Dropzone") != null)
            {
                HandleTextFile.WriteLog("UI Log: Valid Dropzone", GameControler.SName);
                //Debug.Log("***" + this);
                string ZType = getZoneType();
                Card tempCardScript = getCardType(dObject);
                if (tempCardScript != null)
                {
                    HandleTextFile.WriteLog("UI Log: Card " + tempCardScript.name + " Is Selected", GameControler.SName);
                    HandleTextFile.WriteLog("UI Log: Dropzone Type " + ZType, GameControler.SName);
                    switch (ZType)
                    {
                        case "TZone":
                            for (int i = 0; i < 3; i++)
                            {
                                int TypeCheck = 0;
                                if (TzoneTypes[i] == tempCardScript.Type)
                                {
                                    Dropzone tempDropZoneScriptTzone = (Dropzone)this.GetComponent("Dropzone");
                                    if (tempCardScript.Type == TzoneTypes[1])
                                    {
                                        foreach (EquipmentCard e in tempDropZoneScriptTzone.Equipment)
                                        {
                                            if (e.TypeOfEquipment == ((EquipmentCard)tempCardScript).TypeOfEquipment)
                                            {
                                                TypeCheck++;
                                                HandleTextFile.WriteLog("UI Log: Equipmebt Type Already Present " + ((EquipmentCard)tempCardScript).TypeOfEquipment, GameControler.SName);
                                            }
                                        }
                                    }
                                    if (TypeCheck == 0)
                                    {
                                        int UID = GetUniqueID(dObject,tempCardScript,tempDropZoneScriptTzone, tournamentControler);
                                        this.gameObject.GetComponent<PhotonView>().RPC("NetworkExecuteDropTZone", PhotonTargets.Others, UID);

                                    }
                                }
                            }
                            HandleTextFile.WriteLog("UI Log: Card Droped", GameControler.SName);
                            break;

                        case "QZone":
                            for (int i = 0; i < 2; i++)
                            {
                                int TypeCheck = 0;
                                if (QzoneTypes[i] == tempCardScript.Type)
                                {
                                    Dropzone tempDropZoneScriptQzone = (Dropzone)this.GetComponent("Dropzone");
                                    if (tempCardScript.Type == QzoneTypes[1])
                                    {
                                        foreach (EquipmentCard e in tempDropZoneScriptQzone.Equipment)
                                        {
                                            if (e.TypeOfEquipment == ((EquipmentCard)tempCardScript).TypeOfEquipment)
                                            {
                                                TypeCheck++;
                                                HandleTextFile.WriteLog("UI Log: Equipmebt Type Already Present " + ((EquipmentCard)tempCardScript).TypeOfEquipment, GameControler.SName);
                                            }
                                        }
                                    }
                                    else if (tempCardScript.Type == QzoneTypes[0])
                                    {
                                        if (tempDropZoneScriptQzone.ControledFoes.Count > 0)
                                        {
                                            TypeCheck++;
                                        }
                                    }
                                    if (TypeCheck == 0)
                                    {
                                        int UID = GetUniqueID(dObject,tempCardScript,tempDropZoneScriptQzone, questControler);
                                        this.gameObject.GetComponent<PhotonView>().RPC("NetworkExecuteDropQuests", PhotonTargets.Others, UID);
                                    }
                                }
                            }
                            HandleTextFile.WriteLog("UI Log: Card Droped", GameControler.SName);
                            break;

                        case "PlayerQZone":
                            for (int i = 0; i < 3; i++)
                            {
                                int TypeCheck = 0;
                                if (PlayerQzoneTypes[i] == tempCardScript.Type)
                                {
                                    Dropzone tempDropZoneScriptQzone = (Dropzone)this.GetComponent("Dropzone");
                                    if (tempCardScript.Type == TzoneTypes[1])
                                    {
                                        foreach (EquipmentCard e in tempDropZoneScriptQzone.Equipment)
                                        {
                                            if (e.TypeOfEquipment == ((EquipmentCard)tempCardScript).TypeOfEquipment)
                                            {
                                                TypeCheck++;
                                                HandleTextFile.WriteLog("UI Log: Equipmebt Type Already Present " + ((EquipmentCard)tempCardScript).TypeOfEquipment, GameControler.SName);
                                            }
                                        }
                                    }
                                    if(tempCardScript.Type == TzoneTypes[2] && tempDropZoneScriptQzone.SpecificUseCards.Count > 0)
                                    {
                                        TypeCheck++;
                                        HandleTextFile.WriteLog("UI Log: Amour Type Already Present", GameControler.SName);
                                    }
                                    if (TypeCheck == 0)
                                    {
                                        int UID = GetUniqueID(dObject,tempCardScript,tempDropZoneScriptQzone, questControler);
                                        this.gameObject.GetComponent<PhotonView>().RPC("NetworkExecuteDropQuests", PhotonTargets.Others, UID);
                                    }
                                }
                            }
                            HandleTextFile.WriteLog("UI Log: Card Droped", GameControler.SName);
                            break;

                        case "Discard":
                            Debug.Log("i Love Debugs They make me happy");
                            HandleTextFile.WriteLog("Action Log: Discard Card " + tempCardScript.name, GameControler.SName);
                            Dropzone tempDropZoneScriptDiscard = (Dropzone)this.GetComponent<Dropzone>();
                            Controler temp;
                            if(tournamentControler.gameState == true){temp = tournamentControler;}
                            else{temp = questControler;}
                            dObject.returnParent = this.transform;
                            dObject.transform.SetParent(this.transform);
                            Card TempCard = tempCardScript;
                            Hand THand2 = temp.hands[temp.turn];
                            int UID2 = GetCard(THand2, tempCardScript);
                            Remove(tempCardScript, THand2);
                            temp.Discard.cards.Add(TempCard);
                            dObject.gameObject.GetComponent<Image>().sprite = dObject.gameObject.GetComponent<AdventureCard>().cardBack;
                            this.gameObject.GetComponent<PhotonView>().RPC("NetworkExecuteDiscard", PhotonTargets.Others, UID2);
                            temp.ifDisplay.setText(temp.UpdateInfo(0));
                            break;

                       


                        default:
                            Debug.Log("ERROR WILL ROBINSON");
                            break;
                    }
                }
            }
            else
            {
                dObject.returnParent = this.transform;
            }

        }
    }

    public Card getCardType(Dragable Obj)
    {
        List<string> types = new List<string> { "AdventureCard", "AllyCard", "FoeCard", "EquipmentCard", "QuestCard", "TournamentCard" };
        foreach (string s in types)
        {
            //this checks if the obj has a component attached. if it doesn't it will be null
            //example: an event card wont have an ally script
            if (Obj.GetComponent(s) != null)
            {
                return (Card)Obj.GetComponent(s);
            }

        }
        return null;
    }

    public string getZoneType()
    {
        Dropzone tempScript = (Dropzone)this.GetComponent("Dropzone");
        return tempScript.Type;
    }

    public void Remove(Card tempCardScript, Hand TempHand)
    {
        foreach (Card c in TempHand.cards)
        {
            if (tempCardScript.name == c.name)
            {
                TempHand.cards.Remove(c);
                break;
            }
        }

    }

    [PunRPC]
    public void NetworkExecuteDropTZone(int UID)
    {
        Debug.Log(UID);
        Dropzone Dropzone = (Dropzone)this.GetComponent("Dropzone");
        TournementControler Controler = tournamentControler;
        Hand THand = Controler.hands[Controler.turn];
        Card temp = Instantiate(THand.cards[UID]);
        Dropzone.AddCard(temp);
        temp.transform.SetParent(this.transform);
        Destroy(THand.transform.GetChild(UID).gameObject);
        THand.cards.RemoveAt(UID);
        Controler.ifDisplay.setText(Controler.UpdateInfo(0));
    }
    [PunRPC]
    public void NetworkExecuteDropQuests(int UID)
    {
        Debug.Log(UID);
        Dropzone Dropzone = (Dropzone)this.GetComponent("Dropzone");
        QuestController Controler = questControler;
        Hand THand = Controler.hands[Controler.turn];
        Card temp = Instantiate(THand.cards[UID]);
        Dropzone.AddCard(temp);
        temp.transform.SetParent(this.transform);
        Destroy(THand.transform.GetChild(UID).gameObject);
        THand.cards.RemoveAt(UID);
        if(Dropzone.Type == "PlayerQZone"){
            Controler.ifDisplay.setText(Controler.UpdateInfo(0));
        }
    }
    
    [PunRPC]
    public void NetworkExecuteDiscard(int UID)
    {
        Dropzone Dropzone = (Dropzone)this.GetComponent("Dropzone");
        TournementControler Controler = tournamentControler;
        Hand THand = Controler.hands[Controler.turn];
        Controler.Discard.cards.Add(Instantiate(THand.cards[UID]));
        Destroy(THand.transform.GetChild(UID).gameObject);
        THand.cards.RemoveAt(UID);
        Controler.ifDisplay.setText(Controler.UpdateInfo(0));
    }


    public int GetCard(Hand h, Card n)
    {
        for (int i = 0; i < h.cards.Count; i++)
        {
            if (h.cards[i].name == n.name)
            {
                return i;
            }

        }
        return -1;
    }


    public int GetUniqueID(Dragable dObject, Card tempCardScript, Dropzone tempDropZone, Controler Tcontrol)
    {
        //dObject.returnParent.gameObject.GetComponent<Hand>().cards.Remove(tempCardScript);
        dObject.returnParent = this.transform;
        tempDropZone.AddCard(tempCardScript);
        Hand THand = Tcontrol.hands[Tcontrol.turn];
        int UID = GetCard(THand, tempCardScript);
        Remove(tempCardScript, THand);
        Tcontrol.ifDisplay.setText(Tcontrol.UpdateInfo(0));
        return UID;
    }
}