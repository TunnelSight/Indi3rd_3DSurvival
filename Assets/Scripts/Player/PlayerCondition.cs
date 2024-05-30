using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;
    public PlayerController controller;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    private Dictionary<StatCategory, Coroutine> activeBuffCoroutines = new Dictionary<StatCategory, Coroutine>();

    public float noHungerHealthDecay;

    public event Action OnTakeDamage;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
    }

    void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.curValue == 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0f)  
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }


    public void Die()
    {
        Debug.Log("죽었다.");
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        OnTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue - amount < 0f )
        {
            return false;
        }

        stamina.Subtract(amount);
        return true;
    }

    public void UseConsumable(ItemDataConsumable consumable)
    {
        if (consumable.type == ConsumableType.Instant)
        {
            ApplyInstantEffect(consumable.statCategory, consumable.value);
        }
        else if (consumable.type == ConsumableType.Buff)
        {
            if (activeBuffCoroutines.TryGetValue(consumable.statCategory, out Coroutine existingCoroutine))
            {
                StopCoroutine(existingCoroutine);
            }
            Coroutine newCoroutine = StartCoroutine(ApplyBuffEffect(consumable.statCategory, consumable.value, consumable.duration));
            activeBuffCoroutines[consumable.statCategory] = newCoroutine;
        }
    }

    private void ApplyInstantEffect(StatCategory statCategory, float value)
    {
        switch (statCategory)
        {
            case StatCategory.Health:
                Heal(value);
                break;
            case StatCategory.Hunger:
                Eat(value);
                break;
            case StatCategory.Stamina:
                stamina.Add(value);
                break;
            case StatCategory.Speed:
                controller.moveSpeed += value;
                break;
        }
    }
    private IEnumerator ApplyBuffEffect(StatCategory statCategory, float value, float duration)
    {
        switch (statCategory)
        {
            case StatCategory.Health:
                yield return StartCoroutine(HealthBuffCoroutine(duration, value));
                break;
            case StatCategory.Hunger:
                yield return StartCoroutine(HungerBuffCoroutine(duration, value));
                break;
            case StatCategory.Stamina:
                yield return StartCoroutine(StaminaBuffCoroutine(duration, value));
                break;
            case StatCategory.Speed:
                yield return StartCoroutine(SpeedBuffCoroutine(duration, value));
                break;
        }
    }

    private IEnumerator HealthBuffCoroutine(float duration, float healAmount)
    {
        float healPerSecond = healAmount / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Heal(healPerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator HungerBuffCoroutine(float duration, float hungerAmount)
    {
        float hungerPerSecond = hungerAmount / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Eat(hungerPerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator StaminaBuffCoroutine(float duration, float staminaAmount)
    {
        float staminaPerSecond = staminaAmount / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            stamina.Add(staminaPerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SpeedBuffCoroutine(float duration, float speedMultiplier)
    {
        float originalSpeed = controller.moveSpeed;
        controller.moveSpeed *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        controller.moveSpeed = originalSpeed;
    }

}