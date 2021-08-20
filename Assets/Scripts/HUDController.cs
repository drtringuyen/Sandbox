using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer mesh;
    [SerializeField]
    private HUD_hand handL, handR;

    [SerializeField]
    private Transform root;

    private bool lockTargetL = false;
    private bool lockTargetR = false;

    private Transform newPositionL, newPositionR;

    private Material material;

    private void Awake()
    {

        deleteLeafBone();

        //handL = GameObject.Find("Hand.L").gameObject.AddComponent<HUD_hand>();
        //handR = GameObject.Find("Hand.R").gameObject.AddComponent<HUD_hand>();

        material= mesh.sharedMaterial;

    }

    internal void PerformLeft()
    {
        StartCoroutine(handL.GrabTarget());
    }

    internal void PerformRight()
    {
        StartCoroutine(handR.GrabTarget());
    }

    internal void CancelLeft()
    {
        handL.ReleaseTarget();
    }

    internal void CancelRight()
    {
        handR.ReleaseTarget();
    }

    public void startRun()
    {
        LeanTween.moveLocalY(gameObject, 0.3f, 0.5f).setEaseInOutCubic();
        material.SetFloat("_ScrollYSpeed", -0.5f);
    }

    public void stopRun()
    {
        LeanTween.moveLocalY(gameObject, 0f, 0.5f).setEaseInOutCubic();
        material.SetFloat("_ScrollYSpeed", 0.1f);
    }

    private void deleteLeafBone()
    {
        foreach (Transform child in root.transform)
        {
            DestroyImmediate(child.GetChild(0).gameObject);
        }
    }
}
