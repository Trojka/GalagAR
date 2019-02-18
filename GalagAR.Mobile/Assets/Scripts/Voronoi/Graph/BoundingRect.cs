using System;

namespace Voronoi
{
    public struct BoundingRect
    {
        public float xmin;
        public float ymin;
        public float xmax;
        public float ymax;

        public override string ToString()
        {
            return base.ToString() + " xmin:" + xmin + ", ymin:" + ymin + ", xmax:" + xmax + ", ymax:" + ymax;
        }
    }
}
