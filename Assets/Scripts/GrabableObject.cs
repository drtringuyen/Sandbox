using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject TakeEffect;
    [SerializeField]
    private GameObject Indicator;

    private void Awake()
    {
        IndicateOn(false);
        GrabOn(false);
    }

    public void IndicateOn(bool On)
    {
        Indicator.SetActive(On);
    }

    public void GrabOn(bool On)
    {
        TakeEffect.SetActive(On);
        Indicator.SetActive(!On);
    }
}
