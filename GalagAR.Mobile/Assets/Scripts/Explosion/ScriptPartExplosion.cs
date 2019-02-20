using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPartExplosion : MonoBehaviour {
    
    [Range(10, 100)]
    public float explosionForce = 100;

    public Transform explosionCenter;

    public bool applyGravityOnExplosion = false;

    List<GameObject> _siteObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateWall();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
        }
	}

    void Explode() {
        foreach (var part in _siteObjects)
        {
            var partRigidBody = part.GetComponent<Rigidbody>();
            if (applyGravityOnExplosion)
                partRigidBody.useGravity = true;

            partRigidBody.AddExplosionForce(explosionForce, explosionCenter.position, 10f, 0f, ForceMode.Impulse);
        }        
    }

    void CreateWall() {
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


        _siteObjects.Add(CreateAtPositionByMovingMesh(new Vector3(-1, 0, 0)));
        _siteObjects.Add(CreateAtPositionByMovingMesh(new Vector3(0, 0, 0)));
        _siteObjects.Add(CreateAtPositionByMovingMesh(new Vector3(1, 0, 0)));
    }

    GameObject CreateAtPosition(Vector3 pos, Vector3[] meshPoints, int[] triangles) {
        var cell = new GameObject();
        cell.transform.position = pos;
        cell.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = meshPoints;
        mesh.triangles = triangles;

        cell.GetComponent<MeshFilter>().mesh = mesh;

        cell.AddComponent<MeshRenderer>();
        cell.GetComponent<MeshRenderer>().material.color = Color.green;

        //cell.AddComponent<BoxCollider>();
        //cell.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
        //cell.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);

        //cell.AddComponent<MeshCollider>();
        //cell.GetComponent<MeshCollider>().sharedMesh = mesh;
        //cell.GetComponent<MeshCollider>().convex = true;
        ////cell.GetComponent<MeshCollider>().size = new Vector3(1, 1, 1);

        cell.AddComponent<Rigidbody>();
        cell.GetComponent<Rigidbody>().mass = 1;
        cell.GetComponent<Rigidbody>().useGravity = false;

        return cell;
    }

    GameObject CreateAtPositionByMovingObject(Vector3 pos)
    {
        Vector3[] meshPoints = CreateMesh();

        int[] triangles = {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6
        };

        return CreateAtPosition(pos, meshPoints, triangles);
    }

    GameObject CreateAtPositionByMovingMesh(Vector3 pos) {
        Vector3[] meshPoints = CreateMeshAtPosition(pos);

        int[] triangles = {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6
        };

        return CreateAtPosition(new Vector3(0, 0, 0), meshPoints, triangles);
    }

    Vector3[] CreateMeshAtPosition(Vector3 pos) {
        Vector3[] meshPoints = {
            new Vector3 (0 + pos.x, 0, 0),
            new Vector3 (1 + pos.x, 0, 0),
            new Vector3 (1 + pos.x, 1, 0),
            new Vector3 (0 + pos.x, 1, 0),
            new Vector3 (0 + pos.x, 1, 1),
            new Vector3 (1 + pos.x, 1, 1),
            new Vector3 (1 + pos.x, 0, 1),
            new Vector3 (0 + pos.x, 0, 1),
        };

        return meshPoints;
    } 

    Vector3[] CreateMesh() {
        Vector3[] meshPoints = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        return meshPoints;
    } 

}
