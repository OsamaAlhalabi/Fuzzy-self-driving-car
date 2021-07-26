using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingObticale : MonoBehaviour
{
    public Rigidbody rb;
    public float m_Speed;
    // Start is called before the first frame update
    void Start()
    {
       m_Speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
    //    print("Transform"+this.transform.position);
        rb.AddForce(Vector3.left * 0.12f, ForceMode.Impulse);
    }
}
