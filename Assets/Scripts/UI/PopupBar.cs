using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupBar : Controller
{
    [SerializeField]
    float expand_time;

    [SerializeField]
    string _message;
	public string message
	{
		get => _message;
		set => _message = value;
	}

    Canvas canvas;
	TextMeshProUGUI text;

    Timeline timeline;
    Material material;

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
		text = GetComponentInChildren<TextMeshProUGUI>();

        timeline = new Timeline(expand_time);
        material = GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Progress", 0);

        Camera camera = Camera.main;
		CameraFollow camera_follow = camera.GetComponent<CameraFollow>();
        camera_follow.Snap();

        Vector3 view_edge = new Vector3(1, 0.15f, camera.nearClipPlane);
        Vector3 view_center = new Vector3(0.5f, 0.15f, camera.nearClipPlane);

        Vector3 world_edge = camera.ViewportToWorldPoint(view_edge);
        world_edge.z = 0;
        Vector3 world_center = camera.ViewportToWorldPoint(view_center);
        world_center.z = 0;

        float screen_span = 2 * (world_edge - world_center).magnitude;
        transform.localScale = new Vector3(screen_span, 1, 1);
        canvas.transform.localScale = new Vector3(1/screen_span, 1, 1);

        transform.position = world_center;
    }

    void Update()
	{
        material.SetFloat("_Progress", NumTools.Perlinstep(timeline.progress));
        text.text = timeline.progress > 0.5f ? _message : "";

		if(Pressed(InputCode.CONFIRM) && timeline.Evaluate())
		{
			Destroy(gameObject);
		}

        timeline.Tick(Time.deltaTime);
	}
}
