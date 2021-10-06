using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebugger : MonoBehaviour
{
    [SerializeField]
    Color marker_colour = Color.green;
    [SerializeField]
    Color warning_colour = Color.yellow;
    [SerializeField]
    Color error_colour = Color.red;

    [SerializeField]
    Sprite marker_sprite;
    [SerializeField]
    Sprite warning_sprite;
    [SerializeField]
    Sprite error_sprite;

    SpriteRenderer sprite_renderer;
    Color original_colour;

    GameObject debug_object;
    SpriteRenderer debug_renderer;

    public void SetColour(int code)
    {
        switch(code)
        {
            case 0:
                sprite_renderer.color = original_colour;
            break;

            case 1:
                sprite_renderer.color = marker_colour;
            break;

            case 2:
                sprite_renderer.color = warning_colour;
            break;

            case 3:
                sprite_renderer.color = error_colour;
            break;
        }
    }

    public void SetSprite(int code)
    {
        switch(code)
        {
            case 0:
                debug_renderer.sprite = null;
                debug_renderer.enabled = false;
            break;

            case 1:
                debug_renderer.sprite = marker_sprite;
                debug_renderer.enabled = true;
            break;

            case 2:
                debug_renderer.sprite = warning_sprite;
                debug_renderer.enabled = true;
            break;

            case 3:
                debug_renderer.sprite = error_sprite;
                debug_renderer.enabled = true;
            break;
        }
    }

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        original_colour = sprite_renderer.color;

        debug_object = new GameObject("Visual Debugger");
        debug_object.transform.SetParent(transform);
        debug_renderer = debug_object.AddComponent<SpriteRenderer>();
        debug_renderer.enabled = false;
    }

    void Update()
    {
        Vector3 s = transform.localScale;
        debug_object.transform.localScale = new Vector3(1/s.x, 1/s.y, 1/s.z);
    }
}
