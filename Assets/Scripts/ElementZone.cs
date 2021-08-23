using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementZone : MonoBehaviour
{
    [Range(1, 10)]
    public float range;

    public ElementType type;

    public float power = 0.1f;

    public static List<ElementZone> AllAirZone = new List<ElementZone>();
    public static List<ElementZone> AllFireZone = new List<ElementZone>();
    public static List<ElementZone> AllLifeZone = new List<ElementZone>();
    public static List<ElementZone> AllWaterZone = new List<ElementZone>();

    public enum ElementType { air, fire, water, life} ;

    private void OnEnable() => AddToGlobalZones();
    private void OnDisable() => RemoveFromGlobalZones();

    private void OnDrawGizmosSelected()
    {
        switch (this.type)
        {
            case ElementType.air:
                foreach (ElementZone zone in AllAirZone)
                    Gizmos.DrawWireSphere(zone.transform.position, zone.range);
                break;
            case ElementType.fire:
                foreach (ElementZone zone in AllFireZone)
                    Gizmos.DrawWireSphere(zone.transform.position, zone.range);
                break;
            case ElementType.water:
                foreach (ElementZone zone in AllWaterZone)
                    Gizmos.DrawWireSphere(zone.transform.position, zone.range);
                break;
            case ElementType.life:
                foreach (ElementZone zone in AllLifeZone)
                    Gizmos.DrawWireSphere(zone.transform.position, zone.range);
                break;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, range);
    //}

    private void AddToGlobalZones()
    {
        switch (type)
        {
            case ElementType.air:
                AllAirZone.Add(this);
                break;
            case ElementType.fire:
                AllFireZone.Add(this);
                break;
            case ElementType.water:
                AllWaterZone.Add(this);

                break;
            case ElementType.life:
                AllLifeZone.Add(this);
                break;
        }
    }

    private void RemoveFromGlobalZones()
    {
        switch (type)
        {
            case ElementType.air:
                AllAirZone.Remove(this);
                break;
            case ElementType.fire:
                AllFireZone.Remove(this);
                break;
            case ElementType.water:
                AllWaterZone.Remove(this);
                break;
            case ElementType.life:
                AllLifeZone.Remove(this);
                break;
        }
    }


    public static Vector4 GetStatsAtPosition(Vector3 playerPosition)
    {
        float air = 0;
        foreach (var zone in AllAirZone)
        {
            float distance = Vector3.Distance(playerPosition, zone.transform.position);
            if (distance > zone.range) continue;

            else air += zone.power/distance;
        }

        float water = 0;
        foreach (var zone in AllWaterZone)
        {
            float distance = Vector3.Distance(playerPosition, zone.transform.position);
            if (distance < zone.range) water =1;
        }

        float fire = 0;
        foreach (var zone in AllFireZone)
        {
            float distance = Vector3.Distance(playerPosition, zone.transform.position);
            if (distance < zone.range) fire = 1;
        }

        //float life = 0;
        //foreach (var zone in AllLifeZone)
        //{
        //    float distance = Vector3.Distance(playerPosition, zone.transform.position);
        //    if (distance > zone.range) continue;

        //    else life += distance * zone.power;
        //}

        float life = 0;
        foreach (var zone in AllLifeZone)
        {
            float distance = Vector3.Distance(playerPosition, zone.transform.position);
            if (distance > zone.range) continue;

            else life += zone.power / distance;
        }

        return new Vector4(fire, life, water, air);
    }

}
