#pragma once
#include "Option.hpp"
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"

class ConditionalMaxOption : public Option {
public:
    // Constructor
    ConditionalMaxOption(PnlVect* times, int underlying_number, PnlVect* strikes);

    // Compute the payoff of the option
     double get_payoff(PnlMat* underlying_paths, double interestRate) const;
};