using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class EffectManager : Singleton<EffectManager>
{

    [SerializeField]
    private CameraShake cameraShake;
    [SerializeField]
    private Volume volume;
    
    [ContextMenu("ShowFail")]
    public void ShowFail()
    {
        cameraShake.Shake();
        DOTween.To(() => 1f, value => volume.weight = value, 0, 0.3f);
    }

    public void ShowDeath()
    {
        cameraShake.Shake();
        DOTween.To(() => 1f, value => volume.weight = value, 0, 0.7f);
    }
}
