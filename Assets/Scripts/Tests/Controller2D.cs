using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class Controller2D : RayCastController
{
    [SerializeField] float maxClimbAngle = 60f;
    [SerializeField] float maxDescendAngle = 55f;
    public CollisionsInfo collisionsInfo;



    public override void Start()
    {
        base.Start();
    }

    // ref - any change we do that velocity in this moethod will be changed in the Move() method
    void VerticalCollisions(ref Vector2 velocity)
    {
        float DirectionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

        for (int i = 0; i < VerticalRayCastCount; i++)
        {
            Vector2 rayOrigin = (DirectionY == -1)? rayCastOrigins.BottomLeft : rayCastOrigins.TopLeft;
            rayOrigin += Vector2.right * (VerticalRaySpacing * i + velocity.x);

            //The Collision detection happens here as we are using Ray Cast and finding in which layerMask we are colliding
            RaycastHit2D rayCastHit = Physics2D.Raycast(rayOrigin, Vector2.up * DirectionY, rayLength, CollisionMask);

            // We are drawing vertical rays here
            Debug.DrawRay(rayOrigin, Vector2.up * DirectionY * rayLength, Color.red);

            if(rayCastHit)
            {
                velocity.y = (rayCastHit.distance - SkinWidth) * DirectionY;
                rayLength = rayCastHit.distance;

                if (collisionsInfo.ClimbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionsInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisionsInfo.below = DirectionY == - 1;
                collisionsInfo.above = DirectionY == 1;
            }
        }

        if(collisionsInfo.ClimbingSlope)
        {
            float DirectionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            Vector2 rayOrigin = ((DirectionX == -1) ? rayCastOrigins.BottomLeft: rayCastOrigins.BottomRight) * Vector2.up * velocity.y;
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, CollisionMask);
            if(rayHit)
            {
                float slopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);
                if(slopeAngle != collisionsInfo.SlopeAngle)
                {
                    velocity.x = (rayHit.distance - SkinWidth) * DirectionX;
                    collisionsInfo.SlopeAngle = slopeAngle;
                }
            }
        }
    }

    // ref - any change we do that velocity in this moethod will be changed in the Move() method
    void HorizontalCollisions(ref Vector2 velocity)
    {
        float DirectionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

        for (int i = 0; i < horizontalRayCastCount; i++)
        {
            Vector2 rayOrigin = (DirectionX == -1) ? rayCastOrigins.BottomLeft : rayCastOrigins.BottomRight;
            rayOrigin += Vector2.up * (HorizontalRaySpacing * i);

            RaycastHit2D rayCastHit = Physics2D.Raycast(rayOrigin, Vector2.right * DirectionX, rayLength, CollisionMask);

            //// We are drawing vertical rays here
            Debug.DrawRay(rayOrigin, Vector2.right * DirectionX * rayLength, Color.red);

            if (rayCastHit)
            {
                float SlopeAngle = Vector2.Angle(rayCastHit.normal, Vector2.up);

                if(i == 0 && SlopeAngle <= maxClimbAngle)
                {
                    if(collisionsInfo.descendingSlope)
                    {
                        collisionsInfo.descendingSlope = false;
                        velocity = collisionsInfo.velocityOld;
                    }
                    float distancetoSlopeStart = 0f;
                    if(SlopeAngle != collisionsInfo.OldSlopeAngle )
                    {
                        distancetoSlopeStart = rayCastHit.distance - SkinWidth;
                        velocity.x -= distancetoSlopeStart * DirectionX;
                    }

                    ClimbSlope(ref velocity, SlopeAngle);
                    velocity.x += distancetoSlopeStart * DirectionX;
                }

                if (!collisionsInfo.ClimbingSlope || SlopeAngle > maxClimbAngle)
                {
                    velocity.x = (rayCastHit.distance - SkinWidth) * DirectionX;
                    rayLength = rayCastHit.distance;

                    if(collisionsInfo.ClimbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisionsInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisionsInfo.left = DirectionX == -1;
                    collisionsInfo.right = DirectionX == 1;
                }

                
            }
        }
    }

    // ref - any change we do that velocity in this moethod will be changed in the Move() method
    void ClimbSlope(ref Vector2 velocity, float SlopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float ClimbVelocityY = Mathf.Sin(SlopeAngle * Mathf.Deg2Rad) * moveDistance;
        if(velocity.y <= ClimbVelocityY)
        {
            velocity.y = ClimbVelocityY; 
            velocity.x = Mathf.Cos(SlopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisionsInfo.below = true;
            collisionsInfo.ClimbingSlope = true;
            collisionsInfo.SlopeAngle = SlopeAngle;
        }
        
    }

    void DescendSlope(ref Vector2 velocity)
    {
       float Directionx = Mathf.Abs(velocity.x);
       Vector2 rayOrigin = (Directionx == -1)? rayCastOrigins.BottomRight : rayCastOrigins.BottomLeft;
        RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, CollisionMask);
        if(rayHit)
        {
            float SlopeAngle = Vector2.Angle(rayHit.normal, Vector2.up);
            if(SlopeAngle != 0 && SlopeAngle <= maxDescendAngle)
            {
                if(Mathf.Sign(rayHit.normal.x) == Directionx)
                {
                    if(rayHit.distance - SkinWidth <= Mathf.Tan(SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float MoveDistance = Mathf.Abs(velocity.x);
                        float DescendVelocityY = Mathf.Sin(SlopeAngle * Mathf.Deg2Rad) * MoveDistance;
                        velocity.x = Mathf.Cos(SlopeAngle * Mathf.Deg2Rad) * MoveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= DescendVelocityY;

                        collisionsInfo.SlopeAngle = SlopeAngle;
                        collisionsInfo.descendingSlope = true;
                        collisionsInfo.below = true;
                    }
                }
            }
        }
    }

    //ref - The changes that has been done for the velocity in VerticalCollisions/HorizontalCollisions it will be changed here as well 
    public void Move(Vector2 velocity)
    {
        UpdateRayCastOrigins();
        Physics2D.SyncTransforms();
        collisionsInfo.Reset();
        collisionsInfo.velocityOld = velocity;
        if(velocity.y <= 0)
        {
            DescendSlope(ref velocity);
            
        }
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        

        transform.Translate(velocity);
        //transform.position = new Vector2(velocity.x, velocity.y);
    }

    public struct CollisionsInfo
    {
        public bool above, below;
        public bool left, right;
        public bool ClimbingSlope;
        public bool descendingSlope;
        public Vector2 velocityOld;

        public float SlopeAngle, OldSlopeAngle;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            ClimbingSlope = false;
            descendingSlope = false;
            OldSlopeAngle = SlopeAngle;
            SlopeAngle = 0;

        }
    }
}
