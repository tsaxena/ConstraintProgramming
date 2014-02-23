/*
 * Clause.cpp
 *
 *  Created on: Sep 13, 2013
 *      Author: tripti
 */
#include "Clause.h"
using namespace std;
Clause::Clause(const std::vector<int>& lits)
:lits_(lits){
	cout << "new clause" << endl;
	for(std::vector<int>::iterator i = this->lits_.begin(); i != this->lits_.end(); i++ ){
		cout << "   " <<(*i);
	}
	cout << endl;

}

Clause::Clause(int l){
	this->lits_.resize(1);
	this->lits_.push_back(l);
}

Clause::~Clause(){

}

bool Clause::eval(const std::vector<bool> &assign){
	bool res = false;
	for(std::vector<int>::iterator i = this->lits_.begin(); i != this->lits_.end(); i++ ){
		bool val = assign[abs(*i)-1];
		res = res || ((*i) > 0 ? val : !val);
	}
	return res;
}

void Clause::printLiterals(){
	cout << "{";
	for(std::vector<int>::iterator i = this->lits_.begin(); i != this->lits_.end(); i++){
		cout << *i;
	}
	cout << endl;
}

void Clause::addLiteral(int lit){
	this->lits_.push_back(lit);
}



bool Clause::isUnitClause(){
	return (this->lits_.size() == 1);
}
bool Clause::isEmptyClause(){
	return (this->lits_.size() == 0);
}

bool Clause::hasLiteral(int lit){

	for(std::vector<int>::iterator i = this->lits_.begin(); i != this->lits_.end(); i++){
		if(abs(*i) == lit){
			return true;
		}
	}
	return false;
}

void Clause::removeLiteral(int lit){
	//find it
	std::vector<int>::iterator i = std::find(this->lits_.begin(), this->lits_.end(), lit);
}

int Clause::getUnitLiteral(){
	return this->lits_.front();
}

