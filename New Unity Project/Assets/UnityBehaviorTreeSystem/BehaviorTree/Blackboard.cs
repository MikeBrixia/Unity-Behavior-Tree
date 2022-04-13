using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "AI/Blackboard")]
    public class Blackboard : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The variables of this blackboard
        /// </summary>
        [SerializeField] public Dictionary<string, BlackboardPropertyBase> blackboardValues = new Dictionary<string, BlackboardPropertyBase>();
        
        [SerializeField] public List<BlackboardPropertyBase> blackboardProperties = new List<BlackboardPropertyBase>();
        
        public T GetBlackboardValueByName<T>(string name)
        {
            BlackboardProperty<T> property = (BlackboardProperty<T>) blackboardValues[name];
            return property.value;
        }

        public void SetBlackbordValue<T>(string name, T newValue)
        {
            BlackboardProperty<T> property = (BlackboardProperty<T>) blackboardValues[name];
            property.value = newValue;
        }
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            blackboardValues = new Dictionary<string, BlackboardPropertyBase>();
            foreach(BlackboardPropertyBase property in blackboardProperties)
            {
                BlackboardPropertyBase newProperty = property.InitializeProperty();
                blackboardValues.TryAdd(property.name, newProperty);
            }
        }

        public Blackboard Clone()
        {
            return Instantiate(this);
        }
    }
}

