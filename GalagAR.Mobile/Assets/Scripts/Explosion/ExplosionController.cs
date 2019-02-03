using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

    public Material explosionPartMaterial;
    public GameObject partTemplate;
    [Range(10, 100)]
    public float explosionForce = 100;

    float _wallWidth = 10;
    float _wallHeight = 10;
    List<GameObject> _wallParts;

	// Use this for initialization
	void Start () {
        _wallParts = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C)) {
            CreateWall();
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            Explode();
        }
	}

    void CreateWall() {
        foreach(var part in _wallParts) {
            Destroy(part);
        }
        _wallParts.Clear();

        for (int w = 1; w <= _wallWidth; w++)
        {
            var posx = (w * partTemplate.transform.localScale.x) - (_wallWidth / 2);

            for (int h = 0; h < _wallHeight; h++)
            {
                var posy = -2f + (partTemplate.transform.localScale.y / 2) + (h * partTemplate.transform.localScale.y);

                var part = Instantiate(partTemplate);
                part.transform.position = new Vector3(posx, posy, 0);
                part.SetActive(true);

                _wallParts.Add(part);
            }
        }
    }

    void Explode() {
        Debug.Log("Explode");

        //int partIndex = (int)(_wallWidth * (1 + _wallHeight) / 2);
        //var part = _wallParts[partIndex];

        //part.GetComponent<MeshRenderer>().material = explosionPartMaterial;

        foreach(var part in _wallParts)
        {
            var partRigidBody = part.GetComponent<Rigidbody>();
            partRigidBody.AddExplosionForce(explosionForce, this.transform.position, 10f, 0f, ForceMode.Impulse);
        }

    }
}
