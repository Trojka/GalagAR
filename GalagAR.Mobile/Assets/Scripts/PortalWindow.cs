using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalWindow : MonoBehaviour {

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

    bool IsInFront(Vector3 otherPosition) {
        return transform.position.z > otherPosition.z;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.name != "Main Camera")
            return;

        if(IsInFront(other.transform.position))
        {
            UpdateMaterials(false);
        }
        else
        {
            UpdateMaterials(true);
        }
    }

    void OnDestroy()
    {
        UpdateMaterials(true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
