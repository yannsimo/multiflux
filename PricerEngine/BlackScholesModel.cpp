#ifndef BLACK_SCHOLES_MODEL_CPP
#define BLACK_SCHOLES_MODEL_CPP

#include "BlackScholesModel.hpp"
#include <cmath>
#include <stdexcept>
#include <iostream>
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"


// Constructeur
BlackScholesModel::BlackScholesModel(PnlMat* volatility, PnlVect* paymentDates, int nAssets, double interestRate, int nSamples,RandomGeneration generator)
    : volatility(volatility), paymentDates(paymentDates), nAssets(nAssets), interestRate(interestRate), nSamples(nSamples),generator() {}

// Destructeur
BlackScholesModel::~BlackScholesModel() {
    pnl_mat_free(&volatility);
    pnl_vect_free(&paymentDates);
}


const PnlMat* BlackScholesModel::getVolatility() const {
    return volatility;
}

// Getter pour le vecteur des dates de paiement
const PnlVect* BlackScholesModel::getPaymentDates() const {
    return paymentDates;
}

// Getter pour le nombre d'actifs sous-jacents
int BlackScholesModel::getModelSize() const {
    return nAssets;
}

// Getter pour le taux d'intérêt
double BlackScholesModel::getInterestRate() const {
    return interestRate;
}

// Getter pour le nombre d'échantillons
int BlackScholesModel::getNSamples() const {
    return nSamples;
}




void BlackScholesModel::asset(PnlMat* spots) const {
    int modelSize = getModelSize();
    int nbTimeSteps = this->getPaymentDates()->size;

    PnlVect* gaussianVector = pnl_vect_create(modelSize);
    PnlVect* prevSpots = pnl_vect_create(modelSize);
    PnlVect* choleskyLine = pnl_vect_new();

    std::cout << "Nombre d'étapes : " << nbTimeSteps << std::endl;
    std::cout << "Taille du modèle : " << modelSize << std::endl;

    for (int i = 0; i < nbTimeSteps; ++i) { 
        // Génération de nombres gaussiens
        this->generator.get_one_gaussian_sample(gaussianVector);

        // Affichage des nombres gaussiens
        std::cout << "Étape " << i << " - Nombres gaussiens : ";
        for (int j = 0; j < modelSize; ++j) {
            std::cout << pnl_vect_get(gaussianVector, j) << " ";
        }
        std::cout << std::endl;

        // Calcul du pas de temps
        double dt = (i == 0) ? GET(this->getPaymentDates(), i) 
                              : GET(this->getPaymentDates(), i) - GET(this->getPaymentDates(), i-1);
        std::cout << "Pas de temps : " << dt << std::endl;

        // Récupération des spots précédents
        pnl_mat_get_row(prevSpots, spots, i);
        
        // Pour chaque actif
        for (int d = 0; d < modelSize; ++d) {  
            // Récupération de la ligne de la matrice de volatilité
            pnl_mat_get_row(choleskyLine, this->getVolatility(), d);

            // Affichage de la volatilité
            std::cout << "Volatilité actif " << d << " : " 
                      << pnl_vect_get(choleskyLine, d) << std::endl;

            // Produit scalaire avec le vecteur gaussien
            double choleskyProduct = pnl_vect_scalar_prod(choleskyLine, gaussianVector);
            std::cout << "Produit Cholesky : " << choleskyProduct << std::endl;

            // Calcul de la volatilité
            double vol = pnl_vect_get(choleskyLine, d);

            // Termes déterministe et stochastique
            double deterministicTerm = (interestRate - 0.5 * vol * vol) * dt;
            double stochasticTerm =  choleskyProduct * sqrt(dt);
            
            std::cout << "Terme déterministe : " << deterministicTerm 
                      << ", Terme stochastique : " << stochasticTerm << std::endl;

            // Mise à jour du prix
            double expTerm = exp(deterministicTerm + stochasticTerm);
            double previousSpot = pnl_vect_get(prevSpots, d);
            double newSpot = expTerm * previousSpot;
            
            std::cout << "Prix précédent : " << previousSpot 
                      << ", Nouveau prix : " << newSpot << std::endl;

            pnl_vect_set(prevSpots, d, newSpot);
        }
        
        // Mise à jour de la matrice des spots
        pnl_mat_set_row(spots, prevSpots, i+1);
    }

    // Libération de la mémoire
   
}
int  find_min_greater_than_t_sorted(const PnlVect *vect, double t) {
    for (int i = 0; i < vect->size; ++i) {
        double value = GET(vect, i);
        if (value >= t) {
            return i; // Retourner dès que l'on trouve un élément strictement supérieur
        }
    }

    // Si aucun élément strictement supérieur n'est trouvé
     return -1; // Retourne NaN
}







    void BlackScholesModel::asset(PnlMat* path, double t, const PnlMat* past,bool isDate)const {

    int modelSize = getModelSize();
    int lastSeenDateIndex;
    PnlMat* subPastSpots;
    int index=0;

    
    // Set first elements to deterministic Spots
    if(t>0){
    lastSeenDateIndex = find_min_greater_than_t_sorted(this->getPaymentDates(),t);
    
     if(isDate){
        index=lastSeenDateIndex+1;
    }else{
        index=lastSeenDateIndex;
        }
   
   subPastSpots = pnl_mat_create(index,modelSize);

    pnl_mat_extract_subblock(subPastSpots,past,0,index,0,modelSize);
    pnl_mat_set_subblock(path,subPastSpots,0,0);


    pnl_mat_free(&subPastSpots);
    }else{
        lastSeenDateIndex=0;
      
    }

            // Cholesky FactorizationlastSeenDateIndex
        // PnlMat* choleskyMatrix = pnl_mat_copy(correlationMatrix);
        // int choleskyResult = pnl_mat_chol(choleskyMatrix);
        
        
        // Creating useful vectors
        PnlVect* gaussianVector = pnl_vect_create(modelSize);
        PnlVect* choleskyLine = pnl_vect_new();
        
        PnlVect* SpotsTilde = pnl_vect_create(modelSize);
        // double dtInitial = GET(this->getPaymentDates(),lastSeenDateIndex)-t;

        // double sqrtDtInitial = sqrt(dtInitial);
        // // Spots at T
        // PnlVect* spotsAtT = pnl_vect_create(modelSize);
        // pnl_mat_get_row(spotsAtT,past,past->m-1);

        // // //Simulate initial futureSpots
        // //     for (int j = 0; j < modelSize; ++j) {
        // //             double normal = pnl_rng_normal(rng);
        // //             pnl_vect_set(gaussianVector, j, normal);
        // //     }
        // this->generator.get_one_gaussian_sample(gaussianVector);





            
        //     for (int d = 0; d < modelSize; ++d) {  // For each asset
                
        //         pnl_mat_get_row(choleskyLine, choleskyMatrix, d);
        //         double choleskyProduct = pnl_vect_scalar_prod(choleskyLine, gaussianVector);
        //         // Deterministic and stochastic terms
        //         double vol = pnl_vect_get(volatility, d);
        //         double deterministicTerm = (interestRate - 0.5 * vol * vol) * dtInitial;
        //         double stochasticTerm = vol * choleskyProduct * sqrtDtInitial;
        //         double expTerm = exp(deterministicTerm + stochasticTerm);
        //         LET(SpotsTilde,d) = expTerm * GET(spotsAtT,d);
                
        //     }
        //     pnl_mat_set_row(path, SpotsTilde,lastSeenDateIndex+1);




        
        // pnl_vect_free(&SpotsTilde);
        // double dt = maturity / nbTimeSteps;
        // double sqrtDt = sqrt(dt);


        // Loop over each time step and each asset
        PnlVect* prevSpotTilde = pnl_vect_create(modelSize);
        //pnl_mat_print(past);
       

        for (int i=index; i< path->m; i++){
            // for (int j = 0; j < modelSize; ++j) {
            //     double normal = pnl_rng_normal(rng);
            //     pnl_vect_set(gaussianVector, j, normal);

            // }


                this->generator.get_one_gaussian_sample(gaussianVector);

                double dt = (i == index) ? GET(this->getPaymentDates(), i)-t
                              : GET(this->getPaymentDates(), i) - GET(this->getPaymentDates(), i-1);
                
                double sqrtDt = sqrt(dt);


                
            if(i==index){
                    pnl_mat_get_row(prevSpotTilde,past,past->m-1);
                    //pnl_vect_print(prevSpotTilde);

            }else{
            pnl_mat_get_row(prevSpotTilde,path,i-1);
            }
            
            for (int d = 0; d < modelSize; ++d) {  // For each asset
                
                pnl_mat_get_row(choleskyLine, this->getVolatility(), d);
                double choleskyProduct = pnl_vect_scalar_prod(choleskyLine, gaussianVector);
                // Deterministic and stochastic terms
                double vol =pnl_vect_norm_two(choleskyLine);
                double deterministicTerm = (interestRate - 0.5 * vol * vol) * dt;
                double stochasticTerm =  choleskyProduct * sqrtDt;
                double expTerm = exp(deterministicTerm + stochasticTerm);
                LET(prevSpotTilde,d) = expTerm *GET(prevSpotTilde,d);
                
                
            }

            pnl_mat_set_row(path, prevSpotTilde,i);
        }


    }
    
    
    
    

void BlackScholesModel::shiftAsset(PnlMat* path,double h, int d){

    PnlVect* column_d = pnl_vect_new();
    pnl_mat_get_col(column_d,path,d);
    pnl_vect_mult_scalar(column_d,h);
    pnl_mat_set_col(path,column_d,d);
    pnl_vect_free(&column_d);

}

void BlackScholesModel::shiftAsset(PnlMat* spots, double h, int d, double t)const{
    // Calculate the index i where we want to start the shift

    // Extract the column for asset d
    PnlVect* column_d = pnl_vect_new();
    pnl_mat_get_col(column_d, spots, d);

    // Apply the shift 
    int row=find_min_greater_than_t_sorted(this->getPaymentDates(),t);
    
    for ( ; row < column_d->size; row++) {
        LET(column_d, row) *= h; // Multiply each value from row i onward by h
    }

    // Set the modified column back into the matrix
    pnl_mat_set_col(spots, column_d, d);

    // Free memory
    pnl_vect_free(&column_d);

}
    


#endif
