using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RayCastController : MonoBehaviour
{
    public const float SkinWidth = .015f;
    public int horizontalRayCastCount = 4;
    public int VerticalRayCastCount = 4;

    public float HorizontalRaySpacing;
    public float VerticalRaySpacing;

    public new BoxCollider2D collider;
    public RayCastOrigins rayCastOrigins;
    [SerializeField] public LayerMask CollisionMask;

    public virtual void Start()
    {

        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();//We can calculate the spacing btw the Rays at the start of the frame (Doesnt have to be done continuously - In Update)

    }

    public void UpdateRayCastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(SkinWidth * -2); // making sure the ray is not cast from all the way from bottom or top we are multiplying skinwidth(.015) * -2 so that the ray is casted from a little inside from the object

        // Here we are setting up all the coordinates where the rays will be casted - So as mentioned, from the bottom left & right and Top left & right the rays will be casted outwards 
        rayCastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayCastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayCastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayCastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(SkinWidth * -2);

        //Here we are setting the min and max count of rays should be casted 
        horizontalRayCastCount = Mathf.Clamp(horizontalRayCastCount, 2, int.MaxValue);
        VerticalRayCastCount = Mathf.Clamp(VerticalRayCastCount, 2, int.MaxValue);

        // here we are calculating the space between each rays while casted 
        HorizontalRaySpacing = bounds.size.y / (horizontalRayCastCount - 1);
        VerticalRaySpacing = bounds.size.y / (VerticalRayCastCount - 1);


    }

    public struct RayCastOrigins
    {
        public Vector2 BottomLeft, BottomRight;
        public Vector2 TopLeft, TopRight;
    }

}
