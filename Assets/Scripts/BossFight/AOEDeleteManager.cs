using UnityEngine;

public class AOEDeleteManager : MonoBehaviour
{
    public int PlayerDamage = 1;
    //[SerializeField] public GameObject destroyEffectObject;
    private bool isPhoneDestroyed = false;
    // Phone
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DespawnZone" || other.gameObject.tag == "SpecialAOEZone")
        {
            Destroy(gameObject, 0.1f);
        }
        if (other.gameObject.tag == "AOEZone")
        {
            Destroy(other.gameObject);
            Destroy(gameObject, 0.1f);
            isPhoneDestroyed = true;
        }
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript) && isPhoneDestroyed)
        {
            playerScript.DamageTaken(PlayerDamage);
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