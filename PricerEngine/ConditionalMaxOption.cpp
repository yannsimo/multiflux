#include "ConditionalMaxOption.hpp"
#include <algorithm>  // Pour std::max
#include <cmath>     // Pour exp
#include <iostream>


// Constructor
ConditionalMaxOption::ConditionalMaxOption(PnlVect* times, int underlying_number, PnlVect* strikes)
    : Option(times, underlying_number, strikes) {}

// Method to compute the vector of payoffs
double ConditionalMaxOption::get_payoff(PnlMat* underlying_paths, double interestRate) const {
    int M = underlying_paths->m;  // Number of dates t_m
    int N = underlying_paths->n;  // Number of underlyings
    
    double maturity = GET(get_times(), M - 1);
    double discountedPayoffSum = 0.0;
    bool prevPayoffWasZero = true;  // Indicator that previous P_{m-1} was zero

    for (int m = 0; m < M; ++m) {
        double payoff = 0.0;
        
        if (prevPayoffWasZero) {
            // Calculate the maximum value among underlyings S_{t_m, n}
            double maxValue = -INFINITY;  // Initialize with first value
            for (int n = 0; n < N; ++n) {
                maxValue = std::max(maxValue, MGET(underlying_paths, m, n));
            }

            // Calculate P_m
            double strike_m = GET(get_strikes(), m);
            payoff = std::max(maxValue - strike_m, 0.0);
        }
        
        prevPayoffWasZero = (payoff == 0.0);
        
        if (payoff > 0.0) {
            double t_m = GET(get_times(), m);  // Get t_m
            discountedPayoffSum += payoff * exp(-interestRate * (t_m - maturity));
        }
    }

    return discountedPayoffSum;
}