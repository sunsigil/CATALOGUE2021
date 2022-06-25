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

	AudioSource music_source;
	List<AudioSource> effect_source_pool;

	Dictionary<string, AudioClip> clip_map;
	Stack<Pair<GameObject, AudioClip>> music_stack;

	float garbage_timer;

	public void PushMusic(GameObject sender, AudioClip clip)
	{
		music_stack.Push(new Pair<GameObject, AudioClip>(sender, clip));
		music_source.clip = clip;
		music_source.Play();
	}

	public void PushMusic(GameObject sender, string name)
	{
		if(clip_map.ContainsKey(name))
		{
			PushMusic(sender, clip_map[name]);
		}
	}

	public void PopMusic()
	{
		if(music_stack.Count > 0)
		{
			music_stack.Pop();
			music_source.clip = null;
			music_source.Pause();

			if(music_stack.Count > 0)
			{
				music_source.clip = music_stack.Peek().b;
				music_source.Play();
			}
		}
	}

	public void ToggleMusic(bool toggle)
	{
		if(toggle){ music_source.Play(); }
		else{ music_source.Pause(); }
	}

	public void PlayEffect(AudioClip clip)
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

		source.clip = clip;
		source.spatialize = false;
		source.Play();
	}

	public void PlayEffect(string name)
	{
		if(clip_map.ContainsKey(name))
		{
			PlayEffect(clip_map[name]);
		}
	}

	void Awake()
	{
		if(!instance){instance = this;}
		else{Destroy(gameObject);}

		clip_map = AssetTools.MapResources<AudioClip>("Clips");
		music_stack = new Stack<Pair<GameObject, AudioClip>>();

		music_source = new GameObject("Music Source").AddComponent<AudioSource>();
		music_source.spatialize = false;
		music_source.loop = true;

		effect_source_pool = new List<AudioSource>();
	}

	void Update()
	{
		if(music_stack != null && music_stack.Count > 0)
		{
			if(music_stack.Peek().a == null)
			{
				PopMusic();
			}
		}

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
