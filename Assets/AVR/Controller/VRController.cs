// this is the controller script for the VR
// read inputs from the controllers and move the player accordingly

using System.Collections.Generic;
using AVR.Input;
using UnityEngine.XR;

namespace AVR
{
    namespace Controller
    {
        public class VRController : AVR.Base.Controller
        {
            public VRInput Lefthand;
            public VRInput Righthand;

            public VRController()
            {
                Lefthand = new VRInput(XRNode.LeftHand);
                Righthand = new VRInput(XRNode.RightHand);

                Lefthand.OnSecondaryReleasedEvent += (InputEvent<bool> e) => { 
                    AVR.Utils.Debug.Log("Secondary Duration: " + e.Duration);
                    if (e.Duration > 0.75f) { MinimalMenu(); } else InstanceMenu(); };
                Righthand.OnSecondaryReleasedEvent += (InputEvent<bool> e) => { 
                    AVR.Utils.Debug.Log("Secondary Duration: " + e.Duration);
                    if (e.Duration > 0.75f) { MinimalMenu(); } else MainMenu(); };
            }

            public override void Update()
            {
                base.Update();
                Lefthand.Update();
                Righthand.Update();
            }
        }
    }
}