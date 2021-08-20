using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SkinnedMeshRenderer handMeshL, handMeshR;
    private Transform handL, handR;
    private Vector3 originL = new Vector3(0.36f, 0, 0);
    private Vector3 originR = new Vector3(-0.36f, 0, 0);

    private bool lockTargetL = false;
    private bool lockTargetR = false;

    private bool performMoveL = false;
    private bool performMoveR = false;

    private Transform newPositionL, newPositionR;

    public GameObject targetL, targetR;

    private LineRenderer line;

    private void Awake()
    {
        handL = transform.GetChild(0);
        handR = transform.GetChild(1);
        handMeshL = handL.GetComponent<SkinnedMeshRenderer>();
        handMeshR = handR.GetComponent<SkinnedMeshRenderer>();

        line = GetComponent<LineRenderer>();
        line.positionCount = 3;
        line.SetPosition(0, handL.position);
        line.SetPosition(1, transform.position);
        line.SetPosition(2, handR.position);
    }

    private void Update()
    {
        if (performMoveL && targetL && lockTargetL)
        {
            targetL.transform.position = handL.transform.position;
        }
        if (performMoveR && targetR && lockTargetR)
        {
            targetR.transform.position = handR.transform.position;
        }

        line.SetPosition(0, handL.position);
        line.SetPosition(1, transform.position);
        line.SetPosition(2, handR.position);

        if (handL.localPosition.y > 3) line.SetPosition(0, handL.position);

        if (handR.localPosition.y > 3) line.SetPosition(2, handR.position);
    }

    internal void PerformLeft()
    {
        handMeshL.SetBlendShapeWeight(0, 100);

        if (!targetL || lockTargetL) return;
        //after this line, if the target is locked, feel free to move the
        newPositionL = targetL.transform;

        lockTargetL = true;
        
        LeanTween.move(handL.gameObject, newPositionL, 1f);
        StartCoroutine(AllowLeftTargetMove());
    }

    internal void PerformRight()
    {
        handMeshR.SetBlendShapeWeight(0, 100);
        if (!targetR || lockTargetR) return;

        newPositionR = targetR.transform;

        lockTargetR = true;

        LeanTween.move(handR.gameObject, newPositionR, 1f);
        StartCoroutine("AllowRightTargetMove");
    }

    internal void CancelLeft()
    {
        handMeshL.SetBlendShapeWeight(0, 0);
        handMeshL.SetBlendShapeWeight(1, 0);

        lockTargetL = false;
        performMoveL = false;

        LeanTween.moveLocal(handL.gameObject, originL, 1f);
    }

    internal void CancelRight()
    {
        handMeshR.SetBlendShapeWeight(0, 0);
        handMeshR.SetBlendShapeWeight(1, 0);

        lockTargetR = false;
        performMoveR = false;

        LeanTween.moveLocal(handR.gameObject, originR, 1f);
    }

    public void DetectLeft(GameObject interactwith)
    {
        if (lockTargetL) return;
        handMeshL.SetBlendShapeWeight(1, 100);
        targetL = interactwith;
    }

    public void DetectRight(GameObject interactwith)
    {
        if (lockTargetR) return;
        handMeshR.SetBlendShapeWeight(1, 100);
        targetR = interactwith;
    }

    public void ForgetLeft()
    {
        //if left is lock, don't allow cancel or forget
        if (lockTargetL) return;
        
        CancelLeft();
        targetL = null;
    }

    public void ForgetRight()
    {
        if (lockTargetR) return;

        CancelRight();
        targetR = null;
    }

    private IEnumerator AllowLeftTargetMove()
    {
        yield return new WaitForSeconds(1.1f);
        performMoveL = true;

        if (!lockTargetL)
        {
            StopCoroutine(AllowLeftTargetMove());
            performMoveL = false;
        }
    }

    private IEnumerator AllowRightTargetMove()
    {
        yield return new WaitForSeconds(1.1f);
        performMoveR = true;

        if (!lockTargetR)
        {
            StopCoroutine(AllowRightTargetMove());
            performMoveR = false;
        }
    }
}
