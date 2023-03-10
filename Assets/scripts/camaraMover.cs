using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camaraMover : MonoBehaviour
{
    // Start is called before the first frame update

    float xspeed;
    float yspeed;
    [SerializeField]
    float Maxspeed;
    [SerializeField]
    public float Acceleration;

    [SerializeField]
    public float rotationTime;

    private int rotation;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            yspeed = Mathf.Clamp(yspeed + Acceleration * Time.deltaTime, -Maxspeed, Maxspeed);
        }
        else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            yspeed = Mathf.Clamp(yspeed - Acceleration * Time.deltaTime, -Maxspeed, Maxspeed);
        }
        else if(yspeed != 0){
            yspeed = clampToZero(yspeed, Acceleration * Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            xspeed = Mathf.Clamp(xspeed + Acceleration * Time.deltaTime, -Maxspeed, Maxspeed);
        }
        else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            xspeed = Mathf.Clamp(xspeed - Acceleration * Time.deltaTime, -Maxspeed, Maxspeed);
        }
        else if(xspeed != 0){
            xspeed = clampToZero(xspeed, Acceleration * Time.deltaTime);

        }
        


        transform.Translate(new Vector3(xspeed * Time.deltaTime, yspeed * Time.deltaTime, 0));

        if(Input.GetKeyDown("[1]")){
            rotateCam(-1);
        }
        if(Input.GetKeyDown("[2]")){
            rotateCam(1);
        }
    }

    void rotateCam(int deg){
        rotation += deg;
        rotation = rotation % 4;
        iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(0, 0, rotation * 90), "time", rotationTime, "easetype", iTween.EaseType.easeInOutCubic));
        // rotation = rotation % 4;
    }
    float clampToZero(float n, float distance){
        if(n == 0) return 0;
        if(distance > Mathf.Abs(n)) return 0;
        
        return n - Mathf.Sign(n) * distance;

    }
}
