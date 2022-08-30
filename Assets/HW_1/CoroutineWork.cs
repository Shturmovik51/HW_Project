using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoroutineWork : MonoBehaviour
{
    [SerializeField] private int _healingAmount;
    [SerializeField] private int _unitMaxHealth;
    [SerializeField] private Button _punishButton;
    [SerializeField] private Button _healButton;
    [SerializeField] private float _hotDelta;
    [SerializeField] private float _maxHotTime;
    [SerializeField] private int _damageValue;

    private float _hotTime;
    private Unit _unit;
    private Coroutine _healingCoroutine;
    private bool _isHealingActive;

    private void Start()
    {
        _unit = new Unit(_unitMaxHealth);
        _hotTime = _maxHotTime;

        _punishButton.onClick.AddListener(() => _unit.GetDamage(_damageValue));
        _healButton.onClick.AddListener(SetHealing);
    }

    private void SetHealing()
    {
        if(_unit.Health < _unit.MaxHealth)
        {
            StartHealingCoroutine();
        }
    }

    private void StartHealingCoroutine()
    {
        if(_healingCoroutine == null)
        {
            _healingCoroutine = StartCoroutine(Heal());
        }
    }

    private void StopHealingCoroutine()
    {
        if (_healingCoroutine != null)
        {
            StopCoroutine(_healingCoroutine);
            _healingCoroutine = null;
        }
    }

    private void CheckHealRequirements()
    {
        if (_unit.Health >= _unit.MaxHealth)
        {
            _isHealingActive = false;
            StopHealingCoroutine();
        }
    }

    private IEnumerator Heal()
    {
        _isHealingActive = true;
        _hotTime = _maxHotTime;

        while (_isHealingActive)
        {            
            _unit.RecieveHealing(_healingAmount);
           
            CheckHealRequirements();

            _hotTime -= _hotDelta;

            if (_hotTime <= 0)
            {
                _hotTime = 0;
                _isHealingActive = false;
            }

            yield return new WaitForSeconds(_hotDelta);
        }

        _healingCoroutine = null;
    }

    private void OnDestroy()
    {
        _punishButton.onClick.RemoveAllListeners();
        _healButton.onClick.RemoveAllListeners();
    }
}
