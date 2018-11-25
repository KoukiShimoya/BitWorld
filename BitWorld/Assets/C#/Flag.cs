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
        awake,
        tail,
        beforeCharge,
        waitCharge,
        charging,
        afterCharge,
        tobody1,
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
    private float collTime = 0f; float door_waitTime = 0f; float awake_time = 0f; float tobody1_time = 0f; float finish_time = 0f;
    [SerializeField] Vector3[] AstroPosition = new Vector3[6];
    private bool bodyL1 = false; private bool bodyL2 = false; private bool bodyR1 = false; private bool bodyR2 = false; private bool hasLench = false; bool openPanel = false;
    private bool wingR_edge = false; bool wing_L_corner = false;
    [SerializeField] private InputField inputField;
    private Text text; Text text_01;
    private AudioSource[] sounds;
    private Image panelImage;
    private GameObject title; GameObject name;

	// Use this for initialization
	void Start () {
        state = State.awake;
        if(astro == null) { astro = GameObject.Find("Asto_kun"); }
        glitch = GetComponent<GlitchFx>();
        for(int i = 0; i < 9; i++)
        {
            GameObject obj = GameObject.Find("heap_button_" + i);
            heapButtons[i] = new heapButton(obj, false);
        }
        text = GameObject.Find("Text").GetComponent<Text>();
        text_01 = GameObject.Find("Text_01").GetComponent<Text>();
        sounds = GetComponents<AudioSource>();
        inputField.gameObject.SetActive(false);
        panelImage = GameObject.Find("Panel").GetComponent<Image>();
        panelImage.gameObject.SetActive(false);
        title = GameObject.Find("title"); name = GameObject.Find("name");
        title.SetActive(false); name.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        GameObject hitObj = rayObj();
        //if(hitObj == null) { return; }

        switch (state)
        {
            case State.awake:
                AwakeStart();
                break;
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
            case State.tobody1:
                ToBody1();
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
                Finish();
                break;
        }
        collReset();
	}

    GameObject rayObj()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 1))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    void AwakeStart()
    {
        text.text = "飛行士：おい、起きろ！緊急事態だ！脱出するぞ！";
        text_01.text = "";
        awake_time += Time.deltaTime;
        if(awake_time > 2f)
        {
            state = State.tail;
        }
    }

    void TailButtonPush(GameObject hitObj)
    {
        text.text = "宇宙飛行士：ボタンを押して扉を開けてくれ";
        text_01.text = "";
        if (hitObj == null) { return; }
        if(hitObj.name == "tail_button" || hitObj.name == "tail_button_push")
        {
            sounds[0].PlayOneShot(sounds[0].clip);
            sounds[3].PlayOneShot(sounds[3].clip);
            sounds[4].Stop();
            sounds[5].Play();
            state = State.beforeCharge;
            Destroy(GameObject.Find("door_in_tailheap"));
            Destroy(GameObject.Find("door_side_tailheap"));
            Destroy(GameObject.Find("door_collider_tailheap"));
            //音
        }
    }

    void BeforeCharge()
    {
        text.text = "宇宙飛行士：回線がショートしているな、扉の前にいってくれ";
        text_01.text = "";
        if (colliderObj == null) { return; }
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
        text.text = "宇宙飛行士：ちょっと待ってろ";
        text_01.text = "";
    }

    void Charging()
    {
        sounds[0].PlayOneShot(sounds[0].clip);
        glitch.intensity -= 0.01f;

        if(glitch.intensity <= 0)
        {
            state = State.afterCharge;
        }
        text.text = "宇宙飛行士：よし、直ってきたな";
        text_01.text = "";
    }

    void AfterCharge(GameObject hitObj)
    {
        text.text = "宇宙飛行士：扉をハックして開けてくれ" + "\n" +
            "右下の9つのボタンを Z を描く順で押せば開くはずだ";
        text_01.text = "";
        if (hitObj == null) { return; }
        if(!heapButtons[0].boolean && !heapButtons[1].boolean && !heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if(hitObj == heapButtons[2].gameObject) { heapButtons[2].boolean = true; sounds[0].PlayOneShot(sounds[0].clip); }
        }
        else if(!heapButtons[0].boolean && !heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if(heapButtons[1].gameObject == hitObj) { heapButtons[1].boolean = true; sounds[0].PlayOneShot(sounds[0].clip); }
        }
        else if(!heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if(hitObj == heapButtons[0].gameObject) { heapButtons[0].boolean = true; sounds[0].PlayOneShot(sounds[0].clip); }
        }
        else if(heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && !heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[4].gameObject) { heapButtons[4].boolean = true; sounds[0].PlayOneShot(sounds[0].clip); }
        }
        else if (heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && !heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[8].gameObject) { heapButtons[8].boolean = true; sounds[0].PlayOneShot(sounds[0].clip); }
        }
        else if (heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && !heapButtons[7].boolean && heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[7].gameObject) { heapButtons[7].boolean = true; sounds[0].PlayOneShot(sounds[0].clip); }
        }
        else if (heapButtons[0].boolean && heapButtons[1].boolean && heapButtons[2].boolean && !heapButtons[3].boolean && heapButtons[4].boolean && !heapButtons[5].boolean && !heapButtons[6].boolean && heapButtons[7].boolean && heapButtons[8].boolean)
        {
            if (hitObj == heapButtons[6].gameObject) {
                heapButtons[6].boolean = true;
                sounds[1].PlayOneShot(sounds[0].clip);
                sounds[3].PlayOneShot(sounds[3].clip);
                sounds[5].Stop();
                sounds[6].Play();
                Destroy(GameObject.Find("door_in_heapbody"));
                Destroy(GameObject.Find("door_side_heapbody"));
                Destroy(GameObject.Find("door_collider_heapbody"));
                state = State.tobody1;
            }
            
        }
        else
        {
            sounds[2].PlayOneShot(sounds[2].clip);
            for (int i = 0; i < heapButtons.Length; i++)
            {
                heapButtons[i].boolean = false;
            }
        }
    }

    void ToBody1()
    {
        tobody1_time += Time.deltaTime;
        text.text = "宇宙飛行士：いいぞ！船の先端にSOS発信機がある。" + "\n" + "無線室への扉を開けるには、太陽光パネルを修理する必要があるな";
        text_01.text = "";
        if(tobody1_time > 10f)
        {
            state = State.body1;
        }
    }

    void Body1(GameObject hitObj)
    {
        text.text = "宇宙飛行士：次は右の翼の太陽光パネルを修理するぞ！" + "\n" + "扉を開けるから、電源ボックスを押して電気を入れてくれ";
        text_01.text = "";
        AstroMove(astro.transform.position, AstroPosition[0], State.body1);
        if (hitObj == null) { return; }
        if(hitObj.name == "body_button_R1")
        {
            sounds[1].PlayOneShot(sounds[1].clip);
            if (!bodyR1) { bodyR1 = true; }
        }else if(hitObj.name == "body_button_R2")
        {
            sounds[1].PlayOneShot(sounds[1].clip);
            if (!bodyR2) { bodyR2 = true; }
        }

        if(bodyR1 && bodyR2)
        {
            sounds[1].PlayOneShot(sounds[1].clip);
            sounds[3].PlayOneShot(sounds[3].clip);
            Destroy(GameObject.Find("door_in_bodyWingR"));
            Destroy(GameObject.Find("door_side_bodyWingR"));
            Destroy(GameObject.Find("door_collider_bodyWingR"));
            state = State.lench;
        }
    }

    void Lench(GameObject hitObj)
    {
        text.text = "宇宙飛行士：いいぞ、ん？" + "\n" + "修理用レンチがあるはずだから探してきてくれないか？";
        text_01.text = "";
        AstroMove(astro.transform.position, AstroPosition[1], State.lench);
        if (hasLench)
        {
            float dist = Vector3.Distance(astro.transform.position, this.gameObject.transform.position);
            Debug.Log(dist);
            if (dist < 1f)
            {
                sounds[1].PlayOneShot(sounds[1].clip);
                sounds[6].Stop();
                sounds[7].Play();
                state = State.wingR;
            }
        }
        if (hitObj == null) { return; }
        if(hitObj.name == "lench")
        {
            sounds[0].PlayOneShot(sounds[0].clip);
            Destroy(GameObject.Find("lench"));
            hasLench = true;
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
        text.text = "宇宙飛行士：ありがとう。ここからは磁場が強い、俺が作業するから待ってろ";
        text_01.text = "";
    }

    void Body2(GameObject hitObj)
    {
        text.text = "宇宙飛行士：よし！右のパネルは治ったぞ" + "\n" + "同じように左の扉も開けてくれ";
        text_01.text = "";
        if (hitObj == null) { return; }
        if (hitObj.name == "body_button_L1")
        {
            sounds[1].PlayOneShot(sounds[1].clip);
            if (!bodyL1) { bodyL1 = true; }
        }
        else if (hitObj.name == "body_button_L2")
        {
            sounds[1].PlayOneShot(sounds[1].clip);
            if (!bodyL2) { bodyL2 = true; }
        }

        if (bodyL1 && bodyL2)
        {
            sounds[1].PlayOneShot(sounds[1].clip);
            sounds[3].PlayOneShot(sounds[3].clip);
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
            sounds[7].Stop();
            sounds[8].Play();
            Destroy(GameObject.Find("L_collider"));
        }
        text.text = "宇宙飛行士：よし、こっちも直してくる";
        text_01.text = "";
    }

    void WingLOpening(GameObject hitObj)
    {
        text.text = "宇宙飛行士：うわぁぁぁぁぁぁ。窓が開いている！！！" + "\n" + "中のボタンを押して閉めてくれぇぇぇ";
        text_01.text = "";
        if (hitObj == null) { return; }
        if (hitObj.name == "wing_button")
        {
            sounds[0].PlayOneShot(sounds[0].clip);
            sounds[3].PlayOneShot(sounds[3].clip);
            state = State.wingL_after;
        }
    }

    void WingLAfter()
    {
        text.text = "宇宙飛行士：助かった。" + "\n" + "あとは俺が作業するから、外で待っていてくれ";
        text_01.text = "";
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
        text.text = "宇宙飛行士：よし、無線室まで来たな" + "\n" + "ここは生体認証だから、俺が開けるぞ";
        door_waitTime += Time.deltaTime;
        if(door_waitTime > 5f)
        {
            sounds[3].PlayOneShot(sounds[3].clip);
            sounds[8].Stop();
            sounds[9].Play();
            Destroy(GameObject.Find("door_in_bodyhead"));
            Destroy(GameObject.Find("door_side_bodyhead"));
            Destroy(GameObject.Find("door_collider_bodyhead"));
            state = State.head;
        }
    }

    void Head(GameObject hitObj)
    {
        text.text = "宇宙飛行士：よし、最後だ。" + "\n" + "SOSを打ってくれ";
        text_01.text = "S:01010011, O:01001111";
        if (openPanel)
        {

            if (inputField.text == "010100110100111101010011")
            {
                inputField.gameObject.SetActive(false);
                state = State.finish;
            }
        }
        if (hitObj == null) { return; }
        if(hitObj.name == "panel")
        {
            sounds[0].PlayOneShot(sounds[0].clip);
            inputField.gameObject.SetActive(true);
            openPanel = true;
        }
        
    }

    void Finish()
    {
        text.text = "やった！脱出するぞ！";
        text_01.text = "";
        finish_time += Time.deltaTime;
        if(finish_time > 2f)
        {
            if (!panelImage.gameObject.activeSelf) { panelImage.gameObject.SetActive(true); }
            Color color = panelImage.color;
            if (color.a < 1)
            {
                color.a += 0.02f;
                panelImage.color = color;
            }
            else
            {
                title.SetActive(true);
                name.SetActive(true);
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
        if(collider.name == "Ljammer" || collider.name == "Rjammer")
        {
            glitch.intensity = 0.5f;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        colliderObj = null;
        if (collider.name == "Ljammer" || collider.name == "Rjammer")
        {
            glitch.intensity = 0f;
        }
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
