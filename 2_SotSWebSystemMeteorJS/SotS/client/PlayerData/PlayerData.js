// Meteor.subscribe('recipes');  

//console.log(Meteor.settings.public.ga.account);
import { PlayerDataDB } from '/Collections/PlayerDataCollection.js';
import './PlayerData.html';

Template.PlayerData.onCreated (function () {
    var self = this;
    
    self.autorun (function() {
        self.subscribe('playerdatacollection');
    });
});





Template.PlayerData.helpers({ 
    // Basicamente isto é um helper que vai colocar todas as receitas da BD dentro de "recipes"
    playerdata: () => {
        return PlayerDataDB.find({});
    }
}); 


Template.PlayerData.events ({
    'submit .info-playerData-add'(event) {
        event.preventDefault();
        const target = event.target;
    
        const lat = target.lat;
        const lng = target.lng;
        

        const ownerPlayFabID = Meteor.user().username;

        Meteor.call('playerDataDB.insert', 
                ownerPlayFabID,
                lat,
                lng,
                (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        
                        alert("Player Data introduced successfully :)");
                    }
                });
    
        
      }
});
