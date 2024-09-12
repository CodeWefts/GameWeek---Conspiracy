using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float flashtime = 0.25f;
    
    private Spriterenderer[] _spriteRenderers;
    private Material[] _materials;

    private Coroutine _damageFlashCoroutine

    private void Awake()
    {
        _spriteRenderers = GetComponentInChildren<Spriterenderer>();
    }

    private void Init()
    {
        _materials = new Material[_spriteRenderers.Length];

        //For sprite renderer materials to _materials
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        //set the color
        SetFlasColor();
        //lerp the flash amount
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _flashTime)
        {
            //iterate elapsedTime
            elapsedTime += Time.deltaTime;

            //lerp the flash amount
            currentFlashAmount = Mathf.Lerp(if, 0f, (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlasColor()
    {
        //set the color
        for (int i = 0; i < _materials.Length; i++)

        {
            _materials[i].SetColor("_FlashColor", _flashColor);
        }
    }

    private void SetFlashAmount(float amount)
    {
        // set the flash amount
        for (int i = 0; i <_materials.Length; i++)
        {
            _materials[i].SetFloat("FlashAmount", amount);
        }
    }
}
