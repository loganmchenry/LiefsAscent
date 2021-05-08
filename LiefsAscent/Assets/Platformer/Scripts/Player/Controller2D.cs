using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public LayerMask collisionMask; //allows selection of Layer in inspector to check for collisions... It is not set to default 0
    const float skinWidth = 0.015f; // allows rays to be drawn x amount of pixels inwards so when resting on ground they can still detect
    public int horizontalRaycount = 4;
    public int verticalRaycount = 4;
    float max_angle = 80 ; //cant climb surfaces with z > 80

    float horizontalRaySpacing;
    float verticalRaySpacing;

  	BoxCollider2D bcollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

  
    private void Start() {
        bcollider = GetComponent<BoxCollider2D> ();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity){
        UpdateRaycast();
        collisions.Reset();

        if(velocity.x != 0){
            FindObjectOfType<SoundManager>().Play("PlayerWalk");
            HorizontalCollisions(ref velocity);
        }
        else if (velocity.x ==0){FindObjectOfType<SoundManager>().Stop("PlayerWalk");}
        if(velocity.y !=0){
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);
    }
void HorizontalCollisions(ref Vector3 velocity){ //
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for(int i =0; i <horizontalRaycount; i++){
            Vector2 rayOrigin = (directionX == -1)?raycastOrigins.botleft:raycastOrigins.botright;
            rayOrigin += Vector2.up * (horizontalRaySpacing*i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX,rayLength, collisionMask);

            //turn on Gizmos to watch rays during Gameplay
            Debug.DrawRay(rayOrigin ,Vector2.right * rayLength * directionX, Color.red);
            
            if(hit){
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if(i ==0 && slopeAngle <= max_angle){
                    float distanceToSlope = 0; //variable to prevent raycast to allow player to climb slope before actually touching slope

                    if (slopeAngle != collisions.slopeAngleOld){ //starting to climb new slope
                        distanceToSlope = hit.distance - skinWidth;
                        velocity.x -= distanceToSlope * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlope * directionX;
                }

                if(!collisions.climbslope || slopeAngle > max_angle){
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;


                    //tan(θ) = velocity.y / velocity.x
                    // velocity.y = velocity.x * tan(θ)
                    if(collisions.climbslope){ 
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad ) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1; //if we hit something and going left; set to true
                    collisions.right = directionX == 1;
                }
            }
        
        }
    }
    void VerticalCollisions(ref Vector3 velocity){ //
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for(int i =0; i <verticalRaycount; i++){
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.botleft:raycastOrigins.topleft;
            rayOrigin += Vector2.right * (verticalRaySpacing*i+velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY,rayLength, collisionMask);
            
            //turn on Gizmos to watch rays during Gameplay
            Debug.DrawRay(rayOrigin ,Vector2.up * rayLength * directionY, Color.red);
            
            if(hit){
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                //tan(θ) = velocity.y / velocity.x
                // velocity.x = velocity.y / tan(θ)
                if(collisions.climbslope){
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
                
            }
        
        }
    }


/*
*  sin(θ) = y / d  <===> y = d * sin(θ)
*
*  cos(θ) = x / d  <===> x = d * cos(θ)
*/
    void ClimbSlope(ref Vector3 velocity, float slopeAngle){
        float moveDistance = Mathf.Abs(velocity.x); //total distance up slope (distance cannot be neg)
        float climbVelocityY =  Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance; 
        
        if (velocity.y <= climbVelocityY){
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true; 
            collisions.climbslope = true;
            collisions.slopeAngle = slopeAngle;
        }

    }


	void UpdateRaycast(){
        Bounds bounds = bcollider.bounds;
        bounds.Expand (skinWidth *-2);

        //Extremeties
        raycastOrigins.botleft = new Vector2(bounds.min.x,bounds.min.y);
        raycastOrigins.botright = new Vector2(bounds.max.x,bounds.min.y);
        raycastOrigins.topleft = new Vector2(bounds.min.x,bounds.max.y);
        raycastOrigins.topright = new Vector2(bounds.max.x,bounds.max.y);
    }

    /// Clamp func.  https://docs.unity3d.com/ScriptReference/Mathf.Clamp.html
    /// <returns>
    /// value if min ≤ value ≤ max.
    /// -or-
    /// min if value < min.
    /// -or-
    /// max if max < value.
    /// </returns>
    void CalculateRaySpacing(){
        Bounds bounds = bcollider.bounds;
        bounds.Expand (skinWidth *-2);

        
        horizontalRaycount = Mathf.Clamp(horizontalRaycount,2,int.MaxValue);
        verticalRaycount = Mathf.Clamp(verticalRaycount,2,int.MaxValue);
    
        horizontalRaySpacing  = bounds.size.y / (horizontalRaycount-1);
        verticalRaySpacing  = bounds.size.x / (verticalRaycount-1);
    }

    

    struct RaycastOrigins{
        public Vector2 topleft, topright;
        public Vector2 botleft, botright;     
    }

    //shows collision direction
    public struct CollisionInfo{
        public bool above, below;
        public bool left, right;
        public float slopeAngle, slopeAngleOld;
        public bool climbslope;

        public void Reset(){
            above = below = false;
            left = right = false;
            climbslope = false;
            slopeAngleOld= slopeAngle;
            slopeAngle =0;
        }
    }

}
