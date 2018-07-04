// <copyright file="AStar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

/*
From https://github.com/jbaldwin/astar.cs

Copyright (c) 2013 Josh Baldwin

Licensed under the MIT license.
See LICENSE file in the project root for full license information.
*/

namespace OpenTibia.Utilities
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    public class AStar
    {
        /// <summary>
        /// The open list.
        /// </summary>
        private readonly SortedList<int, INode> openList;

        /// <summary>
        /// The closed list.
        /// </summary>
        private readonly SortedList<int, INode> closedList;

        /// <summary>
        /// The current node.
        /// </summary>
        private INode current;

        /// <summary>
        /// The goal node.
        /// </summary>
        private INode goal;

        /// <summary>
        /// Gets the current amount of steps that the algorithm has performed.
        /// </summary>
        public int Steps { get; private set; }

        public int MaxSteps { get; }

        /// <summary>
        /// Gets the current state of the open list.
        /// </summary>
        public IEnumerable<INode> OpenList => this.openList.Values;

        /// <summary>
        /// Gets the current state of the closed list.
        /// </summary>
        public IEnumerable<INode> ClosedList => this.closedList.Values;

        /// <summary>
        /// Gets the current node that the AStar algorithm is at.
        /// </summary>
        public INode CurrentNode => this.current;

        /// <summary>
        /// Initializes a new instance of the <see cref="AStar"/> class.
        /// </summary>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        /// <param name="maxSearchSteps">Optional. The maximum number of Step operations to perform on the search.</param>
        public AStar(INode start, INode goal, int maxSearchSteps = 100)
        {
            var duplicateComparer = new DuplicateComparer();
            this.openList = new SortedList<int, INode>(duplicateComparer);
            this.closedList = new SortedList<int, INode>(duplicateComparer);
            this.Reset(start, goal);
            this.MaxSteps = maxSearchSteps;
        }

        /// <summary>
        /// Resets the AStar algorithm with the newly specified start node and goal node.
        /// </summary>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public void Reset(INode start, INode goal)
        {
            this.openList.Clear();
            this.closedList.Clear();
            this.current = start;
            this.goal = goal;
            this.openList.Add(this.current);
            this.current.IsInOpenList = true;
        }

        /// <summary>
        /// Steps the AStar algorithm forward until it either fails or finds the goal node.
        /// </summary>
        /// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
        public SearchState Run()
        {
            // Continue searching until either failure or the goal node has been found.
            while (true)
            {
                var s = this.Step();
                if (s != SearchState.Searching)
                {
                    return s;
                }
            }
        }

        /// <summary>
        /// Moves the AStar algorithm forward one step.
        /// </summary>
        /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
        public SearchState Step()
        {
            if (this.MaxSteps > 0 && this.Steps == this.MaxSteps)
            {
                return SearchState.Failed;
            }

            this.Steps++;

            while (true)
            {
                // There are no more nodes to search, return failure.
                if (this.openList.IsEmpty())
                {
                    return SearchState.Failed;
                }

                // Check the next best node in the graph by TotalCost.
                this.current = this.openList.Pop();

                // This node has already been searched, check the next one.
                if (this.current.IsInClosedList)
                {
                    continue;
                }

                // An unsearched node has been found, search it.
                break;
            }

            // Remove from the open list and place on the closed list
            // since this node is now being searched.
            this.current.IsInOpenList = false;
            this.closedList.Add(this.current);
            this.current.IsInClosedList = true;

            // Found the goal, stop searching.
            if (this.current.IsGoal(this.goal))
            {
                return SearchState.GoalFound;
            }

            // Node was not the goal so add all children nodes to the open list.
            // Each child needs to have its movement cost set and estimated cost.
            foreach (var child in this.current.Children)
            {
                // If the child has already been searched (closed list) just ignore.
                if (child.IsInClosedList)
                {
                    continue;
                }

                // If the child has already beem searched, lets see if we get a better MovementCost by setting this parent instead.
                if (child.IsInOpenList)
                {
                    var oldCost = child.MovementCost;
                    child.SetMovementCost(this.current);

                    if (oldCost != child.MovementCost)
                    {
                        // it's better with this parent.
                        child.Parent = this.current;
                    }

                    continue;
                }

                child.Parent = this.current;
                child.SetMovementCost(this.current);
                child.SetEstimatedCost(this.goal);
                this.openList.Add(child);
                child.IsInOpenList = true;
            }

            // This step did not find the goal so return status of still searching.
            return SearchState.Searching;
        }

        /// <summary>
        /// Gets the path of the last solution of the AStar algorithm.
        /// Will return a partial path if the algorithm has not finished yet.
        /// </summary>
        /// <returns>Returns null if the algorithm has never been run.</returns>
        public IEnumerable<INode> GetPath()
        {
            if (this.current != null)
            {
                var next = this.current;
                var path = new List<INode>();
                while (next != null)
                {
                    path.Add(next);
                    next = next.Parent;
                }

                path.Reverse();
                return path.ToArray();
            }

            return null;
        }
    }
}
