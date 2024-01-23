namespace AVR
{
    namespace Base
    {
        public class Controller
        {
            public virtual void Start()
            {

            }
            public virtual void Update() { }

            public delegate void OnMainMenu();
            public event OnMainMenu OnMainMenuEvent;
            public void MainMenu() => OnMainMenuEvent?.Invoke();

            public delegate void OnInstanceMenu();
            public event OnInstanceMenu OnInstanceMenuEvent;
            public void InstanceMenu() => OnInstanceMenuEvent?.Invoke();

            public delegate void OnMinimalMenu();
            public event OnMinimalMenu OnMinimalMenuEvent;
            public void MinimalMenu() => OnMinimalMenuEvent?.Invoke();
        }


    }
}