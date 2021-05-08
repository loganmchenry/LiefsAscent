using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider2D))]

//Dynamic Script for different types of items
public class Item : MonoBehaviour
{
    

    // The different types of interactions
    // NONE for default
    // PickUp for items that we can pick up
    // Examine for items that can just be examined but not consumed/equipped
    public enum InteractionType{NONE, PickUp,Examine}
   
    // Items that are Static maybe quest items and the like
    // Items that are consumable like potions, can add on to this like equippable later
    public enum ItemType { Static, Consumables, Equipable }
    [Header("Attributes")]
    public InteractionType interactType;
    public ItemType type;
    [Header("Examine")]
    public string descriptionTxt;
    [Header("Custom Events")]
    public UnityEvent customEvent;
    public UnityEvent consumeEvent;




    //Collider Trigger
    private void Reset()
    {
        //We use Collider2D incase we ever want to do circle collider etc since they are all type collider
        // trigger has to be true else the item won't let us walk through it
        GetComponent<Collider2D>().isTrigger = true;
        // sets the layer to item layer
        /*gameObject.layer = 11;*/
        
    }

    public void Interact()
    {
        switch (interactType)
        {
            case InteractionType.PickUp:
                Debug.Log("PICK UP");

               

                // Changed to adding the object directly into our new inventory System
                //Singleton might be better but for now using this
                FindObjectOfType<InventorySystem>().PickUp(gameObject);
                FindObjectOfType<SoundManager>().Play("ItemPickup");
                // Disable
                gameObject.SetActive(false);
                SceneLoader.instance.RemoveGameObject(gameObject);
                break;
            case InteractionType.Examine:
                // Call the examine item in the interaction system
                // The examine window is handled by ItemsAndSignsDialogManager.cs
                //FindObjectOfType<InteractionSystem>().ExamineItem(this);
                Debug.Log("EXAMINE");
                break;
            default:
                Debug.Log("NULL ITEM");
                break;
        }

        // Call the custom event(s)
        customEvent.Invoke();
    }

    

}

