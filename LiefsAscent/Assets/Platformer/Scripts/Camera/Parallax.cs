using UnityEngine;


public class Parallax : MonoBehaviour
{
    private float length, startpos, ypos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }


    void Update()
    {
        float temp = (cam.transform.position.x * (1- parallaxEffect));
        float dist = (cam.transform.position.x*parallaxEffect);
        float ydist = (cam.transform.position.y*parallaxEffect);
        transform.position = new Vector3(startpos+dist, ypos+ydist, transform.position.z);
    
        if (temp > startpos + length ) startpos += length;
        else if (temp < startpos - length ) startpos -= length;
    }
}
