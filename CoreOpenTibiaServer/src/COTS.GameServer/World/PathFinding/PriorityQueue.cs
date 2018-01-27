using System;
using System.Runtime.CompilerServices;

namespace COTS.GameServer.World.PathFinding {

    /// <summary>
    /// An implementation of a min-Priority Queue using a heap.
    /// This is just an adaption of the code from this project:
    /// https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp/wiki/Getting-Started for more information
    /// </summary>
    public sealed class PriorityQueue {
        private int _numNodes;
        private readonly AStartNode[] _nodes;

        public PriorityQueue(int maxNodes) {
#if DEBUG
            if (maxNodes <= 0) {
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
            }
#endif

            _numNodes = 0;
            _nodes = new AStartNode[maxNodes + 1];
        }

        public int Count => _numNodes;

        /// <summary>
        /// Returns the maximum number of items that can be enqueued at once in this queue.  Once you hit this number (ie. once Count == MaxSize),
        /// attempting to enqueue another item will cause undefined behavior.  O(1)
        /// </summary>
        public int MaxSize => _nodes.Length - 1;

        /// <summary>
        /// Returns (in O(1)!) whether the given node is in the queue.  O(1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(AStartNode node) {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.QueueIndex < 0 || node.QueueIndex >= _nodes.Length) {
                throw new InvalidOperationException("node.QueueIndex has been corrupted. Did you change it manually? Or add this node to another queue?");
            }
#endif

            return (_nodes[node.QueueIndex] == node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryEnqueue(AStartNode node) {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (_numNodes >= _nodes.Length - 1)
                throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
            if (Contains(node))
                throw new InvalidOperationException("Node is already enqueued: " + node);
#endif

            if (_numNodes >= MaxSize)
                return false;
            
            _numNodes++;
            _nodes[_numNodes] = node;
            node.QueueIndex = _numNodes;
            CascadeUp(node);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out AStartNode node) {
#if DEBUG
            if (_numNodes <= 0)
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");

            if (!IsValidQueue()) {
                throw new InvalidOperationException("Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same node to two different queues?)");
            }
#endif
            if (_numNodes == 0) {
                node = null;
                return false;
            }

            var returnMe = _nodes[1];
            //If the node is already the last node, we can remove it immediately
            if (_numNodes == 1) {
                _nodes[1] = null;
                _numNodes = 0;
                node = returnMe;
                return true;
            }

            //Swap the node with the last node
            var formerLastNode = _nodes[_numNodes];
            _nodes[1] = formerLastNode;
            formerLastNode.QueueIndex = 1;
            _nodes[_numNodes] = null;
            _numNodes--;

            //Now bubble formerLastNode (which is no longer the last node) down
            CascadeDown(formerLastNode);
            node = returnMe;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeUp(AStartNode node) {
            //aka Heapify-up
            int parent;
            if (node.QueueIndex > 1) {
                parent = node.QueueIndex >> 1;
                var parentNode = _nodes[parent];
                if (HasHigherOrEqualPriority(parentNode, node))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                _nodes[node.QueueIndex] = parentNode;
                parentNode.QueueIndex = node.QueueIndex;

                node.QueueIndex = parent;
            } else {
                return;
            }
            while (parent > 1) {
                parent >>= 1;
                var parentNode = _nodes[parent];
                if (HasHigherOrEqualPriority(parentNode, node))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                _nodes[node.QueueIndex] = parentNode;
                parentNode.QueueIndex = node.QueueIndex;

                node.QueueIndex = parent;
            }
            _nodes[node.QueueIndex] = node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(AStartNode node) {
            //aka Heapify-down
            int finalQueueIndex = node.QueueIndex;
            int childLeftIndex = 2 * finalQueueIndex;

            // If leaf node, we're done
            if (childLeftIndex > _numNodes) {
                return;
            }

            // Check if the left-child is higher-priority than the current node
            int childRightIndex = childLeftIndex + 1;
            var childLeft = _nodes[childLeftIndex];
            if (HasHigherPriority(childLeft, node)) {
                // Check if there is a right child. If not, swap and finish.
                if (childRightIndex > _numNodes) {
                    node.QueueIndex = childLeftIndex;
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    _nodes[childLeftIndex] = node;
                    return;
                }
                // Check if the left-child is higher-priority than the right-child
                var childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childLeft, childRight)) {
                    // left is highest, move it up and continue
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                } else {
                    // right is even higher, move it up and continue
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            // Not swapping with left-child, does right-child exist?
            else if (childRightIndex > _numNodes) {
                return;
            } else {
                // Check if the right-child is higher-priority than the current node
                var childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childRight, node)) {
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                // Neither child is higher-priority than current, so finish and stop.
                else {
                    return;
                }
            }

            while (true) {
                childLeftIndex = 2 * finalQueueIndex;

                // If leaf node, we're done
                if (childLeftIndex > _numNodes) {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }

                // Check if the left-child is higher-priority than the current node
                childRightIndex = childLeftIndex + 1;
                childLeft = _nodes[childLeftIndex];
                if (HasHigherPriority(childLeft, node)) {
                    // Check if there is a right child. If not, swap and finish.
                    if (childRightIndex > _numNodes) {
                        node.QueueIndex = childLeftIndex;
                        childLeft.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        _nodes[childLeftIndex] = node;
                        break;
                    }
                    // Check if the left-child is higher-priority than the right-child
                    var childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childLeft, childRight)) {
                        // left is highest, move it up and continue
                        childLeft.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    } else {
                        // right is even higher, move it up and continue
                        childRight.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (childRightIndex > _numNodes) {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                } else {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childRight, node)) {
                        childRight.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else {
                        node.QueueIndex = finalQueueIndex;
                        _nodes[finalQueueIndex] = node;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherPriority(AStartNode higher, AStartNode lower) {
            return (higher.Priority < lower.Priority);
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherOrEqualPriority(node, node) (ie. both arguments the same node) will return true
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasHigherOrEqualPriority(AStartNode higher, AStartNode lower) {
            return (higher.Priority <= lower.Priority);
        }

        /// <summary>
        /// This method must be called on a node every time its priority changes while it is in the queue.
        /// <b>Forgetting to call this method will result in a corrupted queue!</b>
        /// Calling this method on a node not in the queue results in undefined behavior
        /// O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateNodeGCost(AStartNode node, int newGCost) {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (!Contains(node))
                throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + node);
#endif

            node.G = newGCost;
            OnNodeUpdated(node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNodeUpdated(AStartNode node) {
            // Bubble the updated node up or down as appropriate
            int parentIndex = node.QueueIndex >> 1;

            if (parentIndex > 0 && HasHigherPriority(node, _nodes[parentIndex])) {
                CascadeUp(node);
            } else {
                //Note that CascadeDown will be called if parentNode == node (that is, node is the root)
                CascadeDown(node);
            }
        }

        /// <summary>
        /// Removes a node from the queue.  The node does not need to be the head of the queue.
        /// If the node is not in the queue, the result is undefined.  If unsure, check Contains() first
        /// O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(AStartNode node) {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (!Contains(node))
                throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + node);
#endif

            //If the node is already the last node, we can remove it immediately
            if (node.QueueIndex == _numNodes) {
                _nodes[_numNodes] = null;
                _numNodes--;
                return;
            }

            //Swap the node with the last node
            var formerLastNode = _nodes[_numNodes];
            _nodes[node.QueueIndex] = formerLastNode;
            formerLastNode.QueueIndex = node.QueueIndex;
            _nodes[_numNodes] = null;
            _numNodes--;

            //Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
            OnNodeUpdated(formerLastNode);
        }

        /// <summary>
        /// <b>Should not be called in production code.</b>
        /// Checks to make sure the queue is still in a valid state.  Used for testing/debugging the queue.
        /// </summary>
        public bool IsValidQueue() {
            for (int i = 1; i < _nodes.Length; i++) {
                if (_nodes[i] != null) {
                    int childLeftIndex = 2 * i;
                    if (childLeftIndex < _nodes.Length && _nodes[childLeftIndex] != null && HasHigherPriority(_nodes[childLeftIndex], _nodes[i]))
                        return false;

                    int childRightIndex = childLeftIndex + 1;
                    if (childRightIndex < _nodes.Length && _nodes[childRightIndex] != null && HasHigherPriority(_nodes[childRightIndex], _nodes[i]))
                        return false;
                }
            }
            return true;
        }

        /*
        /// <summary>
        /// Enqueue a node to the priority queue. Lower values are placed in front. Ties are broken arbitrarily.
        /// If the queue is full, the result is undefined.
        /// If the node is already enqueued, the result is undefined.
        /// O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(AStartNode node, int priority) {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (_numNodes >= _nodes.Length - 1)
                throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
            if (Contains(node))
                throw new InvalidOperationException("Node is already enqueued: " + node);
#endif

            node.Priority = priority;
            _numNodes++;
            _nodes[_numNodes] = node;
            node.QueueIndex = _numNodes;
            CascadeUp(node);
        }

        /// <summary>
        /// Removes every node from the queue.
        /// O(n) (So, don't do this often!)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            Array.Clear(_nodes, 1, _numNodes);
            _numNodes = 0;
        }

        /// <summary>
        /// Enqueue a node to the priority queue.
        /// If the queue is full, the result is undefined.
        /// If the node is already enqueued, the result is undefined.
        /// O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(AStartNode node) {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (_numNodes >= _nodes.Length - 1)
                throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
            if (Contains(node))
                throw new InvalidOperationException("Node is already enqueued: " + node);
#endif

            _numNodes++;
            _nodes[_numNodes] = node;
            node.QueueIndex = _numNodes;
            CascadeUp(node);
        }

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// If queue is empty, result is undefined
        /// O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AStartNode Dequeue() {
#if DEBUG
            if (_numNodes <= 0)
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");

            if (!IsValidQueue()) {
                throw new InvalidOperationException("Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same node to two different queues?)");
            }
#endif

            var returnMe = _nodes[1];
            //If the node is already the last node, we can remove it immediately
            if (_numNodes == 1) {
                _nodes[1] = null;
                _numNodes = 0;
                return returnMe;
            }

            //Swap the node with the last node
            var formerLastNode = _nodes[_numNodes];
            _nodes[1] = formerLastNode;
            formerLastNode.QueueIndex = 1;
            _nodes[_numNodes] = null;
            _numNodes--;

            //Now bubble formerLastNode (which is no longer the last node) down
            CascadeDown(formerLastNode);
            return returnMe;
        }

        /// <summary>
        /// Resize the queue so it can accept more nodes.  All currently enqueued nodes are remain.
        /// Attempting to decrease the queue size to a size too small to hold the existing nodes results in undefined behavior
        /// O(n)
        /// </summary>
        public void Resize(int maxNodes) {
#if DEBUG
            if (maxNodes <= 0) {
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            }

            if (maxNodes < _numNodes) {
                throw new InvalidOperationException("Called Resize(" + maxNodes + "), but current queue contains " + _numNodes + " nodes");
            }
#endif

            AStartNode[] newArray = new AStartNode[maxNodes + 1];
            int highestIndexToCopy = Math.Min(maxNodes, _numNodes);
            Array.Copy(_nodes, newArray, highestIndexToCopy + 1);
            _nodes = newArray;
        }

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// If the queue is empty, behavior is undefined.
        /// O(1)
        /// </summary>
        public AStartNode First {
            get {
#if DEBUG
                if (_numNodes <= 0) {
                    throw new InvalidOperationException("Cannot call .First on an empty queue");
                }
#endif

                return _nodes[1];
            }
        }
       */
    }
}