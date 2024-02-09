using Autohand;
using UnityEngine;

namespace AVR
{
    namespace Entity
    {
        public class Player : Actor
        {
            public static Player Instance { get; private set; } = null;
            public static bool KeepHorizontalMenu = true;
            public UI.Menu MainMenu;
            public UI.MinimalMenu MinimalMenu;
            private Base.Controller _controller;
            public Base.Controller Controller
            {
                get
                {
                    return _controller;
                }
                set
                {
                    _controller = value ?? throw new System.ArgumentNullException("Controller cannot be null");
                    OnControllerChanged?.Invoke(_controller);
                }
            }

            public delegate void ControllerChanged(Base.Controller controller);
            public event ControllerChanged OnControllerChanged;

            void Start()
            {
                if (Instance != null)
                {
                    Utils.Debug.LogWarning("Multiple Player instances detected. Destroying this instance", this);
                    Destroy(this);
                    return;
                }
                else Instance = this;

                OnControllerChanged += (Base.Controller controller) =>
                {
                    controller.OnMainMenuEvent += () => Menu(MenuType.MainMenu);
                    controller.OnInstanceMenuEvent += () => Menu(MenuType.InstanceMenu);
                    controller.OnMinimalMenuEvent += () => Menu(MenuType.MinimalMenu);
                };
                Menu(MenuType.None);
                Controller = UnityEngine.XR.XRSettings.enabled ? new AVR.Controller.VRController() : new AVR.Controller.DesktopController();
            }

            void Update()
            {
                if (Controller != null)
                    Controller.Update();
            }

            public AutoHandPlayer Locomotion => GetComponent<AutoHandPlayer>();


            public enum MenuType
            {
                None,
                MainMenu,
                InstanceMenu,
                MinimalMenu
            };

            public MenuType CurrentMenu = MenuType.None;
            public void Menu(MenuType type)
            {
                CurrentMenu = CurrentMenu == type ? MenuType.None : type;
                switch (CurrentMenu)
                {
                    case MenuType.MainMenu:
                        MainMenu.gameObject.SetActive(true);
                        MainMenu.SetTab(new() { source = null, name = "avr.home" });
                        // MinimalMenu.gameObject.SetActive(false);
                        break;
                    case MenuType.InstanceMenu:
                        MainMenu.gameObject.SetActive(true);
                        var sc = MainMenu.SetTab(new () { source = null, name = "avr.instance"});
                        if(!sc) MainMenu.SetTab(new () { source = null, name = "avr.home"});
                        // MinimalMenu.gameObject.SetActive(false);
                        break;
                    case MenuType.MinimalMenu:
                        MainMenu.gameObject.SetActive(false);
                        // MinimalMenu.gameObject.SetActive(true);
                        break;
                    default:
                        MainMenu.gameObject.SetActive(false);
                        // MinimalMenu.gameObject.SetActive(false);
                        break;
                }

                if (MainMenu.gameObject.activeSelf)
                {
                    var camera = Locomotion.headCamera;
                    var size_head = Locomotion.headRadius;
                    var size_height = Locomotion.bodyCollider.height;

                    // place the menu in front of the player 
                    MainMenu.transform.position = camera.transform.position + camera.transform.forward * (size_head + size_height/3.5f) - camera.transform.up * size_head;
                    MainMenu.transform.rotation = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
                    if (KeepHorizontalMenu)
                        MainMenu.transform.rotation = Quaternion.Euler(MainMenu.transform.rotation.eulerAngles.x, MainMenu.transform.rotation.eulerAngles.y, 0f);
                    // with height of 1m, the scale is 0.05
                    MainMenu.transform.localScale = 0.05f * size_height * Vector3.one;
                }
            }
            public void Teleport(Transform transform)
            {
                AVR.Utils.Debug.Log("Teleporting player to " + transform.position);
                Locomotion.SetPosition(transform.position);
                Locomotion.SetRotation(transform.rotation);
            }
        }
    }
}

