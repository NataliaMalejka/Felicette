using System.Collections;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] AudioClip OpenDoor;
    [SerializeField, Tooltip("Plays when menu is opened up")] AudioClip TimerStop;
    [SerializeField] AudioClip TakePuzzle;
    [SerializeField] AudioClip PutPuzzle;
    [SerializeField] AudioClip TakePuzzleFail;
    [SerializeField] AudioClip PutPuzzleFail;
    [SerializeField] AudioClip Error;
    [SerializeField] AudioClip Finish;
    [SerializeField] AudioClip TimesUp;
    [SerializeField] AudioClip ChangePerspective;
    [SerializeField] AudioClip Teleportation;
    // TO DO Other sounds can be added here like
    // walking, entry to level, transition to different view, interaction, etc.
    [SerializeField] AudioClip BackgroundMusic_A;
    [SerializeField] AudioClip BackgroundMusic_B;
    [SerializeField] AudioClip RocketLaunch;

    private static SoundsManager instance;
    public static SoundsManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Range(0, 1)] public float musicVolume
    {
        get
        {
            return musicAudioSource.volume;
        }
        set
        {
            musicAudioSource.volume = value;
        }
    }
    [Range(0, 1)] public float sfxVolume
    {
        get
        {
            return sfxAudioSource.volume;
        }
        set
        {
            sfxAudioSource.volume = value;
        }
    }

    private Coroutine playBuildUpMusicCoroutine = null;

    public enum Sounds
    {
        OpenDoor,
        TimerStop,
        TakePuzzle,
        PutPuzzle,
        TakePuzzleFail,
        PutPuzzleFail,
        Error,
        Finish,
        TimesUp,
        ChangePerspective,
        Teleportation,
        RocketLaunch
    }

    public enum BackgroundMusic
    {
        BackgroundMusic_A,
        BackgroundMusic_B
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusicBuildingUp(BackgroundMusic.BackgroundMusic_A, 5f);

        // In order not to go crazy during dev set to 0f, remove later and connect to UI settings:
        sfxVolume = 1f;
        musicVolume = 1f;
    }

    public void PlayMusicBuildingUp(BackgroundMusic backgroundMusic, float buildUpTime = 0.5f)
    {
        musicAudioSource.volume = 0.0f;
        PlayMusic(backgroundMusic);
        if (playBuildUpMusicCoroutine != null)
        {
            StopCoroutine(playBuildUpMusicCoroutine);
        }
        playBuildUpMusicCoroutine = StartCoroutine(PlayMusicBuildingUpCoroutine(buildUpTime));
    }

    private IEnumerator PlayMusicBuildingUpCoroutine(float buildUpTime)
    {
        float step = 1f / buildUpTime * Time.deltaTime;
        while (musicAudioSource.volume < 1f)
        {
            musicAudioSource.volume += step;
            yield return new WaitForSeconds(step);
        }
        musicAudioSource.volume = 1f;
    }

    public void PlayMusic(BackgroundMusic backgroundMusic)
    {
        switch (backgroundMusic)
        {
            case BackgroundMusic.BackgroundMusic_A:
                musicAudioSource.clip = BackgroundMusic_A;
                break;
            case BackgroundMusic.BackgroundMusic_B:
                musicAudioSource.clip = BackgroundMusic_B;
                break;
            default:
                break;
        }
        musicAudioSource.Play();
    }

    // Play single sound, no loop music
    public void PlaySound(Sounds sounds)
    {
        switch (sounds)
        {
        case Sounds.OpenDoor:
            if (OpenDoor != null)
                sfxAudioSource.PlayOneShot(OpenDoor);
            break;
        case Sounds.TimerStop:
            if (TimerStop != null)
                sfxAudioSource.PlayOneShot(TimerStop);
            break;
        case Sounds.TakePuzzle:
            if (TakePuzzle != null)
                sfxAudioSource.PlayOneShot(TakePuzzle);
            break;
        case Sounds.TakePuzzleFail:
            if (TakePuzzleFail != null)
                sfxAudioSource.PlayOneShot(TakePuzzleFail);
            break;
        case Sounds.PutPuzzle:
            if (PutPuzzle != null)
                sfxAudioSource.PlayOneShot(PutPuzzle);
            break;
        case Sounds.PutPuzzleFail:
            if (PutPuzzleFail != null)
                sfxAudioSource.PlayOneShot(PutPuzzleFail);
            break;
            case Sounds.Error:
            if (Error != null)
                sfxAudioSource.PlayOneShot(Error);
            break;
        case Sounds.Finish:
            if (Finish != null)
                sfxAudioSource.PlayOneShot(Finish);
            break;
        case Sounds.TimesUp:
            if (TimesUp != null)
                sfxAudioSource.PlayOneShot(TimesUp);
            break;
        case Sounds.ChangePerspective:
            if (ChangePerspective != null)
                sfxAudioSource.PlayOneShot(ChangePerspective);
            break;
        case Sounds.Teleportation:
            if (Teleportation != null)
                sfxAudioSource.PlayOneShot(Teleportation);
            break;
        case Sounds.RocketLaunch:
            if (RocketLaunch != null)
                sfxAudioSource.PlayOneShot(RocketLaunch);
            break;

        default:
            break;
        }
    }
}
