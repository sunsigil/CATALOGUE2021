using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(CircleCollider2D))]

public class Usable : MonoBehaviour
{
    GameObject input_prompt_prefab;
    PopupBubble failure_bubble_prefab;
    PopupBar popup_bar_prefab;

    [SerializeField]
    [Range(0.5f, 3)]
    float use_radius;

    [SerializeField]
    Transform prompt_anchor;
    [SerializeField]
    Transform popup_anchor;

    CircleCollider2D collider;

    Transform _user;
    public Transform user => _user;
    Distline _distline;
    public Distline distline => _distline;

    UnityEvent _on_used;
    public UnityEvent on_used => _on_used;
    public float usability => _distline.progress;

    bool _show_prompt;
    public bool show_prompt
    {
        get => _show_prompt;
        set => _show_prompt = value;
    }

    GameObject input_prompt;

    bool ValidateAnchors()
    {
        bool changes = false;

        if(prompt_anchor == null)
        {
            prompt_anchor = new GameObject("Prompt Anchor").transform;
            prompt_anchor.SetParent(transform);
            prompt_anchor.position = transform.position;
            changes = true;
        }
        if(popup_anchor == null)
        {
            popup_anchor = new GameObject("Popup Anchor").transform;
            popup_anchor.SetParent(transform);
            popup_anchor.position = transform.position;
            changes = true;
        }

        return changes;
    }

    public void RequestUse()
    {
        if(usability >= 1)
        {
            _on_used.Invoke();
        }
    }

    public void Fail(string message)
    {
        PopupBar popup_bar = AssetTools.SpawnComponent(popup_bar_prefab);
        popup_bar.message = message;
    }

    void Awake()
    {
        input_prompt_prefab = Resources.Load<GameObject>("Input Prompt");
        failure_bubble_prefab = Resources.Load<PopupBubble>("Popup Bubble");
        popup_bar_prefab = Resources.Load<PopupBar>("Popup Bar");

        collider = GetComponent<CircleCollider2D>();

        _on_used = new UnityEvent();

        _user = GameObject.FindObjectOfType<User>().transform;
        _distline  = new Distline(_user, transform, use_radius, 3);
    }

    void Update()
    {
        if(show_prompt && usability >= 1)
        {
            if(input_prompt == null){ input_prompt = Instantiate(input_prompt_prefab); }

            Vector3 pos = (_user.position + prompt_anchor.position) * 0.5f;
            input_prompt.transform.position = pos;
        }
        else
        {
            if(input_prompt != null){ Destroy(input_prompt); }
        }
    }

    void OnDrawGizmos()
    {
        if(ValidateAnchors())
        {
            EditorUtility.SetDirty(gameObject);
        }

        collider = GetComponent<CircleCollider2D>();

        Vector3 center = popup_anchor.position;
        Vector3 use_arm = Vector3.right * use_radius;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(popup_anchor.position, use_radius);
    }
}
