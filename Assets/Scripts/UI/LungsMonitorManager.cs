using Doozy.Engine;
using TMPro;
using UnityEngine;

public class LungsMonitorManager : MonoBehaviour
{
    [SerializeField] LungsManager lungsManager = null;

    [SerializeField] TextMeshProUGUI monitorText = null;

    [SerializeField] float regenerationSpeed = 2f;
    [SerializeField] float decreaseOnIdleSpeed = 1f;

    [SerializeField] float startingHealth = 100;

    [SerializeField] float maxHealth = 100;
    [SerializeField] float minHealth = 0;

    [SerializeField] float damagePerFail = 5f;

    [SerializeField] float perfectTimingBonusMultiplier = 1.5f;


    private float currentHealth;

    private bool hasFailedThisBeat = false;
    private bool isClickingCorrect = false;
    private bool isDead = false;

    private float streakMultiplier = 1;

    private void OnEnable()
    {
        if (lungsManager != null)
        {
            lungsManager.onOrganFailed += OnOrganFailedBehavior;
            lungsManager.onOrganFailed += OnOrganPerfectTimingBehavior;
            lungsManager.onClickingCorrect += OnClickingCorrectBehavior;
        }
        else
        {
            Debug.Log(name + " This monitor need some organ manager");
        }
    }
    private void OnDisable()
    {
        if (lungsManager != null)
        {
            lungsManager.onOrganFailed -= OnOrganFailedBehavior;
            lungsManager.onOrganFailed -= OnOrganPerfectTimingBehavior;
            lungsManager.onClickingCorrect -= OnClickingCorrectBehavior;

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

        if (isClickingCorrect)
        {
            SucceedBeat();
        }

        else if (hasFailedThisBeat)
        {
            FailedBeat();
        }

        else
        {
            // decreases the health by idle
            currentHealth = Mathf.Max(minHealth, currentHealth - decreaseOnIdleSpeed * Time.deltaTime);
        }

        if (currentHealth == minHealth)
        {
            Death();
        }

        monitorText.text = ((int)currentHealth).ToString() + "%";
    }

    private void Death()
    {
        if (isDead) { return; }
        isDead = true;
        EffectManager.Instance.ShowDeath();
        GameEventMessage.SendEvent("Death");
    }

    private void SucceedBeat()
    {
        if (currentHealth < maxHealth)
        {
            streakMultiplier += Time.deltaTime * regenerationSpeed;

            currentHealth = Mathf.Min(maxHealth, currentHealth + streakMultiplier * Time.deltaTime);

        }

        isClickingCorrect = false;
    }

    private void FailedBeat()
    {
        streakMultiplier = 1;
        currentHealth = Mathf.Max(minHealth, currentHealth - damagePerFail);
        hasFailedThisBeat = false;
    }

    private void OnOrganFailedBehavior()
    {
        hasFailedThisBeat = true;
    }

    private void OnOrganPerfectTimingBehavior()
    {
        streakMultiplier *= perfectTimingBonusMultiplier;
    }

    private void OnClickingCorrectBehavior()
    {
        isClickingCorrect = true;
    }
}
