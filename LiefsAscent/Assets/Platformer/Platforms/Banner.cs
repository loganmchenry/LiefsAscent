using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// Reminder to set both the parchment and the text transperacies to 0 before calling this code
// the "banner" is no the top most hierarchy but the one with the parchment image on it

public class Banner : MonoBehaviour
{

    public GameObject banner;

    // how long the banner stays up before fading out
    public float fadeCooldown;


    private Image theImage;
    // serialized so we can move the text into the script and also control it's alpha values
    [SerializeField] private Text theText;
    
    // the color values of the banner and the text
    private Color currentColorBanner;
    private Color currentColorText;

    // if we should fade or not
    private bool shouldFade = false;

    private bool hasFadedOnce = false;

    

    private float 
        fadeStartTime,  // when we first started fading
        timePassed,     // how much time has passed since we first started
        fade,           // numeric value that holds the alpha
        fadeVelocity;   // how fast we fade in/out


    private void Start()
    {
        // get the Image component so we can mess with the alphas
        theImage = banner.GetComponent<Image>();
    }


    private void Update()
    {
        // check to fade in/out
        CheckFade();
        
    }


    private void CheckFade()
    {
        if (shouldFade)
        {
            // tracked the time that has passed since we first triggered the item on the ground
            // this gets reset every time the player passes the trigger on the ground
            timePassed = Time.time - fadeStartTime;

            // once we have faded in and out, don't wanna fade in again. 
            if(hasFadedOnce == false)
            {
               
                // Fades in the banner once the user has triggered the checkpoint on the ground
                Debug.Log("Fading in my dudes");
                FadeIn();
                ApplyColorChanges();
                // basically the cooldown that we set in the Unity editor until we transition to fading out
                if(Time.time >= (fadeStartTime + fadeCooldown))
                {
                    // so we transition to fading out
                    hasFadedOnce = true;

                    // reset the start time to NOW to simulate starting over
                    fadeStartTime = Time.time;
                    
                }
            }
            else
            {
                
                Debug.Log("fading out my dudes..");
                /*Debug.Log(theImage.color.a);*/
                // fades out and applies the changes
                FadeOut();
                ApplyColorChanges();

            }
        }
    }



    /* This gets called by BannerTrigger once the player has
     * collided with an empty game object to begin the fade in
     */
    public void BeginFade(float fadeDuration)
    {
        shouldFade = true;
        fadeStartTime = Time.time;

        fadeVelocity = 1 / fadeDuration;
    }


   
    private void FadeIn()
    {
       
        // distance = speed * time
        fade = fadeVelocity * timePassed;
        /*Debug.Log($"{fade} is the current fade...");*/

        if (fade >= 1)
        {
            // finished fading
            fade = 1;
        }
    }


    private void FadeOut()
    {
        
        fade = 1 - fadeVelocity * timePassed;
        
        if (fade <= 0)
        {
            fade = 0;
        }

    }


    /* Apply the alpha changes to the banner 
     * and to the text based on our fade value
     */
    private void ApplyColorChanges()
    {
        currentColorBanner = theImage.color;
        theImage.color = new Color(currentColorBanner.r, currentColorBanner.g, currentColorBanner.b, fade);
        currentColorText = theText.color;
        theText.color = new Color(currentColorText.r, currentColorText.g, currentColorText.b, fade);
    }

}
