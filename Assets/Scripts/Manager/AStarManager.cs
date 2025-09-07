using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
   public static AStarManager instance;

   public List<Node> AllNodesInTheScene;

    private void Awake()
    {
        instance = this;

        if(AllNodesInTheScene == null) AllNodesInTheScene = new List<Node>();
    }

    private void OnEnable()
    {
        GenerateNodes.OnNodesGenerated += CollectNodesData;
    }
    private void OnDisable()
    {
        GenerateNodes.OnNodesGenerated -= CollectNodesData;
    }


    public List<Node> GeneratePath(Node start, Node End)
    {
        if (start == null || End == null) return new List<Node>();         
      
        //Loop all Nodes through the scene
        foreach(Node n in AllNodesInTheScene)
        {
            n.gScore = float.MaxValue;
            n.cameFrom = null;
            n.hScore = 0f;
        }

        if(start == End)
        {
            return new List<Node> { start };
        }

        //Initialize the Start Node:
        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, End.transform.position);
        //Now we add the Start Node to the OpenSet
        List<Node> OpenSet = new List<Node> { start };

        //Now we can begin with the algorithm
        while(OpenSet.Count > 0)
        {
            int LowestF = default;

            for(int i = 1; i < OpenSet.Count; i++)
            {
                //we will loop through the Openset count and find the node in the list with the lowest F score
                if (OpenSet[i].fScore() < OpenSet[LowestF].fScore())
                {
                    LowestF = i;
                }
            }

            Node CurrentNode = OpenSet[LowestF];
            OpenSet.Remove(CurrentNode);

            if(CurrentNode == End)
            {
                //If Condition satisfied then we have found our optimal path
                List<Node> path = new List<Node>();
                Node temp = CurrentNode;

                while(temp != null)
                {
                    path.Add(temp);
                    if(temp == start)
                    {
                        break;
                    }
                    temp = temp.cameFrom;
                }

                path.Reverse();
                return path;
            }

            foreach(Node.NodeConnection connection in CurrentNode.connections)
            {
                Node connectedNode = connection.targetNode;

                float heldGScore = CurrentNode.gScore + Vector2.Distance(CurrentNode.transform.position, connectedNode.transform.position);

                if(heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = CurrentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, End.transform.position);

                    if(!OpenSet.Contains(connectedNode))
                    {
                        OpenSet.Add(connectedNode);
                    }
                }
            }
        }


        return null;
    }

    public void CollectNodesData()
    {

        AllNodesInTheScene = new List<Node>();

        if (GenerateNodes.instance == null) return;

        if (GenerateNodes.instance.nodeList != null)
        {
            AllNodesInTheScene.AddRange(GenerateNodes.instance.nodeList);
        }
        if (GenerateNodes.instance.EdgeNodes != null)
        {
            foreach (var n in GenerateNodes.instance.EdgeNodes)
                if (!AllNodesInTheScene.Contains(n))
                    AllNodesInTheScene.Add(n);
        }
    }
}
