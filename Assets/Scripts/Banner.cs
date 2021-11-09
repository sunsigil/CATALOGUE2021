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
    [Range(0, 2)]
    float inner_dist_bound;
    [SerializeField]
    [Range(2, 10)]
    float outer_dist_bound;

    LerpGroup lerp_group;
    Vector3 start_body_position;
    Vector3 end_body_position;

    float walker_dist;
    float dist_range => outer_dist_bound - inner_dist_bound;
    float offset_dist => Mathf.Clamp(walker_dist - inner_dist_bound, 0, dist_range);
    float dist_progress => 1-(offset_dist / dist_range);

    Walker walker;

    void Awake()
    {
        lerp_group = new LerpGroup();
        lerp_group.RegisterScale(dot, CowTools.ScaleXY(0), dot.localScale);
        lerp_group.RegisterScale(left_dot, CowTools.ScaleXY(0), left_dot.localScale);
        lerp_group.RegisterScale(right_dot, CowTools.ScaleXY(0), right_dot.localScale);
        lerp_group.RegisterFloat("line_width", 0, line_renderer.widthCurve[0].value);
        lerp_group.RegisterPosition(left_dot, dot.position, left_dot.position);
        lerp_group.RegisterPosition(right_dot, dot.position, right_dot.position);
        lerp_group.Offset(0.5f);

        start_body_position = body.position + Vector3.up * 3.1f;
        end_body_position = body.position;

        walker = FindObjectOfType<Walker>();
    }

    void FixedUpdate()
    {
        walker_dist = Mathf.Abs(transform.position.x - walker.transform.position.x);

        lerp_group.UpdateTransforms(dist_progress);

        float line_width = lerp_group.UpdateFloat("line_width", dist_progress);
        line_renderer.SetWidth(line_width, line_width);
        line_renderer.SetPosition(0, left_dot.position);
        line_renderer.SetPosition(1, right_dot.position);

        float half_progress = Mathf.Clamp(2 * dist_progress - 1, 0, 1);
        body.position = Vector3.Lerp(start_body_position, end_body_position, half_progress);
    }
}
