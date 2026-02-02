using UnityEngine;

[CreateAssetMenu(menuName = "Racing/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    public string vehicleName;

    [Header("Prefab")]
    public GameObject vehiclePrefab;

    [Header("Specs")]
    public float acceleration = 35f;
    public float turnStrength = 120f;
    public float maxSpeed = 18f;

    [Header("Audio")]
    public AudioClip hornClip;
}
