using System;
using System.Collections.Generic;

namespace BasicSAT
{
    public class BasicSAT
    {
        private string filename;
        private Solver solver;

        public BasicSAT(string filename)
        {
            this.filename = filename;
            this.solver = new Solver();
        }

        public bool Solve()
        {
            this.ReadFile();
            return this.solver.Search();
        }

        public Dictionary<int, Assign> GetSolution()
        {
            return this.solver.GetSolution();
        }

        private void ReadFile()
        {
            using (System.IO.StreamReader sr = System.IO.File.OpenText(filename))
            {
                bool ok = true;
                string s = "";
                List<Lit> lits = new List<Lit>();
                char[] delims = new char[] { ' ' };
                while (ok && ((s = sr.ReadLine()) != null))
                {
                    s = s.Trim();
                    string[] pieces = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    if (pieces.Length > 1)
                    {
                        if (pieces[0] == "c")
                        {
                            continue;
                        }
                        else if (pieces[0] == "p")
                        {
                            this.solver.SetVarCount(System.Convert.ToInt32(pieces[2]));
                        }
                        else // It's a clause
                        {
                            lits.Clear();
                            foreach (string p in pieces)
                            {
                                int x = System.Convert.ToInt32(p);
                                if (x == 0)
                                {
                                    break;
                                }

                                lits.Add(new Lit(Math.Abs(x), (x < 0) ? true : false));
                            }

                            Clause clause;
                            Clause.ClauseNew(this.solver, lits, false, out clause);
                            this.solver.AddClause(clause);
                        }
                    }
                }
            }
        }

        public void PrintSoln()
        {
            this.solver.PrintSolution();
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(args[0]);
            System.IO.FileInfo[] files = di.GetFiles("*.cnf");
            foreach (var f in files)
            {
                BasicSAT solver = new BasicSAT(args[0] + "\\" + f.Name);
                bool soln = solver.Solve();
                solver.PrintSoln();
                System.Console.WriteLine(f.Name + " : " + (soln ? "SAT" : "UNSAT"));
            }
        }
    }
}
