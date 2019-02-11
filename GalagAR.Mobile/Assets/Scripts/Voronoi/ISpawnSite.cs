using System;
using System.Collections;
using System.Collections.Generic;

namespace Voronoi
{
    interface ISpawnSite
    {
        List<Point> Spawn();
    }
}

