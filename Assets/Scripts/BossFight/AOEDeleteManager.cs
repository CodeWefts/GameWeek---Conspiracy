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
            //GameObject destroyEffect = Instantiate(destroyEffectObject);
            //destroyEffect.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.localScale / 2.0f, gameObject..transform.position.z);
            Destroy(other.gameObject);
            Destroy(gameObject, 0.1f);
            isPhoneDestroyed = true;
        }
        if (other.gameObject.layer == 3 && other.gameObject.TryGetComponent(out PlayerCombat playerScript) && isPhoneDestroyed)
        {
            //if (playerScript.PlayerMovement.IsPlayerVulnerable)
            //{
            playerScript.DamageTaken(PlayerDamage);
            //}
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