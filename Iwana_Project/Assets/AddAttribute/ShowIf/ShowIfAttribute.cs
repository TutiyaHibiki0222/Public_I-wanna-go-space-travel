using UnityEngine;

namespace Attribute.Add
{
    public enum HideMode
    {
        [Tooltip("��\��")]   Hide,
        [Tooltip("�ҏW�s��")] ReadOnly
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }
        public HideMode HideMode    { get; private set; }

        public ShowIfAttribute(string conditionName, HideMode hideMode = HideMode.ReadOnly)
        {
            ConditionName = conditionName;
            HideMode      = hideMode;
        }
    }
}
