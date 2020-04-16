import { ChallengesDB } from '/Collections/Challenges.js';
import { Challenges1DB } from '/Collections/Challenges1.js';
import { PlayerDataDB } from '/Collections/PlayerDataCollection.js';
import { PlayerLaskKnownLocationDB } from '/Collections/PlayerLastKnownLocationCollection.js';
import { TeamLastAccessedChallengeDB } from '/Collections/var_TeamLastAccessedChallenge.js';
import { GamePermissionsDB } from '/Collections/var_GamePermissions.js';
import { TeamsRankingDB } from '/Collections/var_TeamsRanking.js';
import { TeamsDB } from '/Collections/Teams.js';
import { SotSEventsDB } from '/Collections/SotSEvents.js';
import { Markers } from '/Collections/Markers.js';
import { Challenges_HunterDB } from '/Collections/Challenges_Hunter.js';
import { Challenges_VotingDB } from '/Collections/Challenges_Voting.js';
import { Challenges_Timer_Multiplayer_IndependentDB } from '/Collections/Challenges_Timer_Multiplayer_Independent.js';
import { Challenges_Timed_TaskDB } from '/Collections/Challenges_Timed_Task.js';
import { Challenges_OpenQuizDB } from '/Collections/Challenges_OpenQuiz.js';


//Meteor.publish('recipes',function(){
//    return Recipes.find({author: this.userId}) // only publish data that belongs to the user logged in
//});

// this code downloads all the recipes that belong to him
Meteor.publish('challenges',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return ChallengesDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('challenges1',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return Challenges1DB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('Challenges_Hunter',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return Challenges_HunterDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('SotSEvents',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return SotSEventsDB.find(); // only publish data that belongs to the user logged in
});

//SotSEvents
Meteor.publish('Challenges_Voting',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return Challenges_VotingDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('Challenges_OpenQuiz',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return Challenges_OpenQuizDB.find(); // only publish data that belongs to the user logged in
});


Meteor.publish('Challenges_Timer_Multiplayer_Independent',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return Challenges_Timer_Multiplayer_IndependentDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('Challenges_Timed_Task',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return Challenges_Timed_TaskDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('playerdatacollection',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return PlayerDataDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('PlayerLaskKnownLocationCollection',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return PlayerLaskKnownLocationDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('var_TeamLastAccessedChallenge',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return TeamLastAccessedChallengeDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('var_GamePermissions',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return GamePermissionsDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('var_TeamsRanking',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return TeamsRankingDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('TeamsOfPlayersCollection',function(){
    //return Challenges.find({author: this.userId}) // only publish data that belongs to the user logged in
    return TeamsDB.find(); // only publish data that belongs to the user logged in
});

Meteor.publish('PlayerLaskKnownLocationCollectionID',function(id){
    check(id, String);  // this checks whether the id is a string
    return PlayerLaskKnownLocationDB.find({PlayerPlayFabID: id}); // only publish data that belongs to the user logged in
});

Meteor.publish('challenge',function(id){
    check(id, String);  // this checks whether the id is a string
    return ChallengesDB.find({_id: id}); // only publish data that of one recipe, for performance considerations
});

Meteor.publish('challenge1',function(id){
    check(id, String);  // this checks whether the id is a string
    return Challenges1DB.find({_id: id}); // only publish data that of one recipe, for performance considerations
});

Meteor.publish('Challenge_Hunter',function(id){
    check(id, String);  // this checks whether the id is a string
    return Challenges_HunterDB.find({_id: id}); // only publish data that of one recipe, for performance considerations
});

Meteor.publish('Challenge_Voting',function(id){
    check(id, String);  // this checks whether the id is a string
    return Challenges_VotingDB.find({_id: id}); // only publish data that of one recipe, for performance considerations
});

/*Meteor.publish('singleChallenge',function(id){
    check(id, String);  // this checks whether the id is a string

    return ChallengesDB.find({_id: id}); // only publish data that of one recipe, for performance considerations
});*/

Meteor.publish('markers',function(id){
    
    return Markers.find(); // only publish data that of one recipe, for performance considerations
});
