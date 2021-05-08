using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMaster : MonoBehaviour
{
    public static InventoryMaster instance;

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


    void Awake()
    {
        /*if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        if (instance == null)
        {
            //First run
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // instance is not the same as the one we have, destroy old, reset to new
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }


    }
}
