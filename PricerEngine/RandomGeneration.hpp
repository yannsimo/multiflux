#pragma once

#include "pnl/pnl_random.h" // Inclusion de la bibliothèque PNL pour les générateurs aléatoires
#include "pnl/pnl_vector.h" // Pour les vecteurs de la PNL


class RandomGeneration
{
private:
    PnlRng* pnlRandomGen; // Générateur aléatoire de la bibliothèque PNL

public:
    // Constructeur
    RandomGeneration();

    // Destructeur
    ~RandomGeneration();

    // Méthode pour générer un échantillon gaussien
    void get_one_gaussian_sample(PnlVect* const into) const;
};

// namespace RandomGeneration
