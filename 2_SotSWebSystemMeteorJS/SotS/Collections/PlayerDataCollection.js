import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

export const PlayerDataDB = new Mongo.Collection('PlayersDataCollection'); 

// not used at this point
/*Meteor.methods({
    'playerDataDB.insert'(ownerPlayFabID, lat, lng) {
        check(ownerPlayFabID, String);
        
        var gps = {latitude: lat, longitude: lng};
    
        return PlayerDataDB.insert({
            ownerPlayFabID, gps});
      }
});*/



/*
PlayerDataDB.allow({ 
    insert: function(userId, doc) { 
        return !!userId; // this is basically saying who's allowed to insert into the database
        // this comes up true if the userId exists
        // if it exists, it means that the user is logged in, and that he's able to insert a recipe 
    }, 
    update: function(userId, doc) { 
        return !!userId; 
    }
});

GPS = new SimpleSchema ({
    latitude: {
        type: String,
    },
    longitude: {
        type: String,
    },
    createdAt: {
        type: Date,
        label: "Created At",
        autoValue: function() { 
            if (this.isInsert) { 
                return new Date(); 
            } else if (this.isUpsert) { 
                return {$setOnInsert: new Date()}; 
            } else { 
                this.unset(); // Prevent user from supplying their own value 
            } 
        },
        autoform: {
            type: "hidden"
        }
    }
});

PlayerDataSchema = new SimpleSchema({
    ownerPlayFabID: {
        type: String,
        label: "OwnerPlayFabID",
    },
    gps: { 
        type: Array, 
    }, 
    'gps.$':{ 
        type: GPS, 
    }
});




PlayerDataDB.attachSchema (PlayerDataSchema); 
*/
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