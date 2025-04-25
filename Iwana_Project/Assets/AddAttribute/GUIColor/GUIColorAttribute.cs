using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attribute.Add 
{

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class GUIColorAttribute : PropertyAttribute
    {
        public Color color { get; }

        public GUIColorAttribute(float r, float g, float b)
        {
            color = new Color(r, g, b);
        }
        public GUIColorAttribute(Color set)
        {
            color = set;
        }
    }

}

