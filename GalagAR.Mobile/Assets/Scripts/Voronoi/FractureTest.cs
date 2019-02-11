﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voronoi;

public class FractureTest : MonoBehaviour {

    FortuneVoronoi _voronoi;
    VoronoiGraph _result;

    List<Point> _sites;
    List<GameObject> _siteObjects;
    BoundingRect _bounds;

	// Use this for initialization
	void Start () {
        _sites = new List<Point>();
        _voronoi = new FortuneVoronoi();
        _bounds = new BoundingRect() { 
            xmin = this.transform.position.x - this.transform.localScale.x/2, 
            ymin = this.transform.position.y - this.transform.localScale.y / 2, 
            xmax = this.transform.position.x + this.transform.localScale.x / 2, 
            ymax = this.transform.position.y + this.transform.localScale.y / 2 };
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _result = null;
            CreateSites();
            CalculateVoronoi();
            Fracture();
        }
	}

    void CreateSites()
    {
        _sites.Clear();

        foreach (var siteSrc in GenerateRectangulatSiteSources())
        {
            _sites.AddRange(siteSrc.Spawn());
        }
    }

    List<RectSiteSource> GenerateRectangulatSiteSources()
    {
        var result = new List<RectSiteSource>();

        float[] bands = new[] { 0.5f, 1.5f, 3f };
        int[] bandSiteCount = new[] { 30, 10, 5 };

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

    void Fracture()
    {
        if(_siteObjects != null)
        {
            Debug.Log("destroying all siteObjects");
            foreach (var siteObject in _siteObjects)
                Destroy(siteObject);

            _siteObjects.Clear();
        }
        else {
            _siteObjects = new List<GameObject>();
        }

        float z = this.transform.position.z;
        if (_result != null)
        {
            foreach (var cell in _result.cells)
            {
                var meshPoints = new Vector3[1 + 2 * cell.halfEdges.Count];
                var triangles = new int[3 * cell.halfEdges.Count];

                var site = cell.site;

                meshPoints[0] = new Vector3(site.x, site.y, z);

                int meshCount = 1;
                int triangleCount = 0;
                foreach (var halfEdge in cell.halfEdges)
                {
                    var pt1 = halfEdge.GetStartPoint();
                    var pt2 = halfEdge.GetEndPoint();

                    meshPoints[meshCount] = new Vector3(pt1.x, pt1.y, z);
                    meshCount++;
                    meshPoints[meshCount] = new Vector3(pt2.x, pt2.y, z);
                    meshCount++;

                    // clockwise means ordering by x 
                    //  take the first 
                    //      and then order remaining two by y 
                    var triangleCorners = new [] { 
                        new { pt1 = meshPoints[0], i = 0 }, 
                        new { pt1 = meshPoints[meshCount - 1], i = meshCount - 1 }, 
                        new { pt1 = meshPoints[meshCount - 2], i = meshCount - 2 } 
                    }.ToList();

                    var firstCorner = triangleCorners.OrderBy(c => c.pt1.x).First();
                    triangles[triangleCount] = firstCorner.i;
                    triangleCount++;

                    var remainingCorners = triangleCorners.OrderBy(c => c.pt1.x).Skip(1).OrderByDescending(c => c.pt1.y).ToList();
                    foreach(var c in remainingCorners) {
                        triangles[triangleCount] = c.i;
                        triangleCount++;
                    }

                }

                var gameCell = new GameObject();
                gameCell.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();

                mesh.Clear();
                mesh.vertices = meshPoints;
                mesh.triangles = triangles;

                gameCell.GetComponent<MeshFilter>().mesh = mesh;

                gameCell.AddComponent<MeshRenderer>();
                gameCell.GetComponent<MeshRenderer>().material.color = Color.green;

                _siteObjects.Add(gameCell);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var siteSrc in GenerateRectangulatSiteSources())
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(siteSrc.xmin, siteSrc.ymin, 0), new Vector3(siteSrc.xmax, 0, siteSrc.ymax));
        }

        if(_sites != null)
        {
            foreach (var site in _sites)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(site.x, site.y, 0), 0.1f);
            }
        }

        if (_result != null)
        {
            foreach (var cell in _result.cells)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(new Vector3(cell.site.x, cell.site.y, 0), 0.05f);

                foreach (var halfEdge in cell.halfEdges)
                {
                    var pt1 = halfEdge.GetStartPoint();
                    var pt2 = halfEdge.GetEndPoint();

                    Gizmos.DrawLine(
                        new Vector3(pt1.x, pt1.y, 0),
                        new Vector3(pt2.x, pt2.y, 0)
                    );
                }
            }
        }

    }
}
