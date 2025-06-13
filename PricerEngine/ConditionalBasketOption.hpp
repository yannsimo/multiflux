#pragma once
#include "Option.hpp" // Incluez le fichier où Option est défini
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
#include <cmath>

class ConditionalBasketOption : public Option {
public:
    // Constructeur
    ConditionalBasketOption(PnlVect* times, int underlying_number, PnlVect* strikes);

    // Méthode pour calculer le vecteur des payoffs
    double get_payoff( PnlMat*  underlying_paths,double interestRate) const;
};
