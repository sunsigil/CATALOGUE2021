using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterSpawner : MonoBehaviour
{
    [SerializeField]
    ClutterProfile[] profiles;

    Mapper mapper;
    AntiClutterVolume[] anti_clutter_volumes;

    float start;
    float end;

    float ValidateX(Clutter instance)
    {
        float x = instance.x;

        foreach(AntiClutterVolume volume in anti_clutter_volumes)
        {
            if(volume.IsViolating(instance))
            {
                return volume.NextBestX(instance);
            }
        }

        return x;
    }

    List<Clutter> DrawFromProfile(ClutterProfile profile)
    {
        profile.Initialize();

        List<Clutter> pieces = new List<Clutter>();
        Clutter last_piece = null;

        float free_space = end-start;

        if(!profile.ignore_volumes)
        {
            foreach(AntiClutterVolume volume in anti_clutter_volumes)
            {
                free_space -= volume.width;
            }
        }

        free_space *= profile.prevalence;

        while(free_space > 0)
        {
            if(Random.value <= profile.prevalence)
            {
                Clutter piece = profile.GetRandomPiece();

                if(profile.size > 1)
                {
                    while(piece == last_piece)
                    {
                        piece = profile.GetRandomPiece();
                    }
                }

                pieces.Add(piece);
                free_space -= piece.width;

                last_piece = piece;
            }
            else
            {
                pieces.Add(null);
            }
        }

        return pieces;
    }

    void SpawnPieces(ClutterProfile profile, List<Clutter> pieces)
    {
        GameObject clutter_holder = new GameObject(profile.name);

        float x = start;
        foreach(Clutter piece in pieces)
        {
            if(piece != null)
            {
                Clutter instance = Instantiate(piece.gameObject, clutter_holder.transform).GetComponent<Clutter>();
                instance.transform.position = new Vector3(x, profile.spawn_y, 0);

                if(!profile.ignore_volumes)
                {
                    float last_x = System.Single.NaN;
                    while(last_x != x)
                    {
                        last_x = x;
                        x = ValidateX(instance);
                    }

                    instance.transform.position = new Vector3(x, profile.spawn_y, 0);
                }

                x += instance.width;
            }
            else
            {
                x += profile.pad_width;
            }
        }
    }

    void Start()
    {
        mapper = FindObjectOfType<Mapper>();
        anti_clutter_volumes = FindObjectsOfType<AntiClutterVolume>();

        start = mapper.start.transform.position.x;
        end = mapper.end.transform.position.x;

        foreach(ClutterProfile profile in profiles)
        {
            SpawnPieces(profile, DrawFromProfile(profile));
        }
    }
}
