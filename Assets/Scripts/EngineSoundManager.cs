using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class EngineSoundManager : MonoBehaviour
{

    [Header("AudioClips")]
    public AudioClip starting;
    public AudioClip rolling;
    public AudioClip stopping;

    [Header("pitch parameter")]
    public float flatoutSpeed = 20.0f;
    [Range(0.0f, 3.0f)]
    public float minPitch = 0.7f;
    [Range(0.0f, 0.1f)]
    public float pitchSpeed = 0.05f;

    private AudioSource source;
    private AltCarController carController;

    void Start()
    {
        source = GetComponent<AudioSource>();
        carController = GetComponent<AltCarController>();
    }

    void Update()
    {
        if (carController.Handbrake != 1 && (source.clip == stopping || source.clip == null))
        {
            source.volume = 0.5f;
            source.clip = starting;
            source.Play();
            source.pitch = 1;
        }

        if (carController.Handbrake != 1 && !source.isPlaying)
        {
            source.volume = 0.8f;
            source.clip = rolling;
            source.Play();
        }

        if (carController.Handbrake == 1 && source.clip == rolling)
        {
            source.volume = 0.4f;
            source.clip = stopping;
            source.Play();
        }

        if (source.clip == rolling)
        {
            //source.pitch = Mathf.Lerp(source.pitch, minPitch + Mathf.Abs(carController.rb.velocity.magnitude) / flatoutSpeed, pitchSpeed);
        }
    }
}
