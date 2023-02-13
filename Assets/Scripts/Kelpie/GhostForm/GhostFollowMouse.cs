using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostFollowMouse : BaseGameObject
{
    private GhostFormPlayer _ghostForm;
    public InputActionReference MousePosition;
    public InputActionReference MouseDelta;
    private Vector2 _refSpeed;
    private Camera _gameCamera;
    private bool _movedMouseRecently;

    protected override void OnAwake()
    {
        base.OnAwake();
        _ghostForm = SceneObject<Player>.Instance(true).GhostForm;
        _gameCamera = Camera.main;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(BufferMouseMovement());
        DefaultMachinery.AddBasicMachine(HandleFollowMouse());
    }

    private IEnumerable<IEnumerable<Action>> BufferMouseMovement()
    {
        while (ComponentEnabled)
        {
            while (MouseDelta.action.ReadValue<Vector2>() == Vector2.zero)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            _movedMouseRecently = true;
            yield return TimeYields.WaitSeconds(GameTimer, 2,
                resetCondition: () => MouseDelta.action.ReadValue<Vector2>() != Vector2.zero
                || !_ghostForm.MoveController.IsMoving && CheckDistanceToMousePositionGreaterThan(0.25f)) ;
            _movedMouseRecently = false;
        }
    }

    private bool CheckDistanceToMousePositionGreaterThan(float minDistance)
    {
        var mousePos = _gameCamera.ScreenToWorldPoint(MousePosition.action.ReadValue<Vector2>());
        var distance = ((Vector2)(mousePos - _ghostForm.PhysicsObject.transform.position)).magnitude;
        return distance > minDistance;
    }

    private IEnumerable<IEnumerable<Action>> HandleFollowMouse()
    {
        while(ComponentEnabled)
        {
            if (!_ghostForm.MoveController.IsMoving && _movedMouseRecently)
            {
                var mousePos = _gameCamera.ScreenToWorldPoint(MousePosition.action.ReadValue<Vector2>());
                var distance = (Vector2)(mousePos - _ghostForm.PhysicsObject.transform.position);
                var direction = distance.normalized;
                var factor = Mathf.Clamp(distance.magnitude * 0.5f, 0f, 1f);

                _ghostForm.PhysicsObject.ApplySpeed(Vector2.SmoothDamp(_ghostForm.PhysicsObject.LatestSpeed,
                    direction * _ghostForm.MoveController.MaxSpeed * factor, ref _refSpeed,  _ghostForm.MoveController.AccelerationTime, 
                    _ghostForm.MoveController.MaxSpeed, (float) GameTimer.UpdatedTimeInMilliseconds));
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
