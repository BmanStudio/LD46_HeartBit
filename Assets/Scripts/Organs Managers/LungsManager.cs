using System;
using System.Collections;
using UnityEngine;

public class LungsManager : OrganObjectManager
{
    [Header("Lungs Specific Settings:")]
    [SerializeField] private float realeaseKeyMistakeRangeTime = 0.3f;

    private bool isKeyHeld = false;
    private bool shouldReleaseKey = false;
    private bool isKeyHoldStarted = false;
    private float keyReleaseTime;

    public event Action onClickingCorrect;

    protected override void OnBeat()
    {
        OnBeatBehavior();
    } // to disable the normal behaviour. use OnBeatBehavior

    private float releaseKeyTimer = 0;
    private float keyHoldTimer = 0;

    private void Update()
    {
        shouldReleaseKey |= Input.GetKeyUp(m_BeatChannel.organDataGetter().key);

        if (shouldReleaseKey)
        {
            shouldReleaseKey = false;

            isKeyHeld = false;

            Debug.Log("[LUNG] key released");

            if (Time.time >= keyReleaseTime && Time.time <= keyReleaseTime + realeaseKeyMistakeRangeTime)
            {
                Debug.Log("[LUNG] key success");
                SucceedAftarBeat(0);
            }
            else
            {
                Debug.Log("[LUNG] key release failed");
                StartCoroutine("FailedHitBeat", 0);
            }
        }

        if (keyReleaseTime > 0 && !hasDoneThisBeat && Time.time >= keyReleaseTime + realeaseKeyMistakeRangeTime)
        {
            hasDoneThisBeat = true;
            StartCoroutine("FailedHitBeat", 0);
        }


    } // The input check is here

    protected override void OnUserClicked()
    {
        /*if (isHoldStarted)
        {
            StartCoroutine("FailedHitBeat", 0);
        }*/
        
        if (Time.time <= m_BeatChannel.lastBeatTime + Mathf.Min(afterBeatMistakeRangeInSeconds, 
                                                                m_BeatChannel.beatIntervalSeconds - beforeBeatMistakeRangeInSeconds))
        {
            onClickingCorrect?.Invoke();
            hasDoneThisBeat = true;

            isKeyHeld = true;
            m_Animator.SetBool("BreathIn", true);
            StartCoroutine("SetSuccessColor");

            
            keyHoldTimer = 0;
            releaseKeyTimer = 0;

        }

        // all good, keep like that, playing animation
        onClickingCorrect?.Invoke(); // For the monitor UI
    }

    protected override void OnBeatBehavior()
    {
        hasDoneThisBeat = false;

        if (isKeyHeld)
        {
            isKeyHeld = false;
            shouldReleaseKey = true;
        }
        else
        {
            SetColorToFade();
        }

        keyReleaseTime = Time.time + afterBeatMistakeRangeInSeconds;
    }

    protected override IEnumerator SetSuccessColor()
    {
        SetMaterial(pressed);

        yield return new WaitWhile(() => isKeyHeld == true);

        yield return new WaitForSeconds(resetColorAfterSuccessTime);

        if (isInFailRoutine) { yield break; }

        ResetOrganColor();
    }

    protected override void SetColorToFade()
    {
        SetMaterial(needToPress);

        keyUIImage.color = new Color(keyUIImage.color.r, keyUIImage.color.g, keyUIImage.color.b, 255f);

        StartCoroutine("FadeKeyAfterDelay", afterBeatMistakeRangeInSeconds);
    }

    protected override void SucceedAftarBeat(float offAfterBeat)
    {
        base.SucceedAftarBeat(offAfterBeat);

        m_Animator.SetBool("BreathIn", false);
    }

    protected override void PlayFailureAnimation()
    {
        m_Animator.SetBool("BreathIn", false);
    }
}
