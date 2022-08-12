/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a collection of IBindable
/// widgets which are created, rebound,
/// destroyed in
/// a manner which seeks to avoid waste
/// </summary>
public abstract class WidgetGroup<T> : MonoBehaviour
{
	[SerializeField]
	GameObject widget_prefab;

	protected List<GameObject> _subordinates;
	public List<GameObject> subordinates => _subordinates;

	/// <summary>
	/// Rebind an existing subordinate
	/// widget to some data
	/// </summary>
	/// <param name="subordinate"></param>
	/// <param name="data"></param>
	protected void Assign(GameObject subordinate, T data)
	{
		IBindable<T> bindable = subordinate.GetComponent<IBindable<T>>();
		bindable.Bind(data);
	}

	/// <summary>
	/// Add a new subordinate widget
	/// and bind it to some data
	/// </summary>
	/// <param name="data"></param>
	protected void Add(T data)
	{
		GameObject subordinate = Instantiate(widget_prefab);
		subordinate.transform.SetParent(transform);
		_subordinates.Add(subordinate);

		IBindable<T> bindable = subordinate.GetComponent<IBindable<T>>();
		bindable.Bind(data);

		Assign(subordinate, data);
	}

	/// <summary>
	/// Reflect changes to the
	/// subordinate widget collection
	/// </summary>
	protected abstract void Reform();

	/// <summary>
	/// Create, rebind, and delete subordinate
	/// widgets so that each item in a collection
	/// is bound to a subordinate widget
	/// </summary>
	/// <param name="data"></param>
	public void Fill(List<T> data)
	{
		int index = 0;

		// Work with existing widgets
		while(index < _subordinates.Count && index < data.Count)
		{
			Assign(_subordinates[index], data[index]);
			index += 1;
		}
		
		// Clean up excess widgets
		if(_subordinates.Count > data.Count)
		{
			while(index < _subordinates.Count)
			{
				Destroy(_subordinates[index]);
				_subordinates.RemoveAt(index);
				index += 1;
			}
		}
		// Or add new widgets if there's not enough
		else if(_subordinates.Count < data.Count)
		{
			while(index < data.Count)
			{
				Add(data[index]);
				index += 1;
			}
		}
		
		Reform();
	}

	protected virtual void Awake()
	{
		_subordinates = new List<GameObject>();

		for(int i = 0; i < transform.childCount; i++)
		{ _subordinates.Add(transform.GetChild(i).gameObject); }

		Reform();
	}
}*/
