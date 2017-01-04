using System;
using Microsoft.Xna.Framework;

namespace Pathfinding.Maps
{
    public class DiamondSquareMapBuilder : IMapBuilder
    {
        private int _n;
        private int _widthMultiplyer, _heightMultiplyer;

        private float _smoothness;

        private static int _seed;

        private float _deepWaterThreshold,
            _shallowWaterThreshold,
            _desertThreshold,
            _plainsThreshold,
            _grasslandThreshold,
            _forestThreshold,
            _hillsThreshold,
            _mountainsThreshold;

        public DiamondSquareMapBuilder(int n, int? seed)
        {

            // the thresholds which determine cutoffs for different terrain types
            _deepWaterThreshold = 0.5f;
            _shallowWaterThreshold = 0.55f;
            _desertThreshold = 0.58f;
            _plainsThreshold = 0.62f;
            _grasslandThreshold = 0.7f;
            _forestThreshold = 0.8f;
            _hillsThreshold = 0.88f;
            _mountainsThreshold = 0.95f;

            // n partly controls the size of the map, but mostly controls the level of detail available
            _n = n;

            // _widthMultiplyer and _heightMultiplyer are the width and height multipliers.  They set how separate regions there are
            _widthMultiplyer = 4;
            _heightMultiplyer = 2;

            // Smoothness controls how smooth the resultant terain is.  Higher = more smooth
            _smoothness = 3f;

            _seed = seed ?? DateTime.Now.Second + DateTime.Now.Millisecond;
        }

        public int[,] GetMap()
        {

            // get the dimensions of the map
            int power = (int) Math.Pow(2, _n);
            int width = _widthMultiplyer * power + 1;
            int height = _heightMultiplyer * power + 1;

            // initialize arrays to hold values 
            var map = new double[height, width];
            var returnMap = new int[height, width];


            int step = power / 2;
            double sum;
            int count;

            // h determines the fineness of the scale it is working on.  After every step, h
            // is decreased by a factor of "_smoothness"
            double h = 1;

            // Initialize the grid points
            for (int i = 0; i < width; i += 2 * step)
            {
                for (int j = 0; j < height; j += 2 * step)
                {
                    map[j, i] = RandomDouble(0, 2 * h);
                }
            }

            // Do the rest of the magic
            while (step > 0)
            {
                // Diamond step
                for (int x = step; x < width; x += 2 * step)
                {
                    for (int y = step; y < height; y += 2 * step)
                    {
                        sum = map[y - step, x - step] + //down-left
                           map[y - step, x + step] + //up-left
                           map[y + step, x - step] + //down-right
                           map[y + step, x + step];  //up-right
                        map[y, x] = sum / 4 + RandomDouble(-h, h);
                    }
                }

                // Square step
                for (int x = 0; x < width; x += step)
                {
                    for (int y = step * (1 - (x / step) % 2); y < height; y += 2 * step)
                    {
                        sum = 0;
                        count = 0;
                        if (x - step >= 0)
                        {
                            sum += map[y, x - step];
                            count++;
                        }
                        if (x + step < width)
                        {
                            sum += map[y, x + step];
                            count++;
                        }
                        if (y - step >= 0)
                        {
                            sum += map[y - step, x];
                            count++;
                        }
                        if (y + step < height)
                        {
                            sum += map[y + step, x];
                            count++;
                        }
                        if (count > 0) map[y, x] = sum / count + RandomDouble(-h, h);
                        else map[y, x] = 0;
                    }

                }
                h /= _smoothness;
                step /= 2;
            }

            // Normalize the map
            double max = double.MinValue;
            double min = double.MaxValue;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double d = map[j, i];

                    if (d > max) max = d;
                    if (d < min) min = d;
                }
            }

            // Use the thresholds to fill in the return map
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[j, i] = (map[j, i] - min) / (max - min);
                    if (map[j, i] < _deepWaterThreshold) returnMap[j, i] = 0;
                    else if (map[j, i] < _shallowWaterThreshold) returnMap[j, i] = 1;
                    else if (map[j, i] < _desertThreshold) returnMap[j, i] = 2;
                    else if (map[j, i] < _plainsThreshold) returnMap[j, i] = 3;
                    else if (map[j, i] < _grasslandThreshold) returnMap[j, i] = 4;
                    else if (map[j, i] < _forestThreshold) returnMap[j, i] = 5;
                    else if (map[j, i] < _hillsThreshold) returnMap[j, i] = 6;
                    else if (map[j, i] < _mountainsThreshold) returnMap[j, i] = 7;
                    else returnMap[j, i] = 8;
                }
            }

            return returnMap;
        }

        public static double RandomDouble(double from, double to)
        {
            _seed += DateTime.Now.Millisecond;
            var random = new Random(_seed);
            double diff = Math.Abs(from - to);

            return random.NextDouble() * diff + from;
        }
    }
}