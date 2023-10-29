using UnityEngine;

namespace BT.Runtime
{
    ///<summary>
    /// The component responsible for running the behavior treeAsset.
    ///</summary>
    public class BehaviorTreeComponent : MonoBehaviour
    {
        ///<summary>
        /// The behavior treeAsset asset this component is responsible to run
        ///</summary>
        [SerializeField] private BehaviorTree behaviorTree;
        
        /// <summary>
        /// Behavior treeAsset managed by the component.
        /// </summary>
        public BehaviorTree tree => behaviorTree;
        
        ///<summary>
        /// if true, the behavior treeAsset is going to update each frame, otherwise
        /// it will use a user defined update interval(updateInterval). By default
        /// it is set to true.
        ///</summary>
        public bool canTick = true;

        ///<summary>
        /// The rate at which the behavior treeAsset it's going
        /// to be updated. If canTick is set to true this value will
        /// be ignored.
        ///</summary>
        public float updateInterval = 0.1f;
        
        ///<summary>
        /// The blackboard component used by currently assigned Behavior Tree
        ///</summary>
        public Blackboard blackboard => behaviorTree.blackboard;

        private void Awake()
        {
            // When the game starts, execute the assigned behavior treeAsset if
            // there is one.
            if (behaviorTree != null)
            {
                RunBehaviorTree(behaviorTree);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Initialize treeAsset nodes starting from root.
            behaviorTree.rootNode.OnInit_internal();
        }

        // Update is called once per frame
        void Update()
        {
            // Is the tree asset updating at a custom interval?
            if(canTick)
            {
                ExecuteBehaviorTree();
            }
        }
        
        ///<summary>
        /// Start running a copy of a behavior tree asset
        ///</summary>
        ///<param name="treeAsset"> The behavior tree asset you want to run.</param>
        public void RunBehaviorTree(BehaviorTree treeAsset)
        {
            // Are we trying to run a valid behavior treeAsset?
            Debug.Assert(treeAsset != null, "Trying to run an invalid behavior treeAsset. Behavior Tree should not be 'null'");
            
            // Create a copy of the asset.
            behaviorTree = treeAsset.Clone();
            
            // Should we use a custom update interval?
            if (!canTick)
            {
                // When there is a new behavior treeAsset cancel all the updates to the previous treeAsset
                // and clone the new treeAsset
                CancelInvoke(nameof(ExecuteBehaviorTree));
                InvokeRepeating(nameof(ExecuteBehaviorTree), 0f, updateInterval);
            }
        }
        
        ///<summary>
        /// Execute behavior tree asset root node
        ///</summary>
        private void ExecuteBehaviorTree()
        {
            behaviorTree.treeState = behaviorTree.rootNode.ExecuteNode();
        }
    }

}
