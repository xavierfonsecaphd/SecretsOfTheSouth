import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';


/**
 * The challenges1 are the collective challenges
 */
export const Challenges1DB = new Mongo.Collection('challenges1');

/*
ChallengesDB.allow({ 
    insert: function(userId, doc) { 
        return !!userId; // this is basically saying who's allowed to insert into the database
        // this comes up true if the userId exists
        // if it exists, it means that the user is logged in, and that he's able to insert a recipe 
    }, 
    update: function(userId, doc) { 
        return !!userId; 
    }
});

ChallengeSchema = new SimpleSchema({
    // the challenge_ID is accessed through _id from the database
    name: {
        type: String,
        label: "Name",
    },
    description: {
        type: String,
        label: "Description",
    },
    ownerPlayFabID: {
        type: String,
        label: "Owner Play Fab ID",
        autoform: {
            type: "hidden"
        },
    },
    typeOfChallengeIndex: {
        type: Number,   // 0 Quiz | 1 AR | 2 IoT | 3 other
        label: "Type Of Challenge Index [0 - Quiz]",
        defaultValue: 0,
    },
    numberOfPeopleToMeet: {
        type: Number,  
        label: "Number Of People To Meet",
        defaultValue: 0,
    },
    latitude: {
        type: String,
        label: "Latitude",
    },
    longitude: {
        type: String,
        label: "Longitude",
    },


// TYPE OF QUIZ
    question: {
        type: String,
        label: "Question",
    },
    answer: {
        type: String,
        label: "Answer",
    },
    imageURL: {
        type: String,
        label: "ImageURL",
    },


    // TYPE OF AR
    // ...
    
}, { tracker: Tracker });


ChallengesDB.attachSchema (ChallengeSchema); */

Meteor.methods({
    'challenges1DB.insert'(name, description, ownerPlayFabID, typeOfChallengeIndex,
        latitude, longitude, task, imageURL,route, validated) {
        check(name, String);
        check(description, String);
        
        check(latitude, Number);
        check(longitude, Number);
        check(task, String);

        check (validated, Boolean);
        
    
        return Challenges1DB.insert({
            name, description, ownerPlayFabID, typeOfChallengeIndex,
            latitude, longitude, task, imageURL, validated, route});
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
    deleteChallenge1: function (id) {
        Challenges1DB.remove(id);
    },
    updateChallenge1: function (id, this_name, this_description, this_ownerPlayFabID, this_typeOfChallengeIndex,
        this_latitude, this_longitude, this_task, this_imageURL,this_route, this_validated) {
        Challenges1DB.update(id, {
            $set: {
                name: this_name,
                description: this_description,
                ownerPlayFabID: this_ownerPlayFabID,
                typeOfChallengeIndex: this_typeOfChallengeIndex,
                latitude: this_latitude,
                longitude: this_longitude,
                task: this_task,
                imageURL: this_imageURL,
                validated: this_validated,
                route:this_route
            }
        });
    },
    toggleValidation1: function (id, currentState) {
        Challenges1DB.update (id, {
            $set: {
                validated: !currentState
            }
        });
    }
});
