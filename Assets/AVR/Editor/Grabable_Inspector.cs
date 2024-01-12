using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVR
{
    [CustomEditor(typeof(AVR.GrabEntity)), CanEditMultipleObjects]
    public class Grabable_Inspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // load UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/avrgraberentity.uxml");
            VisualTreeAsset visualTree2 = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/avr.grabber.grabber.uxml");
            // clone UXML
            VisualElement container = visualTree.CloneTree();
            VisualElement grabbers = container.Q<VisualElement>("avr-grabber-list");

            AVR.GrabEntity grabEntity = (AVR.GrabEntity)target;
            if (grabEntity != null)
                foreach (var grabber in grabEntity.grabbers)
                {
                    VisualElement grabberElement = visualTree2.CloneTree();
                    grabberElement.Q<ObjectField>("avr-grabber-ref").value = grabber.grabber;
                    grabberElement.Q<ObjectField>("avr-grabber-ref").RegisterCallback<ChangeEvent<Object>>((evt) =>
                    {
                        grabber.grabber = (GameObject)evt.newValue;
                    });
                    grabberElement.Q<Vector3Field>("avr-grabber-relpos").value = grabber.relpos;
                    grabberElement.Q<Vector3Field>("avr-grabber-relpos").RegisterCallback<ChangeEvent<Vector3>>((evt) =>
                    {
                        grabber.relpos = evt.newValue;
                    });
                    grabberElement.Q<Vector3Field>("avr-grabber-relrot").value = grabber.relrot.eulerAngles;
                    grabberElement.Q<Vector3Field>("avr-grabber-relrot").RegisterCallback<ChangeEvent<Vector3>>((evt) =>
                    {
                        grabber.relrot = Quaternion.Euler(evt.newValue);
                    });
                    grabberElement.Q<Vector3Field>("avr-grabber-angleoffset").value = grabber.angleOffset.eulerAngles;
                    grabberElement.Q<Vector3Field>("avr-grabber-angleoffset").RegisterCallback<ChangeEvent<Vector3>>((evt) =>
                    {
                        grabber.angleOffset = Quaternion.Euler(evt.newValue);
                    });
                    grabberElement.Q<Vector3Field>("avr-grabber-zone").value = grabber.zoneSize;
                    grabberElement.Q<Vector3Field>("avr-grabber-zone").RegisterCallback<ChangeEvent<Vector3>>((evt) =>
                    {
                        grabber.zoneSize = evt.newValue;
                    });
                    grabbers.Add(grabberElement);
                }

            // load USS
            return container;

        }
    }
}