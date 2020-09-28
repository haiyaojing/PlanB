using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTouch
{
    public int FingerId;
    public Vector2 RawPosition;
    public Vector2 Position;
    public Vector2 DeltaPosition;
    public float DeltaTime;
    public TouchPhase Phase;
}

public class Test : MonoBehaviour
{
    public Camera Cam;
    public RectTransform Test1;
    public RectTransform Container;
    public RectTransform Stick;
    public MTouch TouchInfo = new MTouch();
    private Vector2 _lastMousePosition;
    private int _fingerId = -1;
    void Start()
    {
        TouchInfo.Phase = TouchPhase.Ended;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
            TouchInfo.Phase = TouchPhase.Began;
            TouchInfo.Position = _lastMousePosition;
            TouchInfo.RawPosition = _lastMousePosition;
            TouchInfo.FingerId = 1;
            TouchInfo.DeltaPosition = Vector2.zero;
            TouchInfo.DeltaTime = 0;
        }
        else if (Input.GetMouseButton(0))
        {
            _lastMousePosition = Input.mousePosition;
            TouchInfo.DeltaPosition = _lastMousePosition - TouchInfo.Position;
            TouchInfo.Position = _lastMousePosition;
            TouchInfo.Phase = (TouchInfo.DeltaPosition).sqrMagnitude > 0 ? TouchPhase.Moved : TouchPhase.Stationary;
            TouchInfo.DeltaTime = 0;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _lastMousePosition = Input.mousePosition;
            TouchInfo.Phase = TouchPhase.Ended;
            TouchInfo.Position = _lastMousePosition;
            TouchInfo.FingerId = 0;
            TouchInfo.DeltaPosition = Vector2.zero;
            TouchInfo.DeltaTime = 0;
        }

        
    }

    Vector2 getSafeArea(Vector2 p)
    {
        var x = p.x < 50 ? 50 : p.x;
        x = x > 150 ? 150 : x;
        var y = p.y < 50 ? 50 : p.y;
        y = y > 150 ? 150 : y;
        return new Vector2(x, y);
    }

    private Vector2 tmp;

    private void LateUpdate()
    {
        switch (TouchInfo.Phase)
        {
            case TouchPhase.Began:
                if (TouchInfo.Position.x > 200 || TouchInfo.Position.y > 200)
                {
                    break;
                }

                RectTransformUtility.ScreenPointToLocalPointInRectangle(Test1, TouchInfo.Position, Cam,
                    out var localPoint1);
                var v = getSafeArea(TouchInfo.Position);
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Test1, v, Cam,
                    out var localPoint))
                {
                    tmp = localPoint - localPoint1;
                    tmp = Vector2.zero;
                    Container.gameObject.SetActive(true);

                    Container.anchoredPosition = localPoint1;
                    Stick.anchoredPosition = Vector2.zero;
                    _fingerId = TouchInfo.FingerId;
                }
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (_fingerId != -1)
                {
                    Container.gameObject.SetActive(false);
                    _fingerId = -1;
                }
                break;
            case TouchPhase.Moved:
                if (_fingerId != -1)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Container, TouchInfo.Position, Cam,
                        out var p))
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(Container, TouchInfo.RawPosition, Cam,
                            out var p1);
                        var direction = (p - p1).normalized;
                        if (direction != Vector2.zero)
                        {
                            var distance = (p - p1).magnitude;
                            if (distance > 50) distance = 50;
                            var pos = direction * distance;
                            Stick.anchoredPosition = pos + tmp;
                                                        
                        }
                    }
                    else
                    {
                        _fingerId = -1;
                        Container.gameObject.SetActive(false);
                    }
                }
                break;
        }
    }
}
