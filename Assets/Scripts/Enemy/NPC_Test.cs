using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class NPC_Test : MonoBehaviour, IHittable
{
    [Header("References")]
    //Lets Store the initialize currentNode and create a list for path
    [SerializeField] Node currentNode;
    [SerializeField] Transform player;
    [SerializeField] Transform Sprite;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D NPCcollider;
    [SerializeField] List<Node> AllNodesinTheScene = new List<Node>();

    //[SerializeField] Animator ChaserAnimator;
    [SerializeField] Vector3 FacingDirection;
    //[SerializeField] bool isFacingRight = true;
    [SerializeField] LayerMask platformLayer;
 

    [Header("Movement / Pathing")]
    [SerializeField] int Movespeed = 5;
    [SerializeField] float pathCheckInterval = 0.5f;

    [Header("Debug")]
    [SerializeField] bool debugLogs = false;
    [SerializeField] List<Node> path = new List<Node>();
    bool isGrounded;

    [Header("NPC health")]
    [SerializeField] int NPCHealth = 100;

    private void Start()
    {
        //RigidBody
        rb = GetComponent<Rigidbody2D>();
        NPCcollider = GetComponent<Collider2D>();
        Sprite = GetComponentInChildren<Transform>();

        if(AStarManager.instance != null)
        {
            AllNodesinTheScene = AStarManager.instance.AllNodesInTheScene;
        }
        Debug.Log("All Nodes"+AllNodesinTheScene.Count);

        currentNode = GetNearestNode(transform.position);

        StartCoroutine(PathUpdater());

        FacingDirection = transform.localScale;

        
    }

    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Platform"));
    }

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    private void LateUpdate()
    {
        Sprite.transform.rotation = Quaternion.identity;    
    }

    void IHittable.RecieveHit(RaycastHit2D RayHit)
    {
        Debug.Log("Got Hit: by Circle");
        NPCHealth -= 10;
        Debug.Log(NPCHealth);

        if (NPCHealth == 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator PathUpdater()
    {
        var wait = new WaitForSeconds(pathCheckInterval);

        while (true)
        {
            Node playerNode = GetNearestNode(player.position);

            //if (playerNode != null)
            //{
            //    if(debugLogs) Debug.Log($"PlayerPosition: {player.position} | Nearest node: {playerNode.name} at {playerNode.transform.position}");
            //}

            if (currentNode != null && playerNode != null)
            {
               // Debug.Log(currentNode+"_-_"+playerNode);
                 var newPath = AStarManager.instance.GeneratePath(currentNode, playerNode);
                if (newPath != null && newPath.Count > 0)
                {
                    path.Clear();
                    path.AddRange(newPath);
                    if (debugLogs) Debug.Log($"PathUpdater: Rebuilt path to predicted node ({path.Count})");
                }
            }
            //If NPC is stuck we are forcing to create a new path
            if(path.Count > 0)
            {
                Node TargetNode = path[0];
                Vector3 targetPos = TargetNode.transform.position;

                bool stuckVertically = Mathf.Abs(transform.position.x - targetPos.x) < 0.5f && targetPos.y > transform.position.y + 0.5f && !isGrounded;

                if(stuckVertically)
                {
                    path = AStarManager.instance.GeneratePath(currentNode, playerNode);
                }

            }

            yield return wait;
        }
    }

    void FollowPlayer()
    {
        if (path.Count == 0 || path == null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }


        Node targetNode = path[0];
        Vector3 targetPos = targetNode.transform.position;

        float extX = 0.5f;
        float extY = 0.5f;

        if (NPCcollider != null)
        {
            //Bounds.extent gives us the half the size of the Object in each axis
            //So if the Object is 2 units in x and 2 units in y it gives 1 and 1 in each axis
            extX = NPCcollider.bounds.extents.x;
            extY = NPCcollider.bounds.extents.y;
        }

        // These vlaue will be acting as box around the nodes that counts as close enough
        // If NPC is 2x2 -> extX =1, extY =1 -> horizThres = 1, VetiThres = 1.2.
        //That means if the NPC is within 1 unit in X and 1.2 units in Y of the Node we will accept it as close enough to consume the node
        float horizThreshold = Mathf.Max(extX * 1.0f, 0.5f); //Minimum sensible x tolerance
        float vertThreshold = Mathf.Max(extY * 1.2f, 0.5f); // allow some Y tolerance

        float direction = Mathf.Sign(targetPos.x - transform.position.x);
        if (Mathf.Approximately(direction, 0f))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else
        {
            //Horizontal Movement
            rb.AddForce(new Vector2(direction * Movespeed, rb.velocity.y));
        }


        //Flip the Character
        if (Mathf.Sign(direction) < 0)
        {
            transform.localScale = FacingDirection;
        }
        else
        {
            transform.localScale = new Vector2(-FacingDirection.x, FacingDirection.y);
        }

        //jump Logic:

        if (isGrounded)
        {
            float dx = targetPos.x - transform.position.x;
            float dy = targetPos.y - transform.position.y;

            float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

            //Clamp minimum jump height to 1 tile
            float minJumpHeight = 3.0f;

            if (dy > 0.2f)
            {

                float jumpHeight = Mathf.Max(dy, minJumpHeight);

                //calculate minimum vertical velocity needed to reach by
                float requiredVy = Mathf.Sqrt(2 * gravity * jumpHeight); //margin 
                float flightTime = (2 * requiredVy) / gravity;// total Flight Time = (2 * vY) / g

                // Horizontal velocity to cover dx in that time
                float requiredVX = dx / flightTime;
                requiredVX = Mathf.Clamp(requiredVX, -Movespeed * 1.5f, Movespeed * 1.5f);

                //Apply Jump with calculated trajectory
                rb.velocity = new Vector2(requiredVX, requiredVy);
            }
            else if(Mathf.Abs(dy) < 0.2f && Mathf.Abs(dx) > 2f) // Jump across gaps
            {
                float jumpHeight = minJumpHeight;

                //calculate minimum vertical velocity needed to reach by
                float requiredVy = Mathf.Sqrt(2 * gravity * jumpHeight); //margin 
                float flightTime = (2 * requiredVy) / gravity;// total Flight Time = (2 * vY) / g

                // Horizontal velocity to cover dx in that time
                float requiredVX = dx / flightTime;
                requiredVX = Mathf.Clamp(requiredVX, -Movespeed * 1.5f, Movespeed * 1.5f);

                //Apply Jump with calculated trajectory
                rb.velocity = new Vector2(requiredVX, requiredVy);
            }
        }
        
        // Near Node check 
        bool closeEnoughX = Mathf.Abs(transform.position.x - targetPos.x) <= horizThreshold;
        bool closeEnoughY = Mathf.Abs(transform.position.y - targetPos.y) <= (vertThreshold * 2f);
        bool closeEnough = closeEnoughX && closeEnoughY;

        if (!isGrounded)
        {
            closeEnough = closeEnoughX;
        }

        bool passedNode = false;

        //If Moving right NPC has gone beyond the node's X position + half of the threshold so we passed it, : same goes for the left side too.
        if (direction > 0)
            passedNode = transform.position.x > (targetPos.x + horizThreshold * 0.5f);
        else if (direction < 0)
            passedNode = transform.position.x < (targetPos.x - horizThreshold * 0.5f);
        else
            passedNode = closeEnough;

        if (closeEnough || passedNode)
        {
            //Consume Node 
            currentNode = targetNode;
            path.RemoveAt(0);
        }


        if (path.Count > 1 && Physics2D.Linecast(transform.position, targetPos, platformLayer))
        {
            Node nextNode = path[1];
            if (Vector2.Distance(transform.position, nextNode.transform.position) < Vector2.Distance(transform.position, targetNode.transform.position))
            {
                //skip ahead
                path.RemoveAt(0);
                targetNode = nextNode;
            }
        }
    }

    Node GetNearestNode(Vector3 pos)
    {
        Node nearestNode = null;

        float shortest = float.MaxValue;

        foreach (var node in AllNodesinTheScene)
        {
            float distance = (pos - node.transform.position).sqrMagnitude;

            if (distance < shortest)
            {
                shortest = distance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.black;
        Vector3 prev = transform.position;
        foreach (var n in path)
        {
            Gizmos.DrawLine(prev, n.transform.position);
            prev = n.transform.position;
        }
    }

}
