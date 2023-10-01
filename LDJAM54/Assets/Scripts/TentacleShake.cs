using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleShake : MonoBehaviour {
    [SerializeField] private bool _enable = true;

    [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float _frequency = 10.0f;

    [SerializeField] private Transform _tentacle = null;

    private float _toggleSpeed = 0.1f;

    private Vector3 _startPosition;
    private CharacterController _controller;

    private void Awake() {
        _controller = GetComponentInParent<CharacterController>();
        _startPosition = _tentacle.localPosition;
    }

    private void Update() {
        if (!_enable) return;
        
        CheckMotion();

    }

    private Vector3 TentacleStepMotion(float playerSpeed) {
        Vector3 pos = UnityEngine.Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * ( _frequency) / 2) * _amplitude / 2;

        return pos;
    }

    private void CheckMotion() {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        ResetPosition();
        if (speed < _toggleSpeed) return;
        if (!_controller.isGrounded) return;

        PlayMotion(TentacleStepMotion(speed));
    }
    
    private void PlayMotion(Vector3 motion){
        _tentacle.localPosition += motion; 
    }

    private void ResetPosition() {
        if (_tentacle.localPosition == _startPosition) return;
        _tentacle.localPosition = Vector3.Lerp((_tentacle.localPosition), _startPosition, Time.deltaTime * 30);
    }
}
 
