import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

export const TeamLastAccessedChallengeDB = new Mongo.Collection('var_TeamLastAccessedChallenge'); 

// not used at this point
Meteor.methods({
    'TeamLastAccessedChallengeDB.insert'(TeamID, LastChallengeIDSeen) {
        check(TeamID, String);
        check(LastChallengeIDSeen, String);
    
        return TeamLastAccessedChallengeDB.insert({ TeamID, LastChallengeIDSeen});
      },
      TeamLastAccessedChallengeDB: function (id, this_TeamID, this_LastChallengeIDSeen) {
        TeamLastAccessedChallengeDB.update(id, {
            $set: {
                TeamID: this_TeamID,
                LastChallengeIDSeen: this_LastChallengeIDSeen
            }
        });
    }
});


