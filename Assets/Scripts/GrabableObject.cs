using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject TakeEffect;
    [SerializeField]
    private GameObject Indicator;
    [SerializeField]
    private GameObject Content;

    private Transform originalParent;

    protected bool isGrabbing = false;

    private void Start()
    {
        originalParent = this.transform.parent;

        IndicateOn(false);
        GrabOn(false,null);

        if(Content)
            LeanTween.rotateAroundLocal(Content, new Vector3(1,1,1), 12f, 3f).setLoopPingPong();
    }

    public void IndicateOn(bool On)
    {
        if(Indicator)
            Indicator.SetActive(On);
    }

    public void GrabOn(bool On, Transform grabBy)
    {
        //important Flag to update the lines
        isGrabbing = On;

        if (On && grabBy != null) this.transform.SetParent(grabBy, true);
        else transform.SetParent(originalParent, true);

        if (!Indicator || !TakeEffect) return;

        TakeEffect.SetActive(On);
        Indicator.SetActive(!On);
    }
}
