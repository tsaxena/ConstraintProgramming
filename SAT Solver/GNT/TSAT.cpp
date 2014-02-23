//============================================================================
// Name        : TSAT.cpp
// Author      : 
// Version     :
// Copyright   : Your copyright notice
// Description : Hello World in C++, Ansi-style
//============================================================================


#include <iostream>
#include <vector>
#include <map>
#include "Clause.h"
using namespace std;


/*struct Lit{
	int x; //variable
	bool sign;
};*/



class Solver{
	public:
		Solver(const int, const int);
		~Solver();
		bool addClause(std::vector<int>&);

		void 	printSolution();
		void 	printProblem();

		bool 	evaluate();

		void  	solveDPLL();
		void  	backTrack();


	private:
		size_t nVariables; //no of literals in the
		size_t nClauses;
		std::vector<Clause*> clauses_;
		std::vector<bool> assgns_;
};


/******************************************************************************************
 *
 *			Common Functions
 *
 *****************************************************************************************/

Solver::Solver(const int nVars, const int nCls)
: nVariables(nVars)
, nClauses(nCls){
	//this->clauses_.resize(nClauses);
	//this->assgns_.resize(nVariables + 1);
}

Solver::~Solver(){
	//should delete the memory allocated to the clauses
}


bool Solver::addClause(std::vector<int> &lits){
	Clause *c = new Clause(lits);
	this->clauses_.push_back(c);
	//cout << "Clause list size" << this->clauses_.size()  ;
	return true;
}



void Solver::printProblem(){
	cout << " CNF " << endl;

	for(std::vector<Clause*>::iterator i = this->clauses_.begin();
									   i != this->clauses_.end();
									   i++){
		if(*i){
			(*i)->printLiterals();
		}
	}
}

void Solver::printSolution(){
	for(std::vector<bool>::iterator i = this->assgns_.begin();
			i != this->assgns_.end();
			i++){
		cout << " " << *i;
	}
}


/******************************************************************************************
 *
 *				ATTEMPT 1: Generate and test
 *
 *****************************************************************************************/
bool Solver::evaluate(){
	bool res = true;
	for(std::vector<Clause*>::iterator i = this->clauses_.begin(); i != this->clauses_.end(); i++){
		res = res  && (*i)->eval(this->assgns_);
	}
	return res;
}


void Solver::solveDPLL(){
	cout << "Attempt 1: Generate and test" << endl;
	cout << "Solutions"<< endl;
	this->backTrack();
}

void Solver::backTrack(){

	if (this->assgns_.size() == this->nVariables){ /* every clause has been removed */
		if(this->evaluate()){
			cout << " Assignment  for literals is: ";
			this->printSolution();
			cout << endl;
		}
		return;
	}

	this->assgns_.push_back(true);
	backTrack();
	this->assgns_.pop_back();

	this->assgns_.push_back(false);
	backTrack();
	this->assgns_.pop_back();
}


int main() {
	cout << "!!!Basic satisfiability solver using backtracking!!!" << endl; // prints !!!Hello World!!!
	cout << "Create the problem instance" << endl;


	Solver s(3,2);
	//add clauses
	int clause_one[] = {1};
	int clause_two[] = {2, 3, -1};
	std::vector<int> vec1(clause_one, clause_one + sizeof(clause_one) / sizeof(int) );
	s.addClause(vec1);
	std::vector<int> vec2(clause_two, clause_two + sizeof(clause_two) / sizeof(int));
	s.addClause(vec2);

	s.printProblem();

	//solve the problem instance
	s.solveDPLL();

	return 0;
}
