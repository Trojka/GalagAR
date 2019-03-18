using System.Collections;
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

    [Range(10, 200)]
    public float explosionForce = 10;

    [Range(0, 100)]
    public float explosionRadius = 10;

    [Range(0, 5)]
    public float fractureMass = 1;

    public bool useGravityOnExplosion;

    public Transform explosionCenter;

	// Use this for initialization
	void Start () {
        _sites = new List<Point>();
        _siteObjects = new List<GameObject>();
        _voronoi = new FortuneVoronoi();
        _bounds = new BoundingRect() { 
            xmin = this.transform.position.x - this.transform.localScale.x/2, 
            ymin = this.transform.position.y - this.transform.localScale.y / 2, 
            xmax = this.transform.position.x + this.transform.localScale.x / 2, 
            ymax = this.transform.position.y + this.transform.localScale.y / 2 };

        Debug.Log("The Bounds: " + _bounds.ToString());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _result = null;
            CreateSites();
            CalculateVoronoi();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fracture();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
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

        float[] bands = new[] { 5f, 15f, 30f };
        int[] bandSiteCount = new[] { 15, 5, 2 };

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

    void Reset()
    {
        if (_siteObjects != null)
        {
            Debug.Log("destroying all siteObjects");
            foreach (var siteObject in _siteObjects)
                Destroy(siteObject);

            _siteObjects.Clear();
        }
        else
        {
            _siteObjects = new List<GameObject>();
        }
    }

    void Fracture()
    {
        float zfront = this.transform.position.z;
        float zback = zfront + 3;
        if (_result != null)
        {
            foreach (var cell in _result.cells)
            {
                var meshPoints = new Vector3[2 * (1 + 2 * cell.halfEdges.Count)];
                var triangles = new int[2 * 3 * cell.halfEdges.Count];

                var site = cell.site;

                var center = new Vector3(site.x, site.y, 0);

                int centerFrontIndex = 0;
                meshPoints[centerFrontIndex] = new Vector3(0, 0, zfront);

                int centerBackIndex = 1 + 2 * cell.halfEdges.Count;
                meshPoints[centerBackIndex] = new Vector3(0, 0, zback);

                int frontMeshCount = 1;
                int backMeshCount = frontMeshCount + 1 + 2 * cell.halfEdges.Count;
                int frontTriangleCount = 0;
                int backTriangleCount = 3 * cell.halfEdges.Count;
                foreach (var halfEdge in cell.halfEdges)
                {
                    var pt1 = halfEdge.GetStartPoint();
                    var pt2 = halfEdge.GetEndPoint();

                    meshPoints[frontMeshCount] = new Vector3(pt1.x - center.x, pt1.y - center.y, zfront);
                    frontMeshCount++;
                    meshPoints[frontMeshCount] = new Vector3(pt2.x - center.x, pt2.y - center.y, zfront);
                    frontMeshCount++;

                    var frontTriangleCorners = new [] { 
                        new Vector4(meshPoints[centerFrontIndex].x, meshPoints[centerFrontIndex].y, meshPoints[centerFrontIndex].z, centerFrontIndex ), 
                        new Vector4(meshPoints[frontMeshCount - 1].x, meshPoints[frontMeshCount - 1].y, meshPoints[frontMeshCount - 1].z, frontMeshCount - 1 ), 
                        new Vector4(meshPoints[frontMeshCount - 2].x, meshPoints[frontMeshCount - 2].y, meshPoints[frontMeshCount - 2].z, frontMeshCount - 2 ) 
                    }.ToList();

                    var frontSorted = Sort.SortClockwise(frontTriangleCorners);

                    triangles[frontTriangleCount] = (int)frontSorted[0].w;
                    frontTriangleCount++;
                    triangles[frontTriangleCount] = (int)frontSorted[1].w;
                    frontTriangleCount++;
                    triangles[frontTriangleCount] = (int)frontSorted[2].w;
                    frontTriangleCount++;


                    meshPoints[backMeshCount] = new Vector3(pt1.x - center.x, pt1.y - center.y, zback);
                    backMeshCount++;
                    meshPoints[backMeshCount] = new Vector3(pt2.x - center.x, pt2.y - center.y, zback);
                    backMeshCount++;

                    var backTriangleCorners = new[] {
                        new Vector4(meshPoints[centerBackIndex].x, meshPoints[centerBackIndex].y, meshPoints[centerBackIndex].z, centerBackIndex ),
                        new Vector4(meshPoints[backMeshCount - 1].x, meshPoints[backMeshCount - 1].y, meshPoints[backMeshCount - 1].z, backMeshCount - 1 ),
                        new Vector4(meshPoints[backMeshCount - 2].x, meshPoints[backMeshCount - 2].y, meshPoints[backMeshCount - 2].z, backMeshCount - 2 )
                    }.ToList();

                    var backSorted = Sort.SortCounterClockwise(backTriangleCorners);

                    triangles[backTriangleCount] = (int)backSorted[0].w;
                    backTriangleCount++;
                    triangles[backTriangleCount] = (int)backSorted[1].w;
                    backTriangleCount++;
                    triangles[backTriangleCount] = (int)backSorted[2].w;
                    backTriangleCount++;

                }


                _siteObjects.Add(CreateCell(center, meshPoints, triangles));
            }
        }
    }

    private GameObject CreateCell(Vector3 center, Vector3[] meshPoints, int[] triangles) {
        var gameCell = new GameObject();
        gameCell.transform.position = center;
        gameCell.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = meshPoints;
        mesh.triangles = triangles;

        gameCell.GetComponent<MeshFilter>().mesh = mesh;

        gameCell.AddComponent<MeshRenderer>();
        gameCell.GetComponent<MeshRenderer>().material.color = Color.green;

        //gameCell.AddComponent<BoxCollider>();
        //gameCell.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
        //gameCell.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);

        gameCell.AddComponent<Rigidbody>();
        gameCell.GetComponent<Rigidbody>().mass = fractureMass;
        gameCell.GetComponent<Rigidbody>().useGravity = false;

        return gameCell;
    }

    void Explode()
    {
        Debug.Log("Explode");

        //int partIndex = (int)(_wallWidth * (1 + _wallHeight) / 2);
        //var part = _wallParts[partIndex];

        //part.GetComponent<MeshRenderer>().material = explosionPartMaterial;

        foreach (var part in _siteObjects)
        {
            var partRigidBody = part.GetComponent<Rigidbody>();
            partRigidBody.useGravity = useGravityOnExplosion;
            partRigidBody.AddExplosionForce(explosionForce, explosionCenter.position, explosionRadius, 0f, ForceMode.Impulse);
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
