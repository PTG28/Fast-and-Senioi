using UnityEngine;

public class PlayerVehicleLoader : MonoBehaviour
{
    [Header("Where to attach the selected car model")]
    public Transform modelParent;   // usually a child empty GameObject like "ModelRoot"

    void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.selectedVehicle == null)
        {
            Debug.LogWarning("No vehicle selected. Keeping existing child model.");
            return;
        }

        var data = GameManager.Instance.selectedVehicle;
        if (data.vehiclePrefab == null)
        {
            Debug.LogError("Selected VehicleData has no prefab assigned!");
            return;
        }

        if (modelParent == null) modelParent = transform;

        // 1) Remove existing model children
        for (int i = modelParent.childCount - 1; i >= 0; i--)
        {
            Destroy(modelParent.GetChild(i).gameObject);
        }

        // 2) Spawn selected model as child
        GameObject model = Instantiate(data.vehiclePrefab, modelParent);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        // 3) Apply specs to controller on Player root
        var controller = GetComponent<ArcadeCarController>();
        if (controller != null)
            controller.ApplySpecs(data.acceleration, data.turnStrength, data.maxSpeed);

        // 4) Apply horn clip
        var horn = GetComponent<Horn>();
        if (horn != null)
            horn.SetHorn(data.hornClip);
    }
}
