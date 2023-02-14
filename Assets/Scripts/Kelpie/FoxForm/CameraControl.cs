using Cinemachine;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _vCam;
    private Player _player;
    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _player = SceneObject<Player>.Instance(true);
    }

    private void OnEnable()
    {
        this.ObserveEvent(StateEvents.OnGhostForm, OnGhostForm);
        this.ObserveEvent(StateEvents.OnFoxForm, OnFoxForm);
    }

    private void OnFoxForm()
    {
        _vCam.Follow = _player.FoxForm.transform;
    }

    private void OnGhostForm()
    {
        _vCam.Follow = _player.GhostForm.transform;
    }

    private void OnDisable()
    {
        this.StopObservingEvent(StateEvents.OnGhostForm, OnGhostForm);
        this.StopObservingEvent(StateEvents.OnFoxForm, OnFoxForm);
    }
}
