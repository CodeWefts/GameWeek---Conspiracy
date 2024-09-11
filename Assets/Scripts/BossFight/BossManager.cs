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

    private List<ACTIONS> m_Previous; // latest actions get added to 0

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

    private bool IsPreviousActionAOE(int _ID = 0) => m_Previous[_ID] == ACTIONS.AOE_Random || m_Previous[_ID] == ACTIONS.AOE_Player || m_Previous[_ID] == ACTIONS.AOE_Follow;

    private bool IsPreviousActionShoot(int _ID = 0) => m_Previous[_ID] == ACTIONS.Shoot_Player || m_Previous[_ID] == ACTIONS.Shoot_Wave;

    private bool IsPreviousActionDash(int _ID = 0) => m_Previous[_ID] == ACTIONS.Dash_Player || m_Previous[_ID] == ACTIONS.Dash_Random;

    private void FirstPhaseAction()
    {
        /* AOERandom
         *  OR
         * AOEFollow
         * THEN
         * ShootPlayer
         * THEN
         * DashRandom
        */
        if (m_Previous[0] == ACTIONS.First_Movement || IsPreviousActionDash())
        {
            //AOE
            m_Previous.Insert(0, ACTIONS.AOE_Random); // TODELETE, here to skip this part
        }
        else if (IsPreviousActionAOE())
            MakeAction(ACTIONS.Shoot_Player);
        else if (IsPreviousActionShoot())
            MakeAction(ACTIONS.Dash_Random);
        else
            Debug.LogError("FIRST WAVE: Previous ACTION not handled");
    }

    private void SecondPhaseAction()
    {
        /* AOERandom
         * AOEWave 2 max / 3
         * ShootPlayer
         * WaveShoot 2 max / 3
         * THEN
         * PlayerDash
        */
        if ((m_Previous[0] == ACTIONS.First_Movement || IsPreviousActionDash()) // back to zero
            )
        {
        }
        else if (IsPreviousActionAOE() && IsPreviousActionShoot() && IsPreviousActionAOE(1) && IsPreviousActionShoot(1))
        // if last two actions are ATTACKS, dashing is unlocked
        {
            if ((IsPreviousActionAOE(2) && IsPreviousActionShoot(2)) || Random.Range(0, 1) == 1)
                MakeAction(ACTIONS.Dash_Player);
        }
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
         * dash player 2/3 cases
         * dash > 2 AOE minimum
        */
    }

    private void MakeAction(ACTIONS _theAction)
    {
        switch (_theAction)
        {
            case ACTIONS.AOE_Random:
                m_Previous.Insert(0, ACTIONS.AOE_Random);
                break;

            case ACTIONS.AOE_Player:
                m_Previous.Insert(0, ACTIONS.AOE_Player);
                break;

            case ACTIONS.AOE_Follow:
                m_Previous.Insert(0, ACTIONS.AOE_Follow);
                break;

            case ACTIONS.Shoot_Player:
                m_ProjScrpt.ShootPlayer(m_NbBulletsAgainstPlayer);
                m_Previous.Insert(0, ACTIONS.Shoot_Player);
                break;

            case ACTIONS.Shoot_Wave:
                m_ProjScrpt.ShootWave();
                m_Previous.Insert(0, ACTIONS.Shoot_Wave);
                break;

            case ACTIONS.Dash_Random:
                m_DashScrpt.DashToWaypoint();
                m_Previous.Insert(0, ACTIONS.Dash_Random);
                break;

            case ACTIONS.Dash_Player:
                m_DashScrpt.DashToPlayer();
                m_Previous.Insert(0, ACTIONS.Dash_Player);
                break;

            case ACTIONS.First_Movement:
            default:
                Debug.Log("MakeAction ACTION not handled");
                return;
        }
        IsBossBussy = true;
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