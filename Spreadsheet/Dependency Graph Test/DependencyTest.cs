// written by Yuntong Lu
// Febuary 1, 2018

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace DependencyTests
{

    [TestClass]
    public class Dependency_Graph_Test
    {
        /// <summary>
        /// test size
        /// </summary>
        [TestMethod()]
        public void TestSize()
        {
            DependencyGraph G = new DependencyGraph();

            // add some values to the graph
            for (int i = 0, k = 2001; i < 2000; i ++, k++)
            {
            G.AddDependency(i.ToString(), k.ToString());        
            }

            Assert.AreEqual(2000, G.Size);
        }

        [TestMethod()]
        public void TestHasdependent()
        {
            DependencyGraph G = new DependencyGraph();

            for (int i = 0, k = 2001; i < 2000; i ++, k++)
            {
                G.AddDependency(i.ToString(), k.ToString());        
            }

            // "50" must be in the dependent
            Assert.IsTrue(G.HasDependents("50"));
        }


        [TestMethod()]
        public void TestHasdependee()
        {
            DependencyGraph G = new DependencyGraph();
            // nothing added in it
            Assert.IsFalse(G.HasDependees("50"));
        }


        [TestMethod()]
        public void TestAddyAndGetDependee()
        {
            DependencyGraph G = new DependencyGraph();

            // add some dependencies into the graph
            for (int i = 0, k = 2001; i < 2000; i++, k++)
            {
                // add the normal values
                G.AddDependency(i.ToString(), k.ToString());
                // add the same values
                G.AddDependency("0", "2001");
                // add one more dependee to the set
                G.AddDependency("9", "2001");

                // check each values, make sure it matches
                foreach (string token in G.GetDependees(i.ToString()))
                {
                    for (int m = 2001; m < k; m++)
                    Assert.AreEqual(m.ToString(), token);
                }
            }

        }


        [TestMethod()]
        public void TestRemoveAndGetDependent()
        {
            DependencyGraph G = new DependencyGraph();

            // add some dependencies into the graph and remove some
            for (int i = 0, k = 2001; i < 2000; i++, k++)
            {
                // after add it, remove them
                G.AddDependency(i.ToString(), k.ToString());
                G.RemoveDependency(i.ToString(), k.ToString());

                // add again, then remove a value
                G.AddDependency(k.ToString(), i.ToString());
                G.RemoveDependency("2002", "1");
                // remove the value doesn't exist
                G.RemoveDependency("0", "2001");
            }

            // the value should exist
            foreach (string token in G.GetDependents("2001"))
                Assert.AreEqual("0", token);

            // the valiue shouldn't exist
            foreach (string token in G.GetDependents("2002"))
                Assert.IsFalse(token.Equals("1"));

        }



        [TestMethod()]
        public void TestReplaceDependentAndCount()
        {
            DependencyGraph G = new DependencyGraph();

            // add stress values
            for (int i = 0; i < 2000; i++)
            {
                for (int k = 2001; k < 4001; k++)
                    G.AddDependency(i.ToString(), k.ToString());
            }

            // replace it
            G.ReplaceDependents("0", new HashSet<string> {"a", "b", "c" });
            Assert.AreEqual(3998003, G.Size);

        }


        [TestMethod()]
        public void TestReplaceDependeeAndCount()
        {
            DependencyGraph G = new DependencyGraph();

            // add the stress values
            for (int i = 0; i < 2000; i++)
            {
                for (int k = 2001; k < 4001; k++)
                    G.AddDependency(i.ToString(), k.ToString());
            }

            G.ReplaceDependees("2001", new HashSet<string> { "a" });
            
            // check if the value is right
            foreach (string token in G.GetDependees("2001"))
            {
                Assert.AreEqual("a", token);
            }

        }

    }
    
}

