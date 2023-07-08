using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsManager : MonoBehaviour
{
    private Animator anim;
    private bool landing;

    AudioSource animationSoundPlayer;

    [Header("GrassRun")]
    [SerializeField] private List<AudioClip> stepAudioClips;
    [Header("GrassJump")]
    [SerializeField] private List<AudioClip> JumpAudioClips;
    [Header("GrassLand")]
    [SerializeField] private List<AudioClip> LandAudioClips;

    [SerializeField] public LayerMask layerMask;
    private BoxCollider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();

        anim = GetComponent<Animator>();

        coll = GetComponent<BoxCollider2D>();

        landing = GetComponent<PlayerMovement>().onGround;

    }

    public void SelectAndPlayFootstep()
    {

        PlayerFootstepSound(stepAudioClips, JumpAudioClips, LandAudioClips);

    }
    public void PlayerFootstepSound(List<AudioClip> TerrainRunClip, List<AudioClip> TerrainJumpClip, List<AudioClip> TerrainLandClip)
    {

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerRunAnim"))
        {
            animationSoundPlayer.clip = TerrainRunClip[Random.Range(0, TerrainRunClip.Count)];
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerRisingAnim"))
        {
            animationSoundPlayer.clip = TerrainJumpClip[Random.Range(0, TerrainJumpClip.Count)];
        }
        else if (GetComponent<PlayerMovement>().onGround)
        {
            animationSoundPlayer.clip = TerrainLandClip[Random.Range(0, TerrainLandClip.Count)];
        }

        animationSoundPlayer.Play();

    }

}
