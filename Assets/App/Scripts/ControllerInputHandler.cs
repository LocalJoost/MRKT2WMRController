using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using UnityEngine;

public class ControllerInputHandler : BaseInputHandler, 
    IMixedRealityInputHandler, IMixedRealityInputHandler<Vector2>, 
    IMixedRealityInputHandler<float>,
    IMixedRealityInputHandler<MixedRealityPose>

{
    [SerializeField]
    private GameObject _eventDisplayParent;

    [SerializeField] 
    private TextMeshPro _touchXYText;

    [SerializeField]
    private TextMeshPro _triggerPostText;

    [SerializeField]
    private TextMeshPro _gripPoseText;

    [SerializeField]
    private TextMeshPro _pointerPoseText;

    private Dictionary<string, SingleShotController> _eventDisplayers = 
        new Dictionary<string, SingleShotController>();

    protected override void Start()
    {
        base.Start();
        foreach (var controller in _eventDisplayParent.GetComponentsInChildren<SingleShotController>())
        {
            _eventDisplayers.Add(controller.gameObject.name.ToLower(), controller);
        }
    }

    protected override void RegisterHandlers()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
    }

    protected override void UnregisterHandlers()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
    }

    Vector2 _lastpressPosition;

    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        var eventName = eventData.MixedRealityInputAction.Description.ToLower();
        if (eventName == "touchpad position")
        {
            _lastpressPosition = eventData.InputData;
            _touchXYText.text = $"Last X: {eventData.InputData.x}{Environment.NewLine}Last Y: { eventData.InputData.y}";
            ShowEvent(eventName);
        }

        if (eventName != "teleport direction")
        {
            ShowEvent("Vector2");
        }
    }
    public void OnInputUp(InputEventData eventData)
    {
    }

    public void OnInputDown(InputEventData eventData)
    {
        var eventName = eventData.MixedRealityInputAction.Description.ToLower();
        if (eventName == "touchpad press")
        {
            // Limit event capture to only when more or less the top or bottom 
            // of the touch pad is pressed
            if (_lastpressPosition.y < -0.7 || _lastpressPosition.y > 0.7)
            {
                ShowEvent(eventName);
            }
        }
        else
        {
            ShowEvent(eventName);
        }
        ShowEvent("Digital");
    }


    public void OnInputChanged(InputEventData<float> eventData)
    {
        var eventName = eventData.MixedRealityInputAction.Description;
        if (eventName == "Trigger")
        {
            _triggerPostText.text = $"Last pos: {eventData.InputData}";
        }
        ShowEvent(eventName);
        ShowEvent("float");
    }


    public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
    {
        var eventName = eventData.MixedRealityInputAction.Description;
        ShowEvent(eventName);
        ShowEvent("MixedRealityPose");
        switch (eventName)
        {
            case "Grip Pose":
                _gripPoseText.text = eventData.InputData.ToString();
                break;
            case "Pointer Pose":
                _pointerPoseText.text = eventData.InputData.ToString();
                break;
        }
    }

    private void ShowEvent(string eventName)
    {
        var controller = GetControllerForEvent(eventName);
        if (controller != null)
        {
            controller.ShowActivated();
        }
    }
    private SingleShotController GetControllerForEvent(string controllerEvent)
    {
        return _eventDisplayers[controllerEvent.ToLower().Replace(" ", "")];
    }

}
