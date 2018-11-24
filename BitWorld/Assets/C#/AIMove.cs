using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour {
    [SerializeField] private float strength = 10f;
    Rigidbody rigidbody;
    [SerializeField] private float velocity = 3f;
	// Use this for initialization
	void Start () {
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * strength;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * strength;

        Vector3 dir = this.gameObject.transform.forward;
        Vector3 newVerDir = dir.normalized;
        Vector3 newHorDir = this.gameObject.transform.right.normalized;
        Vector3 moveHorizontal = new Vector3(newHorDir.x * x, newHorDir.y * x, newHorDir.z * x);
        Vector3 moveVerticalVec = new Vector3(newVerDir.x * z, newVerDir.y * z, newVerDir.z * z);
        Vector3 moveVec = moveHorizontal + moveVerticalVec;
        
        float th = 0.1f;
        if(Mathf.Abs(x) > th || Mathf.Abs(z) > th)
        {
            if (rigidbody.velocity.magnitude < velocity)
            {
                rigidbody.AddForce(moveVec);
            }
        }
        else
        {
            Stop();
        }

        MouseRotate();
	}


    void MouseRotate()
    {
        float rotX = Input.GetAxis("Mouse X");
        float rotY = Input.GetAxis("Mouse Y");

        this.gameObject.transform.Rotate(-rotY, rotX, 0);
    }

    void Stop()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
