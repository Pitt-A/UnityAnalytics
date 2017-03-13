using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float speedMod = 0.3f;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            transform.position += Vector3.forward * speedMod;
        }
        if (Input.GetKey("s"))
        {
            transform.position -= Vector3.forward * speedMod;
        }
        if (Input.GetKey("e"))
        {
            transform.position += Vector3.up * speedMod;
        }
        if (Input.GetKey("q"))
        {
            transform.position -= Vector3.up * speedMod;
        }
        if (Input.GetKey("a"))
        {
            transform.Rotate(0.0f, -4.0f, 0.0f);
        }
        if (Input.GetKey("d"))
        {
            transform.Rotate(0.0f, 4.0f, 0.0f);
        }
    }
}
