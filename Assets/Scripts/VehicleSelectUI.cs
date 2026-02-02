using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleSelectUI : MonoBehaviour
{
    public VehicleData[] vehicles;
    public string raceSceneName = "RaceScene";

    int currentIndex = 0;

    void Start()
    {
        SelectVehicle(0); // default selection
    }

    public void SelectVehicle(int index)
    {
        if (vehicles == null || vehicles.Length == 0) return;

        currentIndex = Mathf.Clamp(index, 0, vehicles.Length - 1);
        GameManager.Instance.selectedVehicle = vehicles[currentIndex];
    }

    public void StartRace()
    {
        if (GameManager.Instance.selectedVehicle == null)
            GameManager.Instance.selectedVehicle = vehicles[0];

        SceneManager.LoadScene(raceSceneName);
    }
}
