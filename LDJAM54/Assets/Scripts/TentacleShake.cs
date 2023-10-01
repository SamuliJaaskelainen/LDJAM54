using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TentacleShake : MonoBehaviour {
    [SerializeField] private bool _enable = true;

    [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float _frequency = 10.0f;

    [SerializeField] private Transform _tentacle = null;

    private float _toggleSpeed = 2f;
    private bool moveCase = false;

    private Vector3 _startPosition;
    private CharacterController _controller;
    private float tentacleMoveTime = 0.0f;

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
        pos.y += Mathf.Sin(Time.time * _frequency) * (_amplitude);//+ Random.Range(-0.05f, 0.05f);
        pos.x += (Mathf.Cos(Time.time * (_frequency) / 2) * (_amplitude) / 2);//+ Random.Range(-0.15f, 0.15f);

        return pos;
    }

    private void CheckMotion() {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        if (speed < _toggleSpeed) {
            ResetPosition();
            return;
        }
        if (!_controller.isGrounded) return;
        if (Time.time - tentacleMoveTime < CalculateTentacleMoveFrequency()) return;
        
        if (moveCase) {
            PlayMotion(TentacleStepMotion(speed));
            moveCase = false;
        }
        else {
            _tentacle.localPosition = _startPosition;
            moveCase = true;
        }
    }
    
    private void PlayMotion(Vector3 motion){
        _tentacle.localPosition = Vector3.Lerp(_tentacle.localPosition, _startPosition + motion, t:Time.deltaTime * 30);
        tentacleMoveTime = Time.time;
    }

    private void ResetPosition() {
        if (_tentacle.localPosition == _startPosition) return;
        _tentacle.localPosition = Vector3.Lerp((_tentacle.localPosition), _startPosition, Time.deltaTime * 30);
    }
    
    private float FrequencyFromMovement(float speed) {
        return speed * 0.1f;
    }
    
    private float CalculateTentacleMoveFrequency()
    {
        // takes square root of the sum of the squares of the x and z velocity
        // then divides by the max speed to get a value between 0 and 1
        // so we know how often to play the footstep sound

        return 1.0f / Mathf.Sqrt((_controller.velocity.x * _controller.velocity.x) + (_controller.velocity.z * _controller.velocity.z));
    }
}
 
