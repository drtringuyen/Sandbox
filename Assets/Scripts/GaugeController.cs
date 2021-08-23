using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeController : MonoBehaviour
{
    public GameObject FireBar, LifeBar , WaterBar, AirBar;

    private Vector3 scaleFire, scaleLife, scaleWater, scaleAir = new Vector3(1, 1, 1);

    [Header("Water Functions")]
    [Range(0, 5)]
    public float maxWater;
    [Range(0, 5)]
    public float maxWaterwhenCarryFire;
    [Range(0,0.01f)]
    public float waterdrop;

    [Header("Fire Functions")]
    [Range(0, 2)]
    public float maxFire;
    [Range(0, 5)]
    public float maxFireWhenCarryWater;
    [Range(0, 0.01f)]
    public float firedrop;

    [Header("Life Functions")]
    [Range(0, 5)]
    public float maxLife;
    [Range(0, 5)]
    public float maxLifeWhenCarryFire;
    [Range(0, 0.01f)]
    public float lifedrop;

    [Header("View stats")]

    [Range(0, 1)]
    public float fire, life, water, air;

    private void Update()
    {
        updateStats();
        updateGaugeVisual();
    }

    private void updateStats()
    {
        Vector4 newStats = ElementZone.GetStatsAtPosition(transform.position);

        fire += newStats[0]-newStats[2];
        //life = mix of air and water, have faloff, drop faster with fire
        //life += newStats[1] - lifedrop;
        life = newStats[1];
        water += newStats[2] - waterdrop;
        air = newStats[3];


        if (water > maxWater) water = maxWater;
        if (water < 0) water = 0;

        if (fire > maxFire) fire = maxFire;
        if (fire < 0) fire = 0;

        if (life > maxLife) life = maxLife;
        if (life < 0) life = 0;

        // if water and fire still in war
        if ((fire>0 && water > 0))
        {
            //put off the fire to it max when carry water
            if (fire> maxFireWhenCarryWater)
            {
                fire = maxFireWhenCarryWater;
            }
            else fire -= firedrop;

            //only if water still in supply
            if (water>maxWaterwhenCarryFire)
            {
                water = maxWaterwhenCarryFire;// stop at maximum
            }
                
            else water -= waterdrop;//water drop 2x
        }
        
    }

    private void updateGaugeVisual()
    {
        scaleAir.Set(1, 1, air);
        scaleFire.Set(1, 1, fire);
        scaleWater.Set(1, 1, water);
        scaleLife.Set(1, 1, life);

        FireBar.transform.localScale = scaleFire;
        LifeBar.transform.localScale = scaleLife;
        WaterBar.transform.localScale = scaleWater;
        AirBar.transform.localScale = scaleAir;
    }

    internal void FireOn()
    {
        fire = maxFire;
        water = water > maxLifeWhenCarryFire? maxWaterwhenCarryFire : water;
        updateGaugeVisual();
    }

    internal void FireOff()
    {
        fire = 0;
        updateGaugeVisual();
    }
}
