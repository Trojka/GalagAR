using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalWindow : MonoBehaviour {

    public Material[] materials;

	// Use this for initialization
	void Start () {
        foreach (var material in materials)
        {
            material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.name != "Main Camera")
            return;

        if(transform.position.z > other.transform.position.z)
        {
            foreach(var material in materials)
            {
                material.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }
    }

    void OnDestroy()
    {
        foreach (var material in materials)
        {
            material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
