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
    /// The blackboard supported propertyValue types
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
    public abstract class BlackboardPropertyBase
    {
        ///<summary>
        /// The name of this blackboard property
        ///</summary>
        [SerializeField] public string name = "NewVar";

        ///<summary>
        /// The propertyValue type of this property
        ///</summary>
        [SerializeField] public BlackboardSupportedTypes valueType = BlackboardSupportedTypes.Boolean;
        
        /// <summary>
        /// Set the value of this property.
        /// </summary>
        /// <param name="propertyValue"> The new value of the property. </param>
        public abstract void SetValue(object propertyValue);
        
        /// <summary>
        /// Get the value of this property.
        /// </summary>
        /// <returns> The value of the property. </returns>
        public abstract object GetValue();

        protected BlackboardPropertyBase(string name, BlackboardSupportedTypes valueType)
        {
            this.name = name;
            this.valueType = valueType;
        }
        
        ///<summary>
        /// Create a new property with the name and type specified in the selector.
        ///</summary>
        ///<returns> A blackboard property of selected type and name. </returns>
        public static BlackboardPropertyBase CreateProperty(PropertySelector selector)
        {
            BlackboardPropertyBase property = null;
            switch (selector.type)
            {
                case BlackboardSupportedTypes.Boolean:
                    property = new BlackboardProperty<bool>(selector.name, BlackboardSupportedTypes.Boolean, false);
                    break;

                case BlackboardSupportedTypes.Float:
                    property = new BlackboardProperty<float>(selector.name, BlackboardSupportedTypes.Float, 0f);
                    break;
                
                case BlackboardSupportedTypes.Double:
                    property = new BlackboardProperty<double>(selector.name, BlackboardSupportedTypes.Double, 0f);
                    break;

                case BlackboardSupportedTypes.Integer:
                    property = new BlackboardProperty<int>(selector.name, BlackboardSupportedTypes.Integer, 0);
                    break;

                case BlackboardSupportedTypes.String:
                    property = new BlackboardProperty<string>(selector.name, BlackboardSupportedTypes.String, "None");
                    break;
                
                case BlackboardSupportedTypes.Object:
                    property = new BlackboardProperty<object>(selector.name, BlackboardSupportedTypes.Object, null);
                    break;
            }
            return property;
        }
    }
    
    /// <summary>
    /// Class used for creating blackboard properties of type T
    /// </summary>
    /// <typeparam name="T"> The type of the blackboard property. </typeparam>
    public class BlackboardProperty<T> : BlackboardPropertyBase
    {
        /// <summary>
        /// The value of this property
        /// </summary>
        public T value;

        public BlackboardProperty(string name, BlackboardSupportedTypes valueType, T value) : base(name, valueType)
        {
            this.value = value;
        }

        public override void SetValue(object propertyValue)
        {
            this.value = (T) propertyValue;
        }

        public override object GetValue()
        {
            return value;
        }
    }
}


