using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyWizardMovement : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] float moveSpeed = 1f;
    public LayerMask groundLayers;
    
    public Transform groundCheck;
    
    // State Variables
    Rigidbody2D myRigidBody;
    bool isFacingLeft;



    // Other
    RaycastHit2D hit;
    Animator anim;
    EnemyManage enemyManage;


    private void Awake()
    {
        // Temporary Fix for Removing Defeated Enemies
        string objectName = gameObject.name;
        enemyManage = FindObjectOfType<EnemyManage>();
        if (objectName.Equals("Wizard Enemy 1") && !enemyManage.enemy1Exist)
        {
            gameObject.SetActive(false);
        }
        else if (objectName.Equals("Wizard Enemy 2") && !enemyManage.enemy2Exist)
        {
            gameObject.SetActive(false);
        }
        else if (objectName.Equals("Wizard Enemy 3") && !enemyManage.enemy3Exist)
        {
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Start off Facing Left
        isFacingLeft = true;
        transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.Raycast(groundCheck.position, -transform.up, 1f, groundLayers);
    }

    private void FixedUpdate()
    {
        if (hit.collider != false)
        {
            if (isFacingLeft)
            {
                myRigidBody.velocity = new Vector2(moveSpeed, myRigidBody.velocity.y);
            }
            else
            {
                myRigidBody.velocity = new Vector2(-moveSpeed, myRigidBody.velocity.y);
            }
        }
        else
        {
            isFacingLeft = !isFacingLeft;
            transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If an enemy comes into contact with the player, the attack animation is played.
        if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetBool("Attack", true);
            myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    // This function is triggered by an Animation Event.
    // Once the attack animation is played out it triggers
    // the next scene to be loaded.
    public void ToBeTriggered()
    {
        FindObjectOfType<Counter>().enemiesHit++;
        
        Debug.Log(gameObject.name);
        // Temporary Fix for Removing Defeated Enemies
        /*string objectName = gameObject.name;
        if (objectName.Equals("Wizard Enemy 1"))
        {
            enemyManage.enemy1Exist = false;
        }
        else if (objectName.Equals("Wizard Enemy 2"))
        {
            enemyManage.enemy2Exist = false;
        }
        else if (objectName.Equals("Wizard Enemy 3"))
        {
            enemyManage.enemy3Exist = false;
        }*/
        
        if (FindObjectOfType<Counter>().enemiesHit == 1)
        {
            SceneLoader.instance.LoadTacticalScene(gameObject);
        }
    }

}
