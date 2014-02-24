namespace BasicSAT
{
	public class Lit
	{
		private int x;

        public int Var    { get { return this.x >> 1; } }
		public int Val	  { get { return this.x; } }
		public int NegVal { get { return this.x ^ 1; } }
        public bool Sign  { get { return ((this.x & 1) == 0) ? false : true; } }

        public Lit() { }

		public Lit(int x, bool sign)
		{
            this.x = x << 1;
			if (sign)
			{
                this.x++;
			}
		}

        public static Lit LitFromVal(int val)
        {
            Lit n = new Lit();
            n.x = val;
            return n;
        }

        public static Lit NegativeLit(Lit l)
        {
            Lit n = new Lit();
            n.x = l.NegVal;
            return n;
        }

        public override string ToString()
        {
            string neg = this.Sign ? "-" : "";
            return neg + this.Var;
        }

        public static readonly Lit LIT_UNDEF = new Lit(-1, false);
	}
}