using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [Header("Alpha Variables")]
    [SerializeField]
    // keep track how long it's been active
    private float activeTime = 0.1f;
    private float timeActivated;
    // what the alpha currently is
    private float alpha;
    [SerializeField]
    // what we set the alpha to
    private float alphaSet = 0.8f;
    // the smaller this number the faster the sprite will fade
    private float alphaMultiplier = 0.85f;

    [Header("Player Variables")]
    //we need player game object to get it's position
    private Transform player;
    // we need reference to the sprite renderer of the afterImage
    private SpriteRenderer SR;
    // SR of the player to get current sprite
    private SpriteRenderer playerSR;

    // we need to change the alpha so we need color
    private Color color;

    // Similar to start/awake but gets called everytime the object gets enabled
    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        // decreasing the alpha
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        SR.color = color;

        // check if afterimage has been active for long enough
        if(Time.time >= (timeActivated + activeTime))
        {
            // add back to the pool
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }

}
