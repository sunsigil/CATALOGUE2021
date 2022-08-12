using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a collection of
/// IngredientWidgets which are created,
/// rebound, destroyed, and arranged radially
/// in a manner which seeks to avoid waste
/// </summary>
public class IngredientGroup : MonoBehaviour
{
	// Prefabs
	[SerializeField]
	IngredientWidget widget_prefab;

	// Settings
	[SerializeField]
    float radius;
    [SerializeField]
    [Range(0, 1)]
    float diameter_fill;
    [SerializeField]
    bool enforce_count_sizing;
    [SerializeField]
    int enforced_sizing_count;

	// Outsiders
	Satchel satchel;

	// State
	protected List<IngredientWidget> _subordinates;
	public List<IngredientWidget> subordinates => _subordinates;

	/// <summary>
	/// Reflect changes to the
	/// subordinate widget collection
	/// </summary>
	void Reform()
	{
		float diameter = 2 * radius;
		float chord = diameter_fill * diameter;

		// chord = 2 * r * sin(theta/2)
		// chord / 2r = sin(theta/2)
		// theta / 2 = arcsin(chord/2r)
		// theta = 2 * arcsin(chord/2r)
		float arc = 2 * Mathf.Asin(chord / diameter);

		float arc_per = arc / _subordinates.Count;
		float arc_offset = 0.5f * (Mathf.PI - arc + arc_per);
		float width_per = chord / (enforce_count_sizing ? enforced_sizing_count : _subordinates.Count);

		for (int i = 0; i < _subordinates.Count; i++)
		{
			Vector3 scale = NumTools.XY_Scale(width_per);
			scale.z = transform.localScale.z;
			Vector3 pos = NumTools.XY_Polar(arc_offset + arc_per * i, radius);

			_subordinates[i].transform.localScale = scale;
			_subordinates[i].transform.localPosition = pos;
		}
	}

	/// <summary>
	/// Add a new subordinate widget
	/// and bind it to some data
	/// </summary>
	/// <param name="data"></param>
	protected void Add(Ingredient data)
	{
		IngredientWidget widget = AssetTools.SpawnComponent(widget_prefab);
		widget.transform.SetParent(transform);
		_subordinates.Add(widget);

		widget.Bind(data);
	}

	/// <summary>
	/// Create, rebind, and delete subordinate
	/// widgets so that each item in a collection
	/// is bound to a subordinate widget
	/// </summary>
	/// <param name="data"></param>
	public void Fill(List<Ingredient> data)
	{
		int index = 0;

		// Work with existing widgets
		while (index < _subordinates.Count && index < data.Count)
		{
			_subordinates[index].Bind(data[index]);
			index += 1;
		}

		// Clean up excess widgets
		if (_subordinates.Count > data.Count)
		{
			while (index < _subordinates.Count)
			{
				Destroy(_subordinates[index]);
				_subordinates.RemoveAt(index);
				index += 1;
			}
		}
		// Or add new widgets if there's not enough
		else if (_subordinates.Count < data.Count)
		{
			while (index < data.Count)
			{
				Add(data[index]);
				index += 1;
			}
		}

		Reform();
	}

	void Awake()
	{
		satchel = FindObjectOfType<Satchel>();

		_subordinates = new List<IngredientWidget>();
		for (int i = 0; i < transform.childCount; i++)
		{ _subordinates.Add(transform.GetChild(i).GetComponent<IngredientWidget>()); }

		Reform();
	}

	void Start()
	{
		satchel.on_add.AddListener(delegate { Fill(satchel.contents); });
		satchel.on_remove.AddListener(delegate { Fill(satchel.contents); });
		Fill(satchel.contents);
	}

	void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
