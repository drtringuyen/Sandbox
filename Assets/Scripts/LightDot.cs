using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDot : MonoBehaviour
{
    public bool isConnected;

    public bool isRoot;

    public static LightDot root;

    public static bool stillConnectedtoRoot = false;

    private LineRenderer line;

    private void OnEnable() => AllDots.Add(this);
    private void OnDisable() => AllDots.Remove(this);

    [Range(0,10)]
    public float range;

    public List<LightDot> connectedDots;

    public static List<LightDot> AllDots = new List<LightDot>();

    private void Awake()
    {

        if (gameObject.GetComponent<LineRenderer>())
            this.line = GetComponent<LineRenderer>();
        else line = gameObject.AddComponent<LineRenderer>();

        if (isRoot) root = this;
    }

    private void Update()
    {
        List<Vector3> positions = new List<Vector3>();
        positions.Add(this.transform.position);

        foreach (LightDot dot in AllDots)
        {
            //check testing itself
            if (dot == this) continue;

            float distance = Vector3.Distance(transform.position, dot.transform.position);
            //check in range?
            if (distance > this.range + dot.range)
            {
                continue;
            }
            //inrange process
            if (dot.isConnected)
            {
                //connect the new dot
                positions.Add(dot.transform.position);
                //make sure the line loop back to itself
                //because Stupid Line Renderer will make a triangle out of it if not return back before draw new target
                positions.Add(transform.position);
                //flag this on
                this.isConnected = true;
                if(!connectedDots.Contains(dot)) connectedDots.Add(dot);
            }
        }

        checkRootConnection();

        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
    }

    private static void checkRootConnection()
    {

        foreach (LightDot checkDot in AllDots)
        {
            if (checkDot.isRoot|| !checkDot.isConnected) continue;

            if (checkDot.connectedDots.Contains(root))
            {
                stillConnectedtoRoot = true;
            }
        }

        stillConnectedtoRoot = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
