using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace AVR
{
    namespace SDK
    {
        [CustomEditor(typeof(AVR.SDK.AvatarDescriptor)), CanEditMultipleObjects]
        public class AvatarDescriptor_Inspector : Editor
        {
            public override VisualElement CreateInspectorGUI()
            {
                // import UXML
                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/AvatarDescriptor_Inspector/UI.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();
                labelFromUXML.Q<Button>("opensdk").clicked += () => AVR.SDK.Panel.Init();

                var target = serializedObject.targetObject as AvatarDescriptor;
                labelFromUXML.Q<Vector3Field>("tracker_head_position").value = target.headPosition;
                labelFromUXML.Q<Vector3Field>("tracker_head_rotation").value = target.headRotation;
                labelFromUXML.Q<Vector3Field>("tracker_lefthand_position").value = target.leftHandPosition;
                labelFromUXML.Q<Vector3Field>("tracker_lefthand_rotation").value = target.leftHandRotation;
                labelFromUXML.Q<Vector3Field>("tracker_righthand_position").value = target.rightHandPosition;
                labelFromUXML.Q<Vector3Field>("tracker_righthand_rotation").value = target.rightHandRotation;

                // get button head detect
                labelFromUXML.Q<Button>("tracker_head_detect").clicked += () =>
                {
                    var target = serializedObject.targetObject as AVR.SDK.AvatarDescriptor;
                    Transform avatar = target.transform;
                    var postion_abs = target.GetBoneTransform(HumanBodyBones.Head).transform.position - avatar.position;
                    var rotation_abs = target.GetBoneTransform(HumanBodyBones.Head).transform.rotation.eulerAngles - avatar.rotation.eulerAngles;
                    labelFromUXML.Q<Vector3Field>("tracker_head_position").value = target.headPosition = postion_abs;
                    labelFromUXML.Q<Vector3Field>("tracker_head_rotation").value = target.headRotation = rotation_abs;
                };
                // get button left hand detect
                labelFromUXML.Q<Button>("tracker_lefthand_detect").clicked += () =>
                {
                    var target = serializedObject.targetObject as AVR.SDK.AvatarDescriptor;
                    Transform avatar = target.transform;
                    var postion_abs = target.GetBoneTransform(HumanBodyBones.LeftHand).transform.position - avatar.position;
                    var rotation_abs = target.GetBoneTransform(HumanBodyBones.LeftHand).transform.rotation.eulerAngles - avatar.rotation.eulerAngles;
                    labelFromUXML.Q<Vector3Field>("tracker_lefthand_position").value = target.leftHandPosition = postion_abs;
                    labelFromUXML.Q<Vector3Field>("tracker_lefthand_rotation").value = target.leftHandRotation = rotation_abs;
                };
                // get button right hand detect
                labelFromUXML.Q<Button>("tracker_righthand_detect").clicked += () =>
                {
                    var target = serializedObject.targetObject as AVR.SDK.AvatarDescriptor;
                    Transform avatar = target.transform;
                    var postion_abs = target.GetBoneTransform(HumanBodyBones.RightHand).transform.position - avatar.position;
                    var rotation_abs = target.GetBoneTransform(HumanBodyBones.RightHand).transform.rotation.eulerAngles - avatar.rotation.eulerAngles;
                    labelFromUXML.Q<Vector3Field>("tracker_righthand_position").value = target.rightHandPosition = postion_abs;
                    labelFromUXML.Q<Vector3Field>("tracker_righthand_rotation").value = target.rightHandRotation = rotation_abs;
                };

                // get button head reset
                labelFromUXML.Q<Button>("tracker_head_reset").clicked += () =>
                {
                    var target = serializedObject.targetObject as AVR.SDK.AvatarDescriptor;
                    labelFromUXML.Q<Vector3Field>("tracker_head_position").value = target.headPosition = Vector3.zero;
                    labelFromUXML.Q<Vector3Field>("tracker_head_rotation").value = target.headRotation = Vector3.zero;
                };
                // get button left hand reset
                labelFromUXML.Q<Button>("tracker_lefthand_reset").clicked += () =>
                {
                    var target = serializedObject.targetObject as AVR.SDK.AvatarDescriptor;
                    labelFromUXML.Q<Vector3Field>("tracker_lefthand_position").value = target.leftHandPosition = Vector3.zero;
                    labelFromUXML.Q<Vector3Field>("tracker_lefthand_rotation").value = target.leftHandRotation = Vector3.zero;
                };
                // get button right hand reset
                labelFromUXML.Q<Button>("tracker_righthand_reset").clicked += () =>
                {
                    var target = serializedObject.targetObject as AVR.SDK.AvatarDescriptor;
                    labelFromUXML.Q<Vector3Field>("tracker_righthand_position").value = target.rightHandPosition = Vector3.zero;
                    labelFromUXML.Q<Vector3Field>("tracker_righthand_rotation").value = target.rightHandRotation = Vector3.zero;
                };

                // name firld 
                labelFromUXML.Q<TextField>("name").value = target.Name;
                labelFromUXML.Q<TextField>("name").RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    target.Name = evt.newValue;
                });

                return labelFromUXML;
            }
        }
    }
}