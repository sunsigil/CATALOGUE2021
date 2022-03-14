// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class WayshrineMenu : Controller
// {
//     enum MenuState
//     {
//         EXPANDING,
//         IDLE,
//         COLLAPSING
//     }
//     MenuState state;
//
//     [SerializeField]
//     GameObject background_object;
//     [SerializeField]
//     GameObject display_object;
//     [SerializeField]
//     GameObject[] stamps;
//
//     Camera camera;
//     Catalogue catalogue;
//
//     LerpGroup lerp_group;
//     float background_radius;
//     float display_radius;
//
//     float expansion_duration = 0.25f;
//     float expansion_timer;
//     float expansion_progress => expansion_timer / expansion_duration;
//
//     float collapse_duration = 0.125f;
//     float collapse_timer;
//     float collapse_progress => collapse_timer / collapse_duration;
//
//     void Awake()
//     {
//         camera = Camera.main;
//         catalogue = FindObjectOfType<Catalogue>();
//
//         for(int i = 0; i < stamps.Length; i++)
//         {
//             stamps[i].SetActive(catalogue.GetWayshrine(i));
//         }
//
//         float w = Screen.width;
//         float h = Screen.height;
//
//         Vector3 screen_corner = new Vector3(w, h, 1);
//         Vector3 screen_center = new Vector3(w/2, h/2, 1);
//         Vector3 corner_point = camera.ScreenToWorldPoint(screen_corner);
//         corner_point.z = 0;
//         Vector3 center_point = camera.ScreenToWorldPoint(screen_center);
//         center_point.z = 0;
//
//         float screen_span = (corner_point - center_point).magnitude;
//         background_radius = screen_span * 1.15f;
//         display_radius = screen_span * 0.35f;
//
//         transform.position = center_point;
//
//         lerp_group = new LerpGroup();
//         lerp_group.RegisterScale(background_object.transform, CowTools.ScaleXY(0), CowTools.ScaleXY(background_radius * 2));
//         lerp_group.RegisterScale(display_object.transform, CowTools.ScaleXY(0), CowTools.ScaleXY(display_radius * 2));
//
//         expansion_timer = 0;
//     }
//
//     void Update()
//     {
//         if(state == MenuState.IDLE)
//         {
//             if
//             (
//                 Pressed(InputCode.INVENTORY) ||
//                 Pressed(InputCode.CANCEL)
//             )
//             {
//                 state = MenuState.COLLAPSING;
//                 collapse_timer = 0;
//             }
//         }
//     }
//
//     void FixedUpdate()
//     {
//         if(state == MenuState.EXPANDING)
//         {
//             lerp_group.UpdateTransforms(expansion_progress);
//
//             expansion_timer += Time.fixedDeltaTime;
//             if(expansion_progress >= 1)
//             {
//                 state = MenuState.IDLE;
//             }
//         }
//         else if(state == MenuState.COLLAPSING)
//         {
//             lerp_group.UpdateTransforms(1-collapse_progress);
//
//             collapse_timer += Time.fixedDeltaTime;
//             if(collapse_progress >= 1)
//             {
//                 Destroy(gameObject);
//             }
//         }
//     }
// }
