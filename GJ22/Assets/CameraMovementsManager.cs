using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementsManager : MonoBehaviour
{
    public float Speed = 200;
    public float MaxDist = 7;
    public float MinDist = 3;
    public float MaxHeight = 4;
    public float MinHeight = 3;
    private float currDist;
    private Rigidbody rb;
    private RaycastHit hit;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currDist = Mathf.Clamp(Mathf.Abs(transform.parent.position.z - transform.position.z),MinDist,MaxDist);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * Input.GetAxis("Mouse Y") * Time.deltaTime * Speed;
        transform.position = new Vector3(transform.position.x, 
            Mathf.Clamp(transform.position.y, MinHeight+transform.parent.position.y, MaxHeight + transform.parent.position.y), transform.position.z);
        //transform.Translate(new Vector3(0, Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * Speed);
        transform.LookAt(transform.parent);
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.zero, out hit, 1))
        {
            transform.position = hit.point;
            Debug.Log("CIAO");

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (Physics.SphereCast(transform.position, 0.5f,Vector3.zero, out hit, 1))
        {
            transform.position = hit.point;
            Debug.Log("CIAO");

        }
    }
   
}
