using Microsoft.Xna.Framework;

namespace Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public bool Walkable;
        public Vector2 Position;
        public Node Parent;

        public Color Color;

        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;

        public int HeapIndex { get; set; }

        public Node(bool walkable, Vector2 posiiton)
        {
            Walkable = walkable;
            Position = posiiton;
            Color = Color.White;
        }

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);

            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }
            return -compare;
        }
    }
}