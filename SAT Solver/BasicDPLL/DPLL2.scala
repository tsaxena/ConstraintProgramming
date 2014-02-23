object DPLL2 {
  case class Literal(val name: String, val pos: Boolean)
  type Clause = Set[Literal]
  
  def negate(l : Literal) = Literal(l.name, !l.pos)
  
  def isUnsatisfiable(formula: Set[Clause]): Boolean = {
     formula exists(clause => clause.isEmpty)
  }

  def isSatisfiable(formula: Set[Clause]): Boolean = {
	  formula isEmpty
  }
  
  def getUnits(formula : Set[Clause]): Set[Literal] = {
	  formula filter (x => x.size == 1) map (x => x.head)
  }
  
   def delUnit(unit: Literal , formula: Set[Clause] ) = {
	  //remove the unit clause and 
      //negative instance of the literal from other clauses
	  var newformula = formula filter( clause => (!clause.contains(unit))) 
	  newformula map( clause => if (clause contains negate(unit)) (clause - negate(unit)) 
	      						else clause)
  }
   
  def delAllUnits(formula: Set[Clause]) = {
      var k = formula
	  for( unit_clause <- getUnits(formula)){
   		  k = delUnit(unit_clause, k)
	  }
      k
  }
  
  def unitProp(formula : Set[Clause]) = {
	  delAllUnits(formula)
  }
  
  def findLiterals(formula : Set[Clause]): Map[String, Boolean] = {
	  var map = Map[String,Boolean]()
	  for(clause <- formula ; literal <- clause){
		 map += literal.name -> literal.pos
	  }
	  map//for( key <- map.keySet if map(key) == 1 ) yield key
  }
  
  def pureLiterals(formula : Set[Clause]) = {
    val literals = formula flatMap (x => x)
    var pures = findLiterals(formula)
    for(literal <- literals){
        if(pures.getOrElse(literal.name, 0) == !literal.pos){
        	pures -= literal.name 
        }
    }
    pures
  }
  
  def pureLiteralElimination(formula: Set[Clause]): Set[Clause] = {
    
      var k = formula
      for((x,y) <- pureLiterals(formula)){
        val pure = new Literal(x,y)
    	k = k filter( clause => (! clause.contains(pure)))
      }  
      k
  }
  
  def DPLL(formula: Set[Clause]):Boolean = {
    if(isUnsatisfiable(formula)){
      return false
    }
    
    if(isSatisfiable(formula)){
      return true
    }
    
    val temp = unitProp(formula)
	val temp2 = pureLiteralElimination(temp)
	println (temp2)
   	
   	
   	if(isUnsatisfiable(temp2)){
      return false
    }
    
    if(isSatisfiable(temp2)){
      return true
    }
    
    val lit = temp2.head.head
   	DPLL(delUnit(lit, temp2)) || DPLL(delUnit(negate(lit), temp2))
  }
  
  def main(args: Array[String]) {
	  	println("SAT Solver with unit propagation and pure literal elimination");
	  	var form = Set.empty[Clause]
		var clause = Set.empty[Literal]
	  	var formula = Set( Set(Literal("x1", true), Literal("x3", false), Literal("x2", false)), 
   						   Set(Literal("x2", true), Literal("x3", true),  Literal("x1", false)) //2 3 -1
   						  ) 
   		
   		println(DPLL(formula))
  }
}