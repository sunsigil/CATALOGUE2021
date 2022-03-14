using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Prop : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField]
    Sprite sprite;
    [SerializeField]
    string layer;
    [SerializeField]
    int order;

    [Header("Randomization")]
    [SerializeField]
    [Range(0, 1)]
    float _weight;
    public float weight => _weight;

    SpriteRenderer sprite_renderer;

    public float width => sprite_renderer.bounds.size.x;
    public float x => transform.position.x;
    public float back => x - width/2;
    public float front => x + width/2;

    public void SetSorting(string layer, int order)
    {
        sprite_renderer.sortingLayerID = SortingLayer.NameToID(layer);
        sprite_renderer.sortingOrder = order;
    }

    public bool Overlapping(Prop other)
    {
        float b_b_difference = other.back - back;
        float f_b_difference = other.front - back;
        float b_f_difference = other.back - front;
        float f_f_difference = other.front - front;

        if(b_b_difference > 0 || f_b_difference > 0){return true;}
        if(b_f_difference < 0 || f_f_difference < 0){return true;}

        return false;
    }

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = sprite;

        SetSorting(layer, order);
    }
}
