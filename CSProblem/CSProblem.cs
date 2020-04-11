using System;
using System.Collections.Generic;
using System.Linq;

namespace CSProblem
{
    public class CSProblem
    {
        public List<ICSConstraint> Constraints;
        public List<CSVariable> Variables;
    }

    public interface ICSConstraint {
        List<CSVariable> GetInvolvedVariables();
        bool IsSatisfied(Environment environment);
    }

    public class CSLambdaConstraint : ICSConstraint
    {
        protected List<CSVariable> variables;
        public Func<List<int>, bool> LambdaFunc { get; set; }
        public CSLambdaConstraint(List<CSVariable> variables, Func<List<int>, bool> func)
        {
            this.variables = variables;
            this.LambdaFunc = func;
        }

        public List<CSVariable> GetInvolvedVariables()
        {
            return variables;
        }

        public bool IsSatisfied(Environment environment)
        {
            return LambdaFunc(variables.Select(v => environment.GetValueFor(v)).ToList());
        }
    }


    public class PeculiarConstraint : ICSConstraint
    {
        protected List<CSVariable> orderedVariables;
        protected int numIncrements;

        public PeculiarConstraint(List<CSVariable> orderedVariables, 
                                  int numIncrements)
        {
            this.orderedVariables = orderedVariables;
            this.numIncrements = numIncrements;
        }

        public List<CSVariable> GetInvolvedVariables()
        {
            return orderedVariables;
        }

        public bool IsSatisfied(Environment environment)
        {
            var orderedValues = orderedVariables.
                Select(v => environment.GetValueFor(v)).ToList();
            var higher = 0;
            var count = 0;
            foreach (var curValue in orderedValues) {
                if (curValue > higher) {
                    higher = curValue;
                    count++;
                }
            }
            return count == numIncrements;
        }
    }

    public class CSVariable {
        public string Name { get; private set; }
        public List<int> Domain { get; private set; }
        public CSVariable(string name, List<int> domain)
        {
            Name = name;
            Domain = domain;
        }
    }


    public class CSVariableUnassignedStatus {
        public CSVariable Variable { get; private set; }
        public List<int> Domain {get; private set; }

        public CSVariableUnassignedStatus(CSVariable variable,
                                          List<int> domain)
        {
            Variable = variable;
            Domain = domain;
        }

        public CSVariableUnassignedStatus RemoveFromDomain(int value) {
            if (Domain.Contains(value)) {
                return new CSVariableUnassignedStatus(
                    Variable, 
                    Domain.Where(v => v != value).ToList());
            }
            throw new Exception("Value not in domain");
        }

    }

    public class Environment {

        List<CSVariableAssignedStatus> variableValues;

        public int GetValueFor(CSVariable variable) {
            return variableValues.First(v => v.Variable == variable).Value;
        }
    }

    public class CSVariableAssignedStatus {
        public CSVariable Variable {get; private set;}
        public int Value {get; private set;}

        public CSVariableAssignedStatus(CSVariable variable,
                                        int value)
        {
            this.Variable = variable;
            this.Value = value;
        }
    }

/*
    public class CSVariableAssignment {
        public CSVariable Variable;
        public int Value;
    }
    */
}
