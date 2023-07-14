using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private static GameMaster instance;
    public Vector2 lastCheckPointPos;
    private MixerController mixer;

    public List<GameObject> crates;
    public List<Vector2> cratesOriginPosition;

    private void Awake()
    {
        for(int i = 0; i < crates.Count; i++)
        {
            cratesOriginPosition[i] = crates[i].transform.position;
        }

        mixer = gameObject.AddComponent<MixerController>();

        mixer.SetMusicVolume(Mathf.Pow(10, (PlayerPrefs.GetFloat("VolumeMus") / 20f)));
        mixer.SetSFXVolume(Mathf.Pow(10, (PlayerPrefs.GetFloat("VolumeSfx") / 20f)));

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {

            Destroy(gameObject);
        }
    }
}
