using System.Collections;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] AudioClip OpenDoor;
    [SerializeField] AudioClip TimerStop;
    [SerializeField] AudioClip TakePuzzle;
    [SerializeField] AudioClip PutPuzzle;
    [SerializeField] AudioClip TakePuzzleFail;
    [SerializeField] AudioClip PutPuzzleFail;
    [SerializeField] AudioClip Error;
    [SerializeField] AudioClip Finish;
    // TO DO Other sounds can be added here like
    // walking, entry to level, transition to different view, interaction, etc.
    [SerializeField] AudioClip BackgroundMusic_A;
    [SerializeField] AudioClip BackgroundMusic_B;

    private static SoundsManager instance;
    public static SoundsManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private AudioSource audioSource;

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
        Finish
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusicBuildingUp(BackgroundMusic.BackgroundMusic_A, 5f);
    }

    public void PlayMusicBuildingUp(BackgroundMusic backgroundMusic, float buildUpTime = 0.5f)
    {
        audioSource.volume = 0.0f;
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
        while (audioSource.volume < 1f)
        {
            audioSource.volume += step;
            yield return new WaitForSeconds(step);
        }
        audioSource.volume = 1f;
    }

    public void PlayMusic(BackgroundMusic backgroundMusic)
    {
        switch (backgroundMusic)
        {
            case BackgroundMusic.BackgroundMusic_A:
                audioSource.clip = BackgroundMusic_A;
                break;
            case BackgroundMusic.BackgroundMusic_B:
                audioSource.clip = BackgroundMusic_B;
                break;
            default:
                break;
        }
        audioSource.Play();
    }

    // Play single sound, no loop music
    public void PlaySound(Sounds sounds)
    {
        switch (sounds)
        {
            case Sounds.OpenDoor:
                audioSource.PlayOneShot(OpenDoor);
                break;
            case Sounds.TimerStop:
                audioSource.PlayOneShot(TimerStop);
                break;
            case Sounds.TakePuzzle:
                audioSource.PlayOneShot(TakePuzzle);
                break;
            case Sounds.PutPuzzle:
                audioSource.PlayOneShot(PutPuzzle);
                break;
            case Sounds.Error:
                audioSource.PlayOneShot(Error);
                break;
            case Sounds.Finish:
                audioSource.PlayOneShot(Finish);
                break;
            default:
                break;
        }
    }
}
