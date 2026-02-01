using UnityEngine;

public class Horn : MonoBehaviour
{
    public AudioSource source;
    public AudioClip hornClip;
    public KeyCode hornKey = KeyCode.H;

    void Awake()
    {
        if (!source) source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!source || !hornClip) return;

        if (Input.GetKeyDown(hornKey))
            source.PlayOneShot(hornClip);
    }
}
