import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';


/**
 * The challenges1 are the collective challenges
 */
export const Challenges_OpenQuizDB = new Mongo.Collection('Challenges_OpenQuiz');

// name, description, ownerPlayFabID, typeOfChallengeIndex,
//latitude, longitude, question, answer, imageURL, validated
Meteor.methods({
    'Challenges_OpenQuizDB.insert'(name, description, ownerPlayFabID, typeOfChallengeIndex,
        latitude, longitude, question, imageURL, route, validated) {
        check(name, String);
        check(description, String);
        
        check(latitude, Number);
        check(longitude, Number);

        check(question, String);

        check (validated, Boolean);
        
    
        return Challenges_OpenQuizDB.insert({
            name, description, ownerPlayFabID, typeOfChallengeIndex,
            latitude, longitude, question, imageURL, validated,route});
      },
    // this is the toggle menu recipe, the one updating the bool from false to true
    // and vice versa
    /*toggleMenuItem: function (id, currentState) {
        Recipes.update (id, {
            $set: {
                inMenu: !currentState
            }
        });
    },*/
    deleteChallenge_OpenQuiz: function (id) {
        Challenges_OpenQuizDB.remove(id);
    },
    updateChallenge_OpenQuiz: function (id, this_name, this_description, this_ownerPlayFabID, this_typeOfChallengeIndex,
        this_latitude, this_longitude, this_question, this_imageURL, this_route, this_validated) {
            Challenges_OpenQuizDB.update(id, {
            $set: {
                name: this_name,
                description: this_description,
                ownerPlayFabID: this_ownerPlayFabID,
                typeOfChallengeIndex: this_typeOfChallengeIndex,
                latitude: this_latitude,
                longitude: this_longitude,
                question: this_question,
                imageURL: this_imageURL,
                validated: this_validated,
                route: this_route
            }
        });
    },
    toggleValidation6: function (id, currentState) {
        Challenges_OpenQuizDB.update (id, {
            $set: {
                validated: !currentState
            }
        });
    }
});
