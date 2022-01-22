using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementsManager : MonoBehaviour
{
    public float Speed = 200;
    public float MaxDist = 7;
    public float MinDist = 3;
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

        transform.Translate(new Vector3(0, Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * Speed);
        transform.LookAt(transform.parent);
    
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.zero, out hit, 1))
        {
            transform.position = hit.point;
            Debug.Log("CIAO");

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (Physics.SphereCast(transform.position, 0.5f,Vector3.zero, out hit, 1))
        {
            transform.position = hit.point;
        }
    }
   
}
