#include "MonteCarloRoutine.hpp"
#include <cmath>
#include <stdexcept>
#include <iostream>


// void MonteCarloRoutine::get_generated_path(PnlMat* path,double t, PnlMat* past, bool isDate) 
// {
    
//         model->asset(path, t, past, isDate);
    
// }



void MonteCarloRoutine::price(double &price, double &pricestdv, PnlMat* past, bool isDate)
{
	double runningSum = 0;
	double runningSquaredSum = 0;
	double payoff;
	double interest_rate = this->model->getInterestRate();
	int N = this->model->getModelSize();
	int M = this->model->getPaymentDates()->size;
	double T  = GET(this->model->getPaymentDates(),M-1);
   
	for (unsigned long i = 0; i < sample_number; i++)
	{
         PnlMat* simulatedPaths = pnl_mat_create(M,N);

		model->asset(simulatedPaths,t,past,isDate);





		payoff = option->get_payoff(simulatedPaths,interest_rate);
		payoff*=exp(-interest_rate*(T-t));
		runningSum += payoff;
		runningSquaredSum += payoff * payoff;
	}
	    double meanPayoff = runningSum/ sample_number;
    double meanPayoffSquared = runningSquaredSum / sample_number;

    // Variance is E[payoff^2] - (E[payoff])^2
    double variancePayoff = meanPayoffSquared - (meanPayoff * meanPayoff);

    // Discount the variance to the present time


    // Compute the standard deviation
    pricestdv = sqrt(variancePayoff)/ sqrt(sample_number);
	price=meanPayoff;

}


void MonteCarloRoutine::delta(double h, double t,  PnlMat* past,PnlVect* &deltas, PnlVect* &deltasStdDev,bool isDate){;
    const double maturity = GET(model->getPaymentDates(), model->getPaymentDates()->size - 1);
    const double interestRate = model->getInterestRate();
    deltas=pnl_vect_create(model->getModelSize());
    deltasStdDev=pnl_vect_create(model->getModelSize());

    PnlMat* spots = pnl_mat_create(model->getPaymentDates()->size, model->getModelSize());
    PnlMat* spotsUp = pnl_mat_create(model->getPaymentDates()->size, model->getModelSize());
    PnlMat* spotsDown = pnl_mat_create(model->getPaymentDates()->size, model->getModelSize());
    
    // For each underlying asset
    for (int d = 0; d < model->getModelSize(); d++) {
        double sumPayoff = 0.0;
        double sumPayoffSquared = 0.0;
        
        // Monte Carlo simulation
        for (int j = 0; j < sample_number; j++) {
            // Generate base path
            model->asset(spots,t, past, isDate);
            
            // Create up and down shifted paths
            pnl_mat_clone(spotsUp, spots);
            pnl_mat_clone(spotsDown, spots);
            
            // Apply shifts
            model->shiftAsset(spotsUp, 1 + h, d, t);    // Shift up by multiplying by (1+h)
            model->shiftAsset(spotsDown, 1 - h, d, t);  // Shift down by multiplying by (1-h)
            
            // Calculate payoffs with shifted paths
            double payoffPlus = option->get_payoff(spotsUp, interestRate);
            double payoffMinus = option->get_payoff(spotsDown, interestRate);
            
            // Calculate difference in payoffs
            double payoff = payoffPlus - payoffMinus;
            sumPayoff += payoff;
            sumPayoffSquared += payoff * payoff;
        }
        
        // Calculate mean and variance
        double meanPayoff = sumPayoff / sample_number;
        double meanPayoffSquared = sumPayoffSquared / sample_number;
        double variancePayoff = meanPayoffSquared - (meanPayoff * meanPayoff);
        
        // Apply discounting
        variancePayoff *= exp(-2.0 * interestRate * (maturity - t));
        
        // Store results - Note that we're using (1+h) - (1-h) = 2h in the denominator
        LET(deltasStdDev, d) = sqrt(variancePayoff / sample_number) / (2.0 * MGET(past, past->m-1, d) * h);
        LET(deltas, d) = meanPayoff * exp(-interestRate * (maturity - t)) / (2.0 * MGET(past, past->m-1, d) * h);
    }
    
  
}    