using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeAnimal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveLocalZ(gameObject, 30, 30).setEaseInOutBack().setLoopPingPong();
    }

}
