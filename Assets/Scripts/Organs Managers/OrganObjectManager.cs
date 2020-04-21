using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrganObjectManager : MonoBehaviour
{
    protected static string COLOR_MATERIAL_PARAM = "_BaseColor";
    protected static string EMISSION_COLOR_MATERIAL_PARAM = "_EmissionColor";

    [SerializeField] protected OrganName m_OrganName;

    [Header("Input Check Settings:")]
    [SerializeField] protected float afterBeatMistakeRangeInSeconds = 0.5f;
    [SerializeField] protected float beforeBeatMistakeRangeInSeconds = 0.2f;
    [SerializeField] protected float perfectBeatMistakeRangeInSeconds = 0.07f;
    [SerializeField] protected GameObject organGameObject = null;


    [Header("Failure Indicator")]
    [SerializeField] [Range(0.3f, 1f)] protected float resetColorAfterFailTime = 0.5f;
    [SerializeField] [Range(0.3f, 1f)] protected float resetColorAfterSuccessTime = 0.5f;

    
    [SerializeField] protected Material regular;
    [SerializeField] protected Material needToPress;
    [SerializeField] protected Material pressed;
    [SerializeField] protected Material failed;

    [SerializeField] protected Image keyUIImage;

    // Components:
    protected BeatChannel m_BeatChannel;
    protected Animator m_Animator;

    // Logic Bools:
    protected bool hasDoneThisBeat = false;
    protected bool hasSucceedOnBeforeBeat = false;
    protected bool isInFailRoutine = false;

    // Stored Colors:
    protected Renderer m_organRenderer;

    // Events to be singed to by the UI
    public event Action onOrganFailed;
    public event Action onOrganPerfectTiming;

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        keyUIImage.color = new Color(keyUIImage.color.r, keyUIImage.color.g, keyUIImage.color.b, 0.1f);
    }

    protected virtual void Start()
    {
        m_BeatChannel = BeatLevelManager.instance.GetOrganChannel(m_OrganName);
        m_BeatChannel.OnUserClicked += OnUserClicked;
        m_BeatChannel.OnBeatReached += OnBeat;

        m_organRenderer = organGameObject.GetComponent<Renderer>();
        ResetOrganColor();

    }

    private void OnDestroy()
    {
        m_BeatChannel.OnUserClicked -= OnUserClicked;
        m_BeatChannel.OnBeatReached -= OnBeat;
    }

    protected virtual void OnBeat()
    {
        StartCoroutine("OnBeatCheckPlayerInput");
    }

    private IEnumerator OnBeatCheckPlayerInput()
    {
        OnBeatBehavior();

        if (hasSucceedOnBeforeBeat)
        {
            hasSucceedOnBeforeBeat = false;
            hasDoneThisBeat = true;
        }

        else
        {
            hasDoneThisBeat = false;
        }

        // To check if the mistake range has over and the player didnt clicked
        yield return new WaitForSeconds(afterBeatMistakeRangeInSeconds);

        if (!hasDoneThisBeat) // Failing due to not responding in time
        {
            StartCoroutine("FailedHitBeat", 0);
            hasDoneThisBeat = true;
        }
    }

    protected virtual void OnBeatBehavior()
    {
        // To be Overriden
        if (!hasSucceedOnBeforeBeat)
        {
            SetColorToFade();
        }
    }

    protected virtual void SetColorToFade()
    {
        SetMaterial(needToPress);

        keyUIImage.color = new Color(keyUIImage.color.r, keyUIImage.color.g, keyUIImage.color.b, 255f);

        StartCoroutine("FadeKeyAfterDelay", afterBeatMistakeRangeInSeconds);
    }

    protected virtual void SetColorToFailure()
    {
        SetMaterial(failed);
    }

    protected virtual void OnUserClicked() // To be overriden
    {
        if (hasSucceedOnBeforeBeat)
        {
            StartCoroutine("FailedHitBeat", 0);
            return;
        }

        float offAfterBeat = Mathf.Abs(Time.time - m_BeatChannel.lastBeatTime);

        float nextBeat = m_BeatChannel.lastBeatTime + m_BeatChannel.beatIntervalSeconds;

        float offBeforeBeat = nextBeat - Time.time;

        if (offBeforeBeat < beforeBeatMistakeRangeInSeconds)
        {
            SucceedBeforeBeat(offBeforeBeat);
            hasSucceedOnBeforeBeat = true;
            return;
        }

        if (hasDoneThisBeat)
        {
            StartCoroutine("FailedHitBeat", 0);
            return;
        }

        if (offAfterBeat < perfectBeatMistakeRangeInSeconds || offBeforeBeat < perfectBeatMistakeRangeInSeconds)
        {
            SucceedExactlyAtBeat();
            hasDoneThisBeat = true;
            return;
        }

        else if (offAfterBeat < afterBeatMistakeRangeInSeconds)
        {
            SucceedAftarBeat(offAfterBeat);
            hasDoneThisBeat = true;
            return;
        }

        else
        {
            StartCoroutine("FailedHitBeat", 0);
            hasDoneThisBeat = true;
            return;
        }
    }

    protected virtual void PlayFailureAnimation()
    {
        // to be overriden
    }

    protected virtual void SucceedBeforeBeat(float offBeforeBeat) //To be overriden
    {
        if (isInFailRoutine) { isInFailRoutine = false; }
        StartCoroutine("SetSuccessColor");

    }

    protected virtual void SucceedExactlyAtBeat() //To be overriden
    {
        if (isInFailRoutine) { isInFailRoutine = false; }

        onOrganPerfectTiming?.Invoke(); // For the UI Monitor

        StartCoroutine("SetSuccessColor");
    }

    protected virtual void SucceedAftarBeat(float offAfterBeat) //To be overriden
    {
        if (isInFailRoutine) { isInFailRoutine = false; }
        StartCoroutine("SetSuccessColor");

        //Debug.Log("Yay! you got " + offAfterBeat + " AFTER the beat for " + m_OrganName);
    }

    protected virtual void ResetOrganColor()
    {
        SetMaterial(regular);
    }

    protected IEnumerator FadeKeyAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        keyUIImage.color = new Color(keyUIImage.color.r, keyUIImage.color.g, keyUIImage.color.b, 0.1f);
    }

    protected virtual IEnumerator FailedHitBeat(float offFromBeatInTIme) //To be overriden
    {
        onOrganFailed?.Invoke(); // for the UIMonitor

        EffectManager.Instance.ShowFail(); // Camera shake


        if (isInFailRoutine)
        {
            yield break;
        }

        else // if the player is already in the yield return part, skip the routine and just decrease points
        {
            isInFailRoutine = true;

            SetColorToFailure();

            PlayFailureAnimation(); // To be overriden

        }
        yield return new WaitForSeconds(resetColorAfterFailTime);

        if (!isInFailRoutine) { yield break; } // was modified outside the routine by input succeed

        ResetOrganColor();

        isInFailRoutine = false;
    }
    
    protected virtual IEnumerator SetSuccessColor()
    {
        SetMaterial(pressed);

        yield return new WaitForSeconds(resetColorAfterSuccessTime);

        if (isInFailRoutine) { yield break; }

        ResetOrganColor();
    }

    protected virtual void SetMaterial(Material mat)
    {
        var mats = m_organRenderer.materials;
        mats[0] = mat;
        m_organRenderer.materials = mats;
    }
}
