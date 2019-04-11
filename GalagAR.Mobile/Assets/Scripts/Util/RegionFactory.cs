using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionFactory {

    public static GameObject CreateRegion(Pose center, List<Vector3> boundaries, Material material)
    {
        var centerPosition = center.position;
        // total meshpoints is number of boundary points and the center
        var vertices = new Vector3[boundaries.Count + 1];
        // total number of triangles is
        var triangles = new int[3 * boundaries.Count];

        int vertexCount = 0;
        int triangleCount = 0;

        vertices[vertexCount] = centerPosition;

        foreach(var point in boundaries)
        {
            vertexCount++;
            vertices[vertexCount] = point;

            triangles[triangleCount] = 0; triangleCount++; 
            triangles[triangleCount] = vertexCount; triangleCount++;
            triangles[triangleCount] = ((vertexCount+1) <= boundaries.Count ? (vertexCount + 1) : 1); triangleCount++;

        }

        return CreateCell(centerPosition, vertices, triangles, material);
    }


    private static GameObject CreateCell(Vector3 center, Vector3[] meshPoints, int[] triangles, Material material)
    {
        var gameCell = new GameObject();
        gameCell.transform.position = center;
        gameCell.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = meshPoints;
        mesh.triangles = triangles;

        gameCell.GetComponent<MeshFilter>().mesh = mesh;

        gameCell.AddComponent<MeshRenderer>();
        gameCell.GetComponent<MeshRenderer>().material = material; //.color = Color.green;

        //gameCell.AddComponent<BoxCollider>();
        //gameCell.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
        //gameCell.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);

        //gameCell.AddComponent<Rigidbody>();
        //gameCell.GetComponent<Rigidbody>().mass = fractureMass;
        //gameCell.GetComponent<Rigidbody>().useGravity = false;

        return gameCell;
    }
}
