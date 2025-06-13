#include "RandomGeneration.hpp"
#include <ctime>


// Constructeur : Initialise le générateur aléatoire PNL
RandomGeneration::RandomGeneration()
{
    pnlRandomGen = pnl_rng_create(PNL_RNG_MERSENNE); // Création du générateur Mersenne Twister
    pnl_rng_sseed(pnlRandomGen, time(NULL));         // Initialisation avec la graine temporelle
}

// Destructeur : Libère la mémoire associée au générateur aléatoire
RandomGeneration::~RandomGeneration()
{
     
}

// Méthode pour générer un échantillon gaussien
void RandomGeneration::get_one_gaussian_sample(PnlVect* const into) const
{
    pnl_vect_rng_normal(into, into->size, pnlRandomGen); // Génère un vecteur gaussien
}

// namespace RandomGeneration
