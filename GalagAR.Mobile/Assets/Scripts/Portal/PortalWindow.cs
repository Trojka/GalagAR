using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalWindow : MonoBehaviour {

    bool wasCameraInFront;
    bool cameraInOtherWorld;
    bool isCameraColliding;

    bool wasCubeInFront;
    bool cubeInOtherWorld = true;
    bool isCubeColliding;

    public Material[] materials;

    public Material virtualWorlMaterial;
    public Material realWorlCubeMaterial;

    public GameObject cube1;

	// Use this for initialization
	void Start () {
        UpdateStencilMaterials(false);
	}

    void UpdateStencilMaterials(bool fullRender) {
        int compareFunction = fullRender ?(int)CompareFunction.NotEqual : (int)CompareFunction.Equal;
        foreach (var material in materials)
        {
            material.SetInt("_StencilTest", compareFunction);
        }
    }

    void UpdateCubeMaterial(bool virtualWorld) {
        cube1.GetComponent<MeshRenderer>().material = virtualWorld ? virtualWorlMaterial : realWorlCubeMaterial;
    }

    bool IsCameraInFront() {
        Vector3 playerPos = Camera.main.transform.position + Camera.main.transform.forward * Camera.main.nearClipPlane;

        return IsEntityInFront(playerPos);
    }

    bool IsEntityInFront(Vector3 entityPos) {
        Vector3 entityPosToPortal = transform.InverseTransformPoint(entityPos);

        //Debug.Log("IsEntityInFront: entityPos=" + entityPos + ", entityPosToPortal=" + entityPosToPortal);

        return entityPosToPortal.z >= 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: other=" + other.name);

        if(other.name == "Cube1")
        {
            Debug.Log("OnTriggerEnter: is Cube1");

            wasCubeInFront = IsEntityInFront(cube1.transform.position);
            isCubeColliding = true;

            Debug.Log("OnTriggerEnter: wasInFront:" + wasCubeInFront + ", isColliding:" + isCubeColliding);
        }

        if (other.name == "Main Camera")
        {
            Debug.Log("OnTriggerEnter: is MainCamera");

            wasCameraInFront = IsCameraInFront();
            isCameraColliding = true;

            Debug.Log("OnTriggerEnter: wasInFront:" + wasCameraInFront + ", isColliding:" + isCameraColliding);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: other=" + other.name);

        if (other.name == "Cube1")
        {
            Debug.Log("OnTriggerExit: is Cube1");

            isCubeColliding = false;

            Debug.Log("OnTriggerExit: isColliding:" + isCubeColliding);
        }

        if (other.name == "Main Camera")
        {
            Debug.Log("OnTriggerExit: is MainCamera");

            isCameraColliding = false;

            Debug.Log("OnTriggerExit: isColliding:" + isCameraColliding);
        }
}

    void SomethingColliding() {

        if(isCubeColliding)
        {
            bool isInFront = IsEntityInFront(cube1.transform.position);
            if ((isInFront && !wasCubeInFront) || (wasCubeInFront && !isInFront))
            {
                cubeInOtherWorld = !cubeInOtherWorld;
                UpdateCubeMaterial(cubeInOtherWorld);
            }

            wasCubeInFront = isInFront;
        }

        if(isCameraColliding)
        {
            bool isInFront = IsCameraInFront();
            if ((isInFront && !wasCameraInFront) || (wasCameraInFront && !isInFront))
            {
                cameraInOtherWorld = !cameraInOtherWorld;
                UpdateStencilMaterials(cameraInOtherWorld);
            }

            wasCameraInFront = isInFront;
        }
    }

    void OnDestroy()
    {
        UpdateStencilMaterials(true);
    }

    // Update is called once per frame
    void Update () {
        SomethingColliding();
	}
}
