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

        float[] bands = new[] { 5f, 15f, 30f };
        int[] bandSize = new[] { 30, 10, 5 };

        float bandMinX = bounds.min.x;
        float bandMaxX = bounds.max.x;
        float bandMinY = bounds.min.z;
        float bandMaxY = bounds.max.z;
        for (int i = 0; i < bands.Count(); i++)
        {
            for (int j = 0; j < bandSize[i]; j++)
            {                    
                float xl = Random.Range(bandMinX, bandMinX + bands[i]);
                float yl = Random.Range(bounds.min.z, bounds.max.z);
                Point sitel = new Point(xl, yl);

                float xr = Random.Range(bandMaxX - bands[i], bandMaxX);
                float yr = Random.Range(bounds.min.z, bounds.max.z);
                Point siter = new Point(xr, yr);

                //float xl = Random.Range(bandMinX, bandMinX + bands[i]);
                //float yl = Random.Range(bandMinY, bandMinY + bands[i]);
                //Point site = new Point(xl, yl);

                //float xl = Random.Range(bandMinX, bandMinX + bands[i]);
                //float yl = Random.Range(bandMinY, bandMinY + bands[i]);
                //Point site = new Point(xl, yl);

                _sites.Add(sitel);
                _sites.Add(siter);
            }
        }
    }

    void CalculateVoronoi()
    {
        _result = _voronoi.Compute(_sites, bounds);
    }

    private void OnDrawGizmos()
    {
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
