using System;
using UnityEngine;
using UnityEngine.XR;

namespace AVR
{
    namespace Input
    {
        public class InputEvent<T>
        {
            public T active;
            public DateTime since;
            public DateTime past;
            public double Duration => (since - past).TotalSeconds;
        }

        public class VRInput
        {
            public XRNode node;
            public InputDevice device;

            public VRInput(XRNode node)
            {
                this.node = node;
            }

            public delegate void OnActive();
            public event OnActive OnActiveEvent;

            public delegate void OnInactive();
            public event OnInactive OnInactiveEvent;

            // input events

            public InputEvent<bool> primaryButton = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnPrimaryPressed(InputEvent<bool> primary);
            public event OnPrimaryPressed OnPrimaryPressedEvent;
            public delegate void OnPrimaryReleased(InputEvent<bool> primary);
            public event OnPrimaryReleased OnPrimaryReleasedEvent;
            public void PrimaryPressed(InputEvent<bool> primary) => OnPrimaryPressedEvent?.Invoke(primary);
            public void PrimaryReleased(InputEvent<bool> primary) => OnPrimaryReleasedEvent?.Invoke(primary);

            public InputEvent<bool> secondaryButton = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnSecondaryPressed(InputEvent<bool> secondary);
            public event OnSecondaryPressed OnSecondaryPressedEvent;
            public delegate void OnSecondaryReleased(InputEvent<bool> secondary);
            public event OnSecondaryReleased OnSecondaryReleasedEvent;
            public void SecondaryPressed(InputEvent<bool> secondary) => OnSecondaryPressedEvent?.Invoke(secondary);
            public void SecondaryReleased(InputEvent<bool> secondary) => OnSecondaryReleasedEvent?.Invoke(secondary);

            public InputEvent<bool> primaryTouch = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnPrimaryTouched(InputEvent<bool> primary);
            public event OnPrimaryTouched OnPrimaryTouchedEvent;
            public delegate void OnPrimaryUntouched(InputEvent<bool> primary);
            public event OnPrimaryUntouched OnPrimaryUntouchedEvent;
            public void PrimaryTouched(InputEvent<bool> primary) => OnPrimaryTouchedEvent?.Invoke(primary);
            public void PrimaryUntouched(InputEvent<bool> primary) => OnPrimaryUntouchedEvent?.Invoke(primary);

            public InputEvent<bool> secondaryTouch = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnSecondaryTouched(InputEvent<bool> secondary);
            public event OnSecondaryTouched OnSecondaryTouchedEvent;
            public delegate void OnSecondaryUntouched(InputEvent<bool> secondary);
            public event OnSecondaryUntouched OnSecondaryUntouchedEvent;
            public void SecondaryTouched(InputEvent<bool> secondary) => OnSecondaryTouchedEvent?.Invoke(secondary);
            public void SecondaryUntouched(InputEvent<bool> secondary) => OnSecondaryUntouchedEvent?.Invoke(secondary);

            public Vector2[] primaryDeadzones = new Vector2[] { new(0.1f, 0.1f) };
            public Vector2[] primaryDeadangles = new Vector2[] { };
            public InputEvent<Vector2> primaryAxis = new() { active = Vector2.zero, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnPrimaryAxis(InputEvent<Vector2> primary);
            public event OnPrimaryAxis OnPrimaryAxisEvent;
            public void PrimaryAxis(InputEvent<Vector2> primary) => OnPrimaryAxisEvent?.Invoke(primary);

            public Vector2[] secondaryDeadzones = new Vector2[] { new(0.1f, 0.1f) };
            public Vector2[] secondaryDeadangles = new Vector2[] { };
            public InputEvent<Vector2> secondaryAxis = new() { active = Vector2.zero, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnSecondaryAxis(InputEvent<Vector2> secondary);
            public event OnSecondaryAxis OnSecondaryAxisEvent;
            public void SecondaryAxis(InputEvent<Vector2> secondary) => OnSecondaryAxisEvent?.Invoke(secondary);

            public InputEvent<bool> primaryAxisClick = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnPrimaryAxisClicked(InputEvent<bool> primary);
            public event OnPrimaryAxisClicked OnPrimaryAxisClickedEvent;
            public delegate void OnPrimaryAxisUnclicked(InputEvent<bool> primary);
            public event OnPrimaryAxisUnclicked OnPrimaryAxisUnclickedEvent;
            public void PrimaryAxisClicked(InputEvent<bool> primary) => OnPrimaryAxisClickedEvent?.Invoke(primary);
            public void PrimaryAxisUnclicked(InputEvent<bool> primary) => OnPrimaryAxisUnclickedEvent?.Invoke(primary);

            public InputEvent<bool> secondaryAxisClick = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnSecondaryAxisClicked(InputEvent<bool> secondary);
            public event OnSecondaryAxisClicked OnSecondaryAxisClickedEvent;
            public delegate void OnSecondaryAxisUnclicked(InputEvent<bool> secondary);
            public event OnSecondaryAxisUnclicked OnSecondaryAxisUnclickedEvent;
            public void SecondaryAxisClicked(InputEvent<bool> secondary) => OnSecondaryAxisClickedEvent?.Invoke(secondary);
            public void SecondaryAxisUnclicked(InputEvent<bool> secondary) => OnSecondaryAxisUnclickedEvent?.Invoke(secondary);

            public InputEvent<bool> primaryAxisTouch = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnPrimaryAxisTouched(InputEvent<bool> primary);
            public event OnPrimaryAxisTouched OnPrimaryAxisTouchedEvent;
            public delegate void OnPrimaryAxisUntouched(InputEvent<bool> primary);
            public event OnPrimaryAxisUntouched OnPrimaryAxisUntouchedEvent;
            public void PrimaryAxisTouched(InputEvent<bool> primary) => OnPrimaryAxisTouchedEvent?.Invoke(primary);
            public void PrimaryAxisUntouched(InputEvent<bool> primary) => OnPrimaryAxisUntouchedEvent?.Invoke(primary);

            public InputEvent<bool> secondaryAxisTouch = new() { active = false, since = DateTime.Now, past = DateTime.Now };
            public delegate void OnSecondaryAxisTouched(InputEvent<bool> secondary);
            public event OnSecondaryAxisTouched OnSecondaryAxisTouchedEvent;
            public delegate void OnSecondaryAxisUntouched(InputEvent<bool> secondary);
            public event OnSecondaryAxisUntouched OnSecondaryAxisUntouchedEvent;
            public void SecondaryAxisTouched(InputEvent<bool> secondary) => OnSecondaryAxisTouchedEvent?.Invoke(secondary);
            public void SecondaryAxisUntouched(InputEvent<bool> secondary) => OnSecondaryAxisUntouchedEvent?.Invoke(secondary);



            public void Update()
            {
                var input = InputDevices.GetDeviceAtXRNode(node);
                if (input.isValid && device == default)
                {
                    device = input;
                    OnActiveEvent?.Invoke();
                }
                else if (!input.isValid && device != default)
                {
                    device = default;
                    OnInactiveEvent?.Invoke();
                }

                if (device == default)
                    return;


                // primary button
                if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool triggerButtonValue))
                {
                    if (triggerButtonValue && !primaryButton.active)
                    {
                        primaryButton.active = true;
                        primaryButton.past = primaryButton.since;
                        primaryButton.since = DateTime.Now;
                        PrimaryPressed(primaryButton);
                        AVR.Utils.Debug.Log("Primary pressed");
                    }
                    else if (!triggerButtonValue && primaryButton.active)
                    {
                        primaryButton.active = false;
                        primaryButton.past = primaryButton.since;
                        primaryButton.since = DateTime.Now;
                        PrimaryReleased(primaryButton);
                        AVR.Utils.Debug.Log("Primary released");
                    }
                }

                // secondary button
                if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool gripButtonValue))
                {
                    if (gripButtonValue && !secondaryButton.active)
                    {
                        secondaryButton.active = true;
                        secondaryButton.past = secondaryButton.since;
                        secondaryButton.since = DateTime.Now;
                        SecondaryPressed(secondaryButton);
                        AVR.Utils.Debug.Log("Secondary pressed");
                    }
                    else if (!gripButtonValue && secondaryButton.active)
                    {
                        secondaryButton.active = false;
                        secondaryButton.past = secondaryButton.since;
                        secondaryButton.since = DateTime.Now;
                        SecondaryReleased(secondaryButton);
                        AVR.Utils.Debug.Log("Secondary released");
                    }
                }

                // primary touch
                if (device.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouchValue))
                {
                    if (primaryTouchValue && !primaryTouch.active)
                    {
                        primaryTouch.active = true;
                        primaryTouch.past = primaryTouch.since;
                        primaryTouch.since = DateTime.Now;
                        PrimaryTouched(primaryTouch);
                        AVR.Utils.Debug.Log("Primary touched");
                    }
                    else if (!primaryTouchValue && primaryTouch.active)
                    {
                        primaryTouch.active = false;
                        primaryTouch.past = primaryTouch.since;
                        primaryTouch.past = DateTime.Now;
                        PrimaryUntouched(primaryTouch);
                        AVR.Utils.Debug.Log("Primary untouched");
                    }
                }

                // secondary touch
                if (device.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouchValue))
                {
                    if (secondaryTouchValue && !secondaryTouch.active)
                    {
                        secondaryTouch.active = true;
                        secondaryTouch.past = secondaryTouch.since;
                        secondaryTouch.since = DateTime.Now;
                        SecondaryTouched(secondaryTouch);
                        AVR.Utils.Debug.Log("Secondary touched");
                    }
                    else if (!secondaryTouchValue && secondaryTouch.active)
                    {
                        secondaryTouch.active = false;
                        secondaryTouch.past = secondaryTouch.since;
                        secondaryTouch.past = DateTime.Now;
                        SecondaryUntouched(secondaryTouch);
                        AVR.Utils.Debug.Log("Secondary untouched");
                    }
                }

                // primary axis
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primaryAxisValue))
                {
                    foreach (var deadzone in primaryDeadzones)
                        if (primaryAxisValue.magnitude < deadzone.x)
                        {
                            primaryAxisValue = Vector2.zero;
                            break;
                        }
                        else if (primaryAxisValue.magnitude < deadzone.y)
                        {
                            primaryAxisValue = primaryAxisValue.normalized * deadzone.y;
                            break;
                        }
                    var angle = Mathf.Atan2(primaryAxisValue.y, primaryAxisValue.x);
                    angle = Mathf.Rad2Deg * angle % 360 / 360;
                    foreach (var deadangle in primaryDeadangles)
                        if (angle > deadangle.x && angle < deadangle.y)
                        {
                            primaryAxisValue = Vector2.zero;
                            break;
                        }

                    if (primaryAxisValue != Vector2.zero && primaryAxis.active == Vector2.zero)
                    {
                        primaryAxis.active = primaryAxisValue;
                        primaryAxis.past = primaryAxis.since;
                        primaryAxis.since = DateTime.Now;
                        PrimaryAxis(primaryAxis);
                        AVR.Utils.Debug.Log("Primary axis active");
                    }
                    else if (primaryAxisValue == Vector2.zero && primaryAxis.active != Vector2.zero)
                    {
                        primaryAxis.active = Vector2.zero;
                        primaryAxis.past = primaryAxis.since;
                        primaryAxis.since = DateTime.Now;
                        PrimaryAxis(primaryAxis);
                        AVR.Utils.Debug.Log("Primary axis inactive");
                    }
                    else
                    {
                        primaryAxis.active = primaryAxisValue;
                        PrimaryAxis(primaryAxis);
                    }
                }

                // secondary axis
                if (device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 secondaryAxisValue))
                {
                    foreach (var deadzone in secondaryDeadzones)
                        if (secondaryAxisValue.magnitude < deadzone.x)
                        {
                            secondaryAxisValue = Vector2.zero;
                            break;
                        }
                        else if (secondaryAxisValue.magnitude < deadzone.y)
                        {
                            secondaryAxisValue = secondaryAxisValue.normalized * deadzone.y;
                            break;
                        }
                    var angle = Mathf.Atan2(secondaryAxisValue.y, secondaryAxisValue.x);
                    angle = Mathf.Rad2Deg * angle % 360 / 360;
                    foreach (var deadangle in secondaryDeadangles)
                        if (angle > deadangle.x && angle < deadangle.y)
                        {
                            secondaryAxisValue = Vector2.zero;
                            break;
                        }

                    if (secondaryAxisValue != Vector2.zero && secondaryAxis.active == Vector2.zero)
                    {
                        secondaryAxis.active = secondaryAxisValue;
                        secondaryAxis.past = secondaryAxis.since;
                        secondaryAxis.since = DateTime.Now;
                        SecondaryAxis(secondaryAxis);
                        AVR.Utils.Debug.Log("Secondary axis active");
                    }
                    else if (secondaryAxisValue == Vector2.zero && secondaryAxis.active != Vector2.zero)
                    {
                        secondaryAxis.active = Vector2.zero;
                        secondaryAxis.past = secondaryAxis.since;
                        secondaryAxis.since = DateTime.Now;
                        SecondaryAxis(secondaryAxis);
                        AVR.Utils.Debug.Log("Secondary axis inactive");
                    }
                    else
                    {
                        secondaryAxis.active = secondaryAxisValue;
                        SecondaryAxis(secondaryAxis);
                    }
                }

                // primary axis click
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primaryAxisClickValue))
                {
                    if (primaryAxisClickValue && !primaryAxisClick.active)
                    {
                        primaryAxisClick.active = true;
                        primaryAxisClick.past = primaryAxisClick.since;
                        primaryAxisClick.since = DateTime.Now;
                        PrimaryAxisClicked(primaryAxisClick);
                        AVR.Utils.Debug.Log("Primary axis clicked");
                    }
                    else if (!primaryAxisClickValue && primaryAxisClick.active)
                    {
                        primaryAxisClick.active = false;
                        primaryAxisClick.past = primaryAxisClick.since;
                        primaryAxisClick.since = DateTime.Now;
                        PrimaryAxisUnclicked(primaryAxisClick);
                        AVR.Utils.Debug.Log("Primary axis unclicked");
                    }
                }

                // secondary axis click
                if (device.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out bool secondaryAxisClickValue))
                {
                    if (secondaryAxisClickValue && !secondaryAxisClick.active)
                    {
                        secondaryAxisClick.active = true;
                        secondaryAxisClick.past = secondaryAxisClick.since;
                        secondaryAxisClick.since = DateTime.Now;
                        SecondaryAxisClicked(secondaryAxisClick);
                        AVR.Utils.Debug.Log("Secondary axis clicked");
                    }
                    else if (!secondaryAxisClickValue && secondaryAxisClick.active)
                    {
                        secondaryAxisClick.active = false;
                        secondaryAxisClick.past = secondaryAxisClick.since;
                        secondaryAxisClick.since = DateTime.Now;
                        SecondaryAxisUnclicked(secondaryAxisClick);
                        AVR.Utils.Debug.Log("Secondary axis unclicked");
                    }
                }

                // primary axis touch
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool primaryAxisTouchValue))
                {
                    if (primaryAxisTouchValue && !primaryAxisTouch.active)
                    {
                        primaryAxisTouch.active = true;
                        primaryAxisTouch.past = primaryAxisTouch.since;
                        primaryAxisTouch.since = DateTime.Now;
                        PrimaryAxisTouched(primaryAxisTouch);
                        AVR.Utils.Debug.Log("Primary axis touched");
                    }
                    else if (!primaryAxisTouchValue && primaryAxisTouch.active)
                    {
                        primaryAxisTouch.active = false;
                        primaryAxisTouch.past = primaryAxisTouch.since;
                        primaryAxisTouch.since = DateTime.Now;
                        PrimaryAxisUntouched(primaryAxisTouch);
                        AVR.Utils.Debug.Log("Primary axis untouched");
                    }
                }

                // secondary axis touch
                if (device.TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out bool secondaryAxisTouchValue))
                {
                    if (secondaryAxisTouchValue && !secondaryAxisTouch.active)
                    {
                        secondaryAxisTouch.active = true;
                        secondaryAxisTouch.past = secondaryAxisTouch.since;
                        secondaryAxisTouch.since = DateTime.Now;
                        SecondaryAxisTouched(secondaryAxisTouch);
                        AVR.Utils.Debug.Log("Secondary axis touched");
                    }
                    else if (!secondaryAxisTouchValue && secondaryAxisTouch.active)
                    {
                        secondaryAxisTouch.active = false;
                        secondaryAxisTouch.past = secondaryAxisTouch.since;
                        secondaryAxisTouch.since = DateTime.Now;
                        SecondaryAxisUntouched(secondaryAxisTouch);
                        AVR.Utils.Debug.Log("Secondary axis untouched");
                    }
                }
            }
        }
    }
}