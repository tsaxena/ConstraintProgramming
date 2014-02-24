using System.Collections.Generic;

namespace BasicSAT
{
	public class Clause
	{
		private bool learnt;
        private List<Lit> lits;

		public Clause()
		{
			this.learnt = false;
            this.lits = new List<Lit>();
		}

		public bool Propogate(Solver s, Lit p)
		{
            if (this.lits[0].Val == p.NegVal)  // ensure lits[1] is the false lit
            {
                Lit temp = this.lits[0];
                this.lits[0] = this.lits[1];
                this.lits[1] = temp;
            }

            if (s.Value(this.lits[0]) == Assign.TRUE)
            {
                s.AddWatch(p, this);
                return true;
            }

            for (int i = 2; i < this.lits.Count; i++)
            {
                if (s.Value(this.lits[i]) != Assign.FALSE)
                {
                    Lit temp = this.lits[1];
                    this.lits[1] = this.lits[i];
                    this.lits[i] = temp;
                    s.AddNegWatch(this.lits[1], this);
                    return true;
                }
            }

            // DEBUGGING
            //System.Console.WriteLine("BCP: {0} implies {1}", p.ToString(), this.ToString());

            s.AddWatch(p, this);
            return s.Enqueue(this.lits[0], this);
		}

		public List<Lit> CalcReason(Solver s, Lit p)
		{
            List<Lit> x = new List<Lit>();
            for (int i = (p == Lit.LIT_UNDEF ? 0 : 1); i < this.lits.Count; i++)
            {
                //x.Add(this.lits[i]);
                x.Add(Lit.NegativeLit(this.lits[i]));
            }

            return x;
		}

        public static bool ClauseNew(Solver s, List<Lit> ps, bool learnt, out Clause out_clause)
		{
			out_clause = null;

			if (!learnt)
			{
                foreach (Lit lit in ps)
                {
                    if (s.Value(lit) == Assign.TRUE)
                    {
                        return true;
                    }
                }
			}

            if (ps.Count == 0)
            {
                return false;
            }
            else if (ps.Count == 1)
            {
                return s.Enqueue(ps[0], null);
            }

			// Else make a new clause
			Clause c = new Clause();
			c.learnt = learnt;
            foreach (Lit lit in ps)
            {
                c.lits.Add(lit);
            }

            // Add clause to watcher lists (!lits[0], !lits[1])
            s.AddNegWatch(c.lits[0], c);
            s.AddNegWatch(c.lits[1], c);

            out_clause = c;
			return true;
		}

        public override string ToString()
        {
            string s = "";
            foreach (Lit l in this.lits)
            {
                s += l.ToString() + " ";
            }

            return s;

        }

        public void PrintClause()
        {
            foreach (Lit l in this.lits)
            {
                System.Console.Write(l.ToString() + " ");
            }

            System.Console.WriteLine();
        }
	}
}