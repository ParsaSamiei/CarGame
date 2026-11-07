using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] AudioSource musicSource;     // loop, assign background music
    [SerializeField] AudioSource engineSource;    // loop, assign engine clip
    [SerializeField] AudioSource sfxSource;       // one-shots

    [Header("Clips")]
    [SerializeField] AudioClip crashClip;
    [SerializeField] AudioClip coinClip;
    [SerializeField] AudioClip fuelClip;
    [SerializeField] AudioClip fireworkClip;

    [Header("Engine")]
    [SerializeField] Vector2 enginePitch = new Vector2(0.8f, 2.2f);
    [SerializeField] float engineVolume = 0.5f;

    void Awake() => Instance = this;

    void Start()
    {
        musicSource.loop = true; musicSource.Play();
        engineSource.loop = true; engineSource.volume = 0f; engineSource.Play();
    }

    void Update()
    {
        bool playing = GameManager.Instance.State == GameState.Playing;
        engineSource.volume = Mathf.MoveTowards(engineSource.volume, playing ? engineVolume : 0f, Time.unscaledDeltaTime * 2f);
        engineSource.pitch = Mathf.Lerp(enginePitch.x, enginePitch.y, GameManager.Instance.Speed01);
    }

    public void PlayCrash() => Play(crashClip, 1f);
    public void PlayCoin() => Play(coinClip, 0.8f);
    public void PlayFuel() => Play(fuelClip, 0.9f);
    public void PlayFirework() => Play(fireworkClip, 0.35f);

    void Play(AudioClip clip, float volume)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, volume);
    }
}
