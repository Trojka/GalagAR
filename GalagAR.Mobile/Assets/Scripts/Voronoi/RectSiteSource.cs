using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voronoi
{
    class RectSiteSource : ISpawnSite
    {
        public float xmin;
        public float xmax;
        public float ymin;
        public float ymax;
        public int siteCount;

        public List<Point> Spawn()
        {
            var result = new List<Point>();

            if (xmin == xmax || ymin == ymax)
                return result;

            for (int i = 0; i < siteCount; i++)
            {
                float x = Random.Range(xmin, xmax);
                float y = Random.Range(ymin, ymax);
                var site = new Point(x, y);
                result.Add(site);
            }

            return result;
        }
    }
}
