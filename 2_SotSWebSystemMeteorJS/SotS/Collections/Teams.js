import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

export const TeamsDB = new Mongo.Collection('TeamsOfPlayersCollection'); 

// not used at this point
Meteor.methods({
    'TeamsDB.insert'(PlayerPlayFabID, TeamID, TeamName, TeamRefIcon) {
        check(PlayerPlayFabID, String);
        check(TeamID, String);
        check(TeamName, String);
        check(TeamRefIcon, String);

    
        return TeamsDB.insert({ PlayerPlayFabID, TeamID, TeamName, TeamRefIcon});
      },
      UpdatePlayerTeam: function (id, this_PlayerPlayFabID, this_TeamID, this_TeamName, this_TeamRefIcon) {
        TeamsDB.update(id, {
            $set: {
                PlayerPlayFabID: this_PlayerPlayFabID,
                TeamID: this_TeamID,
                TeamName: this_TeamName,
                TeamRefIcon: this_TeamRefIcon
            }
        });
    }
});

