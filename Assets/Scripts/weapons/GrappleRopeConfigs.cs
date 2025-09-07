using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRopeConfigs : MonoBehaviour
{
    [Header("General References:")]
     [SerializeField]GrapplingGunConfig grapplingGun;
    [SerializeField] LineRenderer _lineRenderer;

    [Header("General Settings")]
    [SerializeField] int precision = 40; // precision is nothing but the perfection of the rope, not only for perfection this will be assigned to positionCount of the line Renderer where we will be drawing the line with 40 points so that we can get the curve animation
    [Range(0, 20)][SerializeField] float straightenLineSpeed = 5;

    [Header("Rope Animation Settings")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)][SerializeField] float StartWaveSize = 2;
    float WaveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [Range(1, 50)][SerializeField] float ropeProgressionSpeed = 1;
    
    float moveTime = 0;
    public bool isgrappling = true;
    bool StraightLine = true;




    private void Awake()
    {
        grapplingGun = GetComponentInParent<GrapplingGunConfig>();
        _lineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        moveTime = 0;
        _lineRenderer.positionCount = precision;
        WaveSize = StartWaveSize;
        StraightLine = false;

        linePointsToFirePoint();

        _lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        _lineRenderer.enabled = false;
        isgrappling = false;
    }

    
    void linePointsToFirePoint()
    {
        for(int i = 0; i < precision; i++)
        {
            _lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }


    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (_lineRenderer.positionCount != precision)
        {
            _lineRenderer.positionCount = precision;
        }
            
        Vector2 lineEnd = _lineRenderer.GetPosition(precision - 1);
        Vector2 targetpoint = grapplingGun.grapplePoint;

        if (!StraightLine)
        {
            //Debug.Log("still not a straight line");
            // We are checking that if the linerenderer(line) has reached to grapple point, if reached then the line becomes straight, until it reaches, the line will be drawn curvy
            
        if (Vector2.Distance(lineEnd, targetpoint) <= 0.05f)
        {   
            //Debug.Log(" straight line");
            StraightLine = true;
        }
        else
        {
            DrawRopeWaves();
        }
        }
        else
        {
            if(!isgrappling)
            {
                //Debug.Log("is not grappling");
                grapplingGun.GrappleConfigs();
                isgrappling = true;
            }
            if(WaveSize > 0)
            {
                // We are checking if the Wave Size greater than zero if yes then we are making the curvy line to straight line as time passes and multiply with straightlinespeed (how fast the line has to become straight)
                WaveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                WaveSize = 0;
                if(_lineRenderer.positionCount != 2)
                {
                    _lineRenderer.positionCount = 2;
                }
                DrawRopeNoWaves();
            }
        }
    }

    void DrawRopeWaves()
    {
        for(int i = 0; i < precision; i++)
        {
            //delta varaible determines what part of the distance from firePoint to grapple point at the point should pass 
            float delta = (float)i / ((float)precision - 1f);
            Vector2 offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * WaveSize;
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            _lineRenderer.SetPosition(i, currentPosition);
        }
    }

    void DrawRopeNoWaves()
    {
        _lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        _lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }

}
