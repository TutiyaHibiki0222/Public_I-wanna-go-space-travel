using UnityEngine;

namespace Attribute.Add
{
    public enum HideMode
    {
        [Tooltip("非表示")]   Hide,
        [Tooltip("編集不可")] ReadOnly
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
