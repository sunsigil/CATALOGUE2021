using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WidgetGroup : MonoBehaviour
{
	[SerializeField]
	GameObject widget_prefab;

	protected List<GameObject> _subordinates;
	public List<GameObject> subordinates => _subordinates;

	protected void Assign<T>(GameObject subordinate, T data)
	{
		IBindable bindable = subordinate.GetComponent<IBindable>();
		bindable.Bind((object)data);
	}

	protected void Add<T>(T data)
	{
		GameObject subordinate = Instantiate(widget_prefab);
		subordinate.transform.SetParent(transform);
		_subordinates.Add(subordinate);

		IBindable bindable = subordinate.GetComponent<IBindable>();
		bindable.Bind((object)data);

		Assign<T>(subordinate, data);
	}

	protected abstract void Reform();

	public void Fill<T>(List<T> data)
	{
		int index = 0;

		while(index < _subordinates.Count && index < data.Count)
		{
			Assign(_subordinates[index], data[index]);
			index += 1;
		}

		if(_subordinates.Count > data.Count)
		{
			while(index < _subordinates.Count)
			{
				Destroy(_subordinates[index]);
				_subordinates.RemoveAt(index);
				index += 1;
			}
		}
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
		{
			_subordinates.Add(transform.GetChild(i).gameObject);
		}

		Reform();
	}
}
