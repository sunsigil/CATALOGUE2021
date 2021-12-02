using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clutter : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float _weight; public float weight => _weight;

    SpriteRenderer sprite_renderer;

    public float width => sprite_renderer.bounds.size.x;
    public float x => transform.position.x;
    public float back => x - width/2;
    public float front => x + width/2;
    public Vector2 span => new Vector2(back, front);

    public void Initialize()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public void AlignToLayer(int id, int sublayer)
    {
        sprite_renderer.sortingLayerID = id;
        sprite_renderer.sortingOrder = sublayer;
    }

    void Awake()
    {
        Initialize();
    }

    void OnDrawGizmos()
    {
        Initialize();

        float width = sprite_renderer.bounds.size.x;
        float height = sprite_renderer.bounds.size.y;

        Vector3 center = transform.position + Vector3.up * sprite_renderer.bounds.center.y;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(width, height, 1));
    }
}
