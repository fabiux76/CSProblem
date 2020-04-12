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
    }
}