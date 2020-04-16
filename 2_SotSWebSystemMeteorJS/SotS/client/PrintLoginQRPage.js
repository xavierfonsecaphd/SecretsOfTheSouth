// PrintLoginQRPage
import { Markers } from '/Collections/Markers.js';
import { Meteor } from 'meteor/meteor';
import './PrintLoginQRPage.html';

Template.PrintLoginQRPage.onCreated (function () {

    
});



// ******************************************************
// ******************* Helpers **************************
// ******************************************************

Template.PrintLoginQRPage.helpers({ 
    
});

Template.PrintLoginQRPage.events ({
    'submit .PrintCredentials'(event) {
        event.preventDefault();
        const target = event.target;
        const playerID = target.playerid;
    
        //console.info('Player id: ' + playerID.value);
        //FlowRouter.go('/LoginQRCodeCredentials/'+playerID.value);

        var parentOpener = window.opener; 
        window.opener = null; 
        window.open("/LoginQRCodeCredentials/"+playerID.value,"_blank"); 
        window.opener = parentOpener; 
        parentOpener.focus();
        
      },
      'click .close-edit-challenge': function() { 
        //console.info('Clicked.');
      }

});