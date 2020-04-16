import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

/**
 * The challenges_voting are the collective challenges
 */
export const Challenges_VotingDB = new Mongo.Collection('Challenges_Voting');

Meteor.methods({
    'Challenges_VotingDB.insert'(name, description, ownerPlayFabID, typeOfChallengeIndex,
        latitude, longitude, task, imageURL, listOfImagesAndVotes,route, validated) {
        check(name, String);
        check(description, String);
        
        check(latitude, Number);
        check(longitude, Number);

        check(task, String);

        check (validated, Boolean);
        check(route, String)
        
    
        return Challenges_VotingDB.insert({
            name, description, ownerPlayFabID, typeOfChallengeIndex,
            latitude, longitude, task, imageURL, listOfImagesAndVotes, validated, route});
      },
    deleteChallenge_Voting: function (id) {
        Challenges_VotingDB.remove(id);
    },
    updateChallenge_Voting: function (id, this_name, this_description, this_ownerPlayFabID, this_typeOfChallengeIndex,
        this_latitude, this_longitude, this_task, this_imageURL, this_listOfImagesAndVotes, this_route, this_validated) {
            Challenges_VotingDB.update(id, {
            $set: {
                name: this_name,
                description: this_description,
                ownerPlayFabID: this_ownerPlayFabID,
                typeOfChallengeIndex: this_typeOfChallengeIndex,
                latitude: this_latitude,
                longitude: this_longitude,
                task: this_task,
                imageURL: this_imageURL,
                listOfImagesAndVotes: this_listOfImagesAndVotes, 
                validated: this_validated,
                route:this_route
            }
        });
    },
    toggleValidation3: function (id, currentState) {
        Challenges_VotingDB.update (id, {
            $set: {
                validated: !currentState
            }
        });
    }
});
