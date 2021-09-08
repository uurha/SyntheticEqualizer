using System;
using System.Collections;
using System.Collections.Generic;
using Modules.AudioPlayer.UISystem;
using Modules.InputManagement.Handlers;
using UnityEngine;

[RequireComponent(typeof(ISlider<float>))]
public class SliderScrollHandler : MonoBehaviour
{
    [SerializeField] private float sensivity;
    
    private ISlider<float> _slider;

    private void Awake()
    {
        _slider = GetComponent<ISlider<float>>();
    }

    private void Start()
    {
        InputHandler.OnMouseScrollDelta += OnMouseScrollDelta;
    }

    private void OnMouseScrollDelta(Vector2 value)
    {
        _slider.Value += value.y * sensivity;
        Debug.Log(value);
    }
}
