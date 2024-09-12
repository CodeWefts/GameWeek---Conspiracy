using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;
    
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;

    private void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        _materials = new Material[_spriteRenderers.Length];

        //For sprite renderer materials to _materials
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
    }

    public void CallDamageFlash() => StartCoroutine(DamageFlasher());

    private IEnumerator DamageFlasher()
    {
        // set the color
        SetFlasColor();
        float elapsedTime = 0f;
        while (elapsedTime < _flashTime)
        {
            // iterate elapsedTime
            elapsedTime += Time.deltaTime;

            // lerp the flash amount
            // lerp the flash amount
            float currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / _flashTime);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlasColor()
    {
        // set the color
        for (int i = 0; i < _materials.Length; i++)
            _materials[i].SetColor("_FlashColor", _flashColor);
    }

    private void SetFlashAmount(float amount)
    {
        // set the flash amount
        for (int i = 0; i <_materials.Length; i++)
            _materials[i].SetFloat("FlashAmount", amount);
    }
}
