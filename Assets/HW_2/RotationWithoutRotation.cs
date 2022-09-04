using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationWithoutRotation : MonoBehaviour
{
    [SerializeField] private Transform _gobject;
    [SerializeField] private Transform _point;

    private void Update()
    {
        var dir = _point.position - _gobject.position;
        var moveDir = Vector3.Cross(dir, _point.up);

        _gobject.transform.position += moveDir * Time.deltaTime;
    }
}
