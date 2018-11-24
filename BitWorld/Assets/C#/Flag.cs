using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//やること
//jammer付近でintentionを上げる
//AstroPosition設定
//音、テキスト付ける
//最後の入力
public class Flag : MonoBehaviour {
    public enum State
    {
        tail,
        beforeCharge,
        waitCharge,
        charging,
        afterCharge,
        body1,
        lench,
        wingR,
        body2,
        wingL_before,
        wingL_opening,
        wingL_after,
        body3,
        head,
        finish,
    };
    struct heapButton {
        public GameObject gameObject;
        public bool boolean;
        public heapButton(GameObject tempobj, bool tempbool)
        {
            gameObject = tempobj;
            boolean = tempbool;
        }
    };

    public State state = new State();
    private GameObject collisionObj;
    private GameObject colliderObj;
    private GlitchFx glitch;
    [SerializeField] private GameObject astro;
    private heapButton[] heapButtons = new heapButton[9];
    private float collTime = 0f; float door_waitTime = 0f;
    [SerializeField] Vector3[] AstroPosition = new Vector3[6];
    private bool bodyL1 = false; private bool bodyL2 = false; private bool bodyR1 = false; private bool bodyR2 = false; private bool hasLench = false; bool openPanel = false;
    private bool wingR_edge = false; bool wing_L_corner = false;
    private InputField inputField;

	// Use this for initialization
	void Start () {
        state = State.tail;
        if(astro == null) { astro = GameObject.Find("Asto_kun"); }
        glitch = GetComponent<GlitchFx>();
        for(int i = 0; i < 9; i++)
        {
            GameObject obj = GameObject.Find("heap_button_" + i);
            heapButtons[i] = new heapButton(obj, false);
        }
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
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
                AfterCharge(hitObj);
                break;
            case State.body1:
                Body1(hitObj);
                break;
            case State.lench:
                Lench(hitObj);
                break;
            case State.wingR:
                WingR();
                break;
            case State.body2:
                Body2(hitObj);
                break;
            case State.wingL_before:
                WingLBefore();
                break;
            case State.wingL_opening:
                WingLOpening(hitObj);
                break;
            case State.wingL_after:
                WingLAfter();
                break;
            case State.body3:
                Body3();
                break;
            case State.head:
                Head(hitObj);
                break;
            case State.finish:

                break;
        }
        collReset();
	}

    GameObject rayObj()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);
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
        AstroMove(astroPos, cameraPos, State.charging);
        
    }

    void Charging()
    {
        glitch.intensity -= 0.01f;

        if(glitch.intensity <= 0)
        {
            state = State.afterCharge;
        }
    }

    void AfterCharge(GameObject hitObj)
    {
        if(hitObj == null) { return; }
        if(!heapButtons[0].boolean && !heapButtons[1].boolean && !heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if(hitObj == heapButtons[2].gameObject) { heapButtons[2].boolean = true; }
        }
        else if(!heapButtons[0].boolean && !heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if(heapButtons[1].gameObject == hitObj) { heapButtons[1].boolean = true; }
        }
        else if(!heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if(hitObj == heapButtons[0].gameObject) { heapButtons[0].boolean = true; }
        }
        else if(heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[4].gameObject) { heapButtons[4].boolean = true; }
        }
        else if (heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[8].gameObject) { heapButtons[8].boolean = true; }
        }
        else if (heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[7].gameObject) { heapButtons[7].boolean = true; }
        }
        else if (heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && heapButtons[7].boolean && heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[6].gameObject) {
                heapButtons[6].boolean = true;
                Destroy(GameObject.Find("door_in_heapbody"));
                Destroy(GameObject.Find("door_side_heapbody"));
                Destroy(GameObject.Find("door_collider_heapbody"));
                state = State.body1;
            }
            
        }
        else
        {
            for(int i = 0; i < heapButtons.Length; i++)
            {
                heapButtons[i].boolean = false;
            }
        }
    }

    void Body1(GameObject hitObj)
    {
        if (hitObj == null) { return; }
        AstroMove(astro.transform.position, AstroPosition[0], State.body1);
        if(hitObj.name == "body_button_R1")
        {
            Debug.Log("R1");
            if (!bodyR1) { bodyR1 = true; }
        }else if(hitObj.name == "body_button_R2")
        {
            Debug.Log("R2");
            if(!bodyR2) { bodyR2 = true; }
        }

        if(bodyR1 && bodyR2)
        {
            Destroy(GameObject.Find("door_in_bodyWingR"));
            Destroy(GameObject.Find("door_side_bodyWingR"));
            Destroy(GameObject.Find("door_collider_bodyWingR"));
            state = State.lench;
        }
    }

    void Lench(GameObject hitObj)
    {
        if (hitObj == null) { return; }
        AstroMove(astro.transform.position, AstroPosition[1], State.lench);
        if(hitObj.name == "lench")
        {
            Destroy(GameObject.Find("lench"));
            hasLench = true;
        }

        if (hasLench)
        {
            float dist = Vector3.Distance(astro.transform.position, this.gameObject.transform.position);
            if(dist < 1f)
            {
                state = State.wingR;
            }
        }
    }

    void WingR()
    {
        if (!wingR_edge)
        {
            AstroMove(astro.transform.position, AstroPosition[2], State.wingR);
            if(Vector3.Distance(astro.transform.position, AstroPosition[2]) < 1f)
            {
                wingR_edge = true;
            }
        }
        else
        {
            AstroMove(astro.transform.position, AstroPosition[3], State.body2);
        }
    }

    void Body2(GameObject hitObj)
    {
        if (hitObj == null) { return; }
        if (hitObj.name == "body_button_L1")
        {
            if (!bodyL1) { bodyL1 = true; }
        }
        else if (hitObj.name == "body_button_L2")
        {
            if (!bodyL2) { bodyL2 = true; }
        }

        if (bodyL1 && bodyL2)
        {
            Destroy(GameObject.Find("door_in_bodyWingL"));
            Destroy(GameObject.Find("door_side_bodyWingL"));
            Destroy(GameObject.Find("door_collider_bodyWingL"));
            state = State.wingL_before;
        }
    }

    void WingLBefore()
    {
        State preState = state;
        AstroMove(astro.transform.position, AstroPosition[4], State.wingL_opening);
        if(state != preState)
        {
            Destroy(GameObject.Find("wing_jamdoor_L"));
        }
    }

    void WingLOpening(GameObject hitObj)
    {
        if (hitObj == null) { return; }
        if (hitObj.name == "wing_button")
        {
            state = State.wingL_after;
        }
    }

    void WingLAfter()
    {
        if (!wing_L_corner)
        {
            AstroMove(astro.transform.position, AstroPosition[3], State.wingL_after);
            if(Vector3.Distance(astro.transform.position, AstroPosition[3]) < 1f)
            {
                wing_L_corner = true;
            }
        }
        else
        {
            AstroMove(astro.transform.position, AstroPosition[5], State.body3);
        }
    }

    void Body3()
    {
        door_waitTime += Time.deltaTime;
        if(door_waitTime > 5f)
        {
            Destroy(GameObject.Find("door_in_bodyhead"));
            Destroy(GameObject.Find("door_side_bodyhead"));
            Destroy(GameObject.Find("door_collider_bodyhead"));
            state = State.head;
        }
    }

    void Head(GameObject hitObj)
    {
        if(hitObj.name == "panel")
        {
            openPanel = true;
        }
        if (openPanel)
        {
            
            if(inputField.text == "010100110100111101010011")
            {
                state = State.finish;
            }
        }
    }

    void AstroMove(Vector3 from, Vector3 to, State nextState)
    {
        Vector3 moveVec = to - from;
        moveVec.Normalize();
        moveVec *= 1;
        astro.transform.position = (from + moveVec / 30);

        if (Vector3.Distance(from, to) < 0.1f)
        {
            state = nextState;
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

    void collReset()
    {
        if(collTime > 5f)
        {
            colliderObj = null;
            collisionObj = null;
            collTime = 0f;
        }
        else
        {
            collTime += Time.deltaTime;
        }
    }
}
