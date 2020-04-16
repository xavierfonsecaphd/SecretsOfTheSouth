import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';


/**
 * The challenges1 are the collective challenges
 */
export const Challenges_HunterDB = new Mongo.Collection('Challenges_Hunter');


Meteor.methods({
    'Challenges_HunterDB.insert'(name, description, ownerPlayFabID, typeOfChallengeIndex,
        latitude, longitude, question, answer, imageURL, content_text, content_picture, route, validated) {
        check(name, String);
        check(description, String);
        
        //check(route, Number);   // only 0 (no route), 1 (route 1), 2 (route 2), and 3 (route 3)
        check(latitude, Number);
        check(longitude, Number);

        check(question, String);

        check (validated, Boolean);
        
    
        return Challenges_HunterDB.insert({
            name, description, ownerPlayFabID, typeOfChallengeIndex,
            latitude, longitude, question, answer, imageURL, content_text, content_picture, validated, route});
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
    deleteChallenge_Hunter: function (id) {
        Challenges_HunterDB.remove(id);
    },
    updateChallenge_Hunter: function (id, this_name, this_description, this_ownerPlayFabID, this_typeOfChallengeIndex,
        this_latitude, this_longitude, this_question, this_answer, this_imageURL, this_content_text, this_content_picture, this_route, this_validated) {
            Challenges_HunterDB.update(id, {
            $set: {
                name: this_name,
                description: this_description,
                ownerPlayFabID: this_ownerPlayFabID,
                typeOfChallengeIndex: this_typeOfChallengeIndex,
                latitude: this_latitude,
                longitude: this_longitude,
                question: this_question,
                answer: this_answer,
                imageURL: this_imageURL,
                content_text: this_content_text, 
                content_picture: this_content_picture,
                validated: this_validated,
                route :this_route
            }
        });
    },
    toggleValidation2: function (id, currentState) {
        Challenges_HunterDB.update (id, {
            $set: {
                validated: !currentState
            }
        });
    }
});
