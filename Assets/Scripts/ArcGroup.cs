using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcGroup<W, T> : WidgetGroup<W, T>
where W : Widget<T>
where T : class
{
    [SerializeField]
    Vector3 offset;
    [SerializeField]
    float radius;
    [SerializeField]
    [Range(0, 1)]
    float diameter_fill;

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
        float width_per = chord / _subordinates.Count;

        for(int i = 0; i < _subordinates.Count; i++)
        {
            Vector3 scale = NumTools.XY_Scale(width_per);
            scale.z = transform.position.z;
            Vector3 pos = offset + (radius * NumTools.XY_Circle(arc_offset + arc_per * i));

            _subordinates[i].transform.localScale = scale;
            _subordinates[i].transform.position = pos;
        }
    }
}
