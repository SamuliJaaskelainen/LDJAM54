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

    private float _toggleSpeed = 1f;
    private bool moveCase = false;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private CharacterController _controller;
    private float tentacleMoveTime = 0.0f;

    private void Awake() {
        _controller = GetComponentInParent<CharacterController>();
        _startPosition = _tentacle.localPosition;
    }

    private void Update() {
        CheckMotion();

    }

    private Vector3 TentacleStepMotion(float playerSpeed) {
        Vector3 pos = UnityEngine.Vector3.zero; 
        float moveFreq = CalculateTentacleMoveFrequency();
        pos.y += Mathf.Sin(Time.time * _frequency) * (_amplitude + (Random.Range(-0.002f, 0.002f) * moveFreq));
        pos.x += (Mathf.Cos(Time.time * (_frequency) / 2) * (_amplitude + (Random.Range(-0.05f, 0.05f) * moveFreq))/2);
        
        //pos.y = _startPosition.y + (Random.Range(-0.002f, 0.002f) * moveFreq);
        //pos.x = _startPosition.x + (Random.Range(-0.001f, 0.001f) * moveFreq);
        return pos;
    }

    private void CheckMotion() {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        if (speed < _toggleSpeed) {
            ResetPosition();
            return;
        }
        if (!_controller.isGrounded) return;
        if (Time.time - tentacleMoveTime < CalculateFootstepMoveFrequency()) return;
        
        if (moveCase) {
            PlayMotion(TentacleStepMotion(speed));
            RotateTentacle();
            moveCase = false;
        }
        else {
            _tentacle.localPosition = _startPosition;
            ResetRotation();
            moveCase = true;
            
        }
    }
    
    private void PlayMotion(Vector3 motion){
        _tentacle.localPosition = Vector3.Lerp(_tentacle.localPosition, _startPosition + motion, t:Time.deltaTime * 15);
        tentacleMoveTime = Time.time;
    }

    private void ResetPosition() {
        if (_tentacle.localPosition == _startPosition) return;
        _tentacle.localPosition = Vector3.Lerp((_tentacle.localPosition), _startPosition, Time.deltaTime * 1);
    }
    
    
    private float CalculateTentacleMoveFrequency()
    {
        // takes square root of the sum of the squares of the x and z velocity
        // so we know how often to play the footstep sound

        return Mathf.Sqrt((_controller.velocity.x * _controller.velocity.x) + (_controller.velocity.z * _controller.velocity.z));
    }
    
    private void RotateTentacle() {
        float randomRotation = Random.Range(-10, 10);
        _tentacle.localRotation = Quaternion.Euler(0, 0, randomRotation);
    }
    
    private void ResetRotation() {
        _tentacle.localRotation = Quaternion.Lerp(_tentacle.localRotation, _startRotation, Time.deltaTime * 1);
    }
    private float CalculateFootstepMoveFrequency()
    {
        // takes square root of the sum of the squares of the x and z velocity
        // so we know how often to play the footstep sound

        return 1f/Mathf.Sqrt((_controller.velocity.x * _controller.velocity.x) + (_controller.velocity.z * _controller.velocity.z));
    }


}
 
