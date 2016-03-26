**[RXNav](https://rxnav.nlm.nih.gov/)**-drug information resources from NIH.
This is non-allergy related but useful for interaction information.
APIs allow retrieval data from several drug sources (RxNorm, RxTerms, NDF-RT)

**Base URI** - [https://rxnav.nlm.nih.gov/REST/interaction](https://rxnav.nlm.nih.gov/REST/interaction)

**RxNav Drug Interaction RESTful API** - [https://rxnav.nlm.nih.gov/InteractionAPIREST.html](https://rxnav.nlm.nih.gov/InteractionAPIREST.html)

| SOAP Function  |REST Resource   |Description   |   
|---|---|---|
|findDrugInteractions|/interaction|Find the interactions of an RxNorm drug (specified by RxCUI)|
|findInteractionsFromList|/list|Find the interactions between a list of drugs|
|getVersion|/version|Get the version of the data set(s)|