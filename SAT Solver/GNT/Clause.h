/*
 * Clause.h
 *
 *  Created on: Sep 13, 2013
 *      Author: tripti
 */

#ifndef CLAUSE_H_
#define CLAUSE_H_
#include <vector>
#include <iostream>
#include <algorithm>
class Clause{

	public:
		Clause(const std::vector<int>&);
		Clause(int);
		~Clause();
		void printLiterals();
		void addLiteral(int);

		bool isUnitClause();
		bool isEmptyClause();

		bool hasLiteral(int n);
		void removeLiteral(int);

		int getUnitLiteral();

		bool eval(const std::vector<bool> &assign);

	private:
		std::vector<int> lits_;
};



#endif /* CLAUSE_H_ */
