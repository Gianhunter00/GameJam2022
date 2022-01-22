using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseRot : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * Speed);
        
    }
}
