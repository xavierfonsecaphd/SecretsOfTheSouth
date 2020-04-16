import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

/**
 * The Challenges_Timer_Multiplayer_Independent are multiplayer challenges, with timer, and self organized
 * 
 */
export const Challenges_Timer_Multiplayer_IndependentDB = new Mongo.Collection('Challenges_Timer_Multiplayer_Independent');

// (name, description, ownerPlayFabID, typeOfChallengeIndex,
//latitude, longitude, task, imageURL, validated) {

Meteor.methods({
    'Challenges_Timer_Multiplayer_IndependentDB.insert'(name, description, ownerPlayFabID, typeOfChallengeIndex,
        latitude, longitude, task, imageURL, timer,route, validated) {
        check(name, String);
        check(description, String);
        
        check(latitude, Number);
        check(longitude, Number);

        check(task, String);
        check(timer, Number);

        check (validated, Boolean);
        
    
        return Challenges_Timer_Multiplayer_IndependentDB.insert({
            name, description, ownerPlayFabID, typeOfChallengeIndex,
            latitude, longitude, task, imageURL, timer, validated, route});
      },
    deleteChallenge_Timer_Multiplayer_Independent: function (id) {
        Challenges_Timer_Multiplayer_IndependentDB.remove(id);
    },
    updateChallenges_Timer_Multiplayer_Independent: function (id, this_name, this_description, this_ownerPlayFabID, this_typeOfChallengeIndex,
        this_latitude, this_longitude, this_task, this_imageURL, this_timer,this_route, this_validated) {
            Challenges_Timer_Multiplayer_IndependentDB.update(id, {
            $set: {
                name: this_name,
                description: this_description,
                ownerPlayFabID: this_ownerPlayFabID,
                typeOfChallengeIndex: this_typeOfChallengeIndex,
                latitude: this_latitude,
                longitude: this_longitude,
                task: this_task,
                imageURL: this_imageURL,
                timer: this_timer,
                validated: this_validated,
                route:this_route
            }
        });
    },
    toggleValidation4: function (id, currentState) {
        Challenges_Timer_Multiplayer_IndependentDB.update (id, {
            $set: {
                validated: !currentState
            }
        });
    }
});
