using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates and maintains the physical
/// context where combat takes place.
/// </summary>
public class Arena : MonoBehaviour
{
	// Prefabs
	[SerializeField]
    GameObject disc_prefab;
	[SerializeField]
	GameObject wall_piece;
	[SerializeField]
	GameObject[] wreath_pieces;

	// Settings
	[SerializeField]
	float disc_radius;
	[SerializeField]
	float wreath_radius;
	[SerializeField]
	float absolute_radius;

	[SerializeField]
	int wall_resolution;
	[SerializeField]
	int wreath_resolution;

	[SerializeField]
	float wreath_rps;
	
	// Props
	GameObject disc;
	GameObject wall;
	GameObject wreath;
	GameObject grounds;
	
	// State
	public Vector3 center => transform.position;
	public float limit => absolute_radius;
	public float scale => absolute_radius / disc_radius;

	/// <summary>
	/// Instantiates backing disc in local space.
	/// </summary>
	void SpawnDisc()
	{
		if(disc != null){ Destroy(disc); }
		disc = Instantiate(disc_prefab, transform);

		float diameter = disc_radius * 2;
		disc.transform.localScale = NumTools.XY_Scale(diameter);
	}

	/// <summary>
	/// Instantiates wall in local space.
	/// Wall segments form a regular n-gon around the disc
	/// where n is the wall resolution.
	/// Wall segment colliders do not block entities but can
	/// be used for raycast hit detection.
	/// </summary>
	void SpawnWall()
	{
		if(wall != null){ Destroy(wall); }
		wall = new GameObject("Wall");
		wall.transform.SetParent(transform);
		wall.transform.localPosition = Vector3.zero;

        float piece_arc = 2 * Mathf.PI / wall_resolution;
        // Secant length: 2r * sin(theta / 2)
        float piece_length = 2 * disc_radius * Mathf.Sin(piece_arc * 0.5f);

        for(int i = 0; i < wall_resolution; i++)
        {
            GameObject piece = Instantiate(wall_piece, wall.transform);
            BoxCollider2D collider = piece.GetComponent<BoxCollider2D>();
            Vector3 original_size = collider.size;
			// Scale by 1.1f for "airtight" overlap
            collider.size = new Vector3(piece_length * 1.1f, original_size.y, original_size.z);

            float theta = piece_arc * i;
            Vector3 offset = NumTools.XY_Polar(theta, disc_radius);
            piece.transform.localPosition = offset;

            piece.transform.rotation = NumTools.XY_Quat(theta - Mathf.PI * 0.5f);
        }
	}

	/// <summary>
	/// Instantiates wreath in local space.
	/// Wreath segments form a regular n-gon around the disc
	/// where n is the wall resolution.
	/// </summary>
	void SpawnWreath()
    {
		if(wreath != null){ Destroy(wreath); }
		wreath = new GameObject("Wreath");
		wreath.transform.SetParent(transform);
		wreath.transform.localPosition = Vector3.zero;
		
        float piece_arc = 2 * Mathf.PI / wreath_resolution;
        // Secant length: 2r * sin(theta / 2)
        float piece_width = 2 * wreath_radius * Mathf.Sin(piece_arc * 0.5f);

        for(int i = 0; i < wreath_resolution; i++)
        {
            int prefab_index = Random.Range(0, wreath_pieces.Length);
            GameObject prefab = wreath_pieces[prefab_index];

            GameObject piece = Instantiate(prefab, wreath.transform);
            piece.transform.localScale = NumTools.XY_Scale(piece_width);

            float theta = piece_arc * i;
            Vector3 offset = NumTools.XY_Polar(theta, wreath_radius);
            piece.transform.localPosition = offset;

            piece.transform.rotation = NumTools.XY_Quat(theta - Mathf.PI * 0.5f);
        }
    }

	/// <summary>
	/// Instantiates prefab at cartesian position
	/// in grounds space.
	/// </summary>
	/// <param name="prefab">Prefab to be instantiated</param>
	/// <param name="pos">Grounds-space position</param>
	/// <returns></returns>
	public GameObject Add(GameObject prefab, Vector3 pos)
	{
		GameObject instance = Instantiate(prefab, grounds.transform);
		instance.transform.localPosition = pos;

		return instance;
	}

	/// <summary>
	/// Instantiates prefab at polar position
	/// in grounds space.
	/// </summary>
	/// <param name="prefab">Prefab to be instntiated</param>
	/// <param name="theta">Angle component of grounds-space position</param>
	/// <param name="norm_r">Normalized radial component of grounds-space position</param>
	/// <returns></returns>
	public GameObject Add(GameObject prefab, float theta, float norm_r)
	{
		return Add(prefab, NumTools.XY_Polar(theta, disc_radius * norm_r));
	}

	/// <summary>
	/// Destroy grounds and all of its children,
	/// then instantiate it blank.
	/// </summary>
	public void Clear()
	{
		if(grounds != null){ Destroy(grounds); }
		grounds = new GameObject("Grounds");
		grounds.transform.SetParent(transform);
		grounds.transform.localPosition = Vector3.zero;
	}

	/// <summary>
	/// Instantiate base arena components
	/// and adjust their sizes according to settings.
	/// </summary>
	public void Spawn()
	{
		SpawnDisc();
		SpawnWall();
		SpawnWreath();

		Clear();

		disc.transform.localScale *= scale;
		wall.transform.localScale *= scale;
		wreath.transform.localScale *= scale;
		grounds.transform.localScale *= scale;
	}

	void FixedUpdate()
	{
		if(wreath)
		{
			wreath.transform.Rotate(NumTools.XY_Omega(wreath_rps) * Time.fixedDeltaTime);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, absolute_radius);

		if(disc_radius > 0)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, absolute_radius * (wreath_radius / disc_radius));
		}
	}
}
