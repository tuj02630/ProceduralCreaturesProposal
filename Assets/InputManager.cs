using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool Prototyping;
    public GameObject PrototypeCursorPrefab;
    public GameObject MouseLocation;
    public Camera camera;


    private Ray ray;
    private RaycastHit hit;
    private GameObject PrototypeCursor;
    // Start is called before the first frame update
    void Awake()
    {
        if(Prototyping)
        {
            this.PrototypeCursor = Instantiate(PrototypeCursorPrefab, new Vector3(0,0,0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if(Prototyping)
            {
                this.PrototypeCursor.transform.position = hit.point;
            }
            this.MouseLocation.transform.position = hit.point;
        }
    }
}
