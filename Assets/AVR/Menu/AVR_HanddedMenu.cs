using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR
{
    public class HanddedMenu : MonoBehaviour
    {
        public AVR.Player player;
        public AVR.SDK.BaseUI ui;

        void Start()
        {
            ui.animator.SetBool("avr-handded", true);
            Active = false;
        }

        bool Active
        {
            get => !ui.animator.GetBool("avr-hide");
            set => ui.animator.SetBool("avr-hide", !value);
        }

        bool Hand
        {
            get => ui.animator.GetBool("avr-handded-left");
            set => ui.animator.SetBool("avr-handded-left", value);
        }

        public void OnActionHandLeft()
        {
            if (!Active)
                Hand = true;
            if (!Hand)
                Hand = true;
            else Active = !Active;
        }

        public void OnActionHandRight()
        {
            if (!Active)
                Hand = false;
            if (Hand)
                Hand = false;
            else Active = !Active;
        }
    }
}
