//Planning a* script

//in CharacterManagerScript:
//--check when a player ends their turn:
//if the next character in line is an enemy, pass their gameobject to doEnemyTurn()
//doEnemyTurn, use this script to return what they do (coordinates, ability)
//and use CharacterScript tied to enemy object, back in player manager to perform actions
//
//an aside, when an action takes place, i think the camera should zoom in... would be good.
/// <summary>
// EnemyAI script:
// INPUT:
// Enemy game object, including stats and abilities
// OUTPUT: none
// RESULT:
// --calculates enemy movement
// --moves enemy
// --enemy will attack/uses abilities
/// </summary>
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathing : MonoBehaviour
{
    class pathNode
    {
        float _g, _h;
        public float literalCost { get { return _g; } set { _g = value; } } //g = cost of parent + cost of coming into this node
        public float estimatedCost { get { return _h; } set { _h = value; } } //h = heuristic, using euclidean distance 
        public float calculatedCost { get { return literalCost + estimatedCost; } } //f = total estimated cost

        public pathNode ParentNode { get; set; }
        public Vector3Int Position { get; set; }
        public pathNode(Vector3Int currentPos, Vector3Int goalPos)
        {
            estimatedCost = euclideanDist(currentPos, goalPos);
            Position = currentPos;
        }
        public bool Equals(pathNode other)
        {
            if (other == null) return false;
            return (this.Position.Equals(other.Position));
        }
    }

    //euclideana Distance for calculating heuristic
    static float euclideanDist(Vector3Int start, Vector3Int end)
    {
        return Mathf.Sqrt((start.x - end.x) * (start.x - end.x) + (start.y - end.y) * (start.y - end.y));
    }

    //assign these
    public Tilemap map { get; set; }
    public Dictionary<TileBase, TileData> mapData { get; set; }


    public static IEnumerator Run<T>(IEnumerator target, System.Action<T> output)
    {
        object result = null;
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
        output((T)result);
    }
    //given a Vector3Int for a start position and Vector3Int for goal position,
    //return a list of vectors that draws a path to the goal from the start using A*
    public List<Vector3Int> findPath(Vector3Int start, Vector3Int goalPosition)
    {
        // Initialize both open and closed list
        List<pathNode> openList = new List<pathNode>();
        List<pathNode> closedList = new List<pathNode>();

        // Add the start node
        pathNode currentNode = new pathNode(start, goalPosition);
        openList.Add(currentNode);

        while (openList.Count > 0)
        {
            //Get the current node
            //which is the node in openList with least f value
            currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
                if (openList[i].calculatedCost < currentNode.calculatedCost)
                    currentNode = openList[i];

            openList.RemoveAll(node => node.Position == currentNode.Position);

            //if the current node is the goal, finish and return path!
            if (currentNode.Position == goalPosition)
                return reconstructPath(currentNode);

            //generate the children/adjacent nodes
            List<pathNode> adjNodes = new List<pathNode>();
            for (int i = 0; i < 4; i++)
            {
                int x; int y;
                x = 0; y = -1;
                switch (i)
                {
                    case 0: x = 0; y = -1; break;
                    case 1: x = -1; y = 0; break;
                    case 2: x = 0; y = 1; break;
                    case 3: x = 1; y = 0; break;
                }

                //make sure that this neighbor tile is valid (has tile, walkable)
                Vector3Int adjPosition = new Vector3Int(currentNode.Position.x + x, currentNode.Position.y + y, 0);

                if (map.GetTile(adjPosition) == null)
                    continue;
                TileBase adjTile = map.GetTile(adjPosition);
                if (!mapData[adjTile].isWalkable)
                    continue;
                pathNode adjNode = new pathNode(adjPosition, goalPosition);
                //set child's parent to current node
                adjNode.ParentNode = currentNode;
                adjNode.literalCost = mapData[adjTile].movementCost + currentNode.literalCost;
                adjNodes.Add(adjNode);
            }

            foreach(pathNode adjNode in adjNodes)
            {
                //if adjNode is in closed, continue
                if (closedList.Any(node => node.Position == adjNode.Position))
                    continue;
                //create f, g, and h values of child

                //if child is already in openlist
                if (openList.Any(node => node.Position == adjNode.Position))
                {
                    pathNode existingAdj = openList.First(node => node.Position == adjNode.Position);
                        if (adjNode.literalCost > existingAdj.literalCost)
                            continue;
                }

                //add child to openList if the cost of this is less than existing node at the same position in open
                //OR if this node is not in closed and not in open
                openList.Add(adjNode);

            }
        }

            return null;
    }
    //returns list of coordinates in order of minimum path
    List<Vector3Int> reconstructPath(pathNode currentNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        while (currentNode != null)
        {

            path.Insert(0, currentNode.Position);
            currentNode = currentNode.ParentNode;
        }

        return path;
    }
}


