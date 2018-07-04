// <copyright file="BaseItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Parsing.Grammar;
    using OpenTibia.Server.Scripting;

    using Sprache;

    internal abstract class BaseItemEvent : IItemEvent
    {
        public const string IsTypeFunctionName = "IsType";

        private bool isSetup;

        public abstract ItemEventType Type { get; }

        public IThing Obj1 { get; protected set; }

        public IThing Obj2 { get; protected set; }

        public IPlayer User { get; protected set; }

        public IEnumerable<IItemEventFunction> Actions { get; }

        public IEnumerable<IItemEventFunction> Conditions { get; protected set; }

        public bool CanBeExecuted
        {
            get
            {
                return this.isSetup && this.Conditions.All(condition => Functions.InvokeCondition(this.Obj1, this.Obj2, this.User, condition.FunctionName, condition.Parameters));
            }
        }

        public BaseItemEvent(IList<string> conditionSet, IList<string> actionSet)
        {
            this.Conditions = this.ParseFunctions(conditionSet);
            this.Actions = this.ParseFunctions(actionSet);

            this.isSetup = false;
        }

        private IEnumerable<IItemEventFunction> ParseFunctions(IList<string> stringSet)
        {
            var functionList = new List<IItemEventFunction>();

            const string specialCaseNoOperationAction = "NOP";

            foreach (var str in stringSet)
            {
                if (specialCaseNoOperationAction.Equals(str))
                {
                    continue;
                }

                var resultFunction = CipGrammar.Function.TryParse(str);

                if (resultFunction.WasSuccessful)
                {
                    functionList.Add(new ItemEventFunction(resultFunction.Value.Name, resultFunction.Value.Parameters));

                    continue;
                }

                var resultComparison = CipGrammar.Comparison.TryParse(str);

                if (!resultComparison.WasSuccessful)
                {
                    throw new ParseException($"Failed to parse string {str} into a function or function comparison.");
                }

                functionList.Add(new ItemEventFunctionComparison(resultComparison.Value.Name, resultComparison.Value.Type, resultComparison.Value.CompareToIdentifier, resultComparison.Value.Parameters));
            }

            return functionList;
        }

        public bool Setup(IThing obj1, IThing obj2 = null, IPlayer user = null)
        {
            this.Obj1 = obj1;
            this.Obj2 = obj2;
            this.User = user;

            this.isSetup = true;

            return true; // TODO: make this abstract/virtual and let subclasses override.
        }

        public void Execute()
        {
            if (!this.isSetup)
            {
                throw new InvalidOperationException("Cannot execute event without first doing Setup.");
            }

            foreach (var action in this.Actions)
            {
                var obj1Result = this.Obj1;
                var obj2Result = this.Obj2;
                var userResult = this.User;

                Functions.InvokeAction(ref obj1Result, ref obj2Result, ref userResult, action.FunctionName, action.Parameters);

                this.Obj1 = obj1Result;
                this.Obj2 = obj2Result;
                this.User = userResult;
            }
        }
    }
}