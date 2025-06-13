#pragma once
#include "pnl/pnl_vector.h"
#include "RandomGeneration.hpp"
#include "BlackScholesModel.hpp"
#include "Option.hpp"
class MonteCarloPricer
{
private:
	const unsigned long sample_number;


public:
	MonteCarloPricer(const unsigned long sample_nb) : sample_number(sample_nb) {};
	void price_at(const double time, BlackScholesModel * model, Option *option, PnlMat * past, double &price, double &confidence_interval) const;
};
