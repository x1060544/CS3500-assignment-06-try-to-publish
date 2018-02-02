// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.
// the rest of them written by Yuntong Lu
// Febuary 1, 2018

using System.Collections.Generic;



namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        // use two dictionary to hold the dependencies
        // each dependency might contains more than one dependent or dependee
        // such, I use Hashset
        private Dictionary<string, HashSet<string>> dependent;
        private Dictionary<string, HashSet<string>> dependee;
        // trecking the size of the graph
        private int count;
        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            dependent = new Dictionary<string, HashSet<string>>();
            dependee = new Dictionary<string, HashSet<string>>();
            count = 0;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return count; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            // if dependent dictionary contains s, retun ture
            if (dependent.ContainsKey(s) && dependent[s].Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {
            // if dependee dictionary contains s, retun ture
            if (dependee.ContainsKey(s) && dependee[s].Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            // if no s are stored in the dependent, return empty
            if (!dependent.ContainsKey(s))
                return new HashSet<string>();

            return dependent[s];
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            // if no s are stored in the dependee, return empty
            if (!dependee.ContainsKey(s))
                return new HashSet<string>();

            return dependee[s];
 
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            // check if s is already stored in the dictionary
            if (dependent.ContainsKey(s))
            {
                // check if t is already stored
                if (dependent[s].Contains(t))
                {
                    return;
                }

                // add t if t isn't stored in the dictionary
                else
                {
                    dependent[s].Add(t);
                    count++;
                    
                    // store the dependee ditionary for value t
                    // then check every statement as I checked for dependent dictionary
                    if (dependee.ContainsKey(t))
                    {
                        if (dependee[t].Contains(s))
                            {
                            return;
                            }
                        else
                            {
                            dependee[t].Add(s);
                            }
                    }

                    // if t isn't stored in the dependee dictionary
                    else
                    {
                        // create a hashset for it
                         HashSet<string> mySet = new HashSet<string>();
                         dependee.Add(t, mySet);
                         dependee[t].Add(s);
                    }
                }
            }

            // if s isn't stored the dependent dictionary 
            else
            {
                // create a new hashset for it
                HashSet<string> mySet = new HashSet<string>();
                dependent.Add(s, mySet);
                dependent[s].Add(t);
                count++;

                // save dependee, same step with the previous
                if (dependee.ContainsKey(t))
                {
                    if (dependee[t].Contains(s))
                    {
                    return;
                    }
                    else
                    {
                    dependee[t].Add(s);
                    }
                }

                else
                {
                    HashSet<string> mySetee = new HashSet<string>();
                    dependee.Add(t, mySetee);
                    dependee[t].Add(s);
                }
            }

        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            // check if s is stored in the dictionary,
            // if not, do nothing
            if (dependent.ContainsKey(s) && dependee.ContainsKey(t))
            {
                // check if t is stored in the dictionary
                if (dependent[s].Contains(t) && dependee[t].Contains(s))
                {
                    // remove the value and update the count
                    dependent[s].Remove(t);
                    dependee[t].Remove(s);

                    count--;
                }   
                else
                    return;
                    
            }
            else
            {
                return;
            }
              
                        
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // check if the s is the in the dictionary
            // if not, just add them
            if (dependent.ContainsKey(s))
            {
                // go through each element in the dependent dictionary
                // match thme within the dependee dictrionary,
                // then remove the matched value from dependee dictionary first
                foreach (string token in dependent[s])
                {
                    dependee[token].Remove(s);
                    count--;
                }

                // remove the value from the dependent dictionary
                dependent[s].Clear();

                // add the new values to the dictionary
                foreach (string token in newDependents)
                {
                    AddDependency(s, token);
                }
            }

            else
                foreach (string token in newDependents)
                {
                    AddDependency(s, token);
                }

        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            // all the following statemnet share the same reason with the 
            // previous method
            if (dependee.ContainsKey(t))
            {

                foreach (string token in dependee[t])
                {
                    dependent[token].Remove(t);
                    count--;
                }

                dependee[t].Clear();

                foreach (string token in newDependees)
                {
                    AddDependency(token, t);
                }
            }

            else
                foreach (string token in newDependees)
                {
                    AddDependency(token, t);
                }
        }
    }
}
