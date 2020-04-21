using Doozy.Engine;
using System.Collections;
using UnityEngine;

public class HeartManager : OrganObjectManager
{
    [Header("Heart Spesific Settings:")]
    [SerializeField] HeartRoom m_HeartRoom;
    [SerializeField] float heartBeatTime = 1;

    private const string rightHeartAnimatorString = "Pulse1";
    private const string leftHeartAnimatorString = "Pulse2";


    protected override void OnBeatBehavior()
    {
        base.OnBeatBehavior();
    }

    protected override void SetMaterial(Material mat)
    {
        var mats = m_organRenderer.materials;
        if (m_HeartRoom == HeartRoom.Left)
        {
            mats[0] = mat;
        }
        else
        {
            mats[1] = mat;
        }
        m_organRenderer.materials = mats;
    }

    private IEnumerator Exhale(string animatorBool)
    {
        if (!m_Animator.GetBool(animatorBool))
        {
            Debug.Log("Some problem with " + m_HeartRoom + "'s bool parameter. is set to false when it shouldnt");
        }
        yield return new WaitForSeconds(heartBeatTime / 2);

        m_Animator.SetBool(animatorBool, false);
    }

    protected override void SucceedAftarBeat(float offAfterBeat)
    {
        base.SucceedAftarBeat(offAfterBeat);
        if (m_HeartRoom == HeartRoom.Left)
        {
            //GameEventMessage.SendEvent("LeftHeartSucceedAfterBeat");
            m_Animator.SetBool(leftHeartAnimatorString, true);
            StartCoroutine("Exhale", leftHeartAnimatorString);
        }

        if (m_HeartRoom == HeartRoom.Right)
        {
            //GameEventMessage.SendEvent("RightHeartSucceedAfterBeat");
            m_Animator.SetBool(rightHeartAnimatorString, true);
            StartCoroutine("Exhale", rightHeartAnimatorString);
        }
    }

    protected override void SucceedBeforeBeat(float offBeforeBeat)
    {
        base.SucceedBeforeBeat(offBeforeBeat);

        if (m_HeartRoom == HeartRoom.Left)
        {
            m_Animator.SetBool(leftHeartAnimatorString, true);
            StartCoroutine("Exhale", leftHeartAnimatorString);
        }

        if (m_HeartRoom == HeartRoom.Right)
        {
            m_Animator.SetBool(rightHeartAnimatorString, true);
            StartCoroutine("Exhale", rightHeartAnimatorString);
        }
    }

    protected override void PlayFailureAnimation()
    {
        if (m_HeartRoom == HeartRoom.Left)
        {
            GameEventMessage.SendEvent("LeftHeartFailedBeat");
        }

        else if (m_HeartRoom == HeartRoom.Right)
        {
            GameEventMessage.SendEvent("RightHeartFailedBeat");
        }
    }

    protected override void SucceedExactlyAtBeat()
    {
        base.SucceedExactlyAtBeat();

        if (m_HeartRoom == HeartRoom.Left)
        {
            m_Animator.SetBool(leftHeartAnimatorString, true);
            StartCoroutine("Exhale", leftHeartAnimatorString);

        }

        if (m_HeartRoom == HeartRoom.Right)
        {
            m_Animator.SetBool(rightHeartAnimatorString, true);
            StartCoroutine("Exhale", rightHeartAnimatorString);
        }
    }

    public enum HeartRoom
    {
        Left,
        Right
    }
}
