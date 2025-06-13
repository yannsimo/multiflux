#include "Option.hpp"


// Constructor
Option::Option(PnlVect* times, int underlying_number, PnlVect* strikes)
    : times_(pnl_vect_copy(times)),  // Copie de `times`
      strikes_(pnl_vect_copy(strikes)), // Copie de `strikes`
      underlying_number_(underlying_number) {

   
}

// Virtual destructor
Option::~Option() {
    pnl_vect_free(&strikes_);
    pnl_vect_free(&times_);
}

 // namespace options
