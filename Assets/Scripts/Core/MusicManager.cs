using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> _musicList;
    [SerializeField] AudioSource _audio;

    readonly Queue<AudioClip> _currentQueue = new();

    #region Singleton
    public static MusicManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;

        QueueMusic();
    }
    #endregion

    void QueueMusic()
    {
        foreach (var music in _musicList)
        {
            _currentQueue.Enqueue(music);
        }
    }

    public void PlayMusic()
    {
        if (_currentQueue.TryDequeue(out var clip))
        {
            _audio.Stop();
            _audio.clip = clip;
            _audio.Play();
            StartCoroutine(WaitForNextSong(clip.length));
        }
        else
        {
            QueueMusic();
            PlayMusic();
        }
    }

    IEnumerator WaitForNextSong(float duration)
    {
        yield return new WaitForSeconds(duration);

        PlayMusic();
    }

}
