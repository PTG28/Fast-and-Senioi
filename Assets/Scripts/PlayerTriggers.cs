using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    public RaceManager raceManager;

    void OnTriggerEnter(Collider other)
    {
        var cp = other.GetComponent<Checkpoint>();
        if (cp != null)
            raceManager.HitCheckpoint(cp.index);
    }
}
