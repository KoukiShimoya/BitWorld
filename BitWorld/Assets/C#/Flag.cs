using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {
    public enum State
    {
        tail,
        beforeCharge,
        waitCharge,
        charging,
        afterCharge,
        body1,
        wingL,
        lench,
        body2,
        wingR_before,
        wingR_opening,
        wingR_after,
        body3,
        head,
        finish,
    };

    public State state = new State();
    private GameObject collisionObj;
    private GameObject colliderObj;
    private GlitchFx glitch;
    [SerializeField] private GameObject astro;
	// Use this for initialization
	void Start () {
        state = State.tail;
        if(astro == null) { astro = GameObject.Find("Asto_kun"); }
        glitch = GetComponent<GlitchFx>();
	}
	
	// Update is called once per frame
	void Update () {
        GameObject hitObj = rayObj();
        //if(hitObj == null) { return; }

        switch (state)
        {
            case State.tail:
                TailButtonPush(hitObj);
                break;
            case State.beforeCharge:
                BeforeCharge();
                break;
            case State.waitCharge:
                WaitCharge();
                break;
            case State.charging:
                Charging();
                break;
            case State.afterCharge:

                break;
            case State.body1:

                break;
            case State.wingL:

                break;
            case State.lench:

                break;
            case State.body2:

                break;
            case State.wingR_before:

                break;
            case State.wingR_opening:

                break;
            case State.wingR_after:

                break;
            case State.body3:

                break;
            case State.head:

                break;
            case State.finish:

                break;
        }
	}

    GameObject rayObj()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1, Color.red);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 1))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    void TailButtonPush(GameObject hitObj)
    {
        if(hitObj == null) { return; }
        if(hitObj.name == "tail_button" || hitObj.name == "tail_button_push")
        {
            state = State.beforeCharge;
            Destroy(GameObject.Find("door_in_tailheap"));
            Destroy(GameObject.Find("door_side_tailheap"));
            Destroy(GameObject.Find("door_collider_tailheap"));
            //音
        }
    }

    void BeforeCharge()
    {
        if(colliderObj == null) { return; }
        if(colliderObj.name == "ChargePoint")
        {
            state = State.waitCharge;
        }
    }

    void WaitCharge()
    {
        Vector3 astroPos = astro.transform.position;
        Vector3 cameraPos = this.transform.position;
        Vector3 moveVec = cameraPos - astroPos;
        moveVec.Normalize();
        moveVec *= 1;
        astro.transform.position = (astroPos + moveVec/30);

        if(Vector3.Distance(astroPos, cameraPos) < 0.1)
        {
            state = State.charging;
        }
    }

    void Charging()
    {
        glitch.intensity -= 0.01f;

        if(glitch.intensity <= 0)
        {
            state = State.afterCharge;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObj = collision.gameObject;
    }

    void OnTriggerEnter(Collider collider)
    {
        colliderObj = collider.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        colliderObj = null;
    }
}
