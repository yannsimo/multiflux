#pragma once
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"


class Option {
private:
    PnlVect* strikes_;
    PnlVect* times_;
    int underlying_number_;

public:
    // Constructor
    Option(PnlVect* times, int underlying_number, PnlVect* strikes);

    // Virtual destructor
    virtual ~Option();

    // Accessors
    PnlVect* get_strikes() const { return strikes_; }
    PnlVect* get_times() const { return times_; }
    int get_underlying_number() const { return underlying_number_; }
    virtual double get_payoff( PnlMat* underlyingPaths, double r) const = 0;
};


 // namespace options
