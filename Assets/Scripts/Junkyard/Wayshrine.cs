// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class Wayshrine : MonoBehaviour, IUsable
// {
// 	[SerializeField]
// 	Texture2D template;
// 	[SerializeField]
// 	WayshrineToken token;
//
// 	[SerializeField]
// 	GameObject sentinel_prefab;
//
// 	[SerializeField]
// 	float height;
//
// 	Catalogue catalogue;
//
// 	LerpGroup lerp_group;
//
// 	Color[] colours;
// 	int rows;
// 	int cols;
//
// 	Vector3 origin;
// 	float element_size;
// 	List<GameObject> sentinels;
//
// 	bool active;
// 	float duration = 0.25f;
// 	float timer;
// 	float progress => timer / duration;
//
// 	public void Use()
// 	{
// 		catalogue.AddWayshrine(token);
//
// 		/*active = true;
//
// 		foreach(GameObject sentinel in sentinels)
// 		{
// 			sentinel.SetActive(true);
// 		}*/
// 	}
//
// 	void Awake()
// 	{
// 		catalogue = FindObjectOfType<Catalogue>();
//
// 		lerp_group = new LerpGroup();
//
// 		colours = template.GetPixels();
// 		rows = template.height;
// 		cols = template.width;
//
// 		element_size = sentinel_prefab.transform.localScale.x;
// 		origin = transform.position + new Vector3(-(element_size * cols / 2f), height, 0);
// 		sentinels = new List<GameObject>();
//
// 		for(int i = 0; i < rows; i++)
// 		{
// 			for(int j = 0; j < cols; j++)
// 			{
// 				Color colour = colours[i * cols + j];
//
// 				if(colour == Color.black)
// 				{
// 					GameObject sentinel = Instantiate(sentinel_prefab, null);
// 					sentinels.Add(sentinel);
// 					sentinel.transform.position = transform.position + Vector3.up * height;
// 					sentinel.SetActive(false);
//
// 					lerp_group.RegisterPosition(sentinel.transform, sentinel.transform.position, origin + new Vector3(j, i, 0) * element_size);
// 				}
// 			}
// 		}
// 	}
//
// 	void FixedUpdate()
// 	{
// 		if(active)
// 		{
// 			lerp_group.UpdateTransforms(progress);
//
// 			timer += Time.fixedDeltaTime;
// 		}
// 	}
//
// 	void OnDrawGizmos()
// 	{
// 		Gizmos.color = Color.blue;
// 		Gizmos.DrawWireSphere(transform.position + Vector3.up * height, 1);
// 	}
// }
