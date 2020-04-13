﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CSProblem
{
    public class CSProblem
    {
        public List<ICSConstraint> Constraints {get; set;}
        public List<CSVariable> Variables {get; set;}

        private const int ALL_SOLUTIONS = -1;

        public CSProblem(List<CSVariable> variables, List<ICSConstraint> constraints)
        {
            this.Constraints = constraints;
            this.Variables = variables;
        }

        public List<List<CSVariableAssignedStatus>> Solve(int numSolution) {
            var solutions = new List<List<CSVariableAssignedStatus>>();

            var res = Solve1(numSolution, solutions, new List<CSVariableAssignedStatus>(), 
                Variables.Select(v => new CSVariableUnassignedStatus(v, v.Domain)).ToList());

            return solutions;
        }

        public List<CSVariableAssignedStatus> Solve() {
            return Solve(1)[0];
        }

        public List<List<CSVariableAssignedStatus>> SolveAll() {
            return Solve(ALL_SOLUTIONS);
        }

        //true --> terminazione, false --> backtracking
        private bool Solve1(int numSolutions, 
                            List<List<CSVariableAssignedStatus>> solutions,
                            List<CSVariableAssignedStatus> assignedVariables,
                            List<CSVariableUnassignedStatus> unassignedVariables) {
            if (assignedVariables.Count == Variables.Count) {
                //Devo aggiungere la soluzione corrente a quelle da ritornare
                solutions.Add(assignedVariables);

                //Se il numero di soluzioni corrisponde a quello richiesto return true, else return false
                if (solutions.Count == numSolutions) return true;
                else return false;
            }

            //Scelgo la variabile (secondo l'euristica)
            var currVar = ChooseNextVariable(unassignedVariables);
            return Solve2(numSolutions, solutions, currVar, assignedVariables, unassignedVariables);
        }

        private bool Solve2(int numSolutions, 
                            List<List<CSVariableAssignedStatus>> solutions, 
                            CSVariableUnassignedStatus currVar, 
                            List<CSVariableAssignedStatus> assignedVariables,
                            List<CSVariableUnassignedStatus> unassignedVariables) {
            // Se c'è almeno una variabile con dominio vuoto bisogna fare backtracking
            if (unassignedVariables.Any(v => v.Domain.Count == 0)) return false;

            //Scelgo un valore per la variabile corrente secondo l'euristica
            var currValue = ChooseValueForVariable(currVar);

            //Salvataggio dello stato delle variabili
            //TODO: non drovrebbe essere necessario

            //Assegno valore alla variabile corrente
            (var newassignment, var newUnassigned) = AssignVariable(currVar.Variable, currValue, assignedVariables, unassignedVariables);

            //Faccio propagazione dell'algoritmo
            var propagatedUnassigned = Propagate(currVar.Variable, newassignment, newUnassigned);

            //Chiamata ricorsiva per passare alla variabile successiva
            if (Solve1(numSolutions,
                       solutions, 
                       newassignment,
                       propagatedUnassigned))
                return true;
            else {
                //Se c'è stato un fallimento, ripristino lo stato precedente delle variabili
                //TODO: non dovrebbe essere necessario

                //e rimuovo il valore corrente (per la variabile corrente)
                //Se è l'ultimo valore ritorno false
                if (currVar.Domain.Count == 1) return false;
                var currVarLimitedDomain = new CSVariableUnassignedStatus(currVar.Variable, 
                                                                          currVar.Domain.Where(d => d != currValue).ToList());

                if (currVarLimitedDomain.Domain.Count == 0) return false;

                //nelle unassigned varuables però devo modificare il dominio
                var unassignedWithNewDomain = unassignedVariables
                    .Select(v => (v.Variable == currVar.Variable) ? currVarLimitedDomain : v)
                    .ToList();
 
                //quindi passo al valore successivo (sempre per la variabile corrente)
                return Solve2(numSolutions, solutions, currVarLimitedDomain, assignedVariables, unassignedWithNewDomain);
            }
        }

        List<CSVariableUnassignedStatus> Propagate(CSVariable lastAssignment, 
                                                   List<CSVariableAssignedStatus> assignedVariables,
                                                   List<CSVariableUnassignedStatus> unassignedVariables) {
            //Qua devo implementare tutto l'algoritmo di propagazione

            //Devo recuperare la lista di Constraints
            //TODO: questo si potrebbe ottimizzare e momorizzare direttamente nelle variabili, quando si aggiunge un constraint al problema
            
            
            //Per ognuna delle altr variabili unassigned
            //  Prendo i constraints che sono sia sulla variabile corrente che su quella di iterazione
            //  Per ogni valore di dominio della variabile di iterazione
            //      Verifico che esso sia ancora compatibile, cioè che tutti i vincoli siano soddisfacibili
            //      Se questo non è vero cancello tale valore dal dominio della variabile corrente
            //TODO : IMPLEMEMTARE QUESTO

            for (int i = 0; i < unassignedVariables.Count; i++) {
                var currUnassigned = unassignedVariables[i];
                var involvedConstraints = Constraints
                                            .Where(c => c.GetInvolvedVariables().Contains(lastAssignment) &&
                                                        c.GetInvolvedVariables().Contains(currUnassigned.Variable))
                                            .ToList();
                
                //Prendo solo le variabili coinvolte in questi vincoli
                var involvedVariables = involvedConstraints.SelectMany(c => c.GetInvolvedVariables()).Distinct().ToList();
                var involvedUnassignedVariables = unassignedVariables.Where(uv => involvedVariables.Contains(uv.Variable)).ToList();

                var newDomain = new List<int>();

                foreach (var currValue in currUnassigned.Domain) {
                    //      Verifico che esso sia ancora compatibile, cioè che tutti i vincoli siano soddisfacibili
                    //      Se questo non è vero cancello tale valore dal dominio della variabile corrente

                    (var newAssignements, var newUnassigned) = AssignVariable(currUnassigned.Variable, currValue, assignedVariables, involvedUnassignedVariables);

                    //Lo aggiungo al nuovo domincio solo se è soddisfacibile
                    if (AreFulfillable(involvedConstraints, newAssignements, newUnassigned)) {
                        newDomain.Add(currValue);
                    }
                }

                //DEvo modificare il dominio della variabile corrente: 
                unassignedVariables[i] = new CSVariableUnassignedStatus(currUnassigned.Variable, newDomain);
            }

            return unassignedVariables;
        }

        private bool AreFulfillable(List<ICSConstraint> constraints, 
                                    List<CSVariableAssignedStatus> assignedVariables,
                                    List<CSVariableUnassignedStatus> unassignedVariables) {

            //Se non ci sono pi variabili da istanziare, verifico che tutti i vincoli siano soddisfatti dall'environment passato
            if (unassignedVariables.Count == 0) {
                return constraints.All(c => c.IsSatisfied(assignedVariables));
            }

            //Scelgo la variabile corrente
            var currVar = unassignedVariables[0];

            //Scorro il suo dominio
            foreach (var currVal in currVar.Domain) {
                //Assegno quel valore e tolgo dalle variabili non assegnate
                //Rimiovo la variabile da quelle non assegnate

                (var newAssignments, var newUnassigned) = AssignVariable(currVar.Variable, currVal, assignedVariables, unassignedVariables);
                var res = AreFulfillable(constraints, newAssignments, newUnassigned);

                //Se ho trovato una soluzione, la propago
                if (res) return res;
            }

            //SE sono arrivato qui significa che non c'è soluzione
            return false;
        }

        private (List<CSVariableAssignedStatus>, List<CSVariableUnassignedStatus>) AssignVariable(CSVariable variable,
            int value, List<CSVariableAssignedStatus> assignments, List<CSVariableUnassignedStatus> unassignments) {
                var newAssignments = assignments.AddNewAssignment(variable, value);
                var newUnassigned = unassignments.RemoveUnassigned(variable);
                return (newAssignments, newUnassigned);
            }

        CSVariableUnassignedStatus ChooseNextVariable(List<CSVariableUnassignedStatus> unassignedVariables) {
            return unassignedVariables.OrderBy(v => v.Domain.Count()).First();
        }

        public int ChooseValueForVariable(CSVariableUnassignedStatus unassignedVariable) {
            return unassignedVariable.Domain[0];
        }

        
    }

    public interface ICSConstraint {
        List<CSVariable> GetInvolvedVariables();
        bool IsSatisfied(List<CSVariableAssignedStatus> environment);
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

        public bool IsSatisfied(List<CSVariableAssignedStatus> environment)
        {
            return LambdaFunc(variables.Select(v => environment.GetValueFor(v)).ToList());
        }
    }

    public class AllDifferentConstraint : ICSConstraint
    {
        private List<CSVariable> variables;

        public AllDifferentConstraint(params CSVariable[] variables)
        {
            this.variables = variables.ToList();
        }

        public List<CSVariable> GetInvolvedVariables()
        {
            return variables;
        }

        public bool IsSatisfied(List<CSVariableAssignedStatus> environment)
        {
            return variables.Select(v => environment.GetValueFor(v))
                        .Distinct().Count() == variables.Count();
        }
    }


    public class SkyskraperConstraint : ICSConstraint
    {
        protected List<CSVariable> orderedVariables;
        protected int numIncrements;

        public SkyskraperConstraint(List<CSVariable> orderedVariables, 
                                    int numIncrements)
        {
            this.orderedVariables = orderedVariables;
            this.numIncrements = numIncrements;
        }

        public List<CSVariable> GetInvolvedVariables()
        {
            return orderedVariables;
        }

        public bool IsSatisfied(List<CSVariableAssignedStatus> environment)
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

    public static class HelperMethods {
        public static int GetValueFor(this List<CSVariableAssignedStatus> assignements, CSVariable variable) {
            return assignements.First(v => v.Variable == variable).Value;
        }

        public static List<CSVariableAssignedStatus> AddNewAssignment(this List<CSVariableAssignedStatus> assignments, CSVariable variable, int value) {
            return assignments.Concat(new List<CSVariableAssignedStatus>{new CSVariableAssignedStatus(variable, value)}).ToList();
        }

        public static List<CSVariableUnassignedStatus> RemoveUnassigned(this List<CSVariableUnassignedStatus> unassigned, CSVariable variable) {
            return unassigned.Where(v => v.Variable != variable).ToList();
        }
    }

    public class Skyscrapers
    {
        public static int[][] SolvePuzzle(int[] clues)
        {
            const int size = 6;

            var variables = new List<List<CSVariable>>();

            for (var r = 0; r < size; r++) {
                var rowVariables = new List<CSVariable>();
                for (var c = 0; c < size; c++) {
                    rowVariables.Add(new CSVariable($"{r}{c}", Enumerable.Range(1, size).ToList()));
                }
                variables.Add(rowVariables);
            }

            var constraints = new List<ICSConstraint>();

            for (var r = 0; r < size; r++) {
                constraints.Add(new AllDifferentConstraint(GetVariablesPerRow(r, variables).ToArray()));
            }

            for (var c = 0; c < size; c++) {
                constraints.Add(new AllDifferentConstraint(GetVariablesPerColumn(c, variables).ToArray()));
            }


            for (int iClue = 0; iClue < size*4; iClue++) {
                if (clues[iClue] == 0) continue;
                if (iClue < size) {
                    var constraintVariables = GetVariablesPerColumn(iClue, variables);
                    var constraint = new SkyskraperConstraint(constraintVariables, clues[iClue]);
                    constraints.Add(constraint);
                } else if (iClue < size * 2) {
                    var relativeIClue = iClue - size;
                    var constraintVariables = GetVariablesPerRow(relativeIClue, variables);
                    constraintVariables = constraintVariables.AsEnumerable().Reverse().ToList();
                    var constraint = new SkyskraperConstraint(constraintVariables, clues[iClue]);
                    constraints.Add(constraint);
                } else if (iClue < size * 3) {
                    var relativeIClue = size * 3 - iClue - 1;
                    var constraintVariables = GetVariablesPerColumn(relativeIClue, variables);
                    constraintVariables = constraintVariables.AsEnumerable().Reverse().ToList();
                    var constraint = new SkyskraperConstraint(constraintVariables, clues[iClue]);
                    constraints.Add(constraint);
                } else if (iClue < size * 4) {
                    var relativeIClue = size * 4 - iClue - 1;
                    var constraintVariables = GetVariablesPerRow(relativeIClue, variables);
                    var constraint = new SkyskraperConstraint(constraintVariables, clues[iClue]);
                    constraints.Add(constraint);
                }
            }

            var csp = new CSProblem(variables.SelectMany(v => v).ToList(), constraints);
            var solution = csp.Solve();

            var orderedSolution = variables.Select(row => row.Select(variable => solution.GetValueFor(variable)));
            return orderedSolution.Select(row => row.ToArray()).ToArray();
        }

        private static List<CSVariable> GetVariablesPerRow(int row, List<List<CSVariable>> allVariables) {
            return allVariables[row];
        }

        private static List<CSVariable> GetVariablesPerColumn(int column, List<List<CSVariable>> allVariables) {
            return allVariables.Select(rowVar => rowVar[column]).ToList();
        }
    }
}
