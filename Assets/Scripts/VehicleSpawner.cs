using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        var data = GameManager.Instance.selectedVehicle;
        if (data == null || data.vehiclePrefab == null)
        {
            Debug.LogError("No vehicle selected or prefab missing.");
            return;
        }

        GameObject car = Instantiate(data.vehiclePrefab, spawnPoint.position, spawnPoint.rotation);

        // Apply specs to controller
        var controller = car.GetComponentInChildren<ArcadeCarController>();
        if (controller) controller.ApplySpecs(data.acceleration, data.turnStrength, data.maxSpeed);

        // Apply horn
        var horn = car.GetComponentInChildren<Horn>();
        if (horn) horn.SetHorn(data.hornClip);

        // Make sure camera follows it
        var cam = Camera.main;
        var follow = cam ? cam.GetComponent<FollowCamera>() : null;
        if (follow) follow.target = car.transform;
    }
}
