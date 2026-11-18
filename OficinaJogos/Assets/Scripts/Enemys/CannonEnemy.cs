using System.Collections;
using UnityEngine;

public class CannonEnemy : MonoBehaviour
{
    [SerializeField] private bool playerNoCampoDeVisao;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float forgetPlayerDelay = 3f;

    private Coroutine shootingCoroutine;
    private Coroutine forgetCoroutine;
    private PlayerHealth playerHealth;

    public enum CannonState
    {
        Idle,
        TrackingPlayer
    }

    public CannonState cannonCurrentState;

    void Start()
    {
        playerNoCampoDeVisao = false;
        cannonCurrentState = CannonState.Idle;
    }

    void Update()
    {
        Attack();
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayer();
    }

    void Attack()
    {
        if (playerNoCampoDeVisao)
        {
            cannonCurrentState = CannonState.TrackingPlayer;
        }

        if (cannonCurrentState == CannonState.TrackingPlayer && playerTransform != null)
        {
            // Olha para o Player
            transform.LookAt(playerTransform);

            // Inicia o disparo apenas uma vez
            if (shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(SpawnBullet());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health != null && health.IsDead) return; // não mira em player morto

        // Se o player voltou antes dos 3s, cancela o "esquecer"
        if (forgetCoroutine != null)
        {
            StopCoroutine(forgetCoroutine);
            forgetCoroutine = null;
        }

        playerNoCampoDeVisao = true;
        playerTransform = other.transform;

        SubscribeToPlayer(health);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNoCampoDeVisao = false;

        if (forgetCoroutine != null) StopCoroutine(forgetCoroutine);
        forgetCoroutine = StartCoroutine(ForgetPlayer());
    }

    // ---------- Integração com a vida do player ----------
    private void SubscribeToPlayer(PlayerHealth health)
    {
        if (health == playerHealth) return;

        UnsubscribeFromPlayer();
        playerHealth = health;

        if (playerHealth != null) playerHealth.Died += OnPlayerDied;
    }

    private void UnsubscribeFromPlayer()
    {
        if (playerHealth != null) playerHealth.Died -= OnPlayerDied;
        playerHealth = null;
    }

    private void OnPlayerDied()
    {
        // Para de mirar e atirar assim que o player morre
        playerNoCampoDeVisao = false;

        if (forgetCoroutine != null)
        {
            StopCoroutine(forgetCoroutine);
            forgetCoroutine = null;
        }

        StopTracking();
    }

    private void StopTracking()
    {
        cannonCurrentState = CannonState.Idle;

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    IEnumerator ForgetPlayer()
    {
        yield return new WaitForSeconds(forgetPlayerDelay);

        StopTracking();
        forgetCoroutine = null;
    }

    IEnumerator SpawnBullet()
    {
        while (cannonCurrentState == CannonState.TrackingPlayer)
        {
            // Cria a bala no SpawnPoint
            Instantiate(
                bulletPrefab,
                bulletSpawnPoint.position,
                bulletSpawnPoint.rotation
            );

            // Espera antes de atirar novamente
            yield return new WaitForSeconds(fireRate);
        }

        shootingCoroutine = null;
    }
}