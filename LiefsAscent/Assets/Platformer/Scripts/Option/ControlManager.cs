using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;

public class ControlManager : MonoBehaviour
{
    private CanvasGroup controlsMenu;
    private string bindName;
    private GameObject[] controlButtons;

    // Dictionaries for holding keybinds
    public Dictionary<string, KeyCode> MoveControls { get; private set; }
    public Dictionary<string, KeyCode> ActionControls { get; private set; }

    public static ControlManager instance; // Singleton
    public bool inputsActive = true;

    public void Awake()
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
        controlButtons = GameObject.FindGameObjectsWithTag("KeyBind");
    }

    public void Start()
    {
        controlsMenu = GetComponent<CanvasGroup>();
        MoveControls = new Dictionary<string, KeyCode>();
        ActionControls = new Dictionary<string, KeyCode>();
        SetInitialBinds();
    }

    private void SetInitialBinds()
    {
        // Movement
        BindKey("Jump", KeyCode.Z);
        BindKey("Left", KeyCode.LeftArrow);
        BindKey("Right", KeyCode.RightArrow);
        BindKey("Dash", KeyCode.C);

        // Actions
        BindKey("Inventory", KeyCode.I);
        BindKey("PickUp", KeyCode.UpArrow);
        BindKey("Interact", KeyCode.UpArrow);
        BindKey("Attack", KeyCode.X);

    }

    public void OpenControls()
    {
        controlsMenu.alpha = controlsMenu.alpha > 0 ? 0 : 1;
        controlsMenu.blocksRaycasts = controlsMenu.blocksRaycasts == true ? false : true;
        controlsMenu.interactable = controlsMenu.interactable == true ? false : true;
        Time.timeScale = Time.timeScale > 0 ? 0 : 1; // Freeze Game
        inputsActive = inputsActive == true ? false : true;
    }

    private void BindKey(string key, KeyCode keybind)
    {
        Dictionary<string, KeyCode> currentDictionary;
        if (key.Equals("Jump") || key.Equals("Left") || key.Equals("Right") || key.Equals("Dash"))
        {
            currentDictionary = MoveControls;
        }
        else
        {
            currentDictionary = ActionControls;
        }

        // Most of the time, keys CANNOT be reused -- each case where it CAN happen 
        // must be designated explicitly.
        if (!currentDictionary.ContainsKey(key))
        {
            currentDictionary.Add(key, keybind);
            UpdateKeyText(key, keybind);
        }
        else if (currentDictionary.ContainsValue(keybind))
        {
            string myKey = currentDictionary.FirstOrDefault(x => x.Value == keybind).Key; // Grab the Key that contains the keycode we just tried to assign
            currentDictionary[myKey] = KeyCode.None;
            UpdateKeyText(key, KeyCode.None);
        }

        // Bind Key
        currentDictionary[key] = keybind;
        UpdateKeyText(key, keybind);
        bindName = string.Empty;
    }

    public void KeyBindOnClick(string bindName)
    {
        this.bindName = bindName;
    }

    private void OnGUI()
    {
        // If we are in the middle of binding a key
        if (bindName != string.Empty)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                BindKey(bindName, e.keyCode);
            }
        }
    }

    public void UpdateKeyText(string key, KeyCode code)
    {
        // Grab the Text of the Key we just set 
        TextMeshProUGUI temp = Array.Find(controlButtons, x => x.name == key).GetComponentInChildren<TextMeshProUGUI>();
        string result = code.ToString();
        result = result.Replace("Arrow", "");
        temp.text = result;
    }
    /*
    // Temporary - Until Options Menu Enabled
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenControls();
        }
    }*/
}
