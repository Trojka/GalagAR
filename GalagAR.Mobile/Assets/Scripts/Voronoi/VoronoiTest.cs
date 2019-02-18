using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voronoi;

public class VoronoiTest : MonoBehaviour {

    FortuneVoronoi _voronoi;
    VoronoiGraph _result;

    BoundingRect _bounds;

    //int numberOfSites = 10;
    List<Point> _sites;

	// Use this for initialization
	void Start () {
        _sites = new List<Point>();
        //_bounds = new BoundingRect(){ xmin = 0, ymin = 0, xmax = 100, ymax = 100 };
        _bounds = new BoundingRect() { xmin = -50, ymin = 0, xmax = 50, ymax = 100 };
        _voronoi = new FortuneVoronoi();

        Debug.Log("The Bounds: " + _bounds.ToString());

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

    List<RectSiteSource> GenerateRectangulatSiteSources() 
    {
        var result = new List<RectSiteSource>();

        float[] bands = new[] { 5f, 15f, 30f };
        int[] bandSiteCount = new[] { 30, 10, 5 };
        //float[] bands = new[] { 0.5f, 1.5f, 3f };
        //int[] bandSiteCount = new[] { 30, 10, 5 };


        float bandMinX = _bounds.xmin;
        float bandMaxX = _bounds.xmax;
        float bandMinY = _bounds.ymin;
        float bandMaxY = _bounds.ymax;
        for (int i = 0; i < bands.Count(); i++)
        {
            var rl = new RectSiteSource()
            {
                xmin = bandMinX,
                xmax = bandMinX + bands[i],
                ymin = _bounds.ymin,
                ymax = _bounds.ymax,
                siteCount = bandSiteCount[i]
            };
            result.Add(rl);

            var rr = new RectSiteSource()
            {
                xmin = bandMaxX - bands[i],
                xmax = bandMaxX,
                ymin = _bounds.ymin,
                ymax = _bounds.ymax,
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
        _result = _voronoi.Compute(_sites, _bounds);
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
