using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private BossDash m_DashScrpt = null;

    private BossProjectile m_ProjScrpt = null;

    private AOESpawnManager m_AOESpawnManager = null;

    // turn to true when starting an attack, gets turned to false by the other scripts
    [HideInInspector] public bool IsBossBussy = false;

    [HideInInspector] public bool IsBossVulnerable = false;

    public int Health = 10;

    [Header("Boss Phases")]
    public int CurrentBossPhase = 1;

    [SerializeField] private int m_HealthEndFirstPhase = 8;
    [SerializeField] private int m_HealthEndSecondPhase = 5;

    private enum ACTIONS
    {
        AOE_Random,
        AOE_Player,
        AOE_Follow,
        Shoot_Player,
        Shoot_Wave,
        Dash_Random,
        Dash_Player,
        First_Movement,
    }

    private List<ACTIONS> m_Previous;

    [SerializeField] private int m_NbBulletsAgainstPlayer = 10;

    private void Start()
    {
        if (!TryGetComponent(out m_DashScrpt)) Debug.LogError("BossDash script not found in BossManager");
        if (!TryGetComponent(out m_ProjScrpt)) Debug.LogError("BossProjectile script not found in BossManager");
        if (!TryGetComponent(out m_AOESpawnManager)) Debug.LogError("AOESpawnManager script not found in BossManager");

        m_Previous = new()
        {
            ACTIONS.First_Movement
        };

        //m_ProjScrpt.TripleShootLoop(); // Test
        //m_ProjScrpt.ShootPlayer(22000); // Test

        //m_DashScrpt.DashToPlayer(Target.transform.position); // Test
        //m_DashScrpt.DashToWaypoint(Random.Range(0, m_DashScrpt.Waypoints.Length)); // Test
    }

    private void Update()
    {
        if (IsBossBussy || IsBossVulnerable) return;

        switch (CurrentBossPhase)
        {
            case 1:
                FirstPhaseAction(); break;
            case 2:
                SecondPhaseAction(); break;
            case 3:
                ThirdPhaseAction(); break;
            default:
                Debug.Log("Boss phase not handled in BossManager::Update, Nb:" + CurrentBossPhase);
                break;
        }
    }

    private void FirstPhaseAction()
    {
        /* AOERandom
         * Altern
         * AOEFollow
         *
         * >>
         *
         * ShootPlayer
         *
         * >>
         *
         * DashWay
        */
        /* Unused ACTIONS:
         * AOE Wave
         * Shoot Wave
         * Dash Player
         */
        if (m_Previous[0] == ACTIONS.First_Movement || m_Previous[0] == ACTIONS.Dash_Random)
        {
            //AOE
            //m_AOESpawnManager.isBossAOEPhase = true;
            //m_AOESpawnManager.isRandomPhase = true;
            //m_AOESpawnManager.isWavePhase = true;
            //m_AOESpawnManager.isTargetPhase = true;
            m_Previous.Insert(0, ACTIONS.AOE_Random);
            m_Previous.Insert(0, ACTIONS.AOE_Follow);

            //IsBossBussy = true;
        }
        else if (m_Previous[0] == ACTIONS.AOE_Random || m_Previous[0] == ACTIONS.AOE_Follow)
        {
            m_ProjScrpt.ShootPlayer(m_NbBulletsAgainstPlayer);
            m_Previous.Insert(0, ACTIONS.Shoot_Player);

            IsBossBussy = true;
        }
        else if (m_Previous[0] == ACTIONS.Shoot_Player)
        {
            m_DashScrpt.DashToWaypoint();
            m_Previous.Insert(0, ACTIONS.Dash_Random);

            IsBossBussy = true;
        }
        else
            Debug.LogError("FIRST WAVE: Previous ACTION not handled");
    }

    private void SecondPhaseAction()
    {
        /* AOERandom
         * AOEWave 2 max / 3
         * ShootPlayer
         * WaveShoot 2 max / 3
         *
         * >>
         *
         * PlayerDash
        */
        /* Unused ACTIONS:
         * AOE Player
         * Dash Random
         */
    }

    private void ThirdPhaseAction()
    {
        /* AOE rand
         * not same twice in a row
         * & all at least present 1/5 times
         * shoot rand
         * not the same 3 times in a row
         *
         *
         * dash rand
         * dash 2/3 cases
         * dash > 2 AOE minimum
        */
    }

    private void NextPhase()
    {
        m_Previous = new()
        {
            ACTIONS.First_Movement
        };

        CurrentBossPhase++;
        Debug.Log("NEW BOSS PHASE");
    }

    public void TakeDamage(int _dmg)
    {
        Health -= _dmg;
        if (Health <= 0)
            Destroy(gameObject);
        else if ((Health <= m_HealthEndSecondPhase && CurrentBossPhase == 2)
            || (Health <= m_HealthEndFirstPhase && CurrentBossPhase == 1))
            NextPhase();
    }
}