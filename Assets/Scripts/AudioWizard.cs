using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioWizard : MonoBehaviour
{
	static AudioWizard instance;
    public static AudioWizard _ => instance;

	[SerializeField]
	int garbage_threshold;
	[SerializeField]
	float garbage_cooldown;

	Dictionary<string, AudioClip> clip_map;
	AudioSource music_source;
	List<AudioSource> effect_source_pool;

	float garbage_timer;

	public void SetMusic(string name)
	{
		music_source.clip = clip_map[name];
	}

	public void PlayEffect(string name)
	{
		AudioSource source = null;

		foreach(AudioSource candidate in effect_source_pool)
		{
			if(!candidate.isPlaying)
			{
				source = candidate;
				break;
			}
		}

		if(source == null)
		{
			source = new GameObject("Effect Source").AddComponent<AudioSource>();
		}

		source.clip = clip_map[name];
		source.spatialize = false;
		source.Play();
	}

	void Awake()
	{
		if(!instance){instance = this;}
		else{Destroy(gameObject);}

		clip_map = ResourceTools.MapResources<AudioClip>("Clips");

		music_source = new GameObject("Music Source").AddComponent<AudioSource>();
		music_source.spatialize = false;

		effect_source_pool = new List<AudioSource>();
	}

	void Update()
	{
		if
		(
			garbage_timer > garbage_cooldown &&
			effect_source_pool.Count > garbage_threshold
		)
		{
			for(int i = 0; i < effect_source_pool.Count; i++)
			{
				AudioSource candidate = effect_source_pool[i];

				if(!candidate.isPlaying)
				{
					effect_source_pool.RemoveAt(i);
					Destroy(candidate.gameObject);
				}
			}

			garbage_timer = 0;
		}

		garbage_timer += Time.deltaTime;
	}
}
