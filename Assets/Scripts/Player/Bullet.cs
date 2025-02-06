using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bullet_speed = 500;
    float duration = 5f;
    float bornTime;
    void Start()
    {
        bornTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * bullet_speed);

        //print(Time.time);
        if(Time.time - bornTime >= duration){
            Destroy(gameObject);
        }
    }
}
