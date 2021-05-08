using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticsInventorySystem : MonoBehaviour
{
    private List<GameObject> items;
    private Transform itemsParent;
    private GameObject slot;
    private Image slotImage;
    public GameObject unitSelected;       //The unit OBJECT who is currently taking action
    public GameObject characterDisplay;

    // Start is called before the first frame update
    void Start()
    {
        items = PlayerStats.instance.items;
        itemsParent = gameObject.transform.GetChild(0);
        characterDisplay = gameObject.transform.GetChild(4).gameObject;
        characterDisplay.transform.GetChild(0).GetComponent<Image>().sprite = unitSelected.GetComponent<SpriteRenderer>().sprite;
        characterDisplay.transform.GetChild(0).GetComponent<Image>().enabled = true;

        for (int i = 0; i < items.Count; i++)
        {
            slot = itemsParent.GetChild(i).gameObject;
            slotImage = slot.transform.GetChild(0).GetComponent<Image>();
            if(slotImage.isActiveAndEnabled ==  false)
            {
                if(items[i].GetComponent<SpriteRenderer>().sprite != null)
                {
                    slotImage.sprite = items[i].GetComponent<SpriteRenderer>().sprite;
                    slotImage.enabled = true;
                }
            }
        }
    }


    public void onItemClick(GameObject button)
    {
        GameObject itemIcon = button.transform.GetChild(0).gameObject;
        Image icon = itemIcon.GetComponent<Image>();
        CharacterScript unitScript = unitSelected.transform.GetComponent<CharacterScript>();
        int index = button.transform.GetSiblingIndex();
        GameObject inventoryParent = button.transform.parent.gameObject.transform.parent.gameObject;
        GameObject inventoryDescriptionPanel = inventoryParent.transform.GetChild(2).gameObject;
        GameObject displayIcon = inventoryDescriptionPanel.transform.GetChild(1).gameObject;
        GameObject displayDescription = inventoryDescriptionPanel.transform.GetChild(0).gameObject;

        switch (icon.sprite.name)
        {
            case "#2 - Transparent Icons & Drop Shadow_104":        // This is the potion
            {
                    healAlly(10);
                    icon.enabled = false;
                    icon.sprite = null;
                    displayIcon.transform.GetComponent<Image>().enabled = false;
                    displayDescription.transform.GetComponent<TMPro.TextMeshProUGUI>().text = " ";
                    characterDisplay.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Healed for +10!";
                    items.RemoveAt(index);
                    break;
            }
            default:
            {
                break;
            }

        }

    }

    private void healAlly(int amount)
    {
        CharacterScript unitScript = unitSelected.transform.GetComponent<CharacterScript>();
        unitScript.UpdateHealth(amount);
    }

    public void mouseEnter(GameObject button)
    {
        GameObject itemIcon = button.transform.GetChild(0).gameObject;
        Image icon = itemIcon.GetComponent<Image>();
        int index = button.transform.GetSiblingIndex();
        GameObject inventoryParent = button.transform.parent.gameObject.transform.parent.gameObject;
        GameObject inventoryDescriptionPanel = inventoryParent.transform.GetChild(2).gameObject;
        GameObject displayIcon = inventoryDescriptionPanel.transform.GetChild(1).gameObject;
        GameObject displayDescription = inventoryDescriptionPanel.transform.GetChild(0).gameObject;

        switch (icon.sprite.name)
        {
            case "#2 - Transparent Icons & Drop Shadow_104":        // This is the potion
                {
                    displayIcon.transform.GetComponent<Image>().sprite = icon.sprite;
                    displayIcon.transform.GetComponent<Image>().enabled = true;
                    displayDescription.transform.GetComponent<TMPro.TextMeshProUGUI>().text = "Consume potion for +10 HP";
                    break;
                }
            default:
                    break;

        }


    }

    public void mouseLeave(GameObject button)
    {
        GameObject itemIcon = button.transform.GetChild(0).gameObject;
        Image icon = itemIcon.GetComponent<Image>();
        GameObject inventoryParent = button.transform.parent.gameObject.transform.parent.gameObject;
        GameObject inventoryDescriptionPanel = inventoryParent.transform.GetChild(2).gameObject;
        GameObject displayIcon = inventoryDescriptionPanel.transform.GetChild(1).gameObject;
        GameObject displayDescription = inventoryDescriptionPanel.transform.GetChild(0).gameObject;

        if(icon.isActiveAndEnabled == true)
        {
            displayIcon.transform.GetComponent<Image>().enabled = false;
            displayDescription.transform.GetComponent<TMPro.TextMeshProUGUI>().text = " ";
        }
               
    }

    public void onExitClick()
    {
        gameObject.SetActive(false);
        characterDisplay.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = " ";
    }
}
