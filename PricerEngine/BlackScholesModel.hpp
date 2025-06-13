#ifndef BLACK_SCHOLES_MODEL_HPP
#define BLACK_SCHOLES_MODEL_HPP

#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
#include "RandomGeneration.hpp"

class BlackScholesModel {
protected:
    PnlMat *volatility;        // Matrice des volatilités
    PnlVect *paymentDates;     // Vecteur des dates de paiement
    int nAssets;               // Nombre d'actifs sous-jacents
    double interestRate;       // Taux d'intérêt
    int nSamples;              // Nombre d'échantillons pour la simulation
    RandomGeneration generator;

public:
    // Constructeur et destructeur
    BlackScholesModel(PnlMat* volatility, PnlVect* paymentDates, int nAssets, double interestRate, int nSamples,RandomGeneration generator);
    ~BlackScholesModel();

    // Méthodes getter
    const PnlMat* getVolatility() const;
    const PnlVect* getPaymentDates() const;
    int getModelSize() const;
    double getInterestRate() const;
    int getNSamples() const;


        // Simulation d'actifs
    void asset(PnlMat* spots)const; 
    void asset(PnlMat* spots, double t, const PnlMat* past,bool isDate) const;

    // Décalage pour le calcul des deltas
    void shiftAsset(PnlMat* path, double h, int d); 
    void shiftAsset(PnlMat* spots, double h, int d, double t)const;
};

#endif // BLACK_SCHOLES_MODEL_HPP

