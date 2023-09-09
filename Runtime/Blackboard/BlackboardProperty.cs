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
        /// <summary>
        /// Unity GameObject type.
        /// </summary>
        GameObject = 3, 
        /// <summary>
        /// Unity Vector2 type.
        /// </summary>
        Vector2 = 4, 
        /// <summary>
        /// Unity Vector3 type.
        /// </summary>
        Vector3 = 5,
        ///<summary>
        /// Unity Quaternion type
        /// </summary>
        Quaternion = 6,
        /// <summary>
        /// Primitive string type.
        /// </summary>
        String = 7, 
        /// <summary>
        /// Primitive double type.
        /// </summary>
        Double = 8, 
        /// <summary>
        /// Primitive int type.
        /// </summary>
        Integer = 9, 
        /// <summary>
        /// Unity Color type.
        /// </summary>
        Color = 10
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


