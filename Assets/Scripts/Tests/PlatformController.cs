using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RayCastController
{
    public Vector2 move;
    [SerializeField] LayerMask movingPlatform;

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        Vector2 velocity = move * Time.deltaTime;
        transform.Translate(velocity);
    }

    void MovePassenger(Vector2 velocity)
    {
        float DirectionX = Mathf.Sign(velocity.x);
        float DirectionY = Mathf.Sign(velocity.y);

        //Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            for (int i = 0; i < VerticalRayCastCount; i++)
            {
                Vector2 rayOrigin = (DirectionY == -1) ? rayCastOrigins.BottomLeft : rayCastOrigins.TopLeft;
                rayOrigin += Vector2.right * (VerticalRaySpacing * i);

                //The Collision detection happens here as we are using Ray Cast and finding in which layerMask we are colliding
                RaycastHit2D rayCastHit = Physics2D.Raycast(rayOrigin, Vector2.up * DirectionY, rayLength, movingPlatform);

                if(rayCastHit)
                {

                }
            }
        }
    }
}
