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
            LeanTween.moveLocalY(gameObject, 0.05f, 2).setEaseInOutSine().setLoopPingPong();
        }
            
        else LeanTween.moveLocalY(gameObject, 0.05f, 3).setEaseInOutSine().setLoopPingPong();

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
            TurnOnCursorOverIndicator(other.gameObject, true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            target = null;
            TurnOnCursorOverIndicator(other.gameObject, false);
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
            GrabOn(target.gameObject, true);
            isGrabbing = true;
        }
    }

    public void ReleaseTarget()
    {
        if (!target) return;
        GrabOn(target.gameObject, false);
        isGrabbing = false;
        LeanTween.moveLocal(gameObject, origin, 1f);

    }

    private void GrabOn(GameObject other,bool On)
    {
        if (other.CompareTag("Grabbable"))
        {
            other.gameObject.GetComponent<GrabableObject>().GrabOn(On,transform);
        }
    }

    private static void TurnOnCursorOverIndicator(GameObject other, bool On)
    {
        if (other.CompareTag("Grabbable"))
        {
            other.gameObject.GetComponent<GrabableObject>().IndicateOn(On);
        }
    }
}
