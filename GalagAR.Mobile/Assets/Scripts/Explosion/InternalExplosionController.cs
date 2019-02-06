using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalExplosionController : MonoBehaviour {
    public Material explosionPartMaterial;
    public GameObject partTemplate;
    [Range(0, 100)]
    public float explosionForce = 100;
    [Range(0, 10)]
    public float forwardForce = 1;
    public bool explodeImmediately = false;
    public bool applyGravityOnExplosion = false;

    int _hullSize = 4;
    List<GameObject> _hullParts;
    GameObject _refPart;
    Vector3 _refPos;

	// Use this for initialization
	void Start () {
        _hullParts = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Create();
            if (explodeImmediately)
                Explode();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            ApplyForceForward();
        }
	}

    void Create() {
        foreach (var part in _hullParts)
        {
            Destroy(part);
        }
        _hullParts.Clear();
        _refPart = null;

        int halfSize = _hullSize / 2;

        for (int x = -halfSize; x <= halfSize; x++) 
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                for (int z = -halfSize; z <= halfSize; z++)
                {
                    if (x == -halfSize
                       || x == halfSize)
                    {
                        var part = Instantiate(partTemplate);
                        part.transform.position = new Vector3(
                            this.transform.position.x + x, 
                            this.transform.position.y + y, 
                            this.transform.position.z + z);
                        part.SetActive(true);

                        if (_refPart == null)
                        {
                            _refPart = part;
                            _refPos = _refPart.transform.position;
                        }

                            
                        _hullParts.Add(part);
                    }
                    else
                    {
                        if(y == -halfSize
                          || y == halfSize)
                        {
                            var part = Instantiate(partTemplate);
                            part.transform.position = new Vector3(
                                this.transform.position.x + x,
                                this.transform.position.y + y,
                                this.transform.position.z + z);
                            part.SetActive(true);

                            _hullParts.Add(part);
                        }
                        else
                        {
                            if(z == -halfSize
                              || z == halfSize)
                            {
                                var part = Instantiate(partTemplate);
                                part.transform.position = new Vector3(
                                    this.transform.position.x + x,
                                    this.transform.position.y + y,
                                    this.transform.position.z + z);
                                part.SetActive(true);

                                _hullParts.Add(part);
                            }
                        }
                    }
                }
            }
        }


    }

    void Explode() {
        var newPos = _refPart.transform.position;
        var calcPos = newPos - _refPos;
        var explodePos = new Vector3(calcPos.x, calcPos.y, 0);

        //var explodePos = this.transform.position;

        foreach (var part in _hullParts)
        {
            var partRigidBody = part.GetComponent<Rigidbody>();
            if (applyGravityOnExplosion)
                partRigidBody.useGravity = true;
            
            partRigidBody.AddExplosionForce(explosionForce, explodePos, 10f, 0f, ForceMode.Impulse);
        }
    }

    void ApplyForceForward()
    {
        foreach (var part in _hullParts)
        {
            var partRigidBody = part.GetComponent<Rigidbody>();
            partRigidBody.AddForce(new Vector3(forwardForce, 0, 0), ForceMode.Force);
        }
    }
}
