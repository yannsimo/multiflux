#include "ConditionalBasketOption.hpp"
#include <algorithm>
#include <iostream>
 // Pour std::max

// Constructeur
ConditionalBasketOption::ConditionalBasketOption(PnlVect* times, int underlying_number, PnlVect* strikes)
    : Option(times, underlying_number, strikes)  {}

// Méthode pour calculer le vecteur des payoffs
double ConditionalBasketOption::get_payoff( PnlMat*  underlying_paths,double interestRate) const {

    int M = underlying_paths->m; // Nombre de dates t_m
    int N = underlying_paths->n; // Nombre de sous-jacents

    // Initialiser le vecteur de payoffs
    PnlVect* payoffs = pnl_vect_create_from_zero(M);
    double maturity = GET(get_times(), M - 1);
    double discountedPayoffSum = 0.0;
    bool previousZero = true; // Indicateur que tous les P_k précédents sont nuls

    for (int m = 0; m < M; ++m) {
        if (!previousZero) {
            break; // Dès qu'un payoff précédent est non nul, tous les suivants sont 0
        }

        // Calcul de la moyenne des sous-jacents S_{t_m, n}
        double averageValue = 0.0;
        for (int n = 0; n < N; ++n) {
            averageValue += MGET(underlying_paths, m, n);
        }
        averageValue /= N;

        // Calcul de P_m
        double strike_m = GET(get_strikes(), m);
        double payoff = std::max(averageValue - strike_m, 0.0);
        double t_m = GET(get_times(), m); // Récupérer t_m
        discountedPayoffSum += payoff * exp(-interestRate* (t_m - maturity));
        if (payoff > 0) {
            previousZero = false; // Si P_m > 0, les suivants ne seront pas calculés
        }
    }


    return discountedPayoffSum;
}
    

   