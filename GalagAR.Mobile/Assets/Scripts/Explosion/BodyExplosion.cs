using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyExplosion : MonoBehaviour {

    public GameObject movingObject;
    [Range(0, 100)]
    public float explosionForce = 100;
    [Range(0, 10)]
    public float forwardForce = 1;
    [Range(1, 10)]
    public float speedMultiplier = 1;
    public bool applyGravityOnExplosion = false;
    public bool transferSpeed = true;

    List<Transform> _detachedChildren;

	// Use this for initialization
	void Start () {
        _detachedChildren = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyForceForward();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DetachChildren();
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            ApplyVelocity();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            DetachChildren();
            if(transferSpeed)
                ApplyVelocity();
            Explode();
        }

	}

    void ApplyVelocity()
    {
        var pbody = movingObject.GetComponent<Rigidbody>();
        foreach(var t in _detachedChildren) {
            Debug.Log("transfer speed");

            var body = t.GetComponent<Rigidbody>();
            body.velocity = speedMultiplier * pbody.velocity;
        }
    }

    void DetachChildren()
    {
        Destroy(movingObject.GetComponent<Rigidbody>());
        _detachedChildren.Clear();
        foreach (Transform part in movingObject.transform)
        {
            part.gameObject.AddComponent<Rigidbody>();
            var rigidBody = part.gameObject.GetComponent<Rigidbody>();
            rigidBody.mass = 0.1f;
            rigidBody.angularDrag = 0.05f;
            rigidBody.interpolation = RigidbodyInterpolation.None;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            _detachedChildren.Add(part);
        }

        movingObject.transform.DetachChildren();
    }

    void Explode()
    {
        Destroy(movingObject);

        Vector3 posSum = Vector3.zero;
        foreach (Transform part in _detachedChildren)
        {
            posSum = posSum + part.position;
        }

        var explodePos = posSum / _detachedChildren.Count;

        foreach (Transform part in _detachedChildren)
        {
            
            var partRigidBody = part.GetComponent<Rigidbody>();
            if (applyGravityOnExplosion)
                partRigidBody.useGravity = true;

            Debug.Log("apply explosion");

            partRigidBody.AddExplosionForce(explosionForce, explodePos, 10f, 0f, ForceMode.Impulse);
        }
    }

    void ApplyForceForward() {
        var body = movingObject.GetComponent<Rigidbody>();
        body.AddForce(new Vector3(0, 0, forwardForce), ForceMode.Force);
    }
}
