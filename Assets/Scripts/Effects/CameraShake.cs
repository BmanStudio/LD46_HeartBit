
using System;
using UnityEngine;



public class CameraShake : MonoBehaviour
{
    
    [SerializeField] private CameraShakeProfile cameraShakeProfile;
    
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _startScale;

    private float trauma;
    
    private CameraShakeProfile currentProfile;
    
    
    void Start()
    {
        var transformLocalCopy = transform;
        _startPosition = transformLocalCopy.localPosition;
        _startRotation = transformLocalCopy.localRotation;
        _startScale = transformLocalCopy.localScale;
        
    }


    [ContextMenu("Shake")]
    private void ShakeContextMenu()
    {
        Shake();
    }

    public void Shake(CameraShakeProfile profile = null)
    {
        if (profile == null)
        {
            profile = cameraShakeProfile;
        }

        if (profile == null)
        {
            Debug.Log("Profile is null");
            return;
        }

        currentProfile = profile;
        trauma = 1;
    }

    float GetNoise(float seed, float speed)
    {
        return Mathf.PerlinNoise(seed, Time.time * speed) * 2 - 1;
    }
    

    void Update()
    {
        if (currentProfile == null)
        {
            return;
        }
        
        var locationOffset = new Vector3(
            GetNoise(0, currentProfile.shakeSpeed) * currentProfile.xMovement,
            GetNoise(1, currentProfile.shakeSpeed) * currentProfile.yMovement,
            0
            );
        locationOffset *= trauma;
        transform.position = _startPosition + locationOffset;

        var zRotationOffset = GetNoise(10, currentProfile.shakeSpeed) * currentProfile.zRotation;
        zRotationOffset *= (float)Math.Pow(trauma, 2);
        transform.localRotation = Quaternion.Euler(_startRotation.eulerAngles.x, _startRotation.eulerAngles.y, zRotationOffset);
        
        trauma -= (Time.deltaTime / currentProfile.duration);
        if (trauma <= 0)
        {
            currentProfile = null;
            transform.position = _startPosition;
            transform.localRotation = _startRotation;
        }
    }
}
