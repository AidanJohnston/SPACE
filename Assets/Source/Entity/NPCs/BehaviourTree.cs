using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

namespace AI {
    public class BehaviourTree : MonoBehaviour {
        protected BehaviourTrees.Root root;
        protected BehaviourTrees.Node rootNode;
        
        public virtual void Start () { // Run at the end of Start in inheriting classes
            root = new BehaviourTrees.Root(rootNode);
        }

        public virtual void Update () {
            root.Run();
        }
    }
    namespace BehaviourTrees { // holds up to six states
        public enum State {
            FAILED,
            COMPLETED,
            RUNNING
        };

        [System.Serializable]
        public class Node {
            
            public State state;

            protected readonly List<Node> children;

            public Node() {
                children = new List<Node>();
            }

            public virtual State Run() {
                return state;
            }
            public virtual void ResetNode() {

            }

            public void CompleteNode() {
                foreach (Node node in children) {
                    CompleteNode();
                }
                state = State.COMPLETED;
            }
            public void FailNode() {
                foreach (Node node in children) {
                    FailNode();
                }
                state = State.FAILED;
            }

            public void AddChild(Node node) { // Add a rightmost branch to the node
                children.Add(node);
            }
        }

        public class Root {
            Node rootChild;

            public Root(Node rootChild) {
                this.rootChild = rootChild;
            }

            public void Run() {
                rootChild.Run();
            }
        }

        // Selector node: Run children from left to right until a node is completed. If a child fails, move onto next child. If all children fail, node fails. If a child completes, the node completes
        [System.Serializable]
        public class Selector : Node {
            public Selector() : base() {

            }
            public override State Run() {
                bool runNextState = false;
                // Check each child node from left to right
                foreach (Node node in children) {
                    if (runNextState) {
                        node.state = State.RUNNING;
                        runNextState = false;
                        return State.RUNNING; // end function here as an instance of run already occured and we don't want to run the next node until the next frame to prevent overlap
                    }
                    switch (node.state) {
                        // Running state: run the node and see if it failed. If failed, move onto next node, if still running, do nothing, if completed, complete node
                        case State.RUNNING:
                            State progress = node.Run();
                            // Nested switch (gross) determines the outcome of running this node and completes, fails, or lets it keep running
                            switch (progress) {
                                case State.FAILED:
                                    runNextState = true;
                                    continue;
                                case State.COMPLETED:
                                    CompleteNode();
                                    return State.COMPLETED;
                                default:
                                    return State.RUNNING;
                            }
                        // Failed state: continue iterating through loop to see if everything else failed
                        case State.FAILED:
                            continue;
                    }
                }
                // if all child nodes fail, then selector node fails
                FailNode(); // if there were nodes that were fine, the funtion would have ended before this
                return State.FAILED;
            }
        }

        // Sequence node: run child nodes from left to right until a child fails. If all children complete, node completes. If a child fails, node fails
        [System.Serializable]
        public class Sequence : Node {
            public Sequence() : base() {

            }
            public override State Run(){
                bool runNextState = false;
                foreach (Node node in children) {
                    if (runNextState) {
                        node.state = State.RUNNING;
                        runNextState = false;
                        return State.RUNNING; // end function here as an instance of run already occured and we don't want to run the next node until the next frame to prevent overlap
                    }

                    switch (node.state) {
                        case State.RUNNING:
                            State progress = node.Run();
                            switch (progress) {
                                case State.FAILED:
                                    FailNode();
                                    return State.FAILED;
                                case State.COMPLETED:
                                    runNextState = true;
                                    continue;
                                default:
                                    return State.RUNNING;
                            }
                        case State.COMPLETED:
                            continue;
                    }
                }
                CompleteNode();
                return State.COMPLETED;
            }
        }
        public class Parallel : Node {
            public override State Run() {
                foreach (Node node in children) {
                    State progress = node.Run();
                    switch (progress) {
                        case State.RUNNING:
                            continue;
                        default:
                            return EvaluateProgress(progress);
                    }
                }
                return State.RUNNING;
            }
            State EvaluateProgress(State progress) {
                switch (progress) {
                    case State.FAILED:
                        FailNode();
                        return State.FAILED;
                    case State.COMPLETED:
                        CompleteNode();
                        return State.COMPLETED;
                    default:
                        return State.RUNNING;
                }
            }
        }

        public class MoveTo : Node {
            NavMeshAgent agent;

            public override State Run() {
                if (Vector3.Distance(agent.gameObject.transform.position, agent.destination) > 0.1f && agent.hasPath) {
                    return State.RUNNING;
                } else if (agent.hasPath) {
                    CompleteNode();
                    return State.COMPLETED;
                } else {
                    FailNode();
                    return State.FAILED;
                }
            }
        }
    }
}
