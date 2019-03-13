using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Factory : MonoBehaviour {

    Vector3 mirror = new Vector3(1, 1, -1);

    public Transform pathT1_1;

    public Transform endPointHolder;

    public Transform t1_1_End;
    public Transform t1_2_End;
    public Transform t1_3_End;
    public Transform t1_4_End;
    public Transform t2_1_End;
    public Transform t2_2_End;
    public Transform t2_3_End;
    public Transform t2_4_End;

    public Transform EnemyType1;
    public int MaxNumberOfType1Enemies;
    List<BezierSpline> enemyType1Paths;
    int numberOfType1Enemies;

    public float CreateEnemyEverySeconds;
    float progress;

    Transform pathT1_2;
    Transform pathT1_3;
    Transform pathT1_4;

    Transform pathT2_1;
    Transform pathT2_2;
    Transform pathT2_3;
    Transform pathT2_4;

    const int endPointIndex = 9;

    void CreateStage1Wave1Paths() {

        Debug.Log("endPointHolder.position: " + endPointHolder.position);
        Debug.Log("endPointHolder.localPosition: " + endPointHolder.localPosition);


        pathT1_2 = Instantiate(pathT1_1, this.transform, false);
        pathT1_2.name = "PathT1_2";
        pathT1_3 = Instantiate(pathT1_1, this.transform, false);
        pathT1_3.name = "PathT1_3";
        pathT1_4 = Instantiate(pathT1_1, this.transform, false);
        pathT1_3.name = "PathT1_3";

        pathT2_1 = Instantiate(pathT1_1, this.transform, false);
        pathT2_1.name = "PathT2_1";
        pathT2_1.transform.localScale = mirror;

        pathT2_2 = Instantiate(pathT2_1, this.transform, false);
        pathT2_2.name = "PathT2_2";
        pathT2_3 = Instantiate(pathT2_1, this.transform, false);
        pathT2_3.name = "PathT2_3";
        pathT2_4 = Instantiate(pathT2_1, this.transform, false);
        pathT2_4.name = "PathT2_4";

        // tranform from worldspace to localspace
        var t1_1_EndLocal = this.transform.InverseTransformPoint(t1_1_End.position);
        Debug.Log("t1_1_End: " + t1_1_End.position);
        Debug.Log("t1_1_EndLocal: " + t1_1_EndLocal);
        pathT1_1.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, t1_1_EndLocal);
        pathT1_2.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, this.transform.InverseTransformPoint(t1_2_End.position));

        pathT1_3.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, this.transform.InverseTransformPoint(t1_3_End.position));
        pathT1_4.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, this.transform.InverseTransformPoint(t1_4_End.position));

        var t2_1_EndLocal = this.transform.InverseTransformPoint(t2_1_End.position);
        var t2_1_EndLocalScaled = Vector3.Scale(mirror, t2_1_EndLocal);
        Debug.Log("t2_1_End: " + t2_1_End.position);
        Debug.Log("t2_1_EndLocal: " + t2_1_EndLocal);
        Debug.Log("t2_1_EndLocalScaled: " + t2_1_EndLocalScaled);
        pathT2_1.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, t2_1_EndLocalScaled);

        pathT2_2.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, Vector3.Scale(new Vector3(1, 1, -1), this.transform.InverseTransformPoint(t2_2_End.position)));
        pathT2_3.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, Vector3.Scale(new Vector3(1, 1, -1), this.transform.InverseTransformPoint(t2_3_End.position)));
        pathT2_4.GetComponent<BezierSpline>().SetControlPoint(endPointIndex, Vector3.Scale(new Vector3(1, 1, -1), this.transform.InverseTransformPoint(t2_4_End.position)));

        enemyType1Paths = new List<BezierSpline>();
        enemyType1Paths.Add(pathT1_1.GetComponent<BezierSpline>());
        enemyType1Paths.Add(pathT1_2.GetComponent<BezierSpline>());
        enemyType1Paths.Add(pathT1_3.GetComponent<BezierSpline>());
        enemyType1Paths.Add(pathT1_4.GetComponent<BezierSpline>());
    }

	// Use this for initialization
	void Start () {
        CreateStage1Wave1Paths();
        progress = 0;
        numberOfType1Enemies = 0;
	}
	
	// Update is called once per frame
	void Update () {

        progress += Time.deltaTime;
        if((numberOfType1Enemies < MaxNumberOfType1Enemies) && (progress > CreateEnemyEverySeconds)) {
            var et1 = CreateEnemyType1(numberOfType1Enemies);
            et1.Walk();

            numberOfType1Enemies++;
            progress -= CreateEnemyEverySeconds;
        }
	}

    SplineWalker CreateEnemyType1(int enemyNumber) {
        var et1 = Instantiate(EnemyType1);
        et1.GetComponent<SplineWalker>().spline = enemyType1Paths[enemyNumber];

        return et1.GetComponent<SplineWalker>();
    }
}
