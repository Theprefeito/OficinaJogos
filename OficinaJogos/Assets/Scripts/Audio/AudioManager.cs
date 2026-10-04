using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Instância única do Singleton
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;

        [HideInInspector] public AudioSource source;
    }

    [Header("Coleção de Músicas e Efeitos")]
    public Sound[] musicTracks;
    public Sound[] sfxClips;

    [Header("Fonte Principal de Música")]
    [SerializeField] private AudioSource musicSource;

    private Sound currentMusic;

    private void Awake()
    {
        // Garante que só existe uma instância no jogo
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSounds()
    {
        // Se não tiver um AudioSource arrastado no Inspector, cria um automaticamente para SFX
        foreach (Sound s in sfxClips)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        // Garante um AudioSource para a música de fundo
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Toca um efeito sonoro (SFX) pelo nome.
    /// </summary>
    public void PlaySFX(string soundName)
    {
        Sound s = Array.Find(sfxClips, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning($"Efeito sonoro '{soundName}' não foi encontrado!");
            return;
        }
        s.source.Play();
    }

    /// <summary>
    /// Troca a música atual por uma nova.
    /// </summary>
    /// <param name="songName">Nome registrado na lista musicTracks</param>
    public void SwitchSong(string songName)
    {
        Sound s = Array.Find(musicTracks, sound => sound.name == songName);

        if (s == null)
        {
            Debug.LogWarning($"Música '{songName}' não foi encontrada!");
            return;
        }

        // Se já estiver tocando essa música, não reinicia
        if (currentMusic != null && currentMusic.name == songName && musicSource.isPlaying)
            return;

        currentMusic = s;
        musicSource.Stop();
        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.loop = true; // Músicas de fundo geralmente rodam em loop
        musicSource.Play();
    }

    /// <summary>
    /// Para a música atual.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}