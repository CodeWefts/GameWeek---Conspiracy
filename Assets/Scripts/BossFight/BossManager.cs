using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private BossDash m_DashScrpt = null;

    private BossProjectile m_ProjScrpt = null;

    private AOESpawnManager m_AOESpawnManager = null;

    private DamageFlash m_DamageFlash = null;

    private Animator m_Animator = null;

    [SerializeField] private GameObject m_GreenRedCursor = null;

    [SerializeField] private int m_GreenBlueCursorDistance = 150;

    // turn to true when starting an attack, gets turned to false by the other scripts
    [HideInInspector] public bool IsBossBussy = false;

    [HideInInspector] public bool IsBossVulnerable = false;

    [SerializeField] private float m_BossVulnerableTimer = 5f;

    public int Health = 10;

    [Header("Boss Phases")]
    [Range(1, 3)] public int CurrentBossPhase = 1;

    [SerializeField] private int m_HealthEndFirstPhase = 8;
    [SerializeField] private int m_HealthEndSecondPhase = 5;

    private int m_GreenRedBar = 0;
    private int m_GreenRedKarma = 0;
    [SerializeField, Range(1, 150)] private int m_GreenRedMax = 5;

    private enum ACTIONS
    {
        AOE_Random,
        AOE_Wave,
        AOE_Follow,
        Shoot_Player,
        Shoot_Wave,
        Dash_Random,
        Dash_Player,
        First_Movement,
    }

    private System.Collections.Generic.List<ACTIONS> m_Previous; // latest actions get added to 0

    private System.Collections.Generic.List<ACTIONS> m_SecondWavePool;
    private System.Collections.Generic.List<ACTIONS> m_ThirdWavePool;

    [SerializeField] private int m_NbBulletsAgainstPlayer = 10;

    private Vector3 m_Home = Vector3.zero;
    [SerializeField] private Vector3 m_CenterOfMap = Vector3.zero;
    public int PlayerDamage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript))
        {
            playerScript.DamageTaken(PlayerDamage);
        }
    }

    private void Start()
    {
        if (!TryGetComponent(out m_DashScrpt)) Debug.LogError("BossDash script not found in BossManager");
        if (!TryGetComponent(out m_ProjScrpt)) Debug.LogError("BossProjectile script not found in BossManager");
        if (!TryGetComponent(out m_AOESpawnManager)) Debug.LogError("AOESpawnManager script not found in BossManager");
        if (!TryGetComponent(out m_DamageFlash)) Debug.LogError("DamageFlash script not found in BossManager");
        if (!TryGetComponent(out m_Animator)) Debug.LogError("Animation script not found in BossManager");

        if (!m_GreenRedCursor) Debug.LogError("GreenRedCursor not set in BossManager");
        if (m_CenterOfMap == Vector3.zero) Debug.LogWarning("Center Of Map is set as zero zero zero in BossManager");

        m_Home = transform.position;

        m_Previous = new()
        {
            ACTIONS.First_Movement
        };
    }

    private void Update()
    {
        if (IsBossBussy || IsBossVulnerable || CurrentBossPhase == 4) return;

        //m_CurrentAttack = null;

        m_Animator.SetBool("BossAOE", false);
        m_Animator.SetBool("BossProj", false);
        m_Animator.SetBool("BossDash", false);

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

    private bool IsPreviousActionAOE(int _ID = 0)
    {
        if (_ID > m_Previous.Count) return false;
        return m_Previous[_ID] == ACTIONS.AOE_Random || m_Previous[_ID] == ACTIONS.AOE_Wave || m_Previous[_ID] == ACTIONS.AOE_Follow;
    }

    private bool IsPreviousActionShoot(int _ID = 0)
    {
        if (_ID > m_Previous.Count) return false;
        return m_Previous[_ID] == ACTIONS.Shoot_Player || m_Previous[_ID] == ACTIONS.Shoot_Wave;
    }

    private bool IsPreviousActionDash(int _ID = 0)
    {
        if (_ID > m_Previous.Count) return false;
        return m_Previous[_ID] == ACTIONS.Dash_Player || m_Previous[_ID] == ACTIONS.Dash_Random;
    }

    private void FirstPhaseAction()
    {
        if (m_Previous[0] == ACTIONS.First_Movement)
            MakeAction(ACTIONS.AOE_Random);
        else if (IsPreviousActionDash())
            if (m_Previous[2] == ACTIONS.AOE_Random)
                MakeAction(ACTIONS.AOE_Follow);
            else
                MakeAction(ACTIONS.AOE_Random);
        else if (IsPreviousActionAOE())
            MakeAction(ACTIONS.Shoot_Player);
        else if (IsPreviousActionShoot())
            MakeAction(ACTIONS.Dash_Random);
        else
            Debug.LogError("FIRST WAVE: Previous ACTION not handled");
    }

    private ACTIONS GetSecondPhaseAction()
    {
        if (m_SecondWavePool == null || m_SecondWavePool.Count == 0)
        {
            m_SecondWavePool = new()
            {
                ACTIONS.AOE_Random,
                ACTIONS.AOE_Wave,
                ACTIONS.Shoot_Player,
                ACTIONS.Shoot_Wave
            };
        }

        int IDAction = Random.Range(0, m_SecondWavePool.Count);
        ACTIONS returnAction = m_SecondWavePool[IDAction];
        m_SecondWavePool.RemoveAt(IDAction);

        return returnAction;
    }

    private void SecondPhaseAction()
    {
        if (m_Previous[0] == ACTIONS.First_Movement)
            MakeAction(GetSecondPhaseAction());
        else if (!IsPreviousActionDash() && !IsPreviousActionDash(1))
            // if last two actions are ATTACKS, DASH is unlocked
            MakeAction(ACTIONS.Dash_Player);
        else
            MakeAction(GetSecondPhaseAction());
    }

    private ACTIONS GetThirdPhaseAction()
    {
        if (m_ThirdWavePool == null || m_ThirdWavePool.Count == 0)
        {
            m_ThirdWavePool = new()
            {
                ACTIONS.AOE_Wave,
                ACTIONS.AOE_Follow,
                ACTIONS.AOE_Random,
                ACTIONS.Shoot_Player,
                ACTIONS.Shoot_Wave
            };
        }

        int IDAction = Random.Range(0, m_ThirdWavePool.Count);
        ACTIONS returnAction = m_ThirdWavePool[IDAction];
        m_ThirdWavePool.RemoveAt(IDAction);

        return returnAction;
    }

    private void ThirdPhaseAction()
    {
        if (m_Previous[0] == ACTIONS.First_Movement)
            MakeAction(GetThirdPhaseAction());
        else if (!IsPreviousActionDash() && !IsPreviousActionDash(1))
            // if last two actions are ATTACKS, DASH is unlocked
            if (Random.Range(0, 2) == 0)
                MakeAction(ACTIONS.Dash_Random);
            else
                MakeAction(ACTIONS.Dash_Player);
        else
            MakeAction(GetThirdPhaseAction());
    }

    private void MakeAction(ACTIONS _theAction)
    {
        switch (_theAction)
        {
            case ACTIONS.AOE_Random:
                m_AOESpawnManager.PlayRandomAOE();
                m_Previous.Insert(0, ACTIONS.AOE_Random);
                m_Animator.SetBool("BossAOE", true);
                break;

            case ACTIONS.AOE_Wave:
                m_AOESpawnManager.PlayWaveAOE();
                m_Previous.Insert(0, ACTIONS.AOE_Wave);
                m_Animator.SetBool("BossAOE", true);
                break;

            case ACTIONS.AOE_Follow:
                m_AOESpawnManager.PlayTargetAOE();
                m_Previous.Insert(0, ACTIONS.AOE_Follow);
                m_Animator.SetBool("BossAOE", true);
                break;

            case ACTIONS.Shoot_Player:
                m_ProjScrpt.ShootPlayer(m_NbBulletsAgainstPlayer);
                m_Previous.Insert(0, ACTIONS.Shoot_Player);
                m_Animator.SetBool("BossProj", true);
                break;

            case ACTIONS.Shoot_Wave:
                m_ProjScrpt.ShootWave();
                m_Previous.Insert(0, ACTIONS.Shoot_Wave);
                m_Animator.SetBool("BossProj", true);
                break;

            case ACTIONS.Dash_Random:
                m_DashScrpt.DashToWaypoint();
                m_Previous.Insert(0, ACTIONS.Dash_Random);
                m_Animator.SetBool("BossDash", true);
                break;

            case ACTIONS.Dash_Player:
                m_DashScrpt.DashToPlayer();
                m_Previous.Insert(0, ACTIONS.Dash_Player);
                m_Animator.SetBool("BossDash", true);
                break;

            case ACTIONS.First_Movement:
            default:
                Debug.Log("MakeAction ACTION not handled");
                return;
        }
        IsBossBussy = true;
    }

    private IEnumerator BossIsTired()
    {
        Debug.Log("Boss is tired");
        while (IsBossBussy) { yield return null; };

        //m_AOESpawnManager.StopAOE();
        //m_DashScrpt.StopDash();

        m_Animator.SetBool("BossAOE", false);
        m_Animator.SetBool("BossProj", false);
        m_Animator.SetBool("BossDash", false);

        m_DashScrpt.DashToCoord(m_CenterOfMap);

        IsBossBussy = false;
        IsBossVulnerable = true;

        yield return new WaitForSeconds(m_BossVulnerableTimer);
        IsBossVulnerable = false;
        m_GreenRedCursor.transform.localPosition = Vector3.zero;
    }

    private void NextPhase()
    {
        CurrentBossPhase++;
        Debug.Log("NEW BOSS PHASE");

        m_Previous = new()
        {
            ACTIONS.First_Movement
        };

        //StopCoroutine(m_CurrentAttack);
        //m_CurrentAttack = null;
        IsBossVulnerable = false;

        m_DashScrpt.DashToCoord(m_Home);
    }

    public void TakeDamageGreenRed(int _dmg, ProjectileBehaviour.TypeProj _type)
    {
        if (IsBossVulnerable) return;

        if (_type == ProjectileBehaviour.TypeProj.BouncedGreen)
            m_GreenRedBar += _dmg;
        else if (_type == ProjectileBehaviour.TypeProj.BouncedRed)
            m_GreenRedBar -= _dmg;
        else
        {
            Debug.LogWarning("TakeDamageGreenRed Boss was damaged by the wrong type of projectile");
            return;
        }

        Vector3 cursorPos = m_GreenRedCursor.transform.localPosition;
        cursorPos.x = m_GreenRedBar * (m_GreenBlueCursorDistance / (float)m_GreenRedMax);
        m_GreenRedCursor.transform.localPosition = cursorPos;

        m_DamageFlash.CallDamageFlash();
        m_Animator.SetTrigger("BossHit");

        if (m_GreenRedBar >= m_GreenRedMax || m_GreenRedBar <= -m_GreenRedMax)
        {
            if (_type == ProjectileBehaviour.TypeProj.BouncedGreen)
                m_GreenRedKarma += _dmg;
            else if (_type == ProjectileBehaviour.TypeProj.BouncedRed)
                m_GreenRedKarma -= _dmg;

            //StopCoroutine(m_CurrentAttack);
            StartCoroutine(BossIsTired());
            IsBossVulnerable = true;
        }
    }

    public void TakeDamage(int _dmg)
    {
        if (!IsBossVulnerable) return;

        Health -= _dmg;
        m_DamageFlash.CallDamageFlash();
        m_Animator.SetTrigger("BossHit");

        if (Health <= 0)
            BossIsDead();
        else if ((Health <= m_HealthEndSecondPhase && CurrentBossPhase == 2)
            || (Health <= m_HealthEndFirstPhase && CurrentBossPhase == 1))
            NextPhase();
    }

    private void BossIsDead()
    {
        CurrentBossPhase++;
        m_DashScrpt.DashToCoord(m_Home);
        //Destroy(gameObject, 10f);

        if (m_GreenRedKarma > 0)
            m_Animator.SetTrigger("BossGoodDeath");
        else
            m_Animator.SetTrigger("BossBadDeath");
    }
}