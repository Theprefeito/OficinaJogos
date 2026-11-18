using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Reset de cena")]
    public AudioClip resetSound;
    [SerializeField] private float resetDelay = 1f;
    [Tooltip("buildIndex das cenas em que a morte do player reinicia a fase")]
    [SerializeField] private int[] resettableScenes = { 1, 2 };

    private AudioSource audioSource;

    // Player atual (cacheado, sem precisar procurar na cena a cada frame)
    private PlayerHealth playerHealth;
    private bool playerWasFound;

    // Trava para evitar disparar múltiplos resets em fila
    private bool isResetting = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Cobre a primeira cena do jogo
        FindPlayer();
    }

    void OnDestroy()
    {
        if (Instance != this) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeFromPlayer();
    }

    void Update()
    {
        CheckPlayerMissing();
    }

    public void CarregarCena(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // ---------- Detecção do player ----------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nova cena: libera a trava e procura o novo player
        isResetting = false;
        FindPlayer();
    }

    private void FindPlayer()
    {
        UnsubscribeFromPlayer();

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerWasFound = playerHealth != null;

        if (playerHealth != null)
        {
            playerHealth.Died += OnPlayerDied;
        }
    }

    private void UnsubscribeFromPlayer()
    {
        if (playerHealth != null) playerHealth.Died -= OnPlayerDied;
        playerHealth = null;
        playerWasFound = false;
    }

    // Caminho principal: o player morreu (ele continua na cena, só fica sem controle)
    private void OnPlayerDied()
    {
        TriggerReset();
    }

    // Caminho de segurança: o player existia e sumiu da cena (ex.: foi destruído por outro script)
    private void CheckPlayerMissing()
    {
        if (isResetting || !playerWasFound) return;

        // "== null" do Unity também é verdadeiro para objetos destruídos
        if (playerHealth == null)
        {
            TriggerReset();
        }
    }

    // ---------- Reset ----------
    private void TriggerReset()
    {
        if (isResetting) return;
        if (!IsResettableScene()) return;

        if (audioSource != null && resetSound != null)
        {
            audioSource.PlayOneShot(resetSound);
        }

        StartCoroutine(ResetScene());
    }

    private bool IsResettableScene()
    {
        int current = SceneManager.GetActiveScene().buildIndex;

        foreach (int index in resettableScenes)
        {
            if (index == current) return true;
        }
        return false;
    }

    IEnumerator ResetScene()
    {
        // Ativa a trava para não chamar esta Corrotina novamente
        isResetting = true;

        yield return new WaitForSeconds(resetDelay);

        // Recarrega a cena atual. A trava é liberada no OnSceneLoaded, quando a nova cena estiver pronta.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}