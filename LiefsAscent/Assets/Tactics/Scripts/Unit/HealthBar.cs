using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    private GameObject unit;
    void Start()
    {
        slider.maxValue = transform.parent.transform.parent.gameObject.GetComponent<CharacterScript>().MaxHP;
        slider.value = transform.parent.transform.parent.gameObject.GetComponent<CharacterScript>().CurrHP;
    }
    // Start is called before the first frame update
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
