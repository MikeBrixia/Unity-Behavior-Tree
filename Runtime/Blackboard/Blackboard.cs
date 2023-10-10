using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT.Runtime
{
    
    /// <summary>
    /// Only used to expose data useful for creating
    /// blackboard properties defined by the user when
    /// the game starts.
    /// </summary>
    [Serializable]
    public struct PropertySelector
    {
        [Tooltip("The name of the property, must be unique")]
        public string name;
            
        [Tooltip("The type of the property")]
        public BlackboardSupportedTypes type;
    }
    
    /// <summary>
    /// The blackboard is the "brain" of the Behavior Tree, responsible of storing
    /// relevant data, used to make it's own decision.
    /// The blackboard supports a limited number of types which can be used by the tree,
    /// see BlackboardSupportedTypes in BlackboardPropertyBase for more info.
    /// </summary>
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "AI/Blackboard")]
    public class Blackboard : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Dictionary which contains all serialized blackboard key-value pairs.
        /// </summary>
        private Dictionary<string, BlackboardPropertyBase> blackboardDict = new Dictionary<string, BlackboardPropertyBase>();
        
        ///<summary>
        /// All the properties stored in this blackboard.
        ///</summary>
        [SerializeField] private List<PropertySelector> blackboardProperties = new List<PropertySelector>();
        
        ///<summary>
        /// Get a blackboard value of type T by it's key.
        ///</summary>
        ///<param name="key"> the blackboard key used to search for the value</param>
        ///<returns> The value of type T associated with the given blackboard key.</returns>
        public T GetBlackboardValueByKey<T>(string key)
        {
            BlackboardPropertyBase property = blackboardDict[key];
            return (T) property.GetValue();
        }
        
        ///<summary>
        /// Set the blackboard value of type T associated with the given key
        ///</summary>
        ///<param name="key"> the blackboard key used to search and set the blackboard property.</param>
        ///<param name="newValue"> the new property value of type T</param>
        public void SetBlackboardValue<T>(string key, T newValue)
        {
            BlackboardPropertyBase property = blackboardDict[key];
            property.SetValue(newValue);
        }
        
        public void OnBeforeSerialize()
        {
        }
        
        public void OnAfterDeserialize()
        {
            blackboardDict = new Dictionary<string, BlackboardPropertyBase>();
            foreach(PropertySelector propertySelector in blackboardProperties)
            {
                // Ignores properties without a type.
                if (propertySelector.type != BlackboardSupportedTypes.None)
                {
                    BlackboardPropertyBase property = BlackboardPropertyBase.CreateProperty(propertySelector);
                    blackboardDict.TryAdd(property.name, property);
                }
            }
        }
        
        ///<summary>
        ///Clone this blackboard asset.
        ///</summary>
        ///<returns> A copy of this blackboard asset.</returns>
        public Blackboard Clone()
        {
            return Instantiate(this);
        }

#if UNITY_EDITOR

        public string[] GetVariablesNames()
        {
            return blackboardDict.Keys.ToArray();;
        }

        public string[] GetVariableNamesOfType(BlackboardSupportedTypes type)
        {
            List<string> names = new List<string>();
            // Iterate over all blackboard properties and select only the ones which
            // matches the requested type.
            foreach (KeyValuePair<string, BlackboardPropertyBase> pair in blackboardDict)
            {
                if (pair.Value.valueType == type)
                {
                    names.Add(pair.Key);
                }
            }

            return names.ToArray();
        }
#endif
    }
}

