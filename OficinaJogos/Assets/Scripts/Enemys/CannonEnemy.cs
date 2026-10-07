using System.Collections;
using UnityEngine;

public class CannonEnemy : MonoBehaviour
{
    [SerializeField] private bool playerNoCampoDeVisao;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    private SphereCollider sphereCollider;
    private Coroutine shootingCoroutine;

    public enum CannonState
    {
        Idle,
        TrackingPlayer
    }

    public CannonState cannonCurrentState;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        playerNoCampoDeVisao = false;
        cannonCurrentState = CannonState.Idle;
    }

    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (playerNoCampoDeVisao)
        {
            cannonCurrentState = CannonState.TrackingPlayer;
        }

        if (cannonCurrentState == CannonState.TrackingPlayer)
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
        if (other.CompareTag("Player"))
        {
            playerNoCampoDeVisao = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNoCampoDeVisao = false;

            StartCoroutine(ForgetPlayer());
        }
    }

    IEnumerator ForgetPlayer()
    {
        yield return new WaitForSeconds(3f);

        cannonCurrentState = CannonState.Idle;

        // Para a Coroutine de tiro
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
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
