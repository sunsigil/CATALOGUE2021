using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiTreeVolume : MonoBehaviour
{
    public bool TreeViolating(float pos, float size)
    {
        float tree_back = pos - (size/2);
        float tree_front = pos + (size/2);
        float zone_back = transform.position.x - (transform.localScale.x/2);
        float zone_front = transform.position.x + (transform.localScale.x/2);

        if(tree_back < zone_back && tree_front < zone_back){return false;}
        if(tree_back > zone_front && tree_back > zone_front){return false;}

        return true;
    }

    public float NextBestX(float size)
    {
        return transform.position.x + (transform.localScale.x/2) + size;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
