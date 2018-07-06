// <copyright file="BaseItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Parsing.Grammar;
using COMMO.Server.Scripting;

using Sprache;

namespace COMMO.Server.Events
{
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
                return isSetup && Conditions.All(condition => Functions.InvokeCondition(Obj1, Obj2, User, condition.FunctionName, condition.Parameters));
            }
        }

        public BaseItemEvent(IList<string> conditionSet, IList<string> actionSet)
        {
            Conditions = ParseFunctions(conditionSet);
            Actions = ParseFunctions(actionSet);

            isSetup = false;
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
            Obj1 = obj1;
            Obj2 = obj2;
            User = user;

            isSetup = true;

            return true; // TODO: make this abstract/virtual and let subclasses override.
        }

        public void Execute()
        {
            if (!isSetup)
            {
                throw new InvalidOperationException("Cannot execute event without first doing Setup.");
            }

            foreach (var action in Actions)
            {
                var obj1Result = Obj1;
                var obj2Result = Obj2;
                var userResult = User;

                Functions.InvokeAction(ref obj1Result, ref obj2Result, ref userResult, action.FunctionName, action.Parameters);

                Obj1 = obj1Result;
                Obj2 = obj2Result;
                User = userResult;
            }
        }
    }
}