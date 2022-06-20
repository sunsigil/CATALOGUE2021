using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
    [SerializeField]
    Transform body;
    [SerializeField]
    Transform dot;
    [SerializeField]
    Transform left_dot;
    [SerializeField]
    Transform right_dot;
    [SerializeField]
    LineRenderer line_renderer;

    [SerializeField]
    float inner_dist_bound;
    [SerializeField]
    float outer_dist_bound;

    TransformSnapshot body_start;
    TransformSnapshot body_end;
    TransformSnapshot dot_start;
    TransformSnapshot dot_end;
    TransformSnapshot left_dot_start;
    TransformSnapshot left_dot_end;
    TransformSnapshot right_dot_start;
    TransformSnapshot right_dot_end;
    float line_width_target;

    Walker walker;

    [SerializeField]
    Distline distline;

    void Awake()
    {
        walker = FindObjectOfType<Walker>();
        distline = new Distline(walker.transform, transform, inner_dist_bound, outer_dist_bound);

        body_start = new TransformSnapshot(NumTools.XY_Scale(0), body.rotation, body.position);
        body_end = new TransformSnapshot(body);
        dot_start = new TransformSnapshot(NumTools.XY_Scale(0), dot.rotation, dot.position);
        dot_end = new TransformSnapshot(dot);
        left_dot_start = new TransformSnapshot(NumTools.XY_Scale(0), left_dot.rotation, dot.position);
        left_dot_end = new TransformSnapshot(left_dot);
        right_dot_start = new TransformSnapshot(NumTools.XY_Scale(0), right_dot.rotation, dot.position);
        right_dot_end = new TransformSnapshot(right_dot);
        line_width_target = line_renderer.widthCurve[0].value;

        line_renderer.SetWidth(0, 0);
        line_renderer.SetPosition(0, dot.localPosition);
        line_renderer.SetPosition(1, dot.localPosition);
    }

    void FixedUpdate()
    {
        TransformSnapshot.Interpolate(body_start, body_end, distline.progress).Write(body);
        TransformSnapshot.Interpolate(dot_start, dot_end, distline.progress).Write(dot);
        TransformSnapshot.Interpolate(left_dot_start, left_dot_end, distline.progress).Write(left_dot);
        TransformSnapshot.Interpolate(right_dot_start, right_dot_end, distline.progress).Write(right_dot);

        float line_width = Mathf.Lerp(0, line_width_target, distline.progress);
        line_renderer.SetWidth(line_width, line_width);
        line_renderer.SetPosition(0, left_dot.localPosition);
        line_renderer.SetPosition(1, right_dot.localPosition);
    }
}
