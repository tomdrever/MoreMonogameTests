using System;

namespace Pathfinding.Maps
{
    public class RandomMapBuilder : IMapBuilder
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _seed;

        public RandomMapBuilder(int width, int height, int? seed)
        {
            _width = width;
            _height = height;
            _seed = seed ?? DateTime.Now.Second + DateTime.Now.Millisecond;
        }

        public int[,] GetMap()
        {
            var map = new int[_height, _width];
            var random = new Random(_seed);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    map[y, x] = random.Next(0, 2);
                }
            }

            return map;
        }
    }
}