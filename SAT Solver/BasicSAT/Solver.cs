using System;
using System.Collections.Generic;

namespace BasicSAT
{
    public enum Assign
    {
        TRUE, FALSE, UNDEF
    }

	public class Solver
	{
        private List<Clause> clauses, learnts;
        private Dictionary<int, Assign> assigns;
        private Dictionary<int, Clause> reason;
        private Dictionary<int, int> level;
        private Dictionary<int, List<Clause>> watches;
        private Queue<Lit> propQ;
        private Stack<int> trail, trailLim;

		private bool ok;

        private int NVars { get { return assigns.Count; } }
        private int NAssigns { get { return trail.Count; } }
        private int DecisionLevel { get { return this.trailLim.Count; } }

		public Solver()
		{
			this.ok = true;

            this.clauses = new List<Clause>();
            this.learnts = new List<Clause>();
            this.assigns = new Dictionary<int, Assign>();
            this.reason = new Dictionary<int, Clause>();
            this.level = new Dictionary<int, int>();
            this.watches = new Dictionary<int, List<Clause>>();
            this.propQ = new Queue<Lit>();
            this.trail = new Stack<int>();
            this.trailLim = new Stack<int>();
		}

        public void SetVarCount(int numVars)
        {
            int i, j;
            for (i = 1; i <= numVars; i++)
            {
                j = i << 1;
                this.assigns.Add(i, Assign.UNDEF);
                this.watches.Add(j, new List<Clause>());
                this.watches.Add(j + 1, new List<Clause>());
            }
        }

        public void AddClause(Clause c)
		{
            this.clauses.Add(c);
		}

		private Clause Propogate()
		{
            while (propQ.Count > 0)
            {
                Lit p = propQ.Dequeue();
                List<Clause> tmp = this.watches[p.Val];
                this.watches[p.Val] = new List<Clause>();

                for (int i = 0; i < tmp.Count; i++)
                {
                    if (!tmp[i].Propogate(this, p))
                    {
                        for (int j = i + 1; j < tmp.Count; j++)
                        {
                            this.watches[p.Val].Add(tmp[j]);
                        }

                        this.propQ.Clear();
                        return tmp[i];
                    }
                }
            }

			return null;
		}

        public Assign Value(Lit p)
        {
            if (!this.assigns.ContainsKey(p.Var))
            {
                return Assign.UNDEF;
            }

            Assign x = this.assigns[p.Var];
            if (!p.Sign)
            {
                return x;
            }
            else
            {
                if (x == Assign.TRUE)
                {
                    return Assign.FALSE;
                }
                else if (x == Assign.FALSE)
                {
                    return Assign.TRUE;
                }
                else
                {
                    return Assign.UNDEF;
                }
            }
        }

        public void AddWatch(Lit p, Clause c)
        {
            if (!this.watches.ContainsKey(p.Val))
            {
                this.watches.Add(p.Val, new List<Clause>());
            }
            this.watches[p.Val].Add(c);
        }

        public void AddNegWatch(Lit p, Clause c)
        {
            if (!this.watches.ContainsKey(p.NegVal))
            {
                this.watches.Add(p.NegVal, new List<Clause>());
            }

            this.watches[p.NegVal].Add(c);
        }

		public bool Enqueue(Lit p, Clause from)
		{
            if (this.Value(p) != Assign.UNDEF)
            {
                if (this.Value(p) == Assign.FALSE)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else  // FIX: use p.Var for everything
            {
                int x = p.Var;
                this.assigns[x] = (p.Sign) ? Assign.FALSE : Assign.TRUE;
                this.reason[x] = from;
                this.trail.Push(p.Val);
                this.AssignLevel(x);
                this.propQ.Enqueue(p);
                return true;
            }
		}

        private void AssignLevel(int x)
        {
            if (!this.level.ContainsKey(x))
            {
                this.level.Add(x, this.DecisionLevel);
            }

            this.level[x] = this.DecisionLevel;
        }

		// Page 15
		private void Analyze(Clause confl, out List<Lit> out_learnt, out int btlevel)
		{
			int counter = 0;
            Lit p = Lit.LIT_UNDEF;
            List<Lit> p_reason;
            List<int> seen = new List<int>(); // May need to optimize this

            btlevel = 0;
            out_learnt = new List<Lit>();
            out_learnt.Add(Lit.LIT_UNDEF); // Reserve space for lits[0]

			do
			{
                p_reason = confl.CalcReason(this, p);
                foreach (Lit q in p_reason)
                {
                    if (!seen.Contains(q.Var))
                    {
                        seen.Add(q.Var);
                        if (this.level[q.Var] == this.DecisionLevel)
                        {
                            ++counter;
                        }
                        else if (this.level[q.Var] > 0)
                        {
                            //out_learnt.Add(Lit.LitFromVal(q.Val));
                            out_learnt.Add(Lit.NegativeLit(q));
                            btlevel = Math.Max(btlevel, this.level[q.Var]);
                        }
                    }
                }

				do
				{
                    p = Lit.LitFromVal(this.trail.Peek());
                    confl = this.reason[p.Var];
                    this.UndoOne();
				} while (!seen.Contains(p.Var));

				--counter;
			} while(counter > 0);

            out_learnt[0] = Lit.NegativeLit(p);
		}

        private void UndoOne()
        {
            Lit l = Lit.LitFromVal(this.trail.Peek());
            int x = l.Var;
            this.assigns[x] = Assign.UNDEF;
            this.reason[x] = null;
            this.level[x] = -1;
            this.trail.Pop();
        }

		public bool Search()
		{
			while (true)
			{
				Clause confl = this.Propogate();
				if (confl != null)
				{
                    //this.PrintLearnts();

					if (this.DecisionLevel == 0)
					{
						return false;
					}

					int backtrack_level = 0;
                    List<Lit> learnt_clause;
                    this.Analyze(confl, out learnt_clause, out backtrack_level);

                    //System.Console.WriteLine();
                    if (backtrack_level == 0)
                    {
                        int y = 0;
                    }

                    this.CancelUntil(Math.Max(backtrack_level, 0));  // 0 for root level?
                    this.Record(learnt_clause);
				}
				else
				{
                    if (this.NAssigns == this.NVars)
                    {
                        return true;
                    }
                    else // Pick new variable
                    {
                        foreach (var x in this.assigns)
                        {
                            if (x.Value == Assign.UNDEF)
                            {
                                this.Assume(Lit.LitFromVal((x.Key * 2) + 1));
                                break;
                            }
                        }
                    }
				}
			}

			return true;
		}

        private void CancelUntil(int level)
        {
            while (this.DecisionLevel > level)
            {
                this.Cancel();
            }
        }

        private void Cancel()
        {
            int c = this.trail.Count - this.trailLim.Peek();
            for (; c != 0; c--)
            {
                this.UndoOne();
            }

            this.trailLim.Pop();
        }

        private bool Assume(Lit p)
        {
            this.trailLim.Push(this.trail.Count);
            return this.Enqueue(p, null);
        }

        private void Record(List<Lit> clause)
		{
            Clause c;
            Clause.ClauseNew(this, clause, true, out c);
            this.Enqueue(clause[0], c);
            if (c != null)
            {
                learnts.Add(c);
            }
		}

        public void PrintClauses()
        {
            foreach (Clause c in this.clauses)
            {
                c.PrintClause();
            }
        }

        public Dictionary<int, Assign> GetSolution()
        {
            return this.assigns;
        }

        public void PrintSolution()
        {
            int count = 0;
            foreach (var curr in this.assigns)
            {
                if (curr.Value == Assign.TRUE)
                {
                    ++count;
                    System.Console.WriteLine("" + curr.Key + " TRUE");
                    int num = curr.Key;
                    int x = num / 81;
                    int y = (num - (x * 81) - 1) / 9;
                    int z = (num - (x * 81)) - (9 * y);
                    System.Console.WriteLine("{0},{1} = {2}", x, y, z);
                }
            }

            System.Console.WriteLine("Total true: {0}", count);
        }

        private void PrintLearnts()
        {
            System.Console.WriteLine("Learnts\n----------");
            foreach (Clause c in this.learnts)
            {
                c.PrintClause();
            }

            System.Console.WriteLine();
        }
	}
}
