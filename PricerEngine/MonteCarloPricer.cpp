#include <cmath>
#include "MonteCarloPricer.hpp"
#include "MonteCarloRoutine.hpp"
#include "pnl/pnl_matrix.h"


void MonteCarloPricer::price_at(const double time, BlackScholesModel * model, Option *option, PnlMat * past, double &price, double &confidence_interval) const 
{
	MonteCarloRoutineAtTimeT mct(model, option, sample_number, past, time);
	mct.price(price, confidence_interval);
}
