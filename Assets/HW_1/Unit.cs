using UnityEngine;

public class Unit
{
    private int _health;
    private int _maxHealth;

    public int Health => _health;
    public int MaxHealth => _maxHealth;

    public Unit(int unitMaxHealth)
    {
        _maxHealth = unitMaxHealth;
        _health = _maxHealth;
    }

    public void RecieveHealing(int value)
    {
        _health += value;

        if(_health >= _maxHealth)
        {
            _health = _maxHealth;
        }

        Debug.Log($"health is {_health}");
    }

    public void GetDamage(int value)
    {
        _health -= value;

        if (_health <= 0)
        {
            _health = 0;
        }

        Debug.Log($"health is {_health}");
    }
}
