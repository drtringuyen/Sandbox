using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    PlayerController player;
    public Transform respawnPoint;
    [Range(0,250)]
    public float delay;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (player.transform.position.y < -respawnPoint.position.y+delay) respawnPlayerAbove();
    }


    public void respawnPlayerAbove()
    {
        player.enabled = false;

        Vector3 tempos = Vector3.Lerp(player.transform.position,respawnPoint.position,0.5f);

        player.transform.position = tempos;
        player.enabled = true;
    }

}
