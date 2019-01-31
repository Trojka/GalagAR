using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voronoi;

public class VoronoiTest : MonoBehaviour {

    FortuneVoronoi _voronoi;
    VoronoiGraph _result;

    Bounds bounds;

    int numberOfSites = 10;
    List<Point> _sites;

	// Use this for initialization
	void Start () {
        _sites = new List<Point>();
        bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(100f, 100f, 100f));
        _voronoi = new FortuneVoronoi();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.C))
        {
            _result = null;
            CreateSites();
            CalculateVoronoi();
        }
	}

    void CreateSites()
    {
        _sites.Clear();

        foreach(var siteSrc in GenerateRectangulatSiteSources())
        {
            _sites.AddRange(siteSrc.Spawn());
        }
    }

    interface ISpawnSite {
        List<Point> Spawn();
    }

    class RectSiteSource : ISpawnSite {
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

    List<RectSiteSource> GenerateRectangulatSiteSources() 
    {
        var result = new List<RectSiteSource>();

        float[] bands = new[] { 5f, 15f, 30f };
        int[] bandSiteCount = new[] { 30, 10, 5 };

        float bandMinX = bounds.min.x;
        float bandMaxX = bounds.max.x;
        float bandMinY = bounds.min.z;
        float bandMaxY = bounds.max.z;
        for (int i = 0; i < bands.Count(); i++)
        {
            var rl = new RectSiteSource()
            {
                xmin = bandMinX,
                xmax = bandMinX + bands[i],
                ymin = bounds.min.z,
                ymax = bounds.max.z,
                siteCount = bandSiteCount[i]
            };
            result.Add(rl);

            var rr = new RectSiteSource()
            {
                xmin = bandMaxX - bands[i],
                xmax = bandMaxX,
                ymin = bounds.min.z,
                ymax = bounds.max.z,
                siteCount = bandSiteCount[i]
            };
            result.Add(rr);

            var rt = new RectSiteSource()
            {
                xmin = bandMinX + bands[i],
                xmax = bandMaxX - bands[i],
                ymin = bandMinY,
                ymax = bandMinY + bands[i],
                siteCount = bandSiteCount[i]
            };
            result.Add(rt);

            var rb = new RectSiteSource()
            {
                xmin = bandMinX + bands[i],
                xmax = bandMaxX - bands[i],
                ymin = bandMaxY - bands[i],
                ymax = bandMaxY,
                siteCount = bandSiteCount[i]
            };
            result.Add(rb);

            bandMinX = bandMinX + bands[i];
            bandMaxX = bandMaxX - bands[i];
            bandMinY = bandMinY + bands[i];
            bandMaxY = bandMaxY - bands[i];
        }

        return result;
    }

    void CalculateVoronoi()
    {
        _result = _voronoi.Compute(_sites, bounds);
    }

    private void OnDrawGizmos()
    {
        foreach (var siteSrc in GenerateRectangulatSiteSources())
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(siteSrc.xmin, 0, siteSrc.ymin), new Vector3(siteSrc.xmax, 0, siteSrc.ymax));
        }

        foreach(var site in _sites)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(site.x, 0, site.y), 0.9f);
        }

        if(_result != null)
        {
            foreach(var cell in _result.cells)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(new Vector3(cell.site.x, 0, cell.site.y), 0.5f);

                foreach(var halfEdge in cell.halfEdges)
                {
                    var pt1 = halfEdge.GetStartPoint();
                    var pt2 = halfEdge.GetEndPoint();

                    Gizmos.DrawLine(
                        new Vector3(pt1.x, 0, pt1.y),
                        new Vector3(pt2.x, 0, pt2.y)
                    );
                }
            }
        }

    }
}
