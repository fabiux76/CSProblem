using System.Collections.Generic;
using System.Linq;
using CSProblem;
using NUnit.Framework;


namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleTestOneSolution()
        {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,2).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,2).ToList());
            var constraint = new CSLambdaConstraint(new List<CSVariable> {v1, v2}, listValues => {
                return listValues[0] != listValues[1];
            });
            var csp = new CSProblem.CSProblem(new List<CSVariable> {v1, v2}, new List<ICSConstraint> {constraint});

            var solutions = csp.Solve();
            Assert.IsTrue(solutions.First(v => v.Variable.Name == "A").Value == 1);
            Assert.IsTrue(solutions.First(v => v.Variable.Name == "B").Value == 2);
        }

        [Test]
        public void SimpleTestTwoSolutions()
        {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,2).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,2).ToList());
            var constraint = new CSLambdaConstraint(new List<CSVariable> {v1, v2}, listValues => {
                return listValues[0] != listValues[1];
            });
            var csp = new CSProblem.CSProblem(new List<CSVariable> {v1, v2}, new List<ICSConstraint> {constraint});

            var solutions = csp.Solve(2);
            Assert.IsTrue(solutions[0].First(v => v.Variable.Name == "A").Value == 1);
            Assert.IsTrue(solutions[0].First(v => v.Variable.Name == "B").Value == 2);
            Assert.IsTrue(solutions[1].First(v => v.Variable.Name == "A").Value == 2);
            Assert.IsTrue(solutions[1].First(v => v.Variable.Name == "B").Value == 1);

        }

        [Test]
        public void SimpleTestThreeVariables()
        {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,3).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,3).ToList());
            CSVariable v3 = new CSVariable("C", Enumerable.Range(1,3).ToList());
            var constraint1 = new CSLambdaConstraint(new List<CSVariable> {v1, v2}, listValues => {
                return listValues[0] < listValues[1];
            });
            var constraint2 = new CSLambdaConstraint(new List<CSVariable> {v2, v3}, listValues => {
                return listValues[0] < listValues[1];
            });
            var csp = new CSProblem.CSProblem(new List<CSVariable> {v1, v2, v3}, new List<ICSConstraint> {constraint1, constraint2});

            var solutions = csp.Solve();
            Assert.IsTrue(solutions.First(v => v.Variable.Name == "A").Value == 1);
            Assert.IsTrue(solutions.First(v => v.Variable.Name == "B").Value == 2);
            Assert.IsTrue(solutions.First(v => v.Variable.Name == "C").Value == 3);
        }

        [Test]
        public void AllDifferent() {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,4).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,4).ToList());
            CSVariable v3 = new CSVariable("C", Enumerable.Range(1,4).ToList());
            CSVariable v4 = new CSVariable("D", Enumerable.Range(1,4).ToList()); 

            var constraint = new AllDifferentConstraint(v1, v2, v3, v4);

            var csp = new CSProblem.CSProblem(new List<CSVariable>{v1, v2, v3, v4}, new List<ICSConstraint>{constraint});

            var solution = csp.Solve();
            Assert.IsTrue(solution.First(v => v.Variable.Name == "A").Value == 1);
            Assert.IsTrue(solution.First(v => v.Variable.Name == "B").Value == 2);
            Assert.IsTrue(solution.First(v => v.Variable.Name == "C").Value == 3);
            Assert.IsTrue(solution.First(v => v.Variable.Name == "D").Value == 4);
        }

        [Test]
        public void AllDifferentFactorialSolutions() {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,4).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,4).ToList());
            CSVariable v3 = new CSVariable("C", Enumerable.Range(1,4).ToList());
            CSVariable v4 = new CSVariable("D", Enumerable.Range(1,4).ToList()); 

            var constraint = new AllDifferentConstraint(v1, v2, v3, v4);

            var csp = new CSProblem.CSProblem(new List<CSVariable>{v1, v2, v3, v4}, new List<ICSConstraint>{constraint});

            var solution = csp.Solve(24);
            Assert.IsTrue(solution.Count == 24);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "A").Value == 1);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "B").Value == 2);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "C").Value == 3);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "D").Value == 4);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "A").Value == 4);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "B").Value == 3);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "C").Value == 2);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "D").Value == 1);
        }

        [Test]
        public void AllDifferentAllSolutions() {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,4).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,4).ToList());
            CSVariable v3 = new CSVariable("C", Enumerable.Range(1,4).ToList());
            CSVariable v4 = new CSVariable("D", Enumerable.Range(1,4).ToList()); 

            var constraint = new AllDifferentConstraint(v1, v2, v3, v4);

            var csp = new CSProblem.CSProblem(new List<CSVariable>{v1, v2, v3, v4}, new List<ICSConstraint>{constraint});

            var solution = csp.SolveAll();
            Assert.IsTrue(solution.Count == 24);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "A").Value == 1);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "B").Value == 2);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "C").Value == 3);
            Assert.IsTrue(solution[0].First(v => v.Variable.Name == "D").Value == 4);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "A").Value == 4);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "B").Value == 3);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "C").Value == 2);
            Assert.IsTrue(solution[23].First(v => v.Variable.Name == "D").Value == 1);
        }

        [Test]
        public void SolveSkykrapers2() {
            const int size = 2;
            var clues = new[]{ 2, 1, 1, 2, 2, 1, 1, 2};

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

            var csp = new CSProblem.CSProblem(variables.SelectMany(v => v).ToList(), constraints);
            var solution = csp.Solve();

        }

        [Test]
        public void SolvePuzzle1()
        {
            var clues = new[]{ 3, 2, 2, 3, 2, 1,
                            1, 2, 3, 3, 2, 2,
                            5, 1, 2, 2, 4, 3,
                            3, 2, 1, 2, 2, 4};

            var expected = new[]{new []{ 2, 1, 4, 3, 5, 6}, 
                                new []{ 1, 6, 3, 2, 4, 5}, 
                                new []{ 4, 3, 6, 5, 1, 2}, 
                                new []{ 6, 5, 2, 1, 3, 4}, 
                                new []{ 5, 4, 1, 6, 2, 3}, 
                                new []{ 3, 2, 5, 4, 6, 1 }};

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }
        
        [Test]
        public void SolvePuzzle2()
        {
            var clues = new []{ 0, 0, 0, 2, 2, 0,
                                0, 0, 0, 6, 3, 0,
                                0, 4, 0, 0, 0, 0,
                                4, 4, 0, 3, 0, 0};

            var expected = new[]{new []{ 5, 6, 1, 4, 3, 2 }, 
                                new []{ 4, 1, 3, 2, 6, 5 }, 
                                new []{ 2, 3, 6, 1, 5, 4 }, 
                                new []{ 6, 5, 4, 3, 2, 1 }, 
                                new []{ 1, 2, 5, 6, 4, 3 }, 
                                new []{ 3, 4, 2, 5, 1, 6 }};

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }
        
        [Test]
        public void SolvePuzzle3()
        {
            var clues = new[] { 0, 3, 0, 5, 3, 4, 
                                0, 0, 0, 0, 0, 1,
                                0, 3, 0, 3, 2, 3,
                                3, 2, 0, 3, 1, 0};
        
            var expected = new[]{new []{ 5, 2, 6, 1, 4, 3 }, 
                                new []{ 6, 4, 3, 2, 5, 1 }, 
                                new []{ 3, 1, 5, 4, 6, 2 }, 
                                new []{ 2, 6, 1, 5, 3, 4 }, 
                                new []{ 4, 3, 2, 6, 1, 5 }, 
                                new []{ 1, 5, 4, 3, 2, 6 }};
        
            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }

        private List<CSVariable> GetVariablesPerRow(int row, List<List<CSVariable>> allVariables) {
            return allVariables[row];
        }

        private List<CSVariable> GetVariablesPerColumn(int column, List<List<CSVariable>> allVariables) {
            return allVariables.Select(rowVar => rowVar[column]).ToList();
        }
    }
}