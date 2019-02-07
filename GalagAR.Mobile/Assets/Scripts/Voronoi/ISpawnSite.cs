using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voronoi
{
    interface ISpawnSite
    {
        List<Point> Spawn();
    }
}

