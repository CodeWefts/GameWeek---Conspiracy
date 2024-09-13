using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSound : MonoBehaviour
{
    private FMOD.Studio.EventInstance m_BossMusic;

    [SerializeField]
    [Range(0, 4)]
    private float Transitions;

    // Start is called before the first frame update
    void Start()
    {
        // TODOSOUND : play stunned music
        m_BossMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music Events/FightBoss_Music");

        m_BossMusic.start();
        m_BossMusic.release();
    }

    // Update is called once per frame
    void Update()
    {
        m_BossMusic.setParameterByName("Transitions", Transitions);
    }
}