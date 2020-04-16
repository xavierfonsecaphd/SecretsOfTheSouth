import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

// permission 0 -> no record
// permission 1 -> evaluation
// permission 2 -> observation
// permission 3 -> Administration   [E30E387C456145E0]
// permission 4 -> evaluation, observation
// permission 5 -> evaluation, Administration
// permission 6 -> observation, Administration
// permission 7 -> evaluation, observation, Administration



export const TeamsRankingDB = new Mongo.Collection('var_TeamsRanking'); 

// not used at this point
Meteor.methods({
    'TeamsRankingDB.insert'(TeamID, TeamName, TeamRefIcon, Rating, NumberOfChallengesSolved) {
        check(TeamID, String);
        check(TeamName, String);
        check(TeamRefIcon, String);
    
        return TeamsRankingDB.insert({ TeamID, TeamName, TeamRefIcon, Rating, NumberOfChallengesSolved});
      },
      TeamsRankingDB: function (id, this_TeamID, this_TeamName, this_TeamRefIcon, this_Rating, this_NumberOfChallengesSolved) {
        TeamsRankingDB.update(id, {
            $set: {
                TeamID: this_TeamID,
                TeamName: this_TeamName,
                TeamRefIcon: this_TeamRefIcon,
                Rating: this_Rating,
                NumberOfChallengesSolved: this_NumberOfChallengesSolved
            }
        });
    }
});


