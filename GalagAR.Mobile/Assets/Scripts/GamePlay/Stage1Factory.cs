using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Factory : MonoBehaviour {

    public Transform pathR1;

    Transform pathR2;
    Transform pathR3;
    Transform pathR4;

    Transform pathL1;
    Transform pathL2;
    Transform pathL3;
    Transform pathL4;

    int endPointIndex = 9;

	// Use this for initialization
	void Start () {
        pathR2 = Instantiate(pathR1);
        pathR3 = Instantiate(pathR1);
        pathR4 = Instantiate(pathR1);

        Vector3 refEnd = pathR1.GetComponent<BezierSpline>().GetControlPoint(endPointIndex);
        Vector3 offset = new Vector3(0.5f, 0f, 0f);

        Vector3 pathR2End = refEnd + offset;
        pathR2.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, pathR2End);

        Vector3 pathR3End = refEnd + 2 * offset;
        pathR3.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, pathR3End);

        Vector3 pathR4End = refEnd + 3 * offset;
        pathR4.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, pathR4End);

        pathL1 = Instantiate(pathR1);
        pathL1.transform.localScale = new Vector3(1, 1, -1);
        pathL2 = Instantiate(pathL1);
        pathL3 = Instantiate(pathL1);
        pathL4 = Instantiate(pathL1);

        pathL2.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, pathR2End);
        pathL3.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, pathR3End);
        pathL4.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, pathR4End);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
