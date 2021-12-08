using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Clutter Profile")]
public class ClutterProfile : ScriptableObject
{
	[SerializeField]
	Clutter[] _clutter; public Clutter[] clutter => _clutter;
	int _size; public int size => _size;
	float weight_sum;
	float mean_width;

	[SerializeField]
    [Range(0, 1)]
    float _prevalence; public float prevalence => _prevalence;
	[SerializeField]
	bool _ignore_volumes; public bool ignore_volumes => _ignore_volumes;
	[SerializeField]
	string layer;
	[SerializeField]
	int sublayer;
	int layer_ID;
	[SerializeField]
    float _spawn_y; public float spawn_y => _spawn_y;
	public float pad_width => mean_width;

	public void Initialize()
	{
		layer_ID = SortingLayer.NameToID(layer);

		_size = _clutter.Length;
		weight_sum = 0;
		mean_width = 0;

		foreach(Clutter piece in _clutter)
		{
			piece.Initialize();

			piece.AlignToLayer(layer_ID, sublayer);

			weight_sum += piece.weight;
			mean_width += piece.width;
		}

		mean_width /= _size;
	}

	public Clutter GetRandomPiece()
	{
		int i = 0;
		float rand_weight = Random.value * weight_sum;

		while(i < _size)
		{
			rand_weight -= _clutter[i].weight;

			if(rand_weight <= 0){break;}

			i++;
		}

		i = Mathf.Clamp(i, 0, _size-1);
		return _clutter[i];
	}
}
