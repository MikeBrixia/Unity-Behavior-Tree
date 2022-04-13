using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// All the types supported by the blackboard
public enum BlackboardSupportedTypes { Boolean, Float, GameObject, Vector2, Vector3, String, Double, Integer, Color }

[System.Serializable]
public class BlackboardPropertyBase
{

    [SerializeField]
    public string name = "NewVar";

    [SerializeField]
    public BlackboardSupportedTypes valueType = BlackboardSupportedTypes.Boolean;

    ///<summary>
    /// Initialize this property with the given type default value.
    ///</summary>
    ///<returns> returns a copy of this property initialized with it's value type</returns>
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

