using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_hand : MonoBehaviour
{
    [SerializeField]
    HUDController HUD;
    bool left = false;

    [SerializeField]
    MeshCollider trigger;

    private Transform target;
    private bool isGrabbing;

    private Vector3 origin;

    private void Start()
    {
        //HUD = FindObjectOfType<HUDController>();
        origin = transform.localPosition;

        //dangling L and right as different speed
        if (name.Contains("L"))
        {
            left = true;
            LeanTween.moveLocalZ(gameObject, 0.1f, 2).setEaseInOutSine().setLoopPingPong();
        }
            
        else LeanTween.moveLocalZ(gameObject, 0.1f, 3).setEaseInOutSine().setLoopPingPong();

        ////turn off mesh renderer of the Trigger
        //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        //trigger = transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
        //trigger.convex = true;
        //trigger.isTrigger = true;
        //gameObject.AddComponent<Rigidbody>().isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGrabbing) return;
        if (other.gameObject.layer == 6)
        {
            target = other.transform;
            TurnOnIndicatorGizmo(other.gameObject, true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("foundsomething");
        if (other.gameObject.layer == 6)
        {
            target = null;
            TurnOnIndicatorGizmo(other.gameObject, false);
        }

    }

    public IEnumerator GrabTarget()
    {
        if (target)//if detach target, run right to it
            LeanTween.move(gameObject, target.transform.position, 1f);
        else// reach by luck
            LeanTween.moveLocalZ(gameObject, 5f, 1f);

        //wait till the action finish
        yield return new WaitForSeconds(1.1f);

        //if still not find the target, stop 
        if (!target)
        {
            LeanTween.moveLocal(gameObject, origin, 1f);
            StopCoroutine(GrabTarget());
        }


        //if luckily hit the target, take it
        else
        {
            target.transform.SetParent(transform, true);
            TurnOnGrabGizmo(target.gameObject, true);
            isGrabbing = true;
        }
    }

    public void ReleaseTarget()
    {
        if (!target) return;
        target.transform.SetParent(null, true);
        TurnOnGrabGizmo(target.gameObject, false);
        isGrabbing = false;
        LeanTween.moveLocal(gameObject, origin, 1f);

    }

    private static void TurnOnGrabGizmo(GameObject other,bool On)
    {
        if (other.CompareTag("Grabbable"))
        {
            other.gameObject.GetComponent<GrabableObject>().GrabOn(On);
        }
    }

    private static void TurnOnIndicatorGizmo(GameObject other, bool On)
    {
        if (other.CompareTag("Grabbable"))
        {
            other.gameObject.GetComponent<GrabableObject>().IndicateOn(On);
        }
    }
}
