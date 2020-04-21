using Doozy.Engine;
using TMPro;
using UnityEngine;

public class HeartMonitorManager : MonoBehaviour
{
    [SerializeField] OrganObjectManager leftHeartManager = null;

    [SerializeField] OrganObjectManager rightHeartManager = null;

    [SerializeField] TextMeshProUGUI monitorText = null;

    [SerializeField] float regenerationSpeed = 2f;

    [SerializeField] float startingHealth = 100;

    [SerializeField] float maxHealth = 80;

    [SerializeField] float minHealth = 0;

    [SerializeField] float damagePerFail = 5f;

    [SerializeField] float perfectTimingBonusMultiplier = 1.5f;


    private float currentHealth;

    private bool hasFailedThisFrame;
    private bool isDead = false;

    private float streakMultiplier = 1;

    private void OnEnable()
    {
        if (rightHeartManager != null)
        {
            rightHeartManager.onOrganFailed += OnOrganFailedBehavior;
            rightHeartManager.onOrganFailed += OnOrganPerfectTimingBehavior;
        }
        else
        {
            Debug.Log(name + " This monitor need some organ manager");
        }

        if (rightHeartManager != null)
        {
            leftHeartManager.onOrganFailed += OnOrganFailedBehavior;
            leftHeartManager.onOrganFailed += OnOrganPerfectTimingBehavior;
        }
        else
        {
            Debug.Log(name + " This monitor need some organ manager");
        }
    }
    private void OnDisable()
    {
        if (rightHeartManager != null)
        {
            rightHeartManager.onOrganFailed -= OnOrganFailedBehavior;
            rightHeartManager.onOrganFailed -= OnOrganPerfectTimingBehavior;
        }

        if (leftHeartManager != null)
        {
            leftHeartManager.onOrganFailed -= OnOrganFailedBehavior;
            leftHeartManager.onOrganFailed -= OnOrganPerfectTimingBehavior;
        }
    }

    private void Awake()
    {
        if (startingHealth > maxHealth) { startingHealth = maxHealth; }
        currentHealth = startingHealth;
    }

    private void LateUpdate()
    {
        if (isDead) { return; }

        if (!hasFailedThisFrame)
        {
            SucceedBeat();
        }

        else
        {
            FailedBeat();
        }

        if (currentHealth == minHealth)
        {
            Death();
        }

        monitorText.text = ((int)currentHealth).ToString();
    }

    private void FailedBeat()
    {
        streakMultiplier = 1;
        currentHealth = Mathf.Max(minHealth, currentHealth - damagePerFail);
        hasFailedThisFrame = false;
    }

    private void SucceedBeat()
    {
        if (currentHealth < maxHealth)
        {
            streakMultiplier += Time.deltaTime * regenerationSpeed;

            currentHealth = Mathf.Min(maxHealth, currentHealth + streakMultiplier * Time.deltaTime);
        }
    }

    private void Death()
    {
        if (isDead) { return; }
        isDead = true;
        EffectManager.Instance.ShowDeath();
        GameEventMessage.SendEvent("Death");
    }

    private void OnOrganFailedBehavior()
    {
        hasFailedThisFrame = true;
    }

    private void OnOrganPerfectTimingBehavior()
    {
        streakMultiplier *= perfectTimingBonusMultiplier;
    }
}
