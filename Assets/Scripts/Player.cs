using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public vs SerializeField --> both make it editable in Inspector, but SF prevents other game objects accessing
    [SerializeField]
    private float _speed = 30.0f; //C# convention is private var have underscore _

    // Start is called before the first frame update
    void Start()
    {
        //current position assign new = position(0,0,0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * horizontalInput *  _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
    }
}
