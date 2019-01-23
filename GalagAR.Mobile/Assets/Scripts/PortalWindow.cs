using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalWindow : MonoBehaviour {

    bool wasInFront;
    bool inOtherWorld;

    bool isColliding;

    public Material[] materials;

	// Use this for initialization
	void Start () {
        UpdateMaterials(false);
	}

    void UpdateMaterials(bool fullRender) {
        int compareFunction = fullRender ?(int)CompareFunction.NotEqual : (int)CompareFunction.Equal;
        foreach (var material in materials)
        {
            material.SetInt("_StencilTest", compareFunction);
        }
    }

    bool IsInFront() {
        //return transform.position.z > Camera.main.transform.position.z;

        Vector3 playerPos = Camera.main.transform.position + Camera.main.transform.forward * Camera.main.nearClipPlane;
        Vector3 playerPosToPortal = transform.InverseTransformPoint(playerPos);

        return playerPosToPortal.z >= 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "Main Camera")
            return;

        Debug.Log("OnTriggerEnter");

        wasInFront = IsInFront();
        isColliding = true;

        Debug.Log("OnTriggerEnter wasInFront:" + wasInFront + ", isColliding:" + isColliding);

        //if(IsInFront(other.transform.position))
        //{
        //    UpdateMaterials(false);
        //}
        //else
        //{
        //    UpdateMaterials(true);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name != "Main Camera")
            return;

        isColliding = false;

        Debug.Log("OnTriggerExit isColliding:" + isColliding);
    }

    void CameraColliding() {
        if (!isColliding)
            return;

        bool isInFront = IsInFront();
        if ((isInFront && !wasInFront) || (wasInFront && !isInFront))
        {
            inOtherWorld = !inOtherWorld;
            UpdateMaterials(inOtherWorld);
        }

        wasInFront = isInFront;
    }

    void OnDestroy()
    {
        UpdateMaterials(true);
    }

    // Update is called once per frame
    void Update () {
        CameraColliding();
	}
}
