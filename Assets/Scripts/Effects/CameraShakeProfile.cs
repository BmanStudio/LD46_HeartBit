using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeProfile", menuName = "Camera Shake Profile")]
public class CameraShakeProfile : ScriptableObject
{
    [SerializeField] public float shakeSpeed;
        
    [SerializeField] public float xMovement;
    [SerializeField] public float yMovement;
    //[SerializeField] public bool randomMovementSign = true;

    [SerializeField] public float zRotation;
        
    [SerializeField] public float duration;

        
    // [SerializeField] private bool uniformScale;
    // [SerializeField] private float maxXScale;
    // [SerializeField] private float maxYScale;
}