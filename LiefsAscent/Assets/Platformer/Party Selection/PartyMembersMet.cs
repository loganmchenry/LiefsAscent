using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMembersMet : MonoBehaviour
{
    public Dictionary<string, bool> partyMembersMet;

    public void Start()
    {
        partyMembersMet = new Dictionary<string, bool>();
        setInitialValues();
    }

    private void setInitialValues()
    {
        partyMembersMet.Add("Warrior", false);
        partyMembersMet.Add("Thief", false);
        partyMembersMet.Add("Archer", false);
        partyMembersMet.Add("Mage", false);
        partyMembersMet.Add("Healer", false);
    }

    public void metInGame(string character)
    {
        partyMembersMet[character] = true;
        gameObject.GetComponent<PartyMembersUI>().updatePartyMemberUI(character);
    }
}
