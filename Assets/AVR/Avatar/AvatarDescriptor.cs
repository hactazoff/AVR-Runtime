using UnityEngine;

namespace AVR
{
    namespace SDK
    {
        public class AvatarDescriptor : MonoBehaviour
        {
            [HideInInspector]
            public string ContentType = "avatar";
            public Vector3 headPosition;
            public Vector3 headRotation;
            public Vector3 leftHandPosition;
            public Vector3 leftHandRotation;
            public Vector3 rightHandPosition;
            public Vector3 rightHandRotation;

            public string Name
            {
                get => gameObject.name;
                set => gameObject.name = value;
            }

            public Animator AnimatorController => GetComponent<Animator>();
            public GameObject GetBoneTransform(HumanBodyBones humanBoneId) => AnimatorController == null ? null : AnimatorController.GetBoneTransform(humanBoneId).gameObject;
        }
    }
}
