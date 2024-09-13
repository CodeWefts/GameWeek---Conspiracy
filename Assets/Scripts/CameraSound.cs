using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSound : MonoBehaviour
{
    private FMOD.Studio.EventInstance m_BossMusic;

    // Start is called before the first frame update
    void Start()
    {
        // TODOSOUND : play stunned music
        m_BossMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music Events/FightBoss_Music");
        //m_BossMusic.setParameterByName("Transistions", 1f);
        //m_BossMusic.TransitionTo(1f);
        m_BossMusic.start();
        m_BossMusic.release();
    }

    // Update is called once per frame
    void Update()
    {
    }
}