#include <iostream>
#include "json_reader.hpp"
#include "pricer.hpp"
#include "BlackScholesModel.hpp"
#include "ConditionalBasketOption.hpp"
#include "Option.hpp"
#include "MonteCarloRoutine.hpp"
#include "ConditionalMaxOption.hpp"


BlackScholesPricer::BlackScholesPricer(nlohmann::json &jsonParams) {
    jsonParams.at("VolCholeskyLines").get_to(volatility);
    jsonParams.at("MathPaymentDates").get_to(paymentDates);
    jsonParams.at("Strikes").get_to(strikes);
    jsonParams.at("DomesticInterestRate").get_to(interestRate);
    jsonParams.at("RelativeFiniteDifferenceStep").get_to(fdStep);
    jsonParams.at("SampleNb").get_to(nSamples);
    jsonParams.at("PayoffType").get_to(payoffType);

    nAssets = volatility->n;
}

BlackScholesPricer::~BlackScholesPricer() {
    pnl_vect_free(&paymentDates);
    pnl_vect_free(&strikes);
    pnl_mat_free(&volatility);
}

void BlackScholesPricer::print() {
    std::cout << "nAssets: " << nAssets << std::endl;
    std::cout << "fdStep: " << fdStep << std::endl;
    std::cout << "nSamples: " << nSamples << std::endl;
    std::cout << "strikes: ";
    pnl_vect_print_asrow(strikes);
    std::cout << "paymentDates: ";
    pnl_vect_print_asrow(paymentDates);
    std::cout << "volatility: ";
    pnl_mat_print(volatility);
}

void BlackScholesPricer::priceAndDeltas(const PnlMat *past, double currentDate, bool isMonitoringDate, double &price, double &priceStdDev, PnlVect* &deltas, PnlVect* &deltasStdDev) {
            price = 0.;
    priceStdDev = 0.;
    
    // Création du modèle de Black-Scholes
    BlackScholesModel* model = new BlackScholesModel(
        pnl_mat_copy(volatility), 
        pnl_vect_copy(paymentDates), 
        nAssets, 
        interestRate, 
        nSamples,
        RandomGeneration()
    );

        Option* option;
    if (payoffType == "ConditionalBasket") {
        option = new ConditionalBasketOption(
            pnl_vect_copy(paymentDates),
            nAssets,
            pnl_vect_copy(strikes)
        );
    } 
    else if (payoffType == "ConditionalMax") {
        option = new ConditionalMaxOption(
            pnl_vect_copy(paymentDates),
            nAssets,
            pnl_vect_copy(strikes)
        );
    }
    else {
        std::cerr << "Unknown payoff type: " << payoffType << std::endl;
        delete model;
        return;
    }

    
    
    
    // Création de la routine Monte Carlo
    MonteCarloRoutine* mc = new MonteCarloRoutine(
        model,
        option,
        nSamples,
        currentDate
    );

    
    // Calcul du prix
    mc->price(price, priceStdDev, const_cast<PnlMat*>(past), isMonitoringDate);

    
    // Calcul des deltas
    mc->delta(fdStep, currentDate, const_cast<PnlMat*>(past), deltas, deltasStdDev,isMonitoringDate);

    
    
}