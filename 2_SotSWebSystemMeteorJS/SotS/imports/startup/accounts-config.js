

import { Accounts } from 'meteor/accounts-base';

 
// TU TENS DE CRIAR USERNAMES COM OS PlayFabID's  
// adiciona @sots.nl no final de cada conta criada no playfab

Accounts.ui.config({
  passwordSignupFields: 'USERNAME_ONLY',
});

Accounts.config({
  forbidClientAccountCreation : false
});