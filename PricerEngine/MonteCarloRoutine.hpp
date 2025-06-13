#pragma once
#include "BlackScholesModel.hpp"
#include "Option.hpp"
#include "pnl/pnl_matrix.h"

class MonteCarloRoutine
{
protected:
    const unsigned long sample_number;
    const BlackScholesModel* model;
     Option* option;
    const double t;
    public:
    
    MonteCarloRoutine(BlackScholesModel *model, Option* option, const unsigned long sample_number, const double t)
        : model(model), option(option), sample_number(sample_number), t(t)
    {
    }

// protected:
//     void get_generated_path(PnlMat* path,double t, PnlMat* past, bool isDate) ;

public:
    void price(double &price, double &confidence_interval, PnlMat* past, bool isDate) ;
    void delta(double h, double t,  PnlMat* past,PnlVect* &deltas, PnlVect* &deltasStdDev,bool isDate);
};
