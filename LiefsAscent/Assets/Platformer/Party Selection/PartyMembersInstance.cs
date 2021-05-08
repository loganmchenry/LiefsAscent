using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMembersInstance : MonoBehaviour
{
    public static PartyMembersInstance instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void metInGame(string character)
    {
        GetComponentInChildren<PartyMembersMet>().metInGame(character);
    }

    public void updatePartyMemberUIStats()
    {
        GetComponentInChildren<PartyMembersUI>().updatePartyHP();
    }

    public GameObject getPartyList()
    {
        return GetComponentInChildren<PartyMembersUI>().getPartyListUI();
    }

    public GameObject getPartyMember()
    {
        return GetComponentInChildren<PartyMembersUI>().getPartyMemberUI();
    }
}
