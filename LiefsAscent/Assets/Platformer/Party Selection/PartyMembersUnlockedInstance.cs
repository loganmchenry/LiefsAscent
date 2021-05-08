using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMembersUnlockedInstance : MonoBehaviour
{
    public static PartyMembersUnlockedInstance instance;

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
}
