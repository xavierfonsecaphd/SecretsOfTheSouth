import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

/**
 * The Challenges_Timed_Task are the collective challenges
 */
export const Challenges_Timed_TaskDB = new Mongo.Collection('Challenges_Timed_Task');

Meteor.methods({
    'Challenge_Timed_TaskDB.insert'(name, description, ownerPlayFabID, typeOfChallengeIndex,
        latitude, longitude, task, imageURL, questionHowMany, timer,route, validated) {
        check(name, String);
        check(description, String);
        
        check(latitude, Number);
        check(longitude, Number);

        check(task, String);
        check(questionHowMany, String);
        check(timer, Number);

        check (validated, Boolean);
        
    
        return Challenges_Timed_TaskDB.insert({
            name, description, ownerPlayFabID, typeOfChallengeIndex,
            latitude, longitude, task, imageURL, questionHowMany, timer, validated, route});
      },
    deleteChallenge_Timed_Task: function (id) {
        Challenges_Timed_TaskDB.remove(id);
    },
    updateChallenge_Timed_Task: function (id, this_name, this_description, this_ownerPlayFabID, this_typeOfChallengeIndex,
        this_latitude, this_longitude, this_task, this_imageURL, this_questionHowMany, this_timer, this_route, this_validated) {
            Challenges_Timed_TaskDB.update(id, {
            $set: {
                name: this_name,
                description: this_description,
                ownerPlayFabID: this_ownerPlayFabID,
                typeOfChallengeIndex: this_typeOfChallengeIndex,
                latitude: this_latitude,
                longitude: this_longitude,
                task: this_task,
                imageURL: this_imageURL,
                questionHowMany: this_questionHowMany, 
                timer: this_timer,
                validated: this_validated,
                route:this_route
            }
        });
    },
    toggleValidation5: function (id, currentState) {
        Challenges_Timed_TaskDB.update (id, {
            $set: {
                validated: !currentState
            }
        });
    }
});
