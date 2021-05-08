using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ItemGen : MonoBehaviour
{
	[SerializeField]private GameObject KeyPrefab;
    [SerializeField]private GameObject Tutorial_Item;
    [SerializeField]private GameObject Potion;


    // Each level has a set number of items to generate (e.g. level one contains at most 2 keys and (exactly) 1 sign)
    [SerializeField] private int numKeys = 0;
    [SerializeField] private int numMessages = 0;
    [SerializeField] private int numPotions = 3;

    private static int keyOdds = 25; // 1:25 chance of getting a key 
    private static int messageOdds = 25;
    private static int potionOdds = 50;

    // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

    // Called by proceduralGeneration once for every grass tile
    public void generate(Vector3 pos)
    {
        // call these functions to determine if an item will be placed
        generateMessage(pos);
        generateKey(pos);
        generatePotion(pos);
        
    }

    // generate a message at pos with odd 1:messageOdds
    void generateMessage(Vector3 pos)
    {
        // for now just generating one tutorial item at the beginning of the level
        if (numMessages > 0 && pos.x > 10) {
            int dice = Random.Range(0,messageOdds);
                if (dice == 13) {
                    Instantiate(Tutorial_Item, pos, Quaternion.identity);
                    numMessages--;
                }
            }        
    

    }

    // generate a key at pos with odd 1:keyOdds
    void generateKey(Vector3 pos)
    {
        // make sure there are keys left and the random number matches
        if (numKeys > 0) {
            int dice = Random.Range(0,keyOdds);
            if (dice == 7) {
                Instantiate(KeyPrefab, pos, Quaternion.identity);
                numKeys--;
            }
        }
        


    }

    // generate a potion at pos with odd 1:potionOdds
    void generatePotion(Vector3 pos)
    {
        // make sure there are keys left and the random number matches
        if (numPotions > 0) {
            int dice = Random.Range(0,potionOdds);
            if (dice == 7) {
                Instantiate(Potion, pos, Quaternion.identity);
                numPotions--;
            }
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
