using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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
        ///<summary>
        /// Unity Quaternion type
        /// </summary>
        Quaternion,
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

    public enum BlackboardObjectType
    {
        Object,
        StaticArray,
        DynamicArray,
        HashSet
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
        /// The value type of this property
        ///</summary>
        [FormerlySerializedAs("variableType")] [SerializeField]
        public BlackboardObjectType objectType = BlackboardObjectType.Object;
        
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
                    property = InitializeProperty<bool>(false);
                    break;

                case BlackboardSupportedTypes.Float:
                    property = InitializeProperty<float>(0f);
                    break;

                case BlackboardSupportedTypes.Vector2:
                    property = InitializeProperty<Vector2>(Vector2.zero);
                    break;

                case BlackboardSupportedTypes.Vector3:
                    property = InitializeProperty<Vector3>(Vector3.zero);
                    break;
                
                case BlackboardSupportedTypes.Quaternion:
                    property = InitializeProperty<Quaternion>(Quaternion.identity);
                    break;
                
                case BlackboardSupportedTypes.Double:
                    property = InitializeProperty<double>(0f);
                    break;

                case BlackboardSupportedTypes.Integer:
                    property = InitializeProperty<int>(0);
                    break;

                case BlackboardSupportedTypes.String:
                    property = InitializeProperty<string>("None");
                    break;

                case BlackboardSupportedTypes.Color:
                    property = InitializeProperty<Color>(Color.black);
                    break;
                
                case BlackboardSupportedTypes.GameObject:
                    property = InitializeProperty<GameObject>(null);
                    break;
            }
            return property;
        }

        private BlackboardPropertyBase InitializeProperty<T>(T value)
        {
            BlackboardPropertyBase property = null;
            switch (objectType)
            {
                case BlackboardObjectType.StaticArray:
                    property = new BlackboardProperty<T[]>(name, valueType, Array.Empty<T>());
                    break;
                
                case BlackboardObjectType.DynamicArray:
                    property = new BlackboardProperty<List<T>>(name, valueType, null);
                    break;
                
                case BlackboardObjectType.HashSet:
                    property = new BlackboardProperty<HashSet<T>>(name, valueType, null);
                    break;
                
                case BlackboardObjectType.Object:
                    property = new BlackboardProperty<T>(name, valueType, value);
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


