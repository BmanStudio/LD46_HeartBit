using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLevelManager : MonoBehaviour
{
    public static BeatLevelManager instance; // Singelton

    [SerializeField]
    private LevelConfig m_levelConfig = null;

    [SerializeField]
    private List<BeatChannel> m_BeatChannels = new List<BeatChannel>();

    private Dictionary<OrganName, BeatChannel> m_NameToBeatChannel = new Dictionary<OrganName, BeatChannel>();

    private bool m_isPlaying = true;


    public float timeSinceRoundStarted = 0; // for the death text / scoreboard
    private void Awake()
    {
        timeSinceRoundStarted = 0;
        if (BeatLevelManager.instance == null)
        {
            instance = this;
        }

        m_BeatChannels = new List<BeatChannel>();
        foreach (Organ organ in m_levelConfig.levelOrgans)
        {
            Organ tempOrgan = organ;
            BeatChannel channel = new BeatChannel( () => { return tempOrgan; } );
            channel.lastBeatTime = Time.time - channel.beatIntervalSeconds;

            m_NameToBeatChannel.Add(organ.organName, channel);
            StartCoroutine(updateRoutine(channel));
        }

        m_BeatChannels.AddRange(m_NameToBeatChannel.Values);
    }

    public BeatChannel GetOrganChannel(OrganName organName)
    {
        return m_NameToBeatChannel[organName];
    }

    public void SetOrganOffset(OrganName organName, float offsetPercentage)
    {
        BeatChannel activeChannel = m_NameToBeatChannel[organName];
        float originalIntervalSeconds = (60f / activeChannel.organDataGetter().bpm);
        activeChannel.beatIntervalSeconds = originalIntervalSeconds + (offsetPercentage / 100) * originalIntervalSeconds;
    }

    private void Update()
    {
        foreach (BeatChannel channel in m_NameToBeatChannel.Values)
        { 
            if (Input.GetKeyDown(channel.organDataGetter().key))
            {                
                channel.OnUserClicked?.Invoke();
            }
        }
        timeSinceRoundStarted += Time.deltaTime;
    }

    private IEnumerator updateRoutine(BeatChannel channel)
    {
        while (m_isPlaying)
        {
            //if (Time.time < channel.organTimeToStart)
            if (Time.time < channel.organTimeToStart + (channel.effectiveOffsetPercentage / 100) * channel.beatIntervalSeconds) // added from original above
            {
                //yield return new WaitForSeconds(channel.organTimeToStart - Time.time);
                yield return new WaitForSeconds((channel.organTimeToStart - Time.time) + (channel.effectiveOffsetPercentage / 100) * channel.beatIntervalSeconds); // added from original above
            }

            //Debug.Log("beat reached" + channel.organDataGetter().organName);
            channel.lastBeatTime = Time.time;
            channel.OnBeatReached?.Invoke();


            // To update the resume game by the heart beat ;)
/*            if (channel.organDataGetter().organName == OrganName.Heart_Right)
            {
                GameEventMessage.SendEvent("HeartBeat");
            }*/

            yield return new WaitForSeconds(channel.beatIntervalSeconds);
        }
    }
}

[System.Serializable]
public class BeatChannel: ISerializationCallbackReceiver
{
    public Action OnBeatReached;
    public Action OnUserClicked;

    public float beatIntervalSeconds;

    public float lastBeatTime;

    public Func<Organ> organDataGetter;

    public float organTimeToStart;

    public float effectiveOffsetPercentage;

    //public bool isOffsetChanged = false; // added

    //public float previousOffset; // added

    public BeatChannel(Func<Organ> organGetter)
    {
        organDataGetter = organGetter;

        organTimeToStart = organDataGetter().startAtTime;

        effectiveOffsetPercentage = organDataGetter().offsetPercentage;

        float originalIntervalSeconds = (60f / organDataGetter().bpm);
        //beatIntervalSeconds = originalIntervalSeconds + (organDataGetter().offsetPercentage / 100) * originalIntervalSeconds;

        beatIntervalSeconds = originalIntervalSeconds; // added from original above
    }

    public void OnAfterDeserialize()
    {
        if (organDataGetter == null) return;
        float originalIntervalSeconds = (60f / organDataGetter().bpm);
        //beatIntervalSeconds = originalIntervalSeconds + (effectiveOffsetPercentage / 100) * originalIntervalSeconds;
    }

    public void OnBeforeSerialize()
    {
        // do nothing
    }
}