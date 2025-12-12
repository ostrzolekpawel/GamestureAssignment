using System;
using GamestureAssignment.Configs;
using UnityEditor;
using UnityEngine;

namespace GamestureAssignment.Editor
{
    [CustomPropertyDrawer(typeof(CollectableInfo))]
    public class CollectableInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw foldout
            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty typeProp = property.FindPropertyRelative("_type");
            SerializedProperty codeProp = property.FindPropertyRelative("_code");

            DrawLabeledField("Type", typeProp, ref y, position);

            var selectedType = (CollectableType)typeProp.enumValueIndex;

            if (selectedType == CollectableType.Wood)
            {
                CollectableSizeType size = CollectableSizeType.Small;

                if (Enum.TryParse(codeProp.stringValue, out CollectableSizeType parsedSize))
                {
                    size = parsedSize;
                }

                var newSize = DrawEnumDropdown("Size", size, ref y, position);
                codeProp.stringValue = newSize.ToString();
            }
            else if (selectedType == CollectableType.Gem)
            {
                CollectableRarityType size = CollectableRarityType.Common;

                if (Enum.TryParse(codeProp.stringValue, out CollectableRarityType parsedRarity))
                {
                    size = parsedRarity;
                }

                var newSize = DrawEnumDropdown("Rarity", size, ref y, position);
                codeProp.stringValue = newSize.ToString();
            }
            else
            {
                codeProp.stringValue = string.Empty;
            }

            EditorGUI.indentLevel--;
        }

        private void DrawLabeledField(string label, SerializedProperty prop, ref float y, Rect totalRect)
        {
            float line = EditorGUIUtility.singleLineHeight;
            Rect rect = new Rect(totalRect.x, y, totalRect.width, line);

            EditorGUI.PropertyField(rect, prop, new GUIContent(label));
            y += line + EditorGUIUtility.standardVerticalSpacing;
        }

        private TEnum DrawEnumDropdown<TEnum>(string label, TEnum value, ref float y, Rect totalRect)
            where TEnum : Enum
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect rect = new Rect(totalRect.x, y, totalRect.width, lineHeight);

            var result = (TEnum)EditorGUI.EnumPopup(rect, label, value);

            y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            return result;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
            {
                return height;
            }

            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var type = (CollectableType)property.FindPropertyRelative("_type").enumValueIndex;
            if (type == CollectableType.Wood || type == CollectableType.Gem)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}
