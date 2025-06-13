#pragma once
#include "pnl/pnl_vector.h"



struct ConditionalBasketOptionParameters
{
    int underlying_number;          // Nombre de sous-jacents
    PnlVect *strikes;               // Vecteur des strikes (K_m)
    PnlVect *evaluation_dates;      // Vecteur des dates t_m
    int time_steps;                 // Nombre de dates de paiement (M)
    
    // Constructeur par défaut
    ConditionalBasketOptionParameters() :
        underlying_number(0),
        strikes(nullptr),
        evaluation_dates(nullptr),
        time_steps(0)
    {}

    // Constructeur paramétré
    ConditionalBasketOptionParameters(int underlyings, PnlVect *strikes, PnlVect *dates, int steps) :
        underlying_number(underlyings),
        time_steps(steps)
    {
        this->strikes = pnl_vect_copy(strikes);
        this->evaluation_dates = pnl_vect_copy(dates);
    }

    // Constructeur par copie
    ConditionalBasketOptionParameters(const ConditionalBasketOptionParameters &other) :
        underlying_number(other.underlying_number),
        time_steps(other.time_steps)
    {
        strikes = pnl_vect_copy(other.strikes);
        evaluation_dates = pnl_vect_copy(other.evaluation_dates);
    }

    // Opérateur d'affectation par copie
    ConditionalBasketOptionParameters &operator=(const ConditionalBasketOptionParameters &other)
    {
        if (this != &other)
        {
            underlying_number = other.underlying_number;
            time_steps = other.time_steps;

            pnl_vect_free(&strikes);
            pnl_vect_free(&evaluation_dates);

            strikes = pnl_vect_copy(other.strikes);
            evaluation_dates = pnl_vect_copy(other.evaluation_dates);
        }
        return *this;
    }

    // Destructeur
    ~ConditionalBasketOptionParameters()
    {
        pnl_vect_free(&strikes);
        pnl_vect_free(&evaluation_dates);
    }
};
 // namespace options
