using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace BT.Runtime
{
    ///<summary>
    /// The blackboard supported value types
    ///</summary>
    public enum BlackboardSupportedTypes 
    { 
        /// <summary>
        /// None means "no type".
        /// </summary>
        None = 0,
        /// <summary>
        /// Primitive bool type.
        /// </summary>
        Boolean = 1, 
        /// <summary>
        /// Primitive float type.
        /// </summary>
        Float = 2,
        /// /// <summary>
        /// Primitive double type.
        /// </summary>
        Double = 3, 
        /// <summary>
        /// Primitive int type.
        /// </summary>
        Integer = 4, 
        /// <summary>
        /// Primitive string type.
        /// </summary>
        String = 5,
        /// <summary>
        /// Unity Color type.
        /// </summary>
        Object = 6
    }
    
    [Serializable]
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
        public BlackboardPropertyBase CreateProperty()
        {
            BlackboardPropertyBase property = null;
            switch (valueType)
            {
                case BlackboardSupportedTypes.Boolean:
                    property = new BlackboardProperty<bool>(name, BlackboardSupportedTypes.Boolean, false);
                    break;

                case BlackboardSupportedTypes.Float:
                    property = new BlackboardProperty<float>(name, BlackboardSupportedTypes.Float, 0f);
                    break;
                
                case BlackboardSupportedTypes.Double:
                    property = new BlackboardProperty<double>(name, BlackboardSupportedTypes.Double, 0f);
                    break;

                case BlackboardSupportedTypes.Integer:
                    property = new BlackboardProperty<int>(name, BlackboardSupportedTypes.Integer, 0);
                    break;

                case BlackboardSupportedTypes.String:
                    property = new BlackboardProperty<string>(name, BlackboardSupportedTypes.String, "None");
                    break;
                
                case BlackboardSupportedTypes.Object:
                    property = new BlackboardProperty<object>(name, BlackboardSupportedTypes.Object, null);
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


