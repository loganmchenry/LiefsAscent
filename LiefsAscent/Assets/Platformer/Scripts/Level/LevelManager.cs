using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Made it so that it respawns you either at the demo start or the platformer start,
 * right now would only respawn in demo part, if you didn't pick up sword,
 * will probably add other conditions at another time.
 */




public class LevelManager : MonoBehaviour
{
   
    public void Restart()
    {

        // Restart the Scene entirely (just for debugging)
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        // retreives player info, could be demo start, or normal start
        FindObjectOfType<PlayerStats>().retrievePlayerInfo();
        // re-enable all the lives, hard coded
        FindObjectOfType<PlayerLives>().enableLives();
        FindObjectOfType<Movement>().canMove = true;

    }

    public void RestartProcedGen()
    {

        // Restart the Scene entirely (just for debugging)
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        // retreives player info, could be demo start, or normal start
        FindObjectOfType<PlayerPos_Level>().retrievePlayerInfoLevel();
        // re-enable all the lives, hard coded
        FindObjectOfType<PlayerLives>().enableLives();
        FindObjectOfType<Movement>().canMove = true;

    }


}
