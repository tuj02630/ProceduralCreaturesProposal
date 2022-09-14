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
    public float turningSpeed;
    public float segmentSpeed;
    public float target_dist_range;
    public GameObject headPrefab;
    public GameObject segmentPrefab;

    public Transform MouseLocation;

    private GameObject head;
    private GameObject[] segments;
    private GameObject[] targets;
    private float[] heights;
    private float maxCurve;
    private float minCurve;

    private float[] slopes;

    [Range(0.0f, 10.0f)]
    public float maxHeightValue;
    [Range(0.0f, 10.0f)]
    public float minHeightValue;

    public AnimationCurve segmentHeightCurve;
    // Start is called before the first frame update
    void Start()
    {
        segments = new GameObject[segments_len];
        targets = new GameObject[segments_len];
        slopes = new float[segments_len];

        head = Instantiate(headPrefab, new Vector3(0,0,0), Quaternion.identity);
        head.transform.SetParent(this.transform);

        targets[0] = new GameObject("Target");
        targets[0].transform.SetParent(head.transform);
        targets[0].transform.localPosition = new Vector3(0,0, -1 * seg_dist);

        for(int i = 0; i < segments_len; i++)
        {
            segments[i] = Instantiate(segmentPrefab);
            segments[i].transform.SetParent(this.transform);
            Physics.IgnoreCollision(segments[i].GetComponent<Collider>(), head.GetComponent<Collider>());

            if(i < segments_len - 1) //if we aren't at the last segment
            {
                //add a target to the end of our current segment
                targets[i+1] = new GameObject("Target");
                targets[i+1].transform.SetParent(segments[i].transform);
                targets[i+1].transform.localPosition = new Vector3(0,0, -1 * seg_dist);
            }
            segments[i].transform.position = targets[i].transform.position;
        }
        for(int i = 0; i < segments_len - 1; i++)
        {
            for(int j = i+1; j < segments_len; j++)
            {
                Physics.IgnoreCollision(segments[i].GetComponent<Collider>(), segments[j].GetComponent<Collider>());
            }
        }

        heights = splitHeightCurve(segmentHeightCurve, segments_len);
        for(int i = 0; i < segments_len; i++)
        {
            segments[i].transform.GetChild(0).transform.Rotate(slopes[i], 0f, 0f, Space.Self);
            segments[i].transform.localPosition += new Vector3(0,heights[i] * maxHeightValue - (maxHeightValue / 2f),0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 dir = Vector3.Normalize(MouseLocation.position - head.transform.position);
            head.GetComponent<Rigidbody>().velocity = new Vector3(dir.x, head.transform.position.y, dir.z);
        }
        head.transform.LookAt(new Vector3(MouseLocation.position.x, head.transform.position.y, MouseLocation.position.z));   
        
        Vector3 targetRot = new Vector3(0,0,0);
        for(int i = 0; i < segments_len; i++)
        {
            
            //SEGMENT POSITION
            float dist_to_target = Vector3.Distance(removeY(segments[i].transform.position), removeY(targets[i].transform.position));
            Vector3 dir = Vector3.Normalize(removeY(targets[i].transform.position) - removeY(segments[i].transform.position));
            segments[i].GetComponent<Rigidbody>().velocity = dir * segmentSpeed * dist_to_target;

            //SEGMENT ROTATION
            if(i == 0)
            {
                targetRot = new Vector3(head.transform.position.x, segments[i].transform.position.y, head.transform.position.z);
            }
            else
            {
                targetRot = new Vector3(segments[i-1].transform.position.x, segments[i].transform.position.y, segments[i-1].transform.position.z);
            }
            segments[i].transform.LookAt(targetRot);
        }
    }
    private void MoveCharacter(Vector2 _direction, Rigidbody rb)
    {
        Vector3 _directionF = rb.rotation * new Vector3(_direction.x, 0, _direction.y);
        rb.MovePosition(rb.position + (_directionF * Time.fixedDeltaTime));
    }
    float[] splitHeightCurve(AnimationCurve ac, int segments_len)
    {
        float[] result = new float[segments_len];
        Keyframe totalTime = ac[ac.length-1];
        //Debug.Log(totalTime.time);
        for(int i = 0; i < segments_len; i++)
        {
            
            result[i] = ac.Evaluate((totalTime.time / (float) segments_len) * (i));
            slopes[i] = Mathf.Rad2Deg * Mathf.Atan(estimateSlope(ac, (totalTime.time / (float) segments_len)* i));
            Debug.Log("SLOPE: " + slopes[i]);
            if(i == 0)
            {
                maxCurve = minCurve = result[i]; 
            }
            else
            {
                if(result[i] > maxCurve)
                {
                    maxCurve = result[i];
                }
                if(result[i] < minCurve)
                {
                    minCurve = result[i];
                }
            }
        }
        //normalize the array
        maxCurve -= minCurve;
        float maxMod = 1f/maxCurve;
        for(int i = 0; i < segments_len; i++)
        {
            result[i] -= minCurve;
            result[i] *= maxMod;
            Debug.Log(result[i]);
        }
        return result;
    }
    //should only be passed time values between 0 and 1, inclusive
    float estimateSlope(AnimationCurve ac, float time)
    {
        float t1 = time + 0.0001f;
        float t2 = time - 0.0001f;
        if(time <= 0.0001f)
        {
            t2 = time;
        }
        else if(time >= 0.9999f)
        {
            t1 = time;
        }
        Debug.Log("At time t = "+time+", the curve reads: "+ac.Evaluate(time)+"\nThe slope is estimated to be: "+((ac.Evaluate(t1) - ac.Evaluate(t2)) / (t1 - t2))+", Giving us a calculated degree of: ");
        return ((ac.Evaluate(t1) - ac.Evaluate(t2))/ (t1 - t2));
    }
    Vector3 removeY(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    } 
}