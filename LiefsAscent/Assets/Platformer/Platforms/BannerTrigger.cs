using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BannerTrigger : MonoBehaviour
{
    
   
    // Gets the banner with the scroll image on it
    public GameObject 
        Nifl,
        Gnipahellir,
        Valhalla;

    private void Reset()
    {
        // make sure the box collider has isTrigger set to true
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // when the player collides with this invisible transform it should fade in the banner
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            Debug.Log($"{name} Triggered");
            


            // depending on which invisible collider we call the different banners with the diff text
            if(name == "ShowBannerTrigger")
            {
                Nifl.GetComponent<Banner>().BeginFade(3f);
            } else if(name == "ShowBannerTriggerSecond")
            {
                Gnipahellir.GetComponent<Banner>().BeginFade(3f);
            } else
            {
                Valhalla.GetComponent<Banner>().BeginFade(3f);
            }
            
        }
    }



    


}
