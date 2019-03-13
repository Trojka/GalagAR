using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour
{

    public BezierSpline spline;

    public float duration;

    public bool lookForward;

    private float progress;
    private bool walking = false;

    public void Walk()
    {
        walking = true;
    }

    private void Update()
    {
        if (!walking)
            return;
        
        progress += Time.deltaTime / duration;
        if (progress > 1f)
        {
            progress = 1f;
        }
        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}
