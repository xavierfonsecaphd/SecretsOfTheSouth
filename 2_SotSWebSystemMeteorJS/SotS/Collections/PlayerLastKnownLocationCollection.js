import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

export const PlayerLaskKnownLocationDB = new Mongo.Collection('PlayerLaskKnownLocationCollection'); 

// not used at this point
Meteor.methods({
    'PlayerLaskKnownLocationDB.insert'(PlayerPlayFabID, latitude, longitude, timestamp) {
        check(PlayerPlayFabID, String);
    
        return PlayerLaskKnownLocationDB.insert({ PlayerPlayFabID, latitude, longitude, timestamp});
      },
      'PlayerLaskKnownLocationDB.update': function(id, this_PlayerPlayFabID, this_latitude, this_longitude, this_timestamp ){

        PlayerLaskKnownLocationDB.update(id, {
            $set: {
              PlayerPlayFabID: this_PlayerPlayFabID,
              latitude: this_latitude,
              longitude: this_longitude,
              timestamp: this_timestamp
            }
        });
    
      // storyEdit
      },
      UpdatePlayerLaskKnownLocation: function (id, this_PlayerPlayFabID, this_latitude, this_longitude, this_timestamp) {
        PlayerLaskKnownLocationDB.update(id, {
            $set: {
                PlayerPlayFabID: this_PlayerPlayFabID,
                latitude: this_latitude,
                longitude: this_longitude,
                timestamp: this_timestamp
            }
        });
    }
});






/*Meteor.methods({
    // this is the toggle menu recipe, the one updating the bool from false to true
    // and vice versa
    toggleMenuItem: function (id, currentState) {
        Recipes.update (id, {
            $set: {
                inMenu: !currentState
            }
        });
    },
    deleteRecipe: function (id) {
        Recipes.remove(id);
    }
});*/