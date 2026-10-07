using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioClip resetSound;
    private string targetTag = "Player";
    private AudioSource audioSource;
    // Trava para evitar disparar múltiplos resests em fila
    private bool isResetting = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckObjects();
    }

    public void CarregarCena(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void CheckObjects()
    {
        // Se já estiver em processo de reset, ignora a checagem
        if (isResetting) return;

        // Busca objetos com a tag "Player"
        GameObject[] objectsFound = GameObject.FindGameObjectsWithTag(targetTag);

        if (objectsFound.Length > 0)
        {
            // Player está vivo na cena
        }
        else if (SceneManager.GetActiveScene().buildIndex == 0) // Executa apenas se estiver na cena com buildIndex 0
        {
            audioSource.PlayOneShot(resetSound);
            StartCoroutine(ResetScene());
        }
    }

    IEnumerator ResetScene()
    {
        // Ativa a trava para o Update não chamar esta Corrotina novamente
        isResetting = true;

        yield return new WaitForSeconds(1f);

        // Recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Aguarda 1 frame até que a nova cena seja totalmente carregada
        yield return null;

        // Libera a trava para a nova cena
        isResetting = false;
    }

   
}