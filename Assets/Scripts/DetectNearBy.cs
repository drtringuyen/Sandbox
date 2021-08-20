using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectNearBy : MonoBehaviour
{
    SkinnedMeshRenderer mesh;
    HUDController HUD;
    bool left = false;

    private void Start()
    {
        HUD = transform.parent.gameObject.GetComponent<HUDController>();
        mesh = GetComponent<SkinnedMeshRenderer>();
        if (name.Contains("L")) left = true;
        LeanTween.moveLocalY(gameObject, 0.1f, 5).setEaseInOutBack().setLoopPingPong();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (left) HUD.DetectLeft(other.gameObject);
            else HUD.DetectRight(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (left) HUD.ForgetLeft();
            else HUD.ForgetRight();
        }
    }
}
