using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float size;//
    public int neck_len;//how many segments immediately following the head area
    public int torso_len;//how many segments following the neck area, any remaining are tails.
    public int segments_len;//count of all of the non-head segments
    public float seg_dist;//distance between each segment
    public float neck_off;//initial offset of neck  to head

    public GameObject headPrefab;
    public GameObject segmentPrefab;

    private GameObject head;
    private GameObject[] segments;
    // Start is called before the first frame update
    void Start()
    {
        head = Instantiate(headPrefab, new Vector3(0,0,0), Quaternion.identity);
        segments = new GameObject[segments_len];
        for(int i = 0; i < segments_len; i++)
        {
            segments[i] = Instantiate(segmentPrefab, new Vector3(0,0,(i+1)*seg_dist + neck_off), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform target = head.transform;
        for(int i = 0; i < segments_len; i++)
        {

            if(Vector3.Distance(segments[i].transform.position, target.position) > seg_dist)//if we need to drag the segment
            {
                //being dragged
            }
            target = segments[i].transform;
        }
    }
}
