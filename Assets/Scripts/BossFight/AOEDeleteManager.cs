using UnityEngine;

public class AOEDeleteManager : MonoBehaviour
{
    public int PlayerDamage = 1;
    private bool isPhoneDestroyed = false;
    private FMOD.Studio.EventInstance m_PhoneDestroyed;

    // Phone
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DespawnZone" || other.gameObject.tag == "SpecialAOEZone")
        {
            Destroy(gameObject, 0.1f);
        }
        if (other.gameObject.tag == "AOEZone")
        {
            isPhoneDestroyed = true;

            m_PhoneDestroyed = FMODUnity.RuntimeManager.CreateInstance("event:/Boss Events/Boss Phone Attack");
            m_PhoneDestroyed.start();
            m_PhoneDestroyed.release();

            Destroy(other.gameObject);
        }
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript) && isPhoneDestroyed)
        {
            playerScript.DamageTaken(PlayerDamage);
        }

        if (isPhoneDestroyed)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    void Update()
    {
        if (transform.position.y < -1)
        {
            Destroy(gameObject);
        }
    }
}