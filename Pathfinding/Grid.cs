using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pathfinding
{
    public class Grid : IDrawListener
    {
        private Node[,] _nodes;
        private readonly int _width, _height;

        private static Texture2D _nodeTexture;

        public int MaxSize => _width * _height;

        public float NodeTextureSize => _nodeTexture.Height * NodeTextureScale;

        public float NodeTextureScale { get; set; }

        public Grid(int width, int height, Texture2D nodeTexture)
        {
            _width = width;
            _height = height;
            _nodeTexture = nodeTexture;

            NodeTextureScale = 1;

            CreateGrid(null);
        }

        public Grid(int[,] grid, Texture2D nodeTexture)
        {
            _width = grid.GetLength(1);
            _height = grid.GetLength(0);
            _nodeTexture = nodeTexture;

            NodeTextureScale = 1;

            CreateGrid(grid);
        }

        public Node NodeAt(Vector2 position)
        {
            if (position.X >= 0 && position.X < _width && position.Y >= 0 && position.Y < _height)
                return _nodes[(int) position.X, (int) position.Y];
            return null;
        }

        public List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = (int)node.Position.X + x;
                    int checkY = (int)node.Position.Y + y;

                    if (checkX >= 0 && checkX < _width && checkY >= 0 && checkY < _height)
                    {
                        neighbours.Add(_nodes[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        private void CreateGrid(int[,] grid)
        {
            _nodes = new Node[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var position = new Vector2(x, y);

                    // Presume it is walkable; since if it isn't then it will be overwritten later
                    if (grid == null)
                    {
                        _nodes[x, y] = new Node(true, position);
                    }
                    else
                    {
                        _nodes[x, y] = new Node(grid[y, x] < 6 && grid[y, x] > 1, position);

                        var color = Color.White;

                        switch (grid[y, x])
                        {
                            case 0:
                                color = Color.Blue;
                                break;
                            case 1:
                                color = Color.CornflowerBlue;
                                break;
                            case 2:
                                color = Color.SandyBrown;
                                break;
                            case 3:
                                color = Color.Lime;
                                break;
                            case 4:
                                color = Color.Green;
                                break;
                            case 5:
                                color = Color.DarkGreen;
                                break;
                            case 6:
                                color = Color.BurlyWood;
                                break;
                            case 7:
                                color = Color.Gray;
                                break;
                            case 8:
                                color = Color.DarkGray;
                                break;
                        }

                        _nodes[x, y].Color = color;
                    }
                }
            }
        }

        public void AddObstacle(int x, int y, int width, int height)
        {
            for (int xI = x; xI < x + width; xI++)
            {
                for (int yI = y; yI < y + height; yI++)
                {
                    _nodes[xI, yI].Walkable = false;
                    _nodes[xI, yI].Color = Color.Red;
                }
            }
        }

        public List<Node> CurrentPath;
        public void HandleDraw(SpriteBatch spriteBatch)
        {
            foreach (var node in _nodes)
            {
                Color? overrideColour = null;

                if (CurrentPath != null && CurrentPath.Contains(node)) overrideColour = Color.Red;

                spriteBatch.Draw(_nodeTexture, new Vector2(
                    node.Position.X * NodeTextureSize, node.Position.Y * NodeTextureSize), scale:new Vector2(NodeTextureScale), color: overrideColour ?? node.Color);
            }
        }
    }
}