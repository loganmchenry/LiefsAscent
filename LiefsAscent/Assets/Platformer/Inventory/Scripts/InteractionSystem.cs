using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{

    [Header("Detection Fields")]
    //Detection Point
    public Transform detectionPoint;
    //Detection Radius
    private const float detectionRadius = 0.2f;
    //Detection Layer
    public LayerMask detectionLayer;
    //Cached Trigger Object
    public GameObject detectedObject;

    [Header("Examine Fields")]
    // Examine window object
    public GameObject examineWindow;
    public Image examineImage;
    public Text examineText;
    public bool isExamining;


    // Update is called once per frame
    void Update()
    {
        // if we have have detected the object and pressed I, prints out to console
        if (DetectObject())
        {
            if (InteractionInput())
            {
                detectedObject.GetComponent<Item>().Interact();
            }
        }
        
    }

    // Creates a green sphere on the character to see where the detection happens on items
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }




    // When the player gets near an item they press I to interact with it
    bool InteractionInput()
    {
        if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
        {
            return Input.GetKey(ControlManager.instance.ActionControls["PickUp"]);
        }
        else
        {
            return false;
        }
    }

    
    bool DetectObject()
    {
      
      Collider2D obj =  Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
      if(obj == null)
      {
            detectedObject = null;
            return false;
      }
      else
      {
            detectedObject = obj.gameObject;
            return true;
      }



    }

    

    // Examine item function
    // After pressing "I" on an examinable item shows the item with some description text
    // To close press "I" again
    public void ExamineItem(ItemsAndSignsDialogManager item)
    {
        if (isExamining)
        {
            Debug.Log("Close");
            // Hide the examine window
            examineWindow.SetActive(false);
            // disable the boolean
            isExamining = false;

        } else
        {
            Debug.Log("Examine");
            // Show the item's image in the middle
            // goes outside the script level into the sprite render window in unity to get the sprite
            examineImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
            //Display an Examine Window
            examineWindow.SetActive(true);
            // enable the boolean
            isExamining = true;
        }
        
    }


}
