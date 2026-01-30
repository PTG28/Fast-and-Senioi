using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public TMP_Text hud;
    public int totalCheckpoints = 5;

    int nextIndex = 0;
    float time = 0f;
    bool finished = false;

    void Update()
    {
        if (finished) return;

        time += Time.deltaTime;
        if (hud) hud.text = $"Time: {time:0.00}\nCheckpoint: {nextIndex}/{totalCheckpoints}";
    }

    public void HitCheckpoint(int idx)
    {
        if (finished) return;
        if (idx != nextIndex) return;   // forces correct order

        nextIndex++;

        if (nextIndex >= totalCheckpoints)
        {
            finished = true;
            if (hud) hud.text = $"FINISHED!\nTime: {time:0.00}";
        }
    }
}
