using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pathfinding
{
    public class Pathfinder
    {
        private readonly Grid _grid;

        public Pathfinder(Grid grid)
        {
            _grid = grid;
        }

        public void FindPath(Vector2 startPosition, Vector2 targetPosition)
        {
            var startNode = _grid.NodeAt(startPosition);
            var targetNode = _grid.NodeAt(targetPosition);

            var openSet = new Heap<Node>(_grid.MaxSize);
            var closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    _grid.CurrentPath = RetracePath(startNode, targetNode);

                    return;
                }

                foreach (var neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour)) continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        // Recalculate the fCost
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);

                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.Update(neighbour);
                    }
                }
            }
        }

        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);

                currentNode = currentNode.Parent;
            }

            path.Add(startNode);

            path.Reverse();

            return path;
        }

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Math.Abs((int) nodeA.Position.X - (int) nodeB.Position.X);
            int yDistance = Math.Abs((int) nodeA.Position.Y - (int) nodeB.Position.Y);

            // 14 - value for moving diag, 10 - straight
            if (xDistance > yDistance) return 14 * yDistance + 10 * (xDistance - yDistance);

            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
    }
}