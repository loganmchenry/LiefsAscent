using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/*
    Keep track of what is what in the player cards.
    0 -> Player Card | 1 -> Main Image | 2 -> Name Text 
    3 -> Description | 4 -> Lock Image
*/
public class PartyMembersUI : MonoBehaviour
{
    [SerializeField] GameObject warriorCard;
    [SerializeField] GameObject thiefCard;
    [SerializeField] GameObject archerCard;
    [SerializeField] GameObject mageCard;
    [SerializeField] GameObject healerCard;

    [SerializeField] GameObject partyList;
    [SerializeField] GameObject partyMemberUI;

    // Text Areas for Updating Player Cards
    [SerializeField] TextMeshProUGUI liefHp;
    [SerializeField] TextMeshProUGUI warriorHp;
    [SerializeField] TextMeshProUGUI thiefHp;
    [SerializeField] TextMeshProUGUI archerHp;
    [SerializeField] TextMeshProUGUI mageHp;
    [SerializeField] TextMeshProUGUI healerHp;

    // Player Stats
    public UnitClass Lief;
    public UnitClass Warrior;
    public UnitClass Thief;
    public UnitClass Archer;
    public UnitClass Mage;
    public UnitClass Healer;

    DragHandler dragHandler;
    Color white;
    Color grey;
    Color darkred;

    public void Start()
    {
        ColorUtility.TryParseHtmlString("#FFFFFF", out white); // Set White Color
        ColorUtility.TryParseHtmlString("#B0A8A8", out grey);  // Set Grey Color
        ColorUtility.TryParseHtmlString("#650000", out darkred);  // Set Dark Red Color

        // Set Initial HP's
        setInitialHP();
    }

    private void setInitialHP()
    {
        // Grab the Properties of Each Unit Class
        Lief = PlayerStats.instance.Lief;
        Warrior = PlayerStats.instance.Warrior;
        Thief = PlayerStats.instance.Thief;
        Archer = PlayerStats.instance.Archer;
        Mage = PlayerStats.instance.Mage;
        Healer = PlayerStats.instance.Healer;

        // Set the HP to maxHP
        liefHp.text = Lief.MaxHP + "/" + Lief.MaxHP;
        warriorHp.text = Warrior.MaxHP + "/" + Warrior.MaxHP;
        thiefHp.text = Thief.MaxHP + "/" + Thief.MaxHP;
        archerHp.text = Archer.MaxHP + "/" + Archer.MaxHP;
        mageHp.text = Mage.MaxHP + "/" + Mage.MaxHP;
        healerHp.text = Healer.MaxHP + "/" + Healer.MaxHP;
    }

    public void updatePartyMemberUI(string character)
    {
        GameObject card = findCorrectCard(character);
        dragHandler = card.GetComponent<DragHandler>();

        // Fix-Up Card - Set it Active & Fix color/picture
        Transform[] children = card.GetComponentsInChildren<Transform>();
        setOuterColorLock(card.transform.parent);
        children[4].gameObject.SetActive(false);         // Remove Lock Picture
        card.GetComponent<Image>().color = grey;         // Set the player's card to visible (light grey) 
        children[1].GetComponent<Image>().color = white; // Establish Character's Image (Set it to visible)
        children[2].GetComponent<TextMeshProUGUI>().color = white; // Establish Character's Name (Set it to visible)
        dragHandler.CanDrag = true; // Make the card accessible(draggable)
    }

    private GameObject findCorrectCard(string character)
    {
        GameObject card = null;
        switch (character)
        {
            case "Warrior":
            case "Etoile Card":
                card = warriorCard;
                break;
            case "Thief":
            case "Ninja Card":
                card = thiefCard;
                break;
            case "Archer":
            case "Skeletion Card":
                card = archerCard;
                break;
            case "Mage":
            case "Old Man Card":
                card = mageCard;
                break;
            case "Healer":
            case "Adventurer Card":
                card = healerCard;
                break;
        }

        return card;
    }

    public void setTransparent(Transform the_parent)
    {
        Debug.Log(the_parent.gameObject);
        Color tmp = white;
        tmp.a = 0.5f;
        the_parent.gameObject.GetComponent<Image>().color = tmp;
    }

    public void updatePartyHP()
    {
        // Grab the Properties of Each Unit Class
        Lief = PlayerStats.instance.Lief;
        Warrior = PlayerStats.instance.Warrior;
        Thief = PlayerStats.instance.Thief;
        Archer = PlayerStats.instance.Archer;
        Mage = PlayerStats.instance.Mage;
        Healer = PlayerStats.instance.Healer;

        // Set the HP to currentHp
        liefHp.text = Lief.CurrHP + "/" + Lief.MaxHP;
        warriorHp.text = Warrior.CurrHP + "/" + Warrior.MaxHP;
        thiefHp.text = Thief.CurrHP + "/" + Thief.MaxHP;
        archerHp.text = Archer.CurrHP + "/" + Archer.MaxHP;
        mageHp.text = Mage.CurrHP + "/" + Mage.MaxHP;
        healerHp.text = Healer.CurrHP + "/" + Healer.MaxHP;
    }

    public void setOuterColorLock(Transform the_parent)
    {
        Debug.Log(the_parent.gameObject);
        Color tmp = darkred;
        tmp.a = 255;
        the_parent.gameObject.GetComponent<Image>().color = tmp;
    }

    public GameObject getPartyListUI()
    {
        return partyList;
    }

    public GameObject getPartyMemberUI()
    {
        return partyMemberUI;
    }
}
