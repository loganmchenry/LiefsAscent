using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPartySelection : MonoBehaviour
{
    [SerializeField] GameObject partySelectGUI;

    public void resetParty()
    {
        partySelectGUI = PartyMembersInstance.instance.getPartyMember();
    }

    public void OpenParty()
    {
        if (partySelectGUI != null)
        {
            if (!partySelectGUI.activeSelf)
            {
                partySelectGUI.SetActive(true);
            }
            else
            {
                PlayerStats.instance.UpdateActiveParty();
                partySelectGUI.SetActive(false);
            }
        }
    }
}
