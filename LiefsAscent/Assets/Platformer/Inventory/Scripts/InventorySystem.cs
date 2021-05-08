using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("General Fields")]
    // A list of items that were picked up
    public List<GameObject> items = new List<GameObject>();
    // flag indicates if the inventory is open or not
    public bool isOpen;
    [Header("UI Items Section")]
    //Inventory System Window
    public GameObject ui_Window;
    public Image[] items_images;
    [Header("UI Item Description")]
    public GameObject ui_description_Window;
    public Image description_Image;
    public Text description_Title;
    public Text description_Text;

    [Header("Equipment Window")]
    public List<GameObject> equips = new List<GameObject>();

    public GameObject equip_Window;
    public Image[] equipped_images;
    public Image[] equipable_images;

    [HideInInspector]
    public int openSlot = 2;

    public GameObject rewardItem;

    private void Update()
    {
        if (Input.GetKeyDown(ControlManager.instance.ActionControls["Inventory"]))
        {
            if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
            {
                ToggleInventory();
            }
        }

        if (RewardsManager.didWin())
        {
            AddReward();
        }
        
    }

    // Function that opens and closes the inventory menu
    void ToggleInventory()
    {
        isOpen = !isOpen;
        ui_Window.SetActive(isOpen);
        Update_UI();
    }



    // Add the item to the items list
    public void PickUp(GameObject item)
    {
        items.Add(item);
        PlayerStats.instance.items.Add(item);

        if (item.GetComponent<Item>().type == Item.ItemType.Equipable)
        {
            equips.Add(item);

        }

        Update_UI();
    }


    // called when the player reaches the victory state in combat mode
    public void AddReward()
    {
        RewardsManager.setVictoryToFalse();
    }


    // Refresh the UI elements in the Inventory Window
    void Update_UI()
    {
        Hide_All();
        // For each item in the "items" list
        // Show it in the respective slot in the "items_images"
        for (int i = 0; i < items.Count; i++)
        {
            items_images[i].sprite = items[i].GetComponent<SpriteRenderer>().sprite;
            Debug.Log($"{items_images[i].sprite} just got wooo!");
            items_images[i].gameObject.SetActive(true);
        }

        for (int j = 0; j < equips.Count; j++)
        {
            /*Debug.Log($"in loop {j}");*/
            // just a safe check to make sure we are in equipable items
            if (equips[j].GetComponent<Item>().type == Item.ItemType.Equipable)
            {
                Debug.Log($"{equips[j]} is equipable!");
                // gotta do a switch since equipped_images only has two slots and using
                // the index j would get out of bound errors,
                // but basically, if the top slot isn't active, make the bottoms one active
                switch (openSlot)
                {
                    case 2:
                        // if there is two open slots, then noting is equipped
                        // make the items visible
                        /*Debug.Log("no HERE you IDIOT!");*/
                        equipable_images[j].sprite = equips[j].GetComponent<SpriteRenderer>().sprite;
                        equipable_images[j].gameObject.SetActive(true);
                        break;
                    case 1:
                        /*Debug.Log("in case 1!");*/
                        // if the sprites that are equipped at either slot aren't the same as the current item
                        // then we gucci and can make them active 
                        Sprite tempEquip = equips[j].GetComponent<SpriteRenderer>().sprite;
                        Sprite slot0 = equipped_images[0].sprite;
                        Sprite slot1 = equipped_images[1].sprite;

                        if (tempEquip.name != slot0.name && tempEquip.name != slot1.name)
                        {
                            Debug.Log($"{tempEquip.name} isn't the same as {slot0.name} and {slot1.name}");
                            equipable_images[j].sprite = equips[j].GetComponent<SpriteRenderer>().sprite;
                            equipable_images[j].gameObject.SetActive(true);
                        }

                        break;
                    case 0:
                        break;
                }


            }
        }

    }

    // Hide all the items ui images
    void Hide_All()
    {
        foreach (var i in items_images) { i.gameObject.SetActive(false); }

        // Hide the item image, text, etc
        HideDescription();

    }

    // Shows the picture, description, and name of the item in the description window
    public void ShowDescription(int id)
    {

        // Set the image, the same image from our inventory slot
        description_Image.sprite = items_images[id].sprite;
        Debug.Log($"showing description of {items_images[id].sprite}");
        // set the Title, based on picture name
        description_Title.text = items[id].name;
        //Show the description by getting into the Item script and getting the description text from there
        description_Text.text = items[id].GetComponent<Item>().descriptionTxt;
        // show the window

        description_Image.gameObject.SetActive(true);
        //Debug.Log($"{description_Image.gameObject.activeSelf} is active?");
        description_Title.gameObject.SetActive(true);
        description_Text.gameObject.SetActive(true);
    }

    // Another function to display the correct things when hovering over equipment TOP slots
    public void ShowEquipDescription(int id)
    {
        // get the TOP slots from the equipment UI window as the image
        description_Image.sprite = equipped_images[id].sprite;

        // loop the equips array to find the same sprite, then we know we are at the right item to get the name and desc
        for (int i = 0; i < equips.Count; i++)
        {
            if (description_Image.sprite == equips[i].GetComponent<SpriteRenderer>().sprite)
            {
                description_Title.text = equips[i].name;
                description_Text.text = equips[i].GetComponent<Item>().descriptionTxt;
            }
        }

        description_Image.gameObject.SetActive(true);
        description_Title.gameObject.SetActive(true);
        description_Text.gameObject.SetActive(true);

    }

    // for the on pointer enter on the indiv slots to show description
    public void ShowEquipSlotDescription(int id)
    {
        // get the image from the yellow slots in the equipment inventory
        description_Image.sprite = equipable_images[id].sprite;

        // loop the equips array to find the same sprite, then we know we are at the right item to get the name and desc
        for (int i = 0; i < equips.Count; i++)
        {
            if (description_Image.sprite == equips[i].GetComponent<SpriteRenderer>().sprite)
            {
                description_Title.text = equips[i].name;

                description_Text.text = equips[i].GetComponent<Item>().descriptionTxt;
            }
        }
        description_Image.gameObject.SetActive(true);
        description_Title.gameObject.SetActive(true);
        description_Text.gameObject.SetActive(true);
    }




    // Hides the image title, text and image itself for the Inventory Description Window
    public void HideDescription()
    {
        description_Image.gameObject.SetActive(false);
        description_Title.gameObject.SetActive(false);
        description_Text.gameObject.SetActive(false);
    }

    // Function that "consumes" the object linked to custom ConsumeEvent that can be used to do whatever
    public void Consume(int id)
    {
        // Check if the item is consumable
        if (items[id].GetComponent<Item>().type == Item.ItemType.Consumables)
        {
            Debug.Log($"CONSUMED {items[id].name}");
            // Invoke the consume custom event
            items[id].GetComponent<Item>().consumeEvent.Invoke();
            // Destroy the item after a set time, 0.1 seconds
            Destroy(items[id], 0.1f);
            // Clear the item from the list
            items.RemoveAt(id);
            // Update UI (item isn't there anymore so we need to remove it )
            Update_UI();
        }
    }

    /*
     *  This function gets called as a Pointer Click event trigger on the
     *  equipment slot image itself, so that when the user clicks on the image
     *  of the item it will move the item to the top slots of the equipment UI
     * 
     */
    public void Equip(int id)
    {

        // Make sure the item we are trying to equip is Equipable
        if (equips[id].GetComponent<Item>().type == Item.ItemType.Equipable)
        {

            switch (openSlot)
            {
                case 2:
                    Debug.Log("two open slots!");
                    Debug.Log($"EQUIPPED {equips[id].name}");
                    // get the images from the slots in the equipment window
                    equipped_images[0].sprite = equipable_images[id].GetComponent<Image>().sprite;
                    // set the top slot to be active
                    equipped_images[0].gameObject.SetActive(true);


                    // reduce the number of slots opened
                    openSlot--;

                    // // hide the slot that was there before
                    equipable_images[id].gameObject.SetActive(false);

                    // // make the picture null so no weird business happens
                    // equipable_images[id].sprite = null;
                    break;
                case 1:
                    Debug.Log("one open slot!");
                    Debug.Log($"EQUIPPED {equips[id].name}");

                    // check if the "2nd" ( technically 1st array spot), top slot is active, means an item is there
                    // and the user un-equipped their first slot
                    // thus we set the image to the "1st" slot (technically 0th)
                    if (equipped_images[1].gameObject.activeSelf)
                    {
                        equipped_images[0].sprite = equipable_images[id].GetComponent<Image>().sprite;
                        equipped_images[0].gameObject.SetActive(true);
                    } else
                    {
                        // else if the 2nd isn't active, then we can safely move to second (techinally 1st)
                        equipped_images[1].sprite = equipable_images[id].GetComponent<Image>().sprite;
                        equipped_images[1].gameObject.SetActive(true);
                    }

                    openSlot--;
                    // hide the slot that was there before
                    equipable_images[id].gameObject.SetActive(false);
                    // equipable_images[id].sprite = null;
                    break;
                case 0:
                    Debug.Log($"Item: { equips[id].name} was not equipped, all slots full!");
                    // all slots full, do nothing
                    break;
            }


        }

    }

    /* 
     * Same pointer event type as equip but on the top equip slots when the user wants to un-equip them
     * So when the user clicks on the top slots they get un-equipped and they get put back to the nearest open slot
     */
    public void UnEquip(int id)
    {
        bool foundOnce = false;
        // again it shouldn't be the case these items AREN't Equipable but just making sure
        if (equips[id].GetComponent<Item>().type == Item.ItemType.Equipable)
        {
            switch (openSlot)
            {
                case 2:
                    // no items equipped wyd?
                    Debug.Log("bruh, you got no items equipped, loser");
                    break;
                case 1:
                case 0:
                    // one item slot open, therefore one equipped
                    // OR zero slots open, both equipped

                    Debug.Log($"UN-EQUIPPED {equips[id].name}");

                    // set the top slot to unactive
                    equipped_images[id].gameObject.SetActive(false);



                    // loop until we find an open slot
                    for (int i = 0; i < equips.Count; i++)
                    {
                        // if the slot we are looking at isn't active, and we havent found an open slot yet
                        // then there is no item there, set the sprite and set active
                        if (!equipable_images[i].gameObject.activeSelf & !foundOnce)
                        {
                            // set the sprite of the one we just took off
                            equipable_images[i].sprite = equipped_images[id].GetComponent<Image>().sprite;
                            equipable_images[i].gameObject.SetActive(true);
                            openSlot++;
                            // if we have set one of them active, don't gotta keep looping to find another
                            foundOnce = true;
                        }
                    }

                    break;
            }

        }
    }

}
