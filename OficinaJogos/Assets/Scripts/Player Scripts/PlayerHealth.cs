using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fonte única da verdade sobre a vida do player.
/// Não sabe nada de animação, câmera, som ou Charge: só guarda a vida
/// e AVISA os outros scripts através de eventos.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 1;

    [Header("Eventos (opcional, para ligar pelo Inspector)")]
    public UnityEvent onDied;

    // ---------- Estado ----------
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvulnerable => invulnerabilitySources.Count > 0;

    // ---------- Eventos C# (outros scripts se inscrevem aqui) ----------
    /// <summary>(dano recebido, quem causou)</summary>
    public event Action<int, GameObject> Damaged;
    /// <summary>(quantidade curada)</summary>
    public event Action<int> Healed;
    public event Action Died;
    /// <summary>Tentou tomar dano mas estava invulnerável (bom para som/efeito de "defendeu").</summary>
    public event Action<GameObject> DamageBlocked;

    // Cada script que quiser deixar o player invulnerável se registra como "fonte".
    // Assim, Charge, Dash, cutscene etc. não se atrapalham entre si.
    private readonly HashSet<object> invulnerabilitySources = new HashSet<object>();

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    // ---------- Invulnerabilidade ----------
    public void AddInvulnerability(object source) => invulnerabilitySources.Add(source);
    public void RemoveInvulnerability(object source) => invulnerabilitySources.Remove(source);

    // ---------- Dano / Cura ----------
    /// <summary>Retorna true se o dano foi aplicado.</summary>
    public bool TakeDamage(int amount, GameObject source = null)
    {
        if (IsDead || amount <= 0) return false;

        if (IsInvulnerable)
        {
            DamageBlocked?.Invoke(source);
            return false;
        }

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        Damaged?.Invoke(amount, source);

        if (CurrentHealth == 0) Die();
        return true;
    }

    /// <summary>Mata o player, mas respeita a invulnerabilidade (ex.: colisão com inimigo).</summary>
    public bool Kill(GameObject source = null) => TakeDamage(CurrentHealth, source);

    /// <summary>Mata o player ignorando invulnerabilidade (ex.: cair no vazio).</summary>
    public void ForceKill()
    {
        if (IsDead) return;
        CurrentHealth = 0;
        Die();
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;

        int before = CurrentHealth;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        if (CurrentHealth > before) Healed?.Invoke(CurrentHealth - before);
    }

    /// <summary>Útil para respawn/reiniciar fase.</summary>
    public void ResetHealth()
    {
        IsDead = false;
        CurrentHealth = maxHealth;
        invulnerabilitySources.Clear();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Died?.Invoke();
        onDied?.Invoke();
    }
}
