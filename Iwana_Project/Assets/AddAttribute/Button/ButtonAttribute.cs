using System;
using UnityEngine;

namespace Attribute.Add 
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string buttonName { get; }
        public ButtonAttribute(string _buttonName = null)
        {
            this.buttonName = _buttonName;
        }
    }
}