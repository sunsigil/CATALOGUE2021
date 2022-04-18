using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcGroup : WidgetGroup
{
    [SerializeField]
    float radius;
    [SerializeField]
    [Range(0, 1)]
    float diameter_fill;
    [SerializeField]
    bool enforce_count_sizing;
    [SerializeField]
    int enforced_sizing_count;

    protected override void Reform()
    {
        float diameter = 2 * radius;
        float chord = diameter_fill * diameter;

        // chord = 2 * r * sin(theta/2)
        // chord / 2r = sin(theta/2)
        // theta / 2 = arcsin(chord/2r)
        // theta = 2 * arcsin(chord/2r)
        float arc = 2 * Mathf.Asin(chord / diameter);

        float arc_per = arc / _subordinates.Count;
        float arc_offset = 0.5f * (Mathf.PI - arc + arc_per);
        float width_per = chord / (enforce_count_sizing ? enforced_sizing_count : _subordinates.Count);

        for(int i = 0; i < _subordinates.Count; i++)
        {
            Vector3 scale = NumTools.XY_Scale(width_per);
            scale.z = transform.localScale.z;
            Vector3 pos = NumTools.XY_Polar(arc_offset + arc_per * i, radius);

            _subordinates[i].transform.localScale = scale;
            _subordinates[i].transform.localPosition = pos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
