using UnityEngine;

public class Stopsong : MonoBehaviour
{
    private AudioSource audioSource;
    private Player_AnimatorController player_anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player_anim = GameObject.Find("PolumModel3D").GetComponent<Player_AnimatorController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player_anim.isDead == true)
        {
            audioSource.Stop();
        }
    }
}
