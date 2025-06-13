#include <iostream>
#include <cmath>
#include <vector>
#include "BlackScholesModel.hpp"
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
#include "RandomGeneration.hpp"

class BlackScholesModelAssetTest {
    // Classe de génération aléatoire simulée pour des tests reproductibles
    

public:
    void testAssetMethodWithPast() {
        // Paramètres de test
        int nAssets = 2;
        double interestRate = 0.05;
        int nSamples = 5; // Nombre de pas de temps

        // Création de la matrice de volatilité
        PnlMat* volatility = pnl_mat_create(nAssets, nAssets);
        for (int i = 0; i < nAssets; ++i) {
            for (int j = 0; j < nAssets; ++j) {
                pnl_mat_set(volatility, i, j, 0.2); // Volatilité constante à 20%
            }
        }

        // Création du vecteur des dates de paiement
        PnlVect* paymentDates = pnl_vect_create(nSamples);
        for (int i =1; i <= nSamples; ++i) {
            pnl_vect_set(paymentDates, i-1, i * 0.1); // Dates de 0 à 0.5 avec un pas de 0.1
        }

        // Création de la matrice des spots passés
        PnlMat* past = pnl_mat_create(1, nAssets); // 2 lignes pour avoir un historique
        for (int i = 1; i <= past->m; ++i) {
            for (int j = 0; j < nAssets; ++j) {
                pnl_mat_set(past, i-1, j, 100.0+i); // Prix initial de 100 pour chaque actif
            }
        }

        // Création de la matrice pour stocker les futurs chemins
        PnlMat* path = pnl_mat_create(nSamples, nAssets);

        // Création du générateur aléatoire mock
        RandomGeneration mockGenerator;

        // Création du modèle Black-Scholes
        BlackScholesModel model(
            volatility, 
            paymentDates, 
            nAssets, 
            interestRate, 
            nSamples, 
            mockGenerator
        );

        // Temps initial (0.05)
        double t = 0.15;

        // Simulation des chemins des actifs à partir des spots passés
        model.asset(path, t, past,false);

        // Affichage des résultats
        std::cout << "Simulated Asset Paths:" << std::endl;
        for (int i = 0; i < nSamples; ++i) {
            std::cout << "Time step " << i << " (t = " << GET(paymentDates, i) << "): ";
            for (int j = 0; j < nAssets; ++j) {
                std::cout << pnl_mat_get(path, i, j) << " ";
            }
            std::cout << std::endl;
        }

        // Libération de la mémoire
    }

    void runAllTests() {
        std::cout << "Running Black-Scholes Asset Simulation Test with Past..." << std::endl;
        testAssetMethodWithPast();
    }
};

int main() {
    BlackScholesModelAssetTest test;
    test.runAllTests();
    return 0;
}