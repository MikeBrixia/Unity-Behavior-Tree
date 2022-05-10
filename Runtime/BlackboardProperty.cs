using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// The blackboard supported value types
    ///</summary>
    public enum BlackboardSupportedTypes 
    { 
        /// <summary>
        /// Primitive bool type.
        /// </summary>
        Boolean, 
        /// <summary>
        /// Primitive float type.
        /// </summary>
        Float, 
        /// <summary>
        /// Unity GameObject type.
        /// </summary>
        GameObject, 
        /// <summary>
        /// Unity Vector2 type.
        /// </summary>
        Vector2, 
        /// <summary>
        /// Unity Vector3 type.
        /// </summary>
        Vector3, 
        /// <summary>
        /// Primitive string type.
        /// </summary>
        String, 
        /// <summary>
        /// Primitive double type.
        /// </summary>
        Double, 
        /// <summary>
        /// Primitive int type.
        /// </summary>
        Integer, 
        /// <summary>
        /// Unity Color type.
        /// </summary>
        Color 
    }

    [System.Serializable]
    public class BlackboardPropertyBase
    {
        ///<summary>
        /// The name of this blackboard property
        ///</summary>
        [SerializeField]
        public string name = "NewVar";

        ///<summary>
        /// The value type of this property
        ///</summary>
        [SerializeField]
        public BlackboardSupportedTypes valueType = BlackboardSupportedTypes.Boolean;

        ///<summary>
        /// Initialize this property with the given type default value.
        ///</summary>
        ///<returns> A copy of this property initialized with it's value type</returns>
        public BlackboardPropertyBase InitializeProperty()
        {
            BlackboardPropertyBase property = null;
            switch (valueType)
            {
                case BlackboardSupportedTypes.Boolean:
                    property = new BlackboardProperty<bool>(name, valueType, false);
                    break;

                case BlackboardSupportedTypes.Float:
                    property = new BlackboardProperty<float>(name, valueType, 0f);
                    break;

                case BlackboardSupportedTypes.Vector2:
                    property = new BlackboardProperty<Vector2>(name, valueType, Vector2.zero);
                    break;

                case BlackboardSupportedTypes.Vector3:
                    property = new BlackboardProperty<Vector3>(name, valueType, Vector3.zero);
                    break;

                case BlackboardSupportedTypes.Double:
                    property = new BlackboardProperty<double>(name, valueType, 0);
                    break;

                case BlackboardSupportedTypes.Integer:
                    property = new BlackboardProperty<int>(name, valueType, 0);
                    break;

                case BlackboardSupportedTypes.String:
                    property = new BlackboardProperty<string>(name, valueType, "None");
                    break;

                case BlackboardSupportedTypes.Color:
                    property = new BlackboardProperty<Color>(name, valueType, Color.black);
                    break;

                case BlackboardSupportedTypes.GameObject:
                    property = new BlackboardProperty<GameObject>(name, valueType, null);
                    break;
            }
            return property;
        }
    }

    public class BlackboardProperty<TValueType> : BlackboardPropertyBase
    {
        public TValueType value;

        public BlackboardProperty(string name, BlackboardSupportedTypes valueType, TValueType value)
        {
            this.name = name;
            this.valueType = valueType;
            this.value = value;
        }
    }
}


