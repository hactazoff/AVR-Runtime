using Autohand;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace AVR
{
    public class Player : AVR.Actor
    {

        AutoHandPlayer BodyPlayer => GetComponent<AutoHandPlayer>();
        public AVR.Scene.LoadSceneEventArgs CurrentScene { get; private set; }

        void Start()
        {
            AVR.Scene.onLoadSceneSync += OnLoadSceneSync;
        }

        private void OnLoadSceneSync(AVR.Scene.LoadSceneEventArgs scene)
        {
            CurrentScene = scene;
            if (scene.error) return;
            Debug.Log(scene.scene + " " + scene.descriptor + " " + scene.hash);
            GameObject spawn = scene.descriptor.GetSpawn();
            Teleport(spawn.GetComponent<Transform>());
            AVR.Debug.Log("Player spawned at " + spawn.name);
        }

        public override void Teleport(Transform target)
        {
            BodyPlayer.SetPosition(
                target.position,
                Quaternion.Euler(0, target.rotation.eulerAngles.y, 0)
            );
        }

        public Hand GetLeftHand()
        {
            return BodyPlayer.handLeft;
        }

        public Hand GetRightHand()
        {
            return BodyPlayer.handRight;
        }
    }
}